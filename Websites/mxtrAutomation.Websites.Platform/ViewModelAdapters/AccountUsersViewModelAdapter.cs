using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Account.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Common.Attributes;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Enums;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IAccountUsersViewModelAdapter
    {
        AccountUsersViewModel BuildAccountUsersViewModel();
        AccountUsersViewModel BuildAdminCreateAccountUsersPartialViewModel(List<EZShredAccountDataModel> marketPlaces);
        AccountUsersViewModel BuildAdminEditAccountUsersPartialViewModel(mxtrAccount mxtrAccount, List<mxtrUser> users, List<EZShredAccountDataModel> marketPlaces);
    }

    public class AccountUsersViewModelAdapter : IAccountUsersViewModelAdapter
    {
        public AccountUsersViewModel BuildAccountUsersViewModel()
        {
            AccountUsersViewModel model = new AccountUsersViewModel();

            return model;
        }

        public AccountUsersViewModel BuildAdminCreateAccountUsersPartialViewModel(List<EZShredAccountDataModel> marketPlaces)
        {
            AccountUsersViewModel model = new AccountUsersViewModel();

            AddLinks(model, AccountActionKind.Create);
            AddPermissionKinds(model);
            AddMarketPlacesInfo(model, marketPlaces);

            return model;
        }

        public AccountUsersViewModel BuildAdminEditAccountUsersPartialViewModel(mxtrAccount mxtrAccount, List<mxtrUser> users, List<EZShredAccountDataModel> marketPlaces)
        {
            AccountUsersViewModel model = new AccountUsersViewModel();
            model.EZShredIP = mxtrAccount.EZShredIP;
            model.EZShredPort = mxtrAccount.EZShredPort;
            model.DealerId = mxtrAccount.DealerId;
            model.SSLead = mxtrAccount.Lead;
            model.SSContact = mxtrAccount.ContactMade;
            model.SSQuoteSent = mxtrAccount.ProposalSent;
            model.SSClosed = mxtrAccount.Closed;
            model.SSWonNotScheduled = mxtrAccount.WonNotScheduled;
            AddLinks(model, AccountActionKind.Edit);
            AddPermissionKinds(model);
            AddAccountUsersInfo(model, users);
            AddMarketPlacesInfo(model, marketPlaces);

            return model;
        }

        private void AddMarketPlacesInfo(AccountUsersViewModel model, List<EZShredAccountDataModel> marketPlaces)
        {
            model.MarketPlaces = marketPlaces;
        }

        public void AddLinks(AccountUsersViewModel model, AccountActionKind accountAction)
        {
            model.UserSubmitUrl = new AdminAddAccountUserSubmitWebQuery();
            model.UserFinishedUrl = new AdminAccountUserManagementWebQuery();

            if (accountAction == AccountActionKind.Create)
            {
                model.AccountActionKind = AccountActionKind.Create;
            }
            else
            {
                model.AccountActionKind = AccountActionKind.Edit;
            }
        }

        public void AddPermissionKinds(AccountUsersViewModel model)
        {
            IDictionary<PermissionKind, string> mxtrPermissionKinds = EnumExtensions.ToStringValueDictionary<PermissionKind, DisplayAttribute>();

            model.PermissionKinds = mxtrPermissionKinds.Select(x => new AccountUserPermissionsViewData
            {
                PermissionName = x.Value,
                PermissionKind = x.Key,
                Checked = false
            }).ToList();
        }

        public void AddAccountUsersInfo(AccountUsersViewModel model, List<mxtrUser> users)
        {
            if (users != null && users.Count > 0)
            {
                model.Users = users.Select(u => new AccountUserViewData
                {
                    ObjectID = u.ObjectID,
                    MxtrUserID = new Guid(u.MxtrUserID),
                    MxtrAccountID = new Guid(u.MxtrAccountID),
                    AccountObjectID = u.AccountObjectID,
                    FullName = u.FullName,
                    Email = u.Email,
                    UserName = u.UserName,
                    Phone = u.Phone ?? string.Empty,
                    CellPhone = u.CellPhone ?? string.Empty,
                    EZShredAccountMappings = u.EZShredAccountMappings,
                    Role = u.Role,
                    Permissions = string.Join(",", u.Permissions),
                    CreateDate = u.CreateDate,
                    IsActive = u.IsActive,
                    SharpspringPassword = u.SharpspringPassword,
                    SharpspringUserName = u.SharpspringUserName,
                }).ToList();
            }
        }
    }
}
