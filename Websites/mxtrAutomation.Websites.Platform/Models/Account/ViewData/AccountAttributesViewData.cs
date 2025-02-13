using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewData
{
    public class AccountAttributesViewData
    {
        public string ObjectID { get; set; }
        public Guid MxtrAccountID { get; set; }
        public string SharpspringSecretKey { get; set; }
        public string SharpspringAccountID { get; set; }
        public string WebsiteUrl { get; set; }
        public int BullseyeClientId { get; set; }
        public string BullseyeAdminApiKey { get; set; }
        public string BullseyeSearchApiKey { get; set; }
        public int BullseyeLocationId { get; set; }
        public string BullseyeThirdPartyId { get; set; }
        public string GoogleAnalyticsReportingViewId { get; set; }
        public string GoogleServiceAccountEmail { get; set; }
        public string GoogleServiceAccountCredentialFile { get; set; }
        public string GoogleAnalyticsTimeZoneName { get; set; }
        public string GAProfileName { get; set; }
        public string GAWebsiteUrl { get; set; }
        public string KlipfolioSSOSecretKey { get; set; }
        public string KlipfolioCompanyID { get; set; }
        public string EZShredIP { get; set; }
        public string EZShredPort { get; set; }
        public string DealerId { get; set; }
        public string SSLead { get; set; }
        public string SSContact { get; set; }
        public string SSQuoteSent { get; set; }
        public string SSWonNotScheduled { get; set; }
        public string SSClosed { get; set; }
    }
}