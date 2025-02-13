using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class GASearchDataModel : CRMBaseDataModel
    {       
        public int BullseyeLocationId { get; set; }
        public int LandingPageviews { get; set; }
        public int LogoClicks { get; set; }
        public int WebsiteClicks { get; set; }
        public int MoreInfoClicks { get; set; }
        public int DirectionsClicks { get; set; }
        public int PhoneClicks { get; set; }
    }
}
