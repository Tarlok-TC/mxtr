using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Corporate.Data.DataModels;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;

namespace mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels
{
    public class RetailersOverviewViewModel : MainLayoutViewModelBase
    {
        public int TotalGroups { get; set; }
        public int TotalRetailers { get; set; }
        public List<RetailerOverviewLeadsSummaryViewData> LeadsData { get; set; }
        public List<RetailerOverviewSearchSummaryViewData> SearchData { get; set; }
        public List<RetailerOverviewEmailSummaryViewData> EmailData { get; set; }
    }
}
