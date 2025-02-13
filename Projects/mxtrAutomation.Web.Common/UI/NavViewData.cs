using System.Collections.Generic;
using mxtrAutomation.Web.Common.Helpers;

namespace mxtrAutomation.Web.Common.UI
{
    public class NavViewData
    {
        public ViewLink Link { get; set; }
        public IEnumerable<NavViewData> SubLinks { get; set; }
        public bool IsSelected { get; set; }
    }
}
