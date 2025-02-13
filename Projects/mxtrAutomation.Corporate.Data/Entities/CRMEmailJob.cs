using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CRMEmailJob : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public long EmailJobID { get; set; }
        public bool IsList { get; set; }
        public bool IsActive { get; set; }
        public long RecipientID { get; set; }
        public int SendCount { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public List<CRMEmailEventDataModel> Events { get; set; }
    }
}
