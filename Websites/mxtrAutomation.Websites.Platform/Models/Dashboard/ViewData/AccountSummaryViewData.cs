using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Dashboard.ViewData
{
    public class AccountSummaryViewData
    {
        public string AccountName { get; set; }
        public double TotalActivePotentialOpportunitiesAmount { get; set; }
        public int TotalLeads { get; set; }
        public int TotalCampaigns { get; set; }
        public string LeadsUrl { get; set; }
    }
}