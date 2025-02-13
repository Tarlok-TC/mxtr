using System;
using System.Web;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class StringExtensions
    {
        public static string HtmlEncode(this string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        public static string HtmlDecode(this string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        public static string HtmlAttributeEncode(this string input)
        {
            return HttpUtility.HtmlAttributeEncode(input);
        }

        public static string UrlEncode(this string input)
        {
            return HttpUtility.UrlEncode(input);
        }

        public static string UrlDecode(this string input)
        {
            return HttpUtility.UrlDecode(input);
        }

        public static string EncodeBreaks(this string input)
        {
            return input.Replace("\r\n", "<br/>");
        }

        public static String RemoveBreaks(this string input)
        {
            return input.Replace("\r\n", " ");
        }
    }
}
