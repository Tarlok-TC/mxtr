using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.Models.WhiteLabeling.ViewModels;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using System.Web;
using System.IO;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Utils;
using mxtrAutomation.Common.Utils;
namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class WhiteLabelingController : MainLayoutControllerBase
    {
        private readonly IWhiteLabelingViewModelAdapter _viewModelAdapter;
        private readonly IAccountService _accountService;
        public WhiteLabelingController(IWhiteLabelingViewModelAdapter viewModelAdapter, IAccountService accountService)
        {
            _viewModelAdapter = viewModelAdapter;
            _accountService = accountService;
        }
        public ActionResult ViewPage(WhiteLabelingWebQuery query)
        {
            mxtrAccount account = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            WhiteLabelingViewModel model = _viewModelAdapter.BuildWhiteLabelingViewModel(account);
            return View(ViewKind.WhiteLabeling, model, query);
        }

        public ActionResult AddUpdateDomain(WhiteLabelingManageDomainWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn();
            notification = _accountService.AddUpdateDomainName(User.MxtrAccountObjectID, query.DomainName);
            return Json(new { Success = notification.Success, Message = notification.ObjectID });
        }

        public ActionResult AddUpdateHomePageUrl(WhiteLabelingManageHomePageWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn();
            notification = _accountService.AddUpdateHomePageUrl(User.MxtrAccountObjectID, query.HomePageUrl);
            return Json(new { Success = notification.Success, Message = notification.ObjectID });
        }


        [HttpPost]
        [Route("AddApplicationLogo")]
        public ActionResult AddApplicationLogo(WhiteLabelingWebQuery query)
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            CreateNotificationReturn notification = new CreateNotificationReturn();
            string logoURL = uploadFileToS3(file, ConfigManager.AppSettings["AmazoneS3BucketName"] + "/ApplicationLogo");
            if (!string.IsNullOrEmpty(logoURL))
            {
                logoURL = replaceUrl(logoURL);
                mxtrAccount.ApplicationLogoURL = logoURL.Substring(0, logoURL.LastIndexOf('?'));
                notification = _accountService.updateApplicationLogo(User.MxtrAccountObjectID, mxtrAccount.ApplicationLogoURL);
                var resultData = new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) };
                return Json(resultData);
            }
            else
                return Json(new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) });
        }

        [HttpPost]
        [Route("AddBrandingLogo")]
        public ActionResult AddBrandingLogo(WhiteLabelingWebQuery query)
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            CreateNotificationReturn notification = new CreateNotificationReturn();
            string logoURL = uploadFileToS3(file, ConfigManager.AppSettings["AmazoneS3BucketName"] + "/BrandingLogo");
            if (!string.IsNullOrEmpty(logoURL))
            {
                logoURL = replaceUrl(logoURL);
                mxtrAccount.BrandingLogoURL = logoURL.Substring(0, logoURL.LastIndexOf('?'));
                notification = _accountService.updateBrandingLogo(User.MxtrAccountObjectID, mxtrAccount.BrandingLogoURL);
                var resultData = new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) };
                return Json(resultData);
            }
            else
                return Json(new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) });
        }

        [HttpPost]
        [Route("AddFavIcon")]
        public ActionResult AddFavIcon(WhiteLabelingWebQuery query)
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            CreateNotificationReturn notification = new CreateNotificationReturn();
            string logoURL = uploadFileToS3(file, ConfigManager.AppSettings["AmazoneS3BucketName"] + "/FavIcon");
            if (!string.IsNullOrEmpty(logoURL))
            {
                logoURL = replaceUrl(logoURL);
                mxtrAccount.FavIconURL = logoURL.Substring(0, logoURL.LastIndexOf('?'));
                notification = _accountService.updateFavIcon(User.MxtrAccountObjectID, mxtrAccount.FavIconURL);
                var resultData = new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) };
                return Json(resultData);
            }
            else
                return Json(new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) });
        }


        [HttpPost]
        [Route("RemoveBrandingLogo")]
        public ActionResult RemoveBrandingLogo(WhiteLabelingWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn();
            notification = _accountService.updateBrandingLogo(User.MxtrAccountObjectID, "");
            var resultData = new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) };
            return Json(resultData);
        }

        [HttpPost]
        [Route("RemoveApplicationLogo")]
        public ActionResult RemoveApplicationLogo(WhiteLabelingWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn();
            notification = _accountService.updateApplicationLogo(User.MxtrAccountObjectID, "");
            var resultData = new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) };
            return Json(resultData);
        }
        [HttpPost]
        [Route("RemoveFavIcon")]
        public ActionResult RemoveFavIcon(WhiteLabelingWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn();
            notification = _accountService.updateFavIcon(User.MxtrAccountObjectID, "");
            var resultData = new { Success = notification.Success, Data = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID) };
            return Json(resultData);
        }

        public string uploadFileToS3(HttpPostedFileBase file, string bucketName)
        {
            string LinkURL = string.Empty;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = string.Empty;
                    _FileName = User.MxtrAccountObjectID + ".png";

                    if (file.ContentType == "image/svg+xml")
                        _FileName = User.MxtrAccountObjectID + ".svg";

                    string _path = Path.Combine(Server.MapPath("~/Images"), _FileName);
                    file.SaveAs(_path);
                    AmazonS3Utils S3 = new AmazonS3Utils(
                        ConfigManager.AppSettings["AmazoneS3AccessKey"],
                        ConfigManager.AppSettings["AmazoneS3SecretAccessKey"],
                        ConfigManager.AppSettings["AmazoneS3ServiceURL"]);
                    LinkURL = S3.UploadFile(_path, bucketName, _FileName);
                    deleteFile(_path);
                }
            }
            return LinkURL;
        }

        public void deleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public string replaceUrl(string url)
        {
            if (!url.Contains("https"))
                url = url.Replace("http", "https");

            return url;
        }
    }
}
