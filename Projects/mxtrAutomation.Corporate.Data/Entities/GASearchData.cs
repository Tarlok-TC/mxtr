using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class GASearchData : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public int BullseyeLocationId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int LandingPageviews { get; set; }
        public int LogoClicks { get; set; }
        public int WebsiteClicks { get; set; }
        public int MoreInfoClicks { get; set; }
        public int DirectionsClicks { get; set; }
        public int PhoneClicks { get; set; }
    }
}
