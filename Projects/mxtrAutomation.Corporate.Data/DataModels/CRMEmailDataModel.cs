using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class CRMEmailDataModel : CRMBaseDataModel
    {
        public long EmailID { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Thumbnail { get; set; }
    }
}
