using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class CRMRestSearchResponseLogModel : CRMBaseDataModel
    {
        public int LocationID { get; set; }
        public override string AccountObjectID { get; set; }
        public override DateTime CreateDate { get; set; }
        public string AccountName { get; set; }
        public int LocatorPageviews { get; set; }
        public int UrlClicks { get; set; }
        public int EmailClicks { get; set; }
        public int MoreInfoClicks { get; set; }
        public int MapClicks { get; set; }
        public int DirectionsClicks { get; set; }
    }
}
