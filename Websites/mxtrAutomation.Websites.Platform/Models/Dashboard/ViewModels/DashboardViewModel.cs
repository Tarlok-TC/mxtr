using System;
using System.Linq;
using System.Collections.Generic;using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using  mxtrAutomation.Websites.Platform.Models.Dashboard.ViewData;
using mxtrAutomation.Common.Dto;

namespace mxtrAutomation.Websites.Platform.Models.Dashboard.ViewModels
{
    public class DashboardViewModel : MainLayoutViewModelBase
    {
        public int TotalLeads { get; set; }
        public int TotalCampaigns { get; set; }
        public double OpenOpportunityValue { get; set; }
        public string ScoreBoxDataDate { get; set; }

        public double TotalLeadsDelta { get; set; }
        public string TotalLeadsDeltaArrowCss { get; set; }

        public double TotalCampaignsDelta { get; set; }
        public string TotalCampaignsDeltaArrowCss { get; set; }
        
        public double OpenOpportunityValueDelta { get; set; }
        public string OpenOpportunityValueDeltaArrowCss { get; set; }

        public LineChartDataModel LeadsChartData { get; set; }
        public LineChartDataModel CampaignsChartData { get; set; }
        public LineChartDataModel OpenOpportunityValueChartData { get; set; }

        public List<CampaignPerformanceViewData> CampaignPerformances { get; set; }
        public List<AccountSummaryViewData> AccountSummaries { get; set; }

        public string RefreshUrl { get; set; }
        public bool HasData { get; set; }
    }
}
