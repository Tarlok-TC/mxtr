using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewData;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Websites.Platform.Utils
{
    public interface IAccountUtils
    {
        mxtrAccount ConvertToMxtrAccountDataModel(AccountProfileViewData accountData);
        WorkspaceHierarchyViewData BuildHierarchy(mxtrAccount mxtrAccount, List<mxtrAccount> childAccounts);
    }

    public class AccountUtils : IAccountUtils
    {
        public mxtrAccount ConvertToMxtrAccountDataModel(AccountProfileViewData accountData)
        {
            return
                    new mxtrAccount
                    {
                        ObjectID = accountData.ObjectID,
                        MxtrAccountID = accountData.MxtrAccountID != Guid.Empty ? accountData.MxtrAccountID.ToString() : Guid.NewGuid().ToString(),
                        ParentMxtrAccountID = accountData.ParentMxtrAccountID != Guid.Empty ? accountData.ParentMxtrAccountID.ToString() : string.Empty,
                        ParentAccountObjectID = accountData.ParentAccountObjectID,
                        AccountName = accountData.AccountName,
                        StreetAddress = accountData.StreetAddress,
                        Suite = accountData.Suite,
                        City = accountData.City,
                        State = accountData.State,
                        ZipCode = accountData.ZipCode,
                        Country = accountData.Country,
                        Phone = accountData.Phone,
                        AccountType = accountData.AccountType,
                        CreateDate = accountData.CreateDate.ToShortDateString(),
                        IsActive = accountData.IsActive,
                        StoreId = accountData.StoreId,
                    };
        }

        public WorkspaceHierarchyViewData BuildHierarchy(mxtrAccount mxtrAccount, List<mxtrAccount> childAccounts)
        {
            var availableAccounts = childAccounts
                .Select(a => new WorkspaceHierarchyViewData
                {
                    AccountName = a.AccountName,
                    AccountObjectID = a.ObjectID,
                    ParentAccountObjectID = a.ParentAccountObjectID,
                    EditAccountUrl = new AdminEditAccountWebQuery { AccountObjectID = a.ObjectID },
                    AddChildAccountUrl = new AdminAddAccountWebQuery { ParentAccountObjectID = a.ObjectID },
                    Children = null,
                    ChildrenCount = childAccounts.Where(x => x.ParentAccountObjectID == a.ObjectID).Count(),
                    AccountType = a.AccountType
                })
                .ToList().Where(w => !string.IsNullOrEmpty(w.AccountName))
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