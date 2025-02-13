using mxtrAutomation.Data;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class NextDayRouteLogs : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string UserID { get; set; }
        public string MxtrUserID { get; set; }
        public string LocationName { get; set; }
        public string NextRunDate { get; set; }
        public string APIResponse { get; set; }
        public string TicketGenerateFrom { get; set; }
        public string TicketGenerateRequestDate { get; set; }
    }
}
