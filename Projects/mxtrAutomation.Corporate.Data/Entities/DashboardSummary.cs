using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class DashboardSummary : Entity
    {
        public string AccountObjectID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public List<DashboardSummaryDetail> Daily { get; set; }
        public List<DashboardSummaryCampaignPerformance> CampaignPerformance { get; set; }
    }
}
