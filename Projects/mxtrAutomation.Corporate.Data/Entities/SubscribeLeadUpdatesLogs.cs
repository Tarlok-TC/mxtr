using mxtrAutomation.Data;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class SubscribeLeadUpdatesLogs : Entity
    {
        public string AccountObjectID { get; set; }
        public string LocationName { get; set; }
        public string Action { get; set; }//Create/Update
        public string Status { get; set; }//Complete/Failed
        public string RequestFrom { get; set; }//Sharpspring/Testing
        public string SSPostRequestString { get; set; }
        public string LogCreateDate { get; set; }
    }
}
