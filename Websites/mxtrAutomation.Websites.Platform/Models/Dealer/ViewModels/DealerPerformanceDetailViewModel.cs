using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Dealer.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Dealer.ViewModels
{
    public class DealerPerformanceDetailViewModel : MainLayoutViewModelBase
    {
        public DealerPerformanceViewData DealerDetail { get; set; }
        public bool Success { get; set; }
        public List<DealerPerformanceDetailData> DealerLeads { get; set; }
    }    
}