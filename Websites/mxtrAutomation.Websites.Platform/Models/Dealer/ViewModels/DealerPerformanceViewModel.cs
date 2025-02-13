using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Dealer.ViewModels
{
    public class DealerPerformanceViewModel : MainLayoutViewModelBase
    {
        public string UpdateDataUrl { get; set; }
        public bool Success { get; set; }
        public string LeadsCountInDealerFunnel { get; set; }
        public double AverageLeadTimeDealer { get; set; }
        public double ConversionRateDealer { get; set; }
        public List<DealerPerformanceViewData> DealerData { get; set; }
        public DatatableFooter FooterData { get; set; }
    }

    public class DealerPerformanceViewData
    {
        public string AccountName { get; set; }
        public string AccountObjectID { get; set; }
        public string LeadsCount { get; set; }
        public int ColdLeadsCount { get; set; }
        public int WarmLeadsCount { get; set; }
        public int HotLeadsCount { get; set; }
        public int HandedOffLeads { get; set; }
        public double PassOff { get; set; }
        public double AverageTimeInFunnel { get; set; }
        public double ConversionRate { get; set; }
    }

    public class DatatableFooter
    {
        public int TotalLeads { get; set; }
        public int TotalColdLead { get; set; }
        public int TotalWarmLead { get; set; }
        public int TotalHotLead { get; set; }
        public double TotalPassOf { get; set; }
        public double TotalAvgTimeFunnel { get; set; }
        public int TotalHandedOffLeads { get; set; }
    }
}