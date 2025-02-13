using System;
using System.Collections.Generic;
using System.Web.Mvc;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Web.Common.UI;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using Ninject;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Utils;
using mxtrAutomation.Websites.Platform.Helpers;
using System.Web.Security;
using System.Security.Principal;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public abstract class MainLayoutControllerBase : mxtrAutomationControllerBase
    {
        [Inject]
        public IMainLayoutViewModelAdapter ViewModelAdapter { get; set; }

        [Inject]
        public ISiteTopNavViewModelAdapter SiteTopNavViewModelAdapter { get; set; }

        [Inject]
        public IWorkspaceFilterViewModelAdapter WorkspaceFilterViewModelAdapter { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public IAccountUtils AccountUtils { get; set; }

        [Inject]
        public IErrorLogService ErrorLogService { get; set; }

        [Inject]
        public IManageMenuService ManageMenuService { get; set; }

        public override ActionResult View(ViewKindBase viewKind, ViewModelBase model)
        {
            throw new InvalidOperationException("Wrong View() method called from a controller derived from MainLayoutControllerBase.  Use the View() method that takes a querybase instead.");
        }

        public ActionResult View(ViewKind viewKind, MainLayoutViewModelBase model, QueryBase query)
        {
            if (model == null)
                return Redirect(new LoginWebQuery());

            if (User == null || !User.IsAuthenticated)
                return Redirect(new LoginWebQuery());

            if (User != null && (String.IsNullOrEmpty(User.MxtrUserObjectID) || String.IsNullOrEmpty(User.MxtrAccountObjectID)))
            {
                FormsAuthentication.SignOut();
                System.Web.HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                return Redirect(new LoginWebQuery());
            }

            mxtrUser mxtrUser = UserService.GetUserByUserObjectID(User.MxtrUserObjectID);
            mxtrAccount mxtrAccount = AccountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            List<mxtrAccount> childAccounts = AccountService.GetAccountHeirarchy(mxtrAccount.ObjectID).ToList();

            WorkspaceHierarchyViewData workspace = AccountUtils.BuildHierarchy(mxtrAccount, childAccounts);

            mxtrAccount userParentAccount = new mxtrAccount();
            if (mxtrAccount.AccountType == AccountKind.Client.ToString() || mxtrAccount.AccountType == AccountKind.Group.ToString())
            {
                userParentAccount = AccountService.GetAccountByAccountObjectID(mxtrAccount.ParentAccountObjectID);
                userParentAccount = GetParentAccount(userParentAccount);
                ViewModelAdapter.AddData(model, HttpContext.Request.UserAgent, mxtrUser, userParentAccount, childAccounts);
            }
            else
            {
                ViewModelAdapter.AddData(model, HttpContext.Request.UserAgent, mxtrUser, mxtrAccount, childAccounts);
            }

            //ViewModelAdapter.AddData(model, HttpContext.Request.UserAgent, mxtrUser, userParentAccount, childAccounts);

            model.SiteTopNav =
                SiteTopNavViewModelAdapter.BuildSiteTopNavViewModel(mxtrUser, mxtrAccount);

            model.WorkspaceFilter =
                WorkspaceFilterViewModelAdapter.BuildWorkspaceFilterViewModel(workspace, model.CurrentAccountIDs);


            //check if user is client/group
            if (mxtrAccount.AccountType == AccountKind.Client.ToString() || mxtrAccount.AccountType == AccountKind.Group.ToString())
            {
                model.SideMenus = ManageMenuService.GetMenuData(userParentAccount.ObjectID).OrderBy(x => x.SortOrder).ToList();
            }
            else
            {
                model.SideMenus = ManageMenuService.GetMenuData(User.MxtrAccountObjectID).OrderBy(x => x.SortOrder).ToList();
            }
            if (model.SideMenus.Count <= 0)
            {
                model.SideMenus = ManageMenuService.GetMenuMaster();
            }

            return base.View(viewKind, model);
        }
        private mxtrAccount GetParentAccount(mxtrAccount parentAccount)
        {
            if (parentAccount.AccountType == AccountKind.Group.ToString())
            {
                parentAccount = AccountService.GetAccountByAccountObjectID(parentAccount.ParentAccountObjectID);
                //call the function recursively until we get parent
                return GetParentAccount(parentAccount);
            }

            return parentAccount;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            ErrorLogService.CreateErrorLog(new ErrorLogModel
            {
                LogTime = DateTime.UtcNow,
                Description = "Web Page Error",
                LogType = ErrorKind.Website.ToString(),
                ErrorMessage = filterContext.Exception != null ? filterContext.Exception.Message + filterContext.Exception.StackTrace + filterContext.Exception.Source : null,
            });
        }
    }
}