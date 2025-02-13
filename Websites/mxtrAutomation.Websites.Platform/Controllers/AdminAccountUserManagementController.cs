using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Admin.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using Ninject;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Websites.Platform.Utils;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class AdminAccountUserManagementController : MainLayoutControllerBase
    {
        private readonly IAdminAccountUserManagementViewModelAdapter _viewModelAdapter;
        private readonly IAccountService _accountService;

        public AdminAccountUserManagementController(IAdminAccountUserManagementViewModelAdapter viewModelAdapter, IAccountService accountService)
        {
            _viewModelAdapter = viewModelAdapter;
            _accountService = accountService;
        }

        public ActionResult ViewPage(AdminAccountUserManagementWebQuery query)
        {
            if (query.IsDelete)
            {
                DeleteAccountWithChilds(query.AccountObjectID);
            }
            // Get data...
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);

            List<mxtrAccount> childAccounts = _accountService.GetAccountHeirarchy(mxtrAccount.ObjectID).ToList();

            // Adapt data...
            AdminAccountUserManagementViewModel model =
                _viewModelAdapter.BuildAdminAccountUserManagementViewModel(mxtrAccount, childAccounts);

            // Handle...
            if (query.IsAjax)
            {
                return Json(model);
            }

            return View(ViewKind.AdminAccountUserManagement, model, query);
        }

        private void DeleteAccountWithChilds(string accountObjectId)
        {
            mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectId);
            if (account != null)
            {
                DeleteChild(accountObjectId);
                DeleteAccount(account);
            }
        }

        private void DeleteChild(string parentObjectId)
        {
            List<mxtrAccount> child = _accountService.GetAccountsByParentAccountObjectID(parentObjectId).ToList();
            foreach (var item in child)
            {
                //DeleteAccount(item);
                DeleteChild(item.ObjectID);
            }
        }
        private void DeleteAccount(mxtrAccount account)
        {
            account.IsActive = false;            
            CreateNotificationReturn result = _accountService.UpdateAccount(account);
        }
    }
}
