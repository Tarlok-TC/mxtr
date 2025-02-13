using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels
{
    public class RetailersViewModel : MainLayoutViewModelBase
    {
        public List<RetailerActivityReportViewData> RetailerActivityReportViewData { get; set; }
        public RetailersChartViewData RetailersChartViewData { get; set; }
        public string UpdateDataUrl { get; set; }
        public bool Success { get; set; }
    }
}
