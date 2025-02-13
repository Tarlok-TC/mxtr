using System;
using System.Linq;
using System.Collections.Generic;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewData;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IWorkspaceFilterViewModelAdapter
    {
        WorkspaceFilterViewModel BuildWorkspaceFilterViewModel(WorkspaceHierarchyViewData workspace, string currentAccountIDs);
    }

    public class WorkspaceFilterViewModelAdapter : IWorkspaceFilterViewModelAdapter
    {
        public WorkspaceFilterViewModel BuildWorkspaceFilterViewModel(WorkspaceHierarchyViewData workspace, string currentAccountIDs)
        {
            WorkspaceFilterViewModel model = new WorkspaceFilterViewModel();

            // Add stuff here...
            model.CurrentAccountIDs = currentAccountIDs;
            model.Accounts = buildWorkspaceViewData(workspace, string.Empty, currentAccountIDs);
            //model.ShowWorkspaceFilter = model.Accounts.ChildAccounts != null ? model.Accounts.ChildAccounts.Any() : false;

            return model;
        }

        protected WorkspaceFilterViewData buildWorkspaceViewData(WorkspaceHierarchyViewData account, string parentAccountName, string currentAccountIDs)
        {
            List<string> accountObjectIds = String.IsNullOrEmpty(currentAccountIDs) ? new List<string>() : currentAccountIDs.Split(',').ToList();


            WorkspaceFilterViewData newAccount =
                new WorkspaceFilterViewData
                {
                    SelectedClass = GetSelectedClass(account.AccountObjectID, accountObjectIds),
                    SelectedChildClass = account.Children == null ? string.Empty : (account.Children.Count() == 0 ? string.Empty : GetSelectedClass(account.AccountObjectID, accountObjectIds)),
                    AccountObjectID = account.AccountObjectID,
                    AccountName = account.AccountName,
                    ParentAccountObjectID = account.ParentAccountObjectID,
                    ParentAccountName = parentAccountName,
                    ChildAccounts = account.Children == null ? new List<WorkspaceFilterViewData>() : account.Children.Select(a => buildWorkspaceViewData(a, account.AccountName, currentAccountIDs)).ToList(),
                    ChildrenClass = account.Children == null ? string.Empty : (account.Children.Count() == 0 ? string.Empty : "has-children"),
                    AccountType = account.AccountType
                };

            return newAccount;
        }

        private string GetSelectedClass(string accountObjectID, List<string> accountObjectIds)
        {
            if (accountObjectIds.Contains(accountObjectID))
                return "filter-check-all fa fa-check-circle";

            return "filter-check-none fa  fa-circle-o";
        }
    }
}
