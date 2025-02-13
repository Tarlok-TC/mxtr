using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Admin.ViewModels;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IAdminAccountUserManagementViewModelAdapter
    {
        AdminAccountUserManagementViewModel BuildAdminAccountUserManagementViewModel(mxtrAccount mxtrAccount, List<mxtrAccount> childAccounts);
    }

    public class AdminAccountUserManagementViewModelAdapter : IAdminAccountUserManagementViewModelAdapter
    {
        public AdminAccountUserManagementViewModel BuildAdminAccountUserManagementViewModel(mxtrAccount mxtrAccount, List<mxtrAccount> childAccounts)
        {
            AdminAccountUserManagementViewModel model = new AdminAccountUserManagementViewModel();        

            AddPageTitle(model);
            model.Accounts = BuildHierarchy(mxtrAccount, childAccounts);

            return model;
        }

        public void AddPageTitle(AdminAccountUserManagementViewModel model)
        {
            model.PageTitle = "Manage Your Account Heirarchy";
            model.MainPageHeader = "Manage Account Heirarchy";
            model.SubPageHeader = "Edit or Create accounts and users.";
            model.ShowWorkspaceFilter = false;
            model.ShowDateFilter = false;
        }

        public AccountHierarchyViewData BuildHierarchy(mxtrAccount mxtrAccount, List<mxtrAccount> childAccounts)
        {
            var availableAccounts = childAccounts
                .Select(a => new AccountHierarchyViewData
                {
                    AccountName = a.AccountName,
                    AccountObjectID = a.ObjectID,
                    ParentAccountObjectID = a.ParentAccountObjectID,
                    EditAccountUrl = new AdminEditAccountWebQuery { AccountObjectID = a.ObjectID },
                    AddChildAccountUrl = new AdminAddAccountWebQuery { ParentAccountObjectID = a.ObjectID },
                    Children = null,
                    ChildrenCount = childAccounts.Where(x => x.ParentAccountObjectID == a.ObjectID).Count()
                })
                .ToList()
                .GroupBy(a => a.ParentAccountObjectID);

            availableAccounts
                .SelectMany(g => g)
                .ForEach(a => a.Children = availableAccounts.SingleOrDefault(x => x.Key == a.AccountObjectID).Coalesce(x => x.ToList()));

            return availableAccounts
                .SelectMany(g => g)
                .SingleOrDefault(a => a.AccountObjectID == mxtrAccount.ObjectID);

        }
    }
}
