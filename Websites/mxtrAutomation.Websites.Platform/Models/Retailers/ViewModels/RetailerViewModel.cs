using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels
{
    public class RetailerViewModel : MainLayoutViewModelBase
    {
        public RetailerActivityReportViewData RetailerActivityReportViewData { get; set; }
        public string UpdateDataUrl { get; set; }
        public List<LeadViewData> RetailerLeads { get; set; }
        public List<RetailerOverviewLeadsSummaryViewData> LeadsData { get; set; }
        public List<RetailerOverviewSearchSummaryViewData> SearchData { get; set; }
        public List<RetailerOverviewEmailSummaryViewData> EmailData { get; set; }

        //public int TotalPageviewsLocator { get; set; }
        ////  public string TopAccountPageviewsLocator { get; set; }
        //public int TotalPageviewsLP { get; set; }
        //// public string TopAccountPageviewsLP { get; set; }
        //public long TotalLeads { get; set; }
        ////public int AverageConversionRate { get; set; }
        public bool Success { get; set; }
    }
}