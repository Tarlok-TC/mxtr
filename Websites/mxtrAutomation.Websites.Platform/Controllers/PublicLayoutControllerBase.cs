using System;
using System.Web.Mvc;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Web.Common.UI;
//using mxtrAutomation.Websites.Platform.Models.Shared.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using Ninject;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public abstract class PublicLayoutControllerBase : mxtrAutomationControllerBase
    {
        [Inject]
        public IPublicLayoutViewModelAdapter ViewModelAdapter { get; set; }
        [Inject]
        public IAccountService AccountService { get; set; }

        public override ActionResult View(ViewKindBase viewKind, ViewModelBase model)
        {
            throw new InvalidOperationException("Wrong View() method called from a controller derived from MainLayoutControllerBase.  Use the View() method that takes a querybase instead.");
        }

        public ActionResult View(ViewKind viewKind, PublicLayoutViewModelBase model, QueryBase query)
        {
            if (model == null)
                throw new InvalidOperationException("Model is null in MainLayoutControllerBase.");
            string Url = Request.Url.Host.ToLower();
            if (Url.Contains("."))
            {
                Url = Url.Replace("www", "");
                mxtrAccount account = AccountService.GetBrandingLogoURL(Url.Substring(0, Url.IndexOf(".")));
                if (account != null)
                    ViewModelAdapter.AddData(model, System.Web.HttpContext.Current.Request.UserAgent, account);
                else
                {
                    ViewModelAdapter.AddData(model, System.Web.HttpContext.Current.Request.UserAgent, new mxtrAccount());
                }
            }

            return base.View(viewKind, model);
        }

        public new ActionResult Json(object data)
        {
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
