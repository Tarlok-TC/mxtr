using System.Collections.Generic;

namespace mxtrAutomation.Api.Bullseye
{
    public class BullseyeAccountSearchModel
    {
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? CountryId { get; set; }
        public string CategoryIds { get; set; }
        public int? Radius { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Keyword { get; set; }
        public string UserIPAddress { get; set; }
        public bool? ReturnGeocode { get; set; }
        public bool? FillAttr { get; set; }
        public bool? ReturnCoupon { get; set; }
        public bool? ReturnEvent { get; set; }
        public string SearchSourceName { get; set; }
        public int? StartIndex { get; set; }
        public int? PageSize { get; set; }
        public int? SearchTypeOverride { get; set; }
        public string CountryScope { get; set; }
        public int? MaxResults { get; set; }
        public bool? MatchAllCategories { get; set; }
        public bool? ReturnServices { get; set; }
        public bool? FindNearestForNoResults { get; set; }
        public bool? GetHoursForUpcomingWeek { get; set; }
        public string LanguageCode { get; set; }
        public int? LanguageID { get; set; }
        public int? LocationID { get; set; }

        public BullseyeAccountSearchModel() {
            CountryId = 1;
            Radius = -1;
        }
    }
}
