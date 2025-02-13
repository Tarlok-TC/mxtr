using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class mxtrAccount
    {
        public string ObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string ParentMxtrAccountID { get; set; }
        public string ParentAccountObjectID { get; set; }
        public string AccountName { get; set; }
        public string StreetAddress { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string AccountType { get; set; }
        public string ApplicationLogoURL { get; set; }
        public string BrandingLogoURL { get; set; }
        public string FavIconURL { get; set; }

        public string SharpspringSecretKey { get; set; }
        public string SharpspringAccountID { get; set; }
        public string WebsiteUrl { get; set; }

        public int BullseyeClientId { get; set; }
        public string BullseyeSearchApiKey { get; set; }
        public string BullseyeAdminApiKey { get; set; }
        public int BullseyeLocationId { get; set; }
        public string BullseyeThirdPartyId { get; set; }

        public string GoogleAnalyticsReportingViewId { get; set; }
        public string GoogleServiceAccountEmail { get; set; }
        public string GoogleServiceAccountCredentialFile { get; set; }
        public string GoogleAnalyticsTimeZoneName { get; set; }
        public string GAProfileName { get; set; }
        public string GAWebsiteUrl { get; set; }

        public string CreateDate { get; set; }
        public bool IsActive { get; set; }
        public string DomainName { get; set; }
        public string EZShredIP { get; set; }
        public string EZShredPort { get; set; }
        public string KlipfolioSSOSecretKey { get; set; }
        public string KlipfolioCompanyID { get; set; }
        public string StoreId { get; set; }
        public string HomePageUrl { get; set; }
        public string DealerId { get; set; }
        public string Lead { get; set; }
        public string ContactMade { get; set; }
        public string ProposalSent { get; set; }
        public string WonNotScheduled { get; set; }
        public string Closed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long SharpSpringShawFunnelListID { get; set; }
    }
}
