using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class DashboardSummaryCampaignPerformance
    {
        public long CampaignID { get; set; }
        public string CampaignName { get; set; }
        public double OpportunityAmount { get; set; }
        public double Leads { get; set; }
        public double Score { get; set; }
        public double Rank { get; set; }
    }
}
