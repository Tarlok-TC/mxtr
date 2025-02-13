using System;
using System.Linq;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;
using mxtrAutomation.Common.Dto;

namespace mxtrAutomation.Websites.Platform.Models.Leads.ViewModels
{
    public class LeadsViewModel : MainLayoutViewModelBase
    {
        public IEnumerable<LeadViewData> Leads { get; set; }
        public List<string> AccountObjectIDs { get; set; }
        public IEnumerable<LeadsChartViewData> LeadsChartViewData { get; set; }
        public bool Success { get; set; }
        public string UpdateDataUrl { get; set; }
    }
    public class ShawLeadsViewModel : MainLayoutViewModelBase
    {
        public IEnumerable<ShawLeadViewData> Leads { get; set; }
        public List<string> AccountObjectIDs { get; set; }
        public IEnumerable<LeadsChartViewData> LeadsChartViewData { get; set; }
        public bool Success { get; set; }
        public string UpdateDataUrl { get; set; }
    }
    public class ShawDealerLeadsViewModel : MainLayoutViewModelBase
    {
        public IEnumerable<ShawDealerLeadViewData> Leads { get; set; }
        public List<string> AccountObjectIDs { get; set; }
        public IEnumerable<LeadsChartViewData> LeadsChartViewData { get; set; }
        public bool Success { get; set; }
        public string UpdateDataUrl { get; set; }
    }
}
