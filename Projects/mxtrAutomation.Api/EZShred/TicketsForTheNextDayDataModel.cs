using System.Collections.Generic;

namespace mxtrAutomation.Api.EZShred
{
    public class TicketsForTheNextDayDataModel
    {
        public string BuildingID { get; set; }
        public string Ticket { get; set; }
        public string Amount { get; set; }
        public string Email { get; set; }
        public string CustomerID { get; set; }
    }
    public class TicketsForTheDayInfo
    {
        public string Request { get; set; }
        public string status { get; set; }
        public List<TicketsForTheNextDayDataModel> TicketsForTheDay { get; set; }
    }
}
