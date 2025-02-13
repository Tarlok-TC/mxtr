using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CRMDealStage : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public long DealStageID { get; set; }
        public string DealStageName { get; set; }
        public string Description { get; set; }
        public double DefaultProbability { get; set; }
        public int Weight { get; set; }
        public bool IsEditable { get; set; }
    }
}
