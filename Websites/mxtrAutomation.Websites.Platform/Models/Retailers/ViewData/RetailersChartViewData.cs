using System.Collections.Generic;

namespace mxtrAutomation.Websites.Platform.Models.Retailers.ViewData
{
    public class RetailersChartViewData
    {
        // public List<RetailerActivityReportViewDataMini> RetailerActivityReportViewDataMini { get; set; }
        public RetailerActivityReportViewDataMini RetailerActivityReportViewDataMini { get; set; }
        public int AverageConversionRate { get; set; }
        public int TotalPageviewsLocator { get; set; }
        public int TotalPageviewsLP { get; set; }
        public long TotalLeads { get; set; }
        public string TopAccountPageviewsLP { get; set; }
        public string TopAccountPageviewsLocator { get; set; }
        public IEnumerable<RetailerOverviewLeadsSummaryViewData> LeadsData { get; set; }
        public IEnumerable<RetailerOverviewSearchSummaryViewData> SearchData { get; set; }
        public IEnumerable<RetailerOverviewEmailSummaryViewData> EmailData { get; set; }
    }
}