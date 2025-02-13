using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.Helpers
{
    public class SelectableViewLink : ViewLink
    {
        public bool IsActive { get; set; }

        public override string CssClass
        {
            get { return base.CssClass + (IsActive ? " selected" : string.Empty); }
        }
    }
}
