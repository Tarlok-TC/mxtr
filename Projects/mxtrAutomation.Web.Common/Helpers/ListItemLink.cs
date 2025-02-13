using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Web.Common.Helpers
{
    public class ListItemLink : PageLink
    {
        public string Anchor
        {
            get
            {
                string cssClass = CssClass;

                CssClass = string.Empty;
                string anchor = base.ToString();
                CssClass = cssClass;

                return anchor;
            }
        }

        public string AnchorWithCssClass
        {
            get
            {
                base.CssClass = CssClass;
                string anchor = base.ToString();
                return anchor;
            }
        }

        public ListItemLink() { }

        public ListItemLink(ViewLink viewLink) : base(viewLink) { }

        public override string ToString()
        {
            return "<li class=\"{0}\">{1}</li>".With(CssClass, Anchor);
        }

        public string ToAnchorHasCssClassString()
        {
            return "<li>{0}</li>".With(AnchorWithCssClass);
        }
    }
}
