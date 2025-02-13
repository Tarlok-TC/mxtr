using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class CRMLeadSummaryDataModel
    {
        public string AccountObjectID { get; set; }
        public long LeadCount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
