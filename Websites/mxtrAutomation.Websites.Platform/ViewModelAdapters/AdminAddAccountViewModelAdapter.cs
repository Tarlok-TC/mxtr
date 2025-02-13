using mxtrAutomation.Websites.Platform.Models.Admin.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewModels;
using Ninject;
using mxtrAutomation.Corporate.Data.DataModels;
using System.Collections.Generic;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IAdminAddAccountViewModelAdapter
    {
        AdminAddAccountViewModel BuildAdminAddAccountViewModel(string parentAccountObjectID, mxtrAccount parentMxtrAccount, List<EZShredAccountDataModel> clients);
    }

    public class AdminAddAccountViewModelAdapter : IAdminAddAccountViewModelAdapter
    {
        [Inject]
        public IAccountProfileViewModelAdapter AccountProfileViewModelAdapter { get; set; }

        [Inject]
        public IAccountAttributesViewModelAdapter AccountAttributesViewModelAdapter { get; set; }

        [Inject]
        public IAccountUsersViewModelAdapter AccountUsersViewModelAdapter { get; set; }

        public AdminAddAccountViewModel BuildAdminAddAccountViewModel(string parentAccountObjectID, mxtrAccount parentMxtrAccount, List<EZShredAccountDataModel> clients)
        {
            AdminAddAccountViewModel model = new AdminAddAccountViewModel();

            AddPageTitle(model, parentMxtrAccount);
            AddAccountCreateInfo(model, parentAccountObjectID, parentMxtrAccount);
            AddAccountAttributesInfo(model);
            AddAccountUsersInfo(model, clients);

            return model;
        }

        public void AddAccountCreateInfo(AdminAddAccountViewModel model, string parentAccountObjectID, mxtrAccount parentMxtrAccount)
        {
            model.AccountProfile = AccountProfileViewModelAdapter.BuildAdminCreateAccountProfilePartialViewModel(parentMxtrAccount);
        }

        public void AddAccountAttributesInfo(AdminAddAccountViewModel model)
        {
            model.AccountAttributes = AccountAttributesViewModelAdapter.BuildAdminCreateAccountAttributesPartialViewModel();
        }

        public void AddAccountUsersInfo(AdminAddAccountViewModel model, List<EZShredAccountDataModel> clients)
        {
            model.AccountUsers = AccountUsersViewModelAdapter.BuildAdminCreateAccountUsersPartialViewModel(clients);
        }

        public void AddPageTitle(AdminAddAccountViewModel model, mxtrAccount parentMxtrAccount)
        {
            model.PageTitle = "Create Account";
            model.MainPageHeader = "Create Account";
            model.SubPageHeader = string.Format("Create a new child account for {0}.", parentMxtrAccount.AccountName);
        }
    }
}
