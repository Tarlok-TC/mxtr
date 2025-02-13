using System;

namespace mxtrAutomation.Websites.Platform.Models.Dealer.ViewData
{
    public class DealerPerformanceDetailData
    {
        public string ObjectID { get; set; }
        public string AccountObjectID { get; set; }
        public DateTime CreateDate { get; set; }
        public long LeadID { get; set; }
        public long OwnerID { get; set; }
        public long CampaignID { get; set; }
        public string CampaignName { get; set; }
        public string LeadStatus { get; set; }
        public int LeadScore { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int EventsCount { get; set; }
        // public List<EventViewData> EventsViewData { get; set; }
        public string LeadParentAccount { get; set; }
        public string EventLastTouch { get; set; }

        // public IEnumerable<SelectListItem> Clients { get; set; }
        // public IEnumerable<SelectListItem> Parent { get; set; }
        //public Dictionary<string, Tuple<string, bool, long>> ClonedAccount { get; set; }
    }
}