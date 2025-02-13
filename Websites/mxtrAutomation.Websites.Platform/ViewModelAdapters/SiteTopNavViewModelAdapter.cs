using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ISiteTopNavViewModelAdapter
    {
        SiteTopNavViewModel BuildSiteTopNavViewModel(mxtrUser user, mxtrAccount mxtrAccount);
    }

    public class SiteTopNavViewModelAdapter : ISiteTopNavViewModelAdapter
    {
        public SiteTopNavViewModel BuildSiteTopNavViewModel(mxtrUser user, mxtrAccount mxtrAccount)
        {
            SiteTopNavViewModel model = new SiteTopNavViewModel();

            // Add stuff here...
            AddLinks(model);
            AddUserData(model, user, mxtrAccount);

            return model;
        }

        public void AddLinks(SiteTopNavViewModel model)
        {
            model.LogoutUrl = new LogoutWebQuery();
            model.AccountAdminUrl = new AdminAccountUserManagementWebQuery();
        }

        public void AddUserData(SiteTopNavViewModel model, mxtrUser user, mxtrAccount mxtrAccount)
        {
            model.Fullname = user.FullName;
            if (user.Permissions.Contains(PermissionKind.ManageAccountUsers.ToString()))
            {
                model.HasManageAccountUserPermission = true;
            }
            else
            {
                model.HasManageAccountUserPermission = false;
            }
            if (user.Role.ToLower() == "admin")
            {
                model.HasAdminRole = true;
            }
            model.AccountType = mxtrAccount.AccountType;
        }
    }
}
