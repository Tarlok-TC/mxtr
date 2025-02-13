using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class CRMBaseDataModel
    {
        public string ObjectID { get; set; }
        public virtual string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
