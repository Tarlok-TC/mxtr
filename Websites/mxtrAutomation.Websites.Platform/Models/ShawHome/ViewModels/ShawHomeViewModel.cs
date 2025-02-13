using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.ShawHome.ViewModels
{
    public class ShawHomeViewModel : MainLayoutViewModelBase
    {
        public string MemberCount { get; set; }
        public string ParticipatingDealerCount { get; set; }
        public string AverageLeadScore { get; set; }
        public string AveragePassToDealerDays { get; set; }
        public string AverageCreateDateToSaleDate { get; set; }
        public double PassOffRate { get; set; }
        public double ConversionRate { get; set; }
        public string UpdateDataUrl { get; set; }
        public string WonLeadCount { get; set; }
        public string LeadScoreCount { get; set; }
        public string LeadScoreMin { get; set; }
        public string LeadScoreMax { get; set; }
        public string PassOffLeadCount { get; set; }
    }
    public class DealerDetails
    {
        public List<List<double>> Coords { get; set; }
        public List<string> Names { get; set; }
        public List<string> Cities { get; set; }
        public List<int> LeadCount { get; set; }
    }
    public class ParticipatingDealerChartData
    {       
        public List<string> CreatedDate { get; set; }
        public List<int> DealerCount { get; set; }
    }
    public class DealerData
    {
        public DealerDetails Dealer { get; set; }
        public ParticipatingDealerChartData ParticipatingDealerChartData { get; set; }
        public int SouthEastDealers { get; set; }
        public int SouthEastLeads { get; set; }
        public int NorthCentralDealers { get; set; }
        public int NorthCentralLeads { get; set; }
        public int SouthCentralDealers { get; set; }
        public int SouthCentralLeads { get; set; }
        public int WestDealers { get; set; }
        public int WestLeads { get; set; }
        public int NorthEastDealers { get; set; }
        public int NorthEastLeads { get; set; }
    }
}