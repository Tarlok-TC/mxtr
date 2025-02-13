using mxtrAutomation.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Enums
{
    public enum GAEventAction
    {
        [Display("Logo")]
        Logo = 1,

        [Display("Website")]
        Website = 2,

        [Display("More Info")]
        MoreInfo = 3,

        [Display("Direction")]
        Direction = 4,

        [Display("Phone Number")]
        Phone = 5,
    }
}
