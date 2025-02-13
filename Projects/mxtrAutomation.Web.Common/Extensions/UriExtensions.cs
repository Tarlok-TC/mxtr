using System;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class UriExtensions
    {
        public static string ToUrl(this Uri url)
        {
            if (url != null)
            {
                return string.Concat(url.Scheme, Uri.SchemeDelimiter, url.Authority, url.PathAndQuery, url.Fragment);
            }
            return null;
        }

        public static string ToUrlWithoutQueryString(this Uri url)
        {
            if (url != null)
            {
                return string.Concat(url.Scheme, Uri.SchemeDelimiter, url.Authority, string.Join("", url.Segments));
            }
            return null;
        }

        public static string ToSecureUrl(this Uri url)
        {
            if (url != null)
            {
                return string.Concat(Uri.UriSchemeHttps, Uri.SchemeDelimiter, url.Authority, url.PathAndQuery, url.Fragment);
            }
            return null;
        }

        public static string ToHost(this Uri url)
        {
            if (url != null)
            {
                return string.Concat(url.Scheme, Uri.SchemeDelimiter, url.Authority);
            }
            return null;
        }

        public static Uri BasePath(this Uri url)
        {
            if (url != null)
            {
                return new Uri(string.Concat(url.Scheme, Uri.SchemeDelimiter, url.Authority));
            }
            return null;
        }

        public static string Path(this Uri url)
        {
            if (url != null)
            {
                return string.Join("", url.Segments);
            }
            return null;
        }

        public static bool TryParse(string url, out Uri parsedUri)
        {
            bool success;
            try
            {
                parsedUri = new Uri(url);
                success = true;
            }
            catch
            {
                parsedUri = null;
                success = false;
            }
            return success;
        }
    }
}
