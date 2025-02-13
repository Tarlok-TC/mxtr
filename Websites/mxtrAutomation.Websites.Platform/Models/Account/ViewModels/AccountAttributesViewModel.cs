using System;
using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewModels
{
    public class AccountAttributesViewModel : ViewModelBase
    {
        public string ObjectID { get; set; }
        public Guid MxtrAccountID { get; set; }
        public string SharpspringSecretKey { get; set; }
        public string SharpspringAccountID { get; set; }
        public string WebsiteUrl { get; set; }
        public string AccountAttributesSubmitUrl { get; set; }
        public string AccountAttributesSubmitText { get; set; }
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
        public string EZShredIP { get; set; }
        public string EZShredPort { get; set; }
        public string KlipfolioSSOSecretKey { get; set; }
        public string KlipfolioCompanyID { get; set; }
        public string DealerId { get; set; }
        public string SSLead { get; set; }
        public string SSContact { get; set; }
        public string SSQuoteSent { get; set; }
        public string SSWonNotScheduled { get; set; }
        public string SSClosed { get; set; }

        public bool IsConnectedToGoogleAnalytics()
        {
            if (!string.IsNullOrEmpty(this.GoogleAnalyticsReportingViewId) && !string.IsNullOrEmpty(this.GoogleServiceAccountEmail) && !string.IsNullOrEmpty(this.GoogleServiceAccountCredentialFile))
                return true;
            return false;
        }
    }
}
