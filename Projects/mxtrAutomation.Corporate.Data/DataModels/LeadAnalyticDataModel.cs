using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class LeadAnalyticDataModel
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public long LeadId { get; set; }
        public int LeadScore { get; set; }
        public int? LeadScoreForToday { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedOnMXTR { get; set; }
    }
}
