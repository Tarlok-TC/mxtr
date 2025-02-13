using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class DashboardSummaryDetail
    {
        public DateTime Date { get; set; }
        public int ActivePotentialOpportunitiesCount { get; set; }
        public double ActivePotentialOpportunitiesAmount { get; set; }
        public double TotalActivePotentialOpportunitiesAmount { get; set; }
        public int WonOpportunitiesCount { get; set; }
        public double WonOpportunitiesAmount { get; set; }
        public int ClosedOpportunitiesCount { get; set; }
        public double ClosedOpportunitiesAmount { get; set; }
        public int TotalLeads { get; set; }
        public int NewLeads { get; set; }
        public int UpdatedLeads { get; set; }
        public int TotalCampaigns { get; set; }
        public int NewCampaigns { get; set; }
        public int UpdatedCampaigns { get; set; }
    }
}
