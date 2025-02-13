using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class DashboardSummaryDataModel
    {
        public string ObjectID { get; set; }
        public string AccountObjectID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public List<DashboardSummaryDetail> Daily { get; set; }
        public List<DashboardSummaryCampaignPerformance> CampaignPerformance { get; set; }
    }
}
