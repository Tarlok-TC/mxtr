using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class MinerRunAuditTrail : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public List<MinerRunMinerDetails> MinerRunDetails { get; set; }
    }
}
