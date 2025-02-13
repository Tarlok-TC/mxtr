using mxtrAutomation.Websites.Platform.Models.Index.ViewData;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System.Collections.Generic;

namespace mxtrAutomation.Websites.Platform.Models.Index.ViewModels
{
    public class IndexViewModel : MainLayoutViewModelBase
    {
        public int TotalRetailers { get; set; }
        public int TotalLeads { get; set; }
        public decimal AverageLead { get; set; }
        public bool Success { get; set; }
        public List<IndexActivityAccountViewData> IndexActivityAccountViewData { get; set; }
        public List<GroupLeadsViewData> GroupLeadsViewData { get; set; }
        public string UpdateDataUrl { get; set; }
    }
}
