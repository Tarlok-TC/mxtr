using System.Collections.Generic;

namespace mxtrAutomation.Api.EZShred
{
    public class BuildingInfo
    {
        public string BuildingID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string SiteContact1 { get; set; }
        public string Phone1 { get; set; }
        public string SiteContact2 { get; set; }
        public string Phone2 { get; set; }
        public string SalesmanID { get; set; }
        public string Directions { get; set; }
        public string RoutineInstructions { get; set; }
        public string ScheduleDescription { get; set; }
        public string ScheduleFrequency { get; set; }
        public string ScheduleWeek1 { get; set; }
        public string ScheduleWeek2 { get; set; }
        public string ScheduleWeek3 { get; set; }
        public string ScheduleWeek4 { get; set; }
        public string ScheduleWeek5 { get; set; }
        public string ScheduleDOWsun { get; set; }
        public string ScheduleDOWmon { get; set; }
        public string ScheduleDOWtue { get; set; }
        public string ScheduleDOWwed { get; set; }
        public string ScheduleDOWthu { get; set; }
        public string ScheduleDOWfri { get; set; }
        public string ScheduleDOWsat { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ServiceTypeID { get; set; }
        public string RouteID { get; set; }
        public string Stop { get; set; }
        public string TimeTaken { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Notes { get; set; }
        public string BuildingTypeID { get; set; }
        public string SalesTaxRegionID { get; set; }
        public string LastServiceDate { get; set; }
        public string NextServiceDate { get; set; }
        public string ezTimestamp { get; set; }
        public string operation { get; set; }
        public string userID { get; set; }
    }
    public class BuildingInfoResult
    {
        public string Request { get; set; }
        public string status { get; set; }
        public string BuildingID { get; set; }
        public List<BuildingInfo> tblBuilding { get; set; }
    }
}
