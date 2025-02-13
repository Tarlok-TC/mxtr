using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CRMLeadAnalytics : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public long LeadId { get; set; }
        public int LeadScore { get; set; }
        public int? LeadScoreForToday { get; set; }
        public int LeadScoreWeighted { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreatedDate { get; set; } // SS Creation date
        public DateTime? UpdatedDate { get; set; } // SS Updation date
        public DateTime CreatedOnMXTR { get; set; }
    }
}
