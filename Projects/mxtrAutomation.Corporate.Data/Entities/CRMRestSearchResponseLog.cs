using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CRMRestSearchResponseLog : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public string AccountName { get; set; }
        public int LocatorPageviews { get; set; }
        public int UrlClicks { get; set; }
        public int EmailClicks { get; set; }
        public int MoreInfoClicks { get; set; }
        public int MapClicks { get; set; }
        public int DirectionsClicks { get; set; }
    }
}
