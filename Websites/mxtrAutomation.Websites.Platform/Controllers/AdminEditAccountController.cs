using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Admin.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Websites.Platform.Utils;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Text;


using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Analytics.v3;
using Google.Apis.Util.Store;
using Google.Apis.Analytics.v3.Data;
using mxtrAutomation.Common.Utils;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.Auth.OAuth2.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Web;
using mxtrAutomation.Websites.Platform.Helpers;
using mxtrAutomation.Websites.Platform.Models.Account.ViewModels;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class AdminEditAccountController : MainLayoutControllerBase
    {
        private readonly IAdminEditAccountViewModelAdapter _viewModelAdapter;
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        private readonly IAccountUtils _accountUtils;
        private readonly IUserUtils _userUtils;

        public AdminEditAccountController(IAdminEditAccountViewModelAdapter viewModelAdapter, IAccountService accountService, IUserService userService, IAccountUtils accountUtils, IUserUtils userUtils)
        {
            _viewModelAdapter = viewModelAdapter;
            _accountService = accountService;
            _userService = userService;

            _accountUtils = accountUtils;
            _userUtils = userUtils;
        }

        public ActionResult ViewPage(AdminEditAccountWebQuery query)
        {
            // Get data...
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(query.AccountObjectID);

            List<mxtrUser> mxtrUsers = _userService.GetUsersByAccountObjectID(query.AccountObjectID).ToList();

            List<mxtrAccount> moveToAccounts = _accountService.GetFlattenedAccountsForMoving(User.MxtrAccountObjectID, query.AccountObjectID);

            //List<EZShredAccountDataModel> clients = _accountService.GetFlattenedChildClientAccounts(User.MxtrAccountObjectID);
            List<EZShredAccountDataModel> clients = new List<EZShredAccountDataModel>();
            if (mxtrAccount.AccountType != AccountKind.Group.ToString()) // No need to show market places for group
            {
                mxtrAccount parentAccount = new mxtrAccount();
                parentAccount = GetParentAccountOfLead(mxtrAccount);
                clients = _accountService.GetFlattenedChildClientAccounts(parentAccount.ObjectID);
            }

            // Adapt data...
            AdminEditAccountViewModel model =
                _viewModelAdapter.BuildAdminEditAccountViewModel(mxtrAccount, mxtrUsers, moveToAccounts, clients);

            // Handle...
            return View(ViewKind.AdminEditAccount, model, query);
        }

        private mxtrAccount GetParentAccountOfLead(mxtrAccount parentAccount)
        {
            if (parentAccount.AccountType != AccountKind.Organization.ToString() && parentAccount.AccountType.ToLower() != "reseller")
            {
                parentAccount = _accountService.GetAccountByAccountObjectID(parentAccount.ParentAccountObjectID);
                //call the function recursively until we get parent
                return GetParentAccountOfLead(parentAccount);
            }

            return parentAccount;
        }

        public ActionResult EditAccountSubmit(AdminEditAccountSubmitWebQuery query)
        {
            mxtrAccount account = new mxtrAccount();
            CreateNotificationReturn notification = new CreateNotificationReturn();
            List<EZShredAccountDataModel> marketPlaces = new List<EZShredAccountDataModel>();

            if (query.IsValid)
            {
                AccountProfileViewData accountData = query.GetValues();

                account = _accountUtils.ConvertToMxtrAccountDataModel(accountData);

                notification = _accountService.UpdateAccount(account);

                //notification.Success = true;
            }

            return Json(new { Success = notification.Success, MxtrAccountID = account.MxtrAccountID, AccountObjectID = notification.ObjectID });
        }

        public ActionResult EditAccountAttributesSubmit(AdminEditAccountAttributesSubmitWebQuery query)
        {
            mxtrAccount account = new mxtrAccount();
            CreateNotificationReturn notification = new CreateNotificationReturn();

            if (query.IsValid)
            {
                AccountAttributesViewData accountData = query.GetValues();

                account.ObjectID = accountData.ObjectID;
                account.MxtrAccountID = accountData.MxtrAccountID.ToString();
                account.SharpspringSecretKey = accountData.SharpspringSecretKey;
                account.SharpspringAccountID = accountData.SharpspringAccountID;
                account.BullseyeClientId = accountData.BullseyeClientId;
                account.BullseyeAdminApiKey = accountData.BullseyeAdminApiKey;
                account.BullseyeSearchApiKey = accountData.BullseyeSearchApiKey;
                account.BullseyeLocationId = accountData.BullseyeLocationId;
                account.BullseyeThirdPartyId = accountData.BullseyeThirdPartyId;
                account.WebsiteUrl = accountData.WebsiteUrl;
                account.GoogleAnalyticsReportingViewId = accountData.GoogleAnalyticsReportingViewId;
                account.GoogleAnalyticsTimeZoneName = accountData.GoogleAnalyticsTimeZoneName;
                account.GoogleServiceAccountCredentialFile = accountData.GoogleServiceAccountCredentialFile;
                account.GoogleServiceAccountEmail = accountData.GoogleServiceAccountEmail;
                account.GAProfileName = accountData.GAProfileName;
                account.GAWebsiteUrl = accountData.GAWebsiteUrl;
                account.EZShredIP = accountData.EZShredIP;
                account.EZShredPort = accountData.EZShredPort;
                account.KlipfolioCompanyID = accountData.KlipfolioCompanyID;
                account.KlipfolioSSOSecretKey = accountData.KlipfolioSSOSecretKey;
                account.DealerId = accountData.DealerId;
                account.Lead = accountData.SSLead;
                account.ContactMade = accountData.SSContact;
                account.ProposalSent = accountData.SSQuoteSent;
                account.Closed = accountData.SSClosed;
                account.WonNotScheduled = accountData.SSWonNotScheduled;
                notification = _accountService.UpdateAccountAttributes(account);
            }

            return Json(new { Success = notification.Success, MxtrAccountID = account.MxtrAccountID, AccountObjectID = notification.ObjectID, EZShredIP = account.EZShredIP, EZShredPort = account.EZShredPort });
        }

        #region GA OAuth2 Authentication
        //[Route("AuthCallback/IndexAsync/")]
        //public ActionResult AuthCallbackHandler(FormCollection fcData, AuthorizationCodeResponseUrl authorizationCode)
        //{
        //    if (!string.IsNullOrEmpty(fcData["AccountobjectId"]))
        //    {
        //        var btnCancel = fcData.AllKeys.Where(k => k.Equals("btnCancel")).FirstOrDefault();
        //        if (!string.IsNullOrEmpty(btnCancel))
        //        {
        //            RevokeGACredentials(fcData["AccountobjectId"]);
        //        }
        //        return Redirect(new AdminEditAccountWebQuery { AccountObjectID = fcData["AccountobjectId"] });
        //    }
        //    else
        //    {
        //        string id = Convert.ToString(TempData["objectId"]);
        //        if (!string.IsNullOrEmpty(authorizationCode.Code))
        //        {
        //            IAuthorizationCodeFlow flow = AppAuthFlowMetadata.flow;
        //            TokenResponse response = flow.ExchangeCodeForTokenAsync(id, authorizationCode.Code, "http://" + Request.Url.Authority + Request.Url.AbsolutePath, CancellationToken.None).Result; // response.accessToken now should be populated
        //            var token = new TokenResponse { RefreshToken = response.AccessToken };
        //            var credentials = new UserCredential(flow,
        //                id,
        //                token);
        //            string removeTempFile = HttpContext.Server.MapPath(ConfigManager.AppSettings["GAFilePath"]) + "System.String-oauth_" + id;
        //            if (System.IO.File.Exists(removeTempFile))
        //            {
        //                System.IO.File.Delete(removeTempFile);
        //            }
        //            return GetGADataOnUserAllow(id, credentials);
        //        }
        //        else
        //        {
        //            return Redirect(new AdminEditAccountWebQuery { AccountObjectID = id });
        //        }
        //    }
        //}

        public ActionResult GoogleAnalytics(string id)
        {
            TempData["objectId"] = id;
            var result = new AuthorizationCodeMvcApp(this,
                new AppAuthFlowMetadata()).AuthorizeAsync(CancellationToken.None).Result;

            if (result.Credential == null)
                return new RedirectResult(result.RedirectUri + "&approval_prompt=auto&from_login=1");

            return GetGADataOnUserAllow(id, result.Credential);
        }

        [HttpPost]
        public ActionResult GoogleAnalytics(FormCollection fcData)
        {
            var btnCancel = fcData.AllKeys.Where(k => k.Equals("btnCancel")).FirstOrDefault();
            if (!string.IsNullOrEmpty(btnCancel))
            {
                RevokeGACredentials(fcData["AccountobjectId"]);
            }

            return Redirect(new AdminEditAccountWebQuery
            {
                AccountObjectID = fcData["AccountobjectId"]
            });
        }

        [Route("AdminEditAccountController/DisconnectGA/")]
        public ActionResult DisconnectGAData(string id)
        {
            string strGAFilePath = Server.MapPath(ConfigManager.AppSettings["GAFilePath"]);
            mxtrAccount account = _accountService.GetAccountByAccountObjectID(id);
            DisconnectGA(account, strGAFilePath);

            return Redirect(new AdminEditAccountWebQuery
            {
                AccountObjectID = id,
            });
        }

        private ActionResult GetGADataOnUserAllow(string id, UserCredential credential)
        {
            try
            {
                var service = new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Mxtr"
                });

                var gaManagementAccount = service.Management.Accounts.List().Execute();
                List<GoogleAnalyticsAccount> lstGAAccount = new List<GoogleAnalyticsAccount>();

                foreach (var item in gaManagementAccount.Items)
                {
                    Profiles profiles = service.Management.Profiles.List(item.Id, "~all").Execute();
                    lstGAAccount.Add(new GoogleAnalyticsAccount
                    {
                        Id = item.Id,
                        Name = item.Name,
                        lstGAProfile = GetGAProfile(profiles),
                    });
                }

                return View(ViewKind.GoogleAnalytics, new GoogleAnalyticsViewData
                {
                    AccountObjectId = id,
                    CredentialFile = credential.Token + "-" + id,
                    lstGAAccount = lstGAAccount,
                }, null);

            }
            catch (Exception ex)
            {
                //delete credentials file in case of error
                if (credential != null)
                {
                    string strGAFilePath = Server.MapPath(ConfigManager.AppSettings["GAFilePath"]) + credential.Token + "-" + id;
                    DeleteGAFiles(strGAFilePath);
                }
                ViewBag.GAErrorMessage = "There was a problem while connecting to google analytics.";
                Helper.WriteErrorLog(ex, Server.MapPath("~/GAError.txt"));

                return ViewPage(new AdminEditAccountWebQuery { AccountObjectID = id });
                //return Redirect(new AdminEditAccountWebQuery { AccountObjectID = id });
            }
        }

        [Route("SaveGAData")]
        public ActionResult SaveGAData(string id, string viewid, string timezone, string username, string credentialFile, string gaProfileName, string gaWebsiteUrl)
        {
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(id);
            mxtrAccount account = new mxtrAccount()
            {
                ObjectID = id,
                GoogleAnalyticsReportingViewId = viewid,
                GoogleAnalyticsTimeZoneName = timezone,
                GoogleServiceAccountCredentialFile = credentialFile,
                GoogleServiceAccountEmail = username,
                GAProfileName = gaProfileName,
                GAWebsiteUrl = gaWebsiteUrl,
                BullseyeAdminApiKey = mxtrAccount.BullseyeAdminApiKey,
                BullseyeClientId = mxtrAccount.BullseyeClientId,
                BullseyeLocationId = mxtrAccount.BullseyeLocationId,
                BullseyeSearchApiKey = mxtrAccount.BullseyeSearchApiKey,
                SharpspringAccountID = mxtrAccount.SharpspringAccountID,
                SharpspringSecretKey = mxtrAccount.SharpspringSecretKey,
            };
            _accountService.UpdateAccountAttributes(account);
            return Json("ok");
        }

        private static List<GoogleAnalyticsProfile> GetGAProfile(Profiles profiles)
        {
            List<GoogleAnalyticsProfile> lstGoogleAnalyticsProfile = new List<GoogleAnalyticsProfile>();
            int index = 0;
            foreach (var profileItem in profiles.Items)
            {
                lstGoogleAnalyticsProfile.Add(new GoogleAnalyticsProfile
                {
                    ViewId = profileItem.Id,
                    Timezone = profileItem.Timezone,
                    WebsiteUrl = profileItem.WebsiteUrl.EndsWith("/") ? profileItem.WebsiteUrl.Substring(0, profileItem.WebsiteUrl.Length - 1) : profileItem.WebsiteUrl,
                    UserName = profiles.Username,
                    PropertyId = profiles.Items[index].InternalWebPropertyId,
                    ViewName = profiles.Items[index].Name,
                });
                index++;
            }
            return lstGoogleAnalyticsProfile;
        }

        private CreateNotificationReturn DisconnectGA(mxtrAccount account, string strGAPath)
        {
            RevokeGACredentials(account.ObjectID);
            strGAPath = strGAPath + account.GoogleServiceAccountCredentialFile;
            DeleteGAFiles(strGAPath);
            account.ObjectID = account.ObjectID;
            account.GoogleAnalyticsReportingViewId = string.Empty;
            account.GoogleAnalyticsTimeZoneName = string.Empty;
            account.GoogleServiceAccountCredentialFile = string.Empty;
            account.GoogleServiceAccountEmail = string.Empty;
            account.GAProfileName = string.Empty;
            account.GAWebsiteUrl = string.Empty;

            return _accountService.UpdateAccountAttributes(account);
        }

        private void RevokeGACredentials(string id)
        {
            TempData["objectId"] = id;
            var result = new AuthorizationCodeMvcApp(this, new AppAuthFlowMetadata()).AuthorizeAsync(CancellationToken.None).Result;
            if (result.Credential != null)
            {
                //Disconnect google api
                AppAuthFlowMetadata.flow.RevokeTokenAsync(id, result.Credential.Token.AccessToken, CancellationToken.None);
            }
        }

        private void DeleteGAFiles(string strGAFilePath)
        {
            if (System.IO.File.Exists(strGAFilePath))
            {
                System.IO.File.Delete(strGAFilePath);
            }

            //var hasAuthFile = (Directory.Exists(strGAFilePath) == true && Directory.GetFiles(strGAFilePath, "*").Length > 0) ? true : false; // TODO: Need to get info from database
            //if (hasAuthFile)
            //{
            //    string[] filePaths = Directory.GetFiles(strGAFilePath);
            //    foreach (string filePath in filePaths)
            //        System.IO.File.Delete(filePath);
            //}
        }
        #endregion

    }
}
