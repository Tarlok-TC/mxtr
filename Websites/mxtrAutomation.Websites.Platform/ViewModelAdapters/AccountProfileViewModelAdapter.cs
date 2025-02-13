using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Account.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Common.Attributes;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Enums;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IAccountProfileViewModelAdapter
    {
        AccountProfileViewModel BuildAccountProfileViewModel();
        //AccountProfileViewModel BuildAdminCreateAccountProfilePartialViewModel(string parentAccountObjectID);
        AccountProfileViewModel BuildAdminCreateAccountProfilePartialViewModel(mxtrAccount parentMxtrAccount);
        AccountProfileViewModel BuildAdminEditAccountProfilePartialViewModel(mxtrAccount mxtrAccount, List<mxtrAccount> moveToAccounts);
    }

    public class AccountProfileViewModelAdapter : IAccountProfileViewModelAdapter
    {
        public AccountProfileViewModel BuildAccountProfileViewModel()
        {
            AccountProfileViewModel model = new AccountProfileViewModel();

            return model;
        }

        public AccountProfileViewModel BuildAdminCreateAccountProfilePartialViewModel(mxtrAccount parentMxtrAccount)
        {
            AccountProfileViewModel model = new AccountProfileViewModel();

            model.ParentAccountObjectID = parentMxtrAccount.ObjectID; //just the object id since this is the parent
            model.ParentAccountName = parentMxtrAccount.AccountName;

            AddLinks(model, AccountActionKind.Create);
            AddAccountKinds(model);
            SetDefaultCountry(model);
            return model;
        }

        public void SetDefaultCountry(AccountProfileViewModel model)
        {
            model.Country = "USA";
        }

        public AccountProfileViewModel BuildAdminEditAccountProfilePartialViewModel(mxtrAccount mxtrAccount, List<mxtrAccount> moveToAccounts)
        {
            AccountProfileViewModel model = new AccountProfileViewModel();

            AddLinks(model, AccountActionKind.Edit);
            AddAccountInfo(model, mxtrAccount);
            AddAccountKinds(model);
            AddMoveToAccounts(model, moveToAccounts, mxtrAccount);

            return model;
        }

        public void AddLinks(AccountProfileViewModel model, AccountActionKind accountAction)
        {
            if (accountAction == AccountActionKind.Create)
            {
                model.AccountSubmitUrl = new AdminAddAccountSubmitWebQuery();
                model.AccountSubmitText = "Create Account";
                model.AccountActionKind = AccountActionKind.Create;
            }
            else
            {
                model.AccountSubmitUrl = new AdminEditAccountSubmitWebQuery();
                model.AccountSubmitText = "Save";
                model.AccountActionKind = AccountActionKind.Edit;
            }
        }

        public void AddAccountInfo(AccountProfileViewModel model, mxtrAccount mxtrAccount)
        {
            model.ObjectID = mxtrAccount.ObjectID;
            model.MxtrAccountID = new Guid(mxtrAccount.MxtrAccountID);
            model.ParentMxtrAccountID = !string.IsNullOrEmpty(mxtrAccount.ParentMxtrAccountID) ? new Guid(mxtrAccount.ParentMxtrAccountID) : Guid.Empty;
            model.ParentAccountObjectID = mxtrAccount.ParentAccountObjectID;
            model.AccountName = mxtrAccount.AccountName;
            model.StreetAddress = mxtrAccount.StreetAddress;
            model.Suite = mxtrAccount.Suite;
            model.City = mxtrAccount.City;
            model.State = mxtrAccount.State;
            model.ZipCode = mxtrAccount.ZipCode;
            model.Country = mxtrAccount.Country;
            model.Phone = mxtrAccount.Phone;
            model.AccountType = mxtrAccount.AccountType;
            //model.CreateDate = mxtrAccount.CreateDate;
            model.IsActive = mxtrAccount.IsActive;
            model.DomainName = mxtrAccount.DomainName;
            model.StoreId = mxtrAccount.StoreId;
            model.HomePageUrl = mxtrAccount.HomePageUrl;
        }

        public void AddAccountKinds(AccountProfileViewModel model)
        {
            IDictionary<AccountKind, string> mxtrAccountKinds = EnumExtensions.ToStringValueDictionary<AccountKind, DisplayAttribute>();

            model.AccountTypeOptions = mxtrAccountKinds.Select(x => new SelectListItem
            {
                Value = x.Key == AccountKind.None ? string.Empty : x.Key.ToString(),
                Text = x.Value,
                Selected = x.Key.ToString() == model.AccountType ? true : false
            }).ToList();
        }

        public void AddMoveToAccounts(AccountProfileViewModel model, List<mxtrAccount> moveToAccounts, mxtrAccount mxtrAccount)
        {
            model.MoveToAccountOptions = moveToAccounts.Select(x => new SelectListItem
            {
                Value = x.ObjectID,
                Text = x.AccountName,
                Selected = x.ObjectID == mxtrAccount.ParentAccountObjectID ? true : false
            }).ToList();
        }

    }
}
