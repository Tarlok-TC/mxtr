using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class CookieExtensions
    {
        public static IDictionary<string, string> AsDictionary(this HttpCookieCollection cookieCollection)
        {
            if (cookieCollection == null)
                return new Dictionary<string, string>();

            return
                cookieCollection
                    .OfType<string>()
                    .Select(cookieName => cookieCollection[cookieName])
                    .ToDictionary(cookie => cookie.Name, cookie => cookie.Value);
        }

        public static bool IsOptOut(this HttpCookieCollection cookieCollection)
        {
            if (cookieCollection == null)
                return false;

            HttpCookie cookie =
                cookieCollection
                    .OfType<string>()
                    .Select(cookieName => cookieCollection["OPT_OUT"])
                    .FirstOrDefault();

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                return true;

            return false;
        }

        public static string GetCookie(this HttpCookieCollection cookieCollection, string cookieName)
        {
            if (cookieCollection == null)
                return string.Empty;

            HttpCookie cookie =
                cookieCollection
                    .OfType<string>()
                    .Select(c => cookieCollection[cookieName])
                    .FirstOrDefault();

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                return HttpUtility.UrlDecode(cookie.Value, System.Text.Encoding.Default);

            return string.Empty;
        }

        public static void SetCookie(this HttpCookieCollection cookieCollection, string cookieName, string cookieValue, DateTime expiration)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Value = cookieValue;
            cookie.Expires = expiration;
            cookieCollection.Add(cookie);
        }
    }
}
