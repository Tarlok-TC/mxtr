using System.Collections.Generic;

namespace mxtrAutomation.Websites.Platform.Models.Email.ViewData
{
    public class EmailChartViewData
    {
        public IEnumerable<EmailJobStatsViewData> EmailJobStatsViewData { get; set; }
        //public IEnumerable<EmailActivityViewDataMini> EmailActivityViewDataMini { get; set; }
        public EmailActivityViewDataMini EmailActivityViewDataMini { get; set; }
        public long TotalEmailSends { get; set; }
        public long TotalEmailOpens { get; set; }
        public long TotalEmailClicks { get; set; }
        public double OverallOpenRate { get; set; }
        public double OverallClickRate { get; set; }
    }
}