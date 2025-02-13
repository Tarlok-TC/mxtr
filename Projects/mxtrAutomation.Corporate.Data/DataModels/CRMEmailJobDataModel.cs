using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class CRMEmailJobDataModel : CRMBaseDataModel
    {
        public string AccountObjectID { get; set; }
        public long EmailJobID { get; set; }
        public bool IsList { get; set; }
        public bool IsActive { get; set; }
        public long RecipientID { get; set; }
        public int SendCount { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public List<CRMEmailEventDataModel> Events { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
