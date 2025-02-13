using System;
using mxtrAutomation.Web.Common.Helpers;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class ViewLinkExtensions
    {
        public static bool IsNullOrNoUrl(this ViewLink link)
        {
            return link == null || String.IsNullOrEmpty(link.Url);
        }

        public static bool IsNullOrNoText(this ViewLink link)
        {
            return link == null || String.IsNullOrEmpty(link.Text);
        }
    }
}
