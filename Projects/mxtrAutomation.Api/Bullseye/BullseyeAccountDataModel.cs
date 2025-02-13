using System.Collections.Generic;

namespace mxtrAutomation.Api.Bullseye
{
    public class BullseyeAccountDataModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string MobileNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactPosition { get; set; }
        public string ThirdPartyId { get; set; }
        public double? Distance { get; set; }
        public string CategoryIds { get; set; }
        //Comma-separated list of category ID's to which this location is assigned
        public string CategoryNames { get; set; }
        //Comma-separated list of category names to which this location is assigned

        //public List<RestAttribute> Attributes { get; set; }
        //List of attributes for this location.
        //Only text, numeric, and yes/no attributes are supported.
        public string CountryCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int GeoCodeStatusId { get; set; }
        //0=None, 1=Zip Level, 2=Street Level
        public bool? InternetLocation { get; set; }
        //True if this is an internet location.
        public string BusinessHours { get; set; }
        //A formatted string, representing the location’s business hours.
        public string LocationTypeName { get; set; }
        //Standard, Preferred, or Exclusive
        public string TimeZone { get; set; }
        public string FacebookPageId { get; set; }
        //Associates a Facebook page with the location.
        public string ImageFileUrl { get; set; }
        //Location thumbnail image
        //public List<RestCoupon> Coupons { get; set; }
        //List of Coupons for the location
        //public List<RestEvent> Events { get; set; }
        //List of upcoming Events which will be held at this location.
        public bool? IsStoreLocator { get; set; }
        //Indicates whether a location has the Store Locator service.Ignore if ReturnServices is not requested.
        public bool? IsLeadManager { get; set; }
        //Indicates whether a location has the Lead Manager service.Ignore if ReturnServices is not requested.
        //public List<RestDailyHours> DailyHoursList { get; set; }
        //Returns the hours for the upcoming 7 days.
        public bool IsActive { get; set; }
    }
}
