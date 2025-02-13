using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class MinerRunAuditTrailDataModel
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public List<MinerRunMinerDetails> MinerRunDetails { get; set; }
    }
}
