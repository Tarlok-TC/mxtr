using System.Text;
using System.Web;

namespace mxtrAutomation.Web.Common.Helpers
{
    public class PageLink : ViewLink
    {
        public string CompareText { get; set; }
        public new string CssClass { get; set; }
        public string Target { get; set; }
        public string Title { get; set; }
        public string Style { get; set; }
        public string ID { get; set; }

        public string Rel { get; set; }
        public string NamedAnchor { get; set; }

        public WebImage Image { get; set; }
        //public string ImageStyle { get; set; }
        //public string ImageClass { get; set; }
        //public string ImageInlineText { get; set; }

        // ADDED BY CHRIS
        public string IconClass { get; set; }
        public bool ButtonArrow { get; set; }
        // END ADD

        public PageLink(ViewLink viewLink)
        {
            if (viewLink == null)
            {
                IsNullLink = true;
            }
            else
            {
                Url = viewLink.Url;
                Text = viewLink.Text;
                Image = viewLink.Image;
                OnClick = viewLink.OnClick;
                CssClass = viewLink.CssClass;
                if (viewLink.IsExternal)
                    Target = "_blank";
            }
        }

        public PageLink() { }

        /// <summary>
        /// Used exclusively for the concept of a NullLink.  NullLink makes it easier to
        /// use PageLinks directly in the page, because a NullLink can have ToString() called
        /// on it, whereas a PageLink that is actually null can not.
        /// </summary>
        public bool IsNullLink { get; set; }

        public static PageLink NullLink
        {
            get { return new PageLink { IsNullLink = true }; }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(Url); }
        }

        public override bool IsExternal
        {
            set
            {
                base.IsExternal = value;
                Target = (base.IsExternal) ? "_blank" : string.Empty;
            }
        }

        public static implicit operator HtmlString(PageLink pageLink)
        {
            return new HtmlString(pageLink.ToString());            
        }

        public override string ToString()
        {
            return ToString(false, true, null);
        }

        public string ToString(int maxDisplayChars)
        {
            return ToString(false, true, maxDisplayChars);
        }

        public string ToString(bool shouldOnClickReturnFalse)
        {
            return ToString(shouldOnClickReturnFalse, true);
        }

        public string ToString(bool shouldOnClickReturnFalse, bool shouldHtmlEncodeText)
        {
            return ToString(shouldOnClickReturnFalse, shouldHtmlEncodeText, null);
        }

        public string ToString(bool shouldOnClickReturnFalse, bool shouldHtmlEncodeText, int? maxDisplayChars)
        {
            if (IsNullLink) return string.Empty;

            if ((string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(Text)) && Image == null)
                return string.Empty;

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(Url))
                sb.AppendFormat("<a href=\"{0}{1}\"", HttpUtility.HtmlAttributeEncode(Url), string.IsNullOrEmpty(NamedAnchor) ? "" : "#" + NamedAnchor);
            else if (Image == null)
                sb.Append("<span");

            if (!string.IsNullOrEmpty(Url) || Image == null)
            {
                if (!string.IsNullOrEmpty(ID))
                    sb.AppendFormat(" id=\"{0}\"", ID);
                if ((!string.IsNullOrEmpty(Url)) && (!string.IsNullOrEmpty(Target)))
                    sb.AppendFormat(" target=\"{0}\"", Target);
                if (!string.IsNullOrEmpty(OnClick))
                {
                    if (shouldOnClickReturnFalse && !OnClick.ToLower().EndsWith("return false;"))
                        sb.AppendFormat(" onclick=\"{0}return false;\"", HttpUtility.HtmlAttributeEncode(OnClick));
                    else
                        sb.AppendFormat(" onclick=\"{0}\"", HttpUtility.HtmlAttributeEncode(OnClick));
                }
                if (!string.IsNullOrEmpty(CssClass))
                    sb.AppendFormat(" class=\"{0}\"", CssClass);
                if (!string.IsNullOrEmpty(Title))
                    sb.AppendFormat(" title=\"{0}\"", HttpUtility.HtmlAttributeEncode(Title));
                if (!string.IsNullOrEmpty(Style))
                    sb.AppendFormat(" style=\"{0}\"", Style);
                if (!string.IsNullOrEmpty(Rel))
                    sb.AppendFormat(" rel=\"{0}\"", Rel);
                sb.Append(">");
            }

            //    string displayText = GetDisplayText(maxDisplayChars);

            if (Image != null)
                sb.Append(Image);
            else if (shouldHtmlEncodeText)
                sb.Append(HttpUtility.HtmlEncode(GetDisplayText(maxDisplayChars)));
            else
                sb.Append(GetDisplayText(maxDisplayChars));

            // ADDED BY CHRIS
            if (Image == null && !string.IsNullOrEmpty(IconClass))
                sb.AppendFormat("<span class=\"small-icon {0}\"></span>", IconClass);

            // Output button arrow
            if (ButtonArrow) 
                sb.AppendFormat("<span class=\"btn-arrow\"></span>");
            // END CHRIS

            if (!string.IsNullOrEmpty(Url))
                sb.Append("</a>");
            else if (Image == null)
                sb.Append("</span>");

            return sb.ToString();
        }

        private string GetDisplayText(int? maxDisplayChars)
        {
            string displayText = Text;
            if (!string.IsNullOrEmpty(displayText) && maxDisplayChars != null)
            {
                displayText = Text.Length > maxDisplayChars
                                ? Text.Substring(0, maxDisplayChars.Value) + "..."
                                : Text;
            }
            return displayText;
        }
    }
}
