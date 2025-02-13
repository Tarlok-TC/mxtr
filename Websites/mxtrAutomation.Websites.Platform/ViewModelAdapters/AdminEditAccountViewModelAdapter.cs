using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Admin.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewModels;
using Ninject;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IAdminEditAccountViewModelAdapter
    {
        AdminEditAccountViewModel BuildAdminEditAccountViewModel(mxtrAccount mxtrAccount, List<mxtrUser> mxtrUsers, List<mxtrAccount> moveToAccounts, List<EZShredAccountDataModel> clients);
    }

    public class AdminEditAccountViewModelAdapter : IAdminEditAccountViewModelAdapter
    {
        [Inject]
        public IAccountProfileViewModelAdapter AccountProfileViewModelAdapter { get; set; }

        [Inject]
        public IAccountAttributesViewModelAdapter AccountAttributesViewModelAdapter { get; set; }

        [Inject]
        public IAccountUsersViewModelAdapter AccountUsersViewModelAdapter { get; set; }


        public AdminEditAccountViewModel BuildAdminEditAccountViewModel(mxtrAccount mxtrAccount, List<mxtrUser> mxtrUsers, List<mxtrAccount> moveToAccounts, List<EZShredAccountDataModel> clients)
        {
            AdminEditAccountViewModel model = new AdminEditAccountViewModel();

            AddPageTitle(model, mxtrAccount);
            AddAccountInfo(model, mxtrAccount, moveToAccounts);
            AddAccountAttributesInfo(model, mxtrAccount);
            AddAccountUsersInfo(model, mxtrUsers, clients, mxtrAccount);

            return model;
        }

        public void AddPageTitle(AdminEditAccountViewModel model, mxtrAccount mxtrAccount)
        {
            model.PageTitle = "Edit Account";
            model.MainPageHeader = "Edit Account";
            model.SubPageHeader = string.Format("Edit account for {0}.", mxtrAccount.AccountName);
        }

        public void AddAccountInfo(AdminEditAccountViewModel model, mxtrAccount mxtrAccount, List<mxtrAccount> moveToAccounts)
        {
            model.AccountProfile = AccountProfileViewModelAdapter.BuildAdminEditAccountProfilePartialViewModel(mxtrAccount, moveToAccounts);
        }

        public void AddAccountAttributesInfo(AdminEditAccountViewModel model, mxtrAccount mxtrAccount)
        {
            model.AccountAttributes = AccountAttributesViewModelAdapter.BuildAdminEditAccountAttributesPartialViewModel(mxtrAccount);
        }

        public void AddAccountUsersInfo(AdminEditAccountViewModel model, List<mxtrUser> mxtrUsers, List<EZShredAccountDataModel> marketPlaces, mxtrAccount mxtrAccount)
        {
            model.AccountUsers = AccountUsersViewModelAdapter.BuildAdminEditAccountUsersPartialViewModel(mxtrAccount, mxtrUsers, marketPlaces);
        }
    }
}
