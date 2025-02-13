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
using System.IO;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IAccountAttributesViewModelAdapter
    {
        AccountAttributesViewModel BuildAccountAttributesViewModel();
        AccountAttributesViewModel BuildAdminCreateAccountAttributesPartialViewModel();
        AccountAttributesViewModel BuildAdminEditAccountAttributesPartialViewModel(mxtrAccount mxtrAccount);
    }

    public class AccountAttributesViewModelAdapter : IAccountAttributesViewModelAdapter
    {
        public AccountAttributesViewModel BuildAccountAttributesViewModel()
        {
            AccountAttributesViewModel model = new AccountAttributesViewModel();

            // Add stuff here...

            return model;
        }

        public AccountAttributesViewModel BuildAdminCreateAccountAttributesPartialViewModel()
        {
            AccountAttributesViewModel model = new AccountAttributesViewModel();

            AddLinks(model, AccountActionKind.Create);

            return model;
        }

        public AccountAttributesViewModel BuildAdminEditAccountAttributesPartialViewModel(mxtrAccount mxtrAccount)
        {
            AccountAttributesViewModel model = new AccountAttributesViewModel();

            AddLinks(model, AccountActionKind.Edit);
            AddAccountAttributesInfo(model, mxtrAccount);

            return model;
        }

        public void AddLinks(AccountAttributesViewModel model, AccountActionKind accountAction)
        {
            if (accountAction == AccountActionKind.Create)
            {
                model.AccountAttributesSubmitUrl = new AdminAddAccountAttributesSubmitWebQuery();
                model.AccountAttributesSubmitText = "Save Attributes";
            }
            else
            {
                model.AccountAttributesSubmitUrl = new AdminEditAccountAttributesSubmitWebQuery();
                model.AccountAttributesSubmitText = "Update Attributes";
            }
        }

        public void AddAccountAttributesInfo(AccountAttributesViewModel model, mxtrAccount mxtrAccount)
        {
            model.ObjectID = mxtrAccount.ObjectID;
            model.MxtrAccountID = new Guid(mxtrAccount.MxtrAccountID);
            model.SharpspringSecretKey = mxtrAccount.SharpspringSecretKey;
            model.SharpspringAccountID = mxtrAccount.SharpspringAccountID;
            model.BullseyeClientId = mxtrAccount.BullseyeClientId;
            model.BullseyeAdminApiKey = mxtrAccount.BullseyeAdminApiKey;
            model.BullseyeSearchApiKey = mxtrAccount.BullseyeSearchApiKey;
            model.BullseyeLocationId = mxtrAccount.BullseyeLocationId;
            model.BullseyeThirdPartyId = mxtrAccount.BullseyeThirdPartyId;
            model.WebsiteUrl = mxtrAccount.WebsiteUrl;
            model.GoogleAnalyticsReportingViewId = mxtrAccount.GoogleAnalyticsReportingViewId;
            model.GoogleAnalyticsTimeZoneName = mxtrAccount.GoogleAnalyticsTimeZoneName;
            model.GoogleServiceAccountCredentialFile = mxtrAccount.GoogleServiceAccountCredentialFile;
            model.GoogleServiceAccountEmail = mxtrAccount.GoogleServiceAccountEmail;
            model.GAProfileName = mxtrAccount.GAProfileName;
            model.GAWebsiteUrl = mxtrAccount.GAWebsiteUrl;
            model.EZShredIP = mxtrAccount.EZShredIP;
            model.EZShredPort = mxtrAccount.EZShredPort;
            model.KlipfolioCompanyID = mxtrAccount.KlipfolioCompanyID;
            model.KlipfolioSSOSecretKey = mxtrAccount.KlipfolioSSOSecretKey;
            model.DealerId = mxtrAccount.DealerId;
            model.SSLead = mxtrAccount.Lead;
            model.SSContact = mxtrAccount.ContactMade;
            model.SSQuoteSent = mxtrAccount.ProposalSent;
            model.SSClosed = mxtrAccount.Closed;
            model.SSWonNotScheduled = mxtrAccount.WonNotScheduled;
        }
    }
}
