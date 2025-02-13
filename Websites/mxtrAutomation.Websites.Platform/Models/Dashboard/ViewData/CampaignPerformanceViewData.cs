using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Dashboard.ViewData
{
    public class CampaignPerformanceViewData
    {
        public string AccountName { get; set; }
        public string CampaignName { get; set; }
        public int Width { get; set; }
        public int Leads { get; set; }
        public int OpportunityAmount { get; set; }
        public double Score { get; set; }
    }
}