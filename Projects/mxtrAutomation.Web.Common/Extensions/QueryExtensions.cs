using System;
using System.Collections.Specialized;
using System.Web;
using mxtrAutomation.Web.Common.Helpers;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class QueryExtensions
    {
        public static T From<T>(this T query, Uri uri)
            where T : QueryBase
        {
            query.Deserialize(uri);
            return query;
        }

        public static T From<T>(this T query, Uri uri, NameValueCollection requestForm)
            where T : QueryBase
        {
            query.Deserialize(uri, requestForm);
            return query;
        }

        public static ViewLink<T> AsViewLink<T>(this T query)
            where T : QueryBase
        {
            return new ViewLink<T>(query);
        }

        public static ViewLink<T> AsViewLink<T>(this T query, string text)
            where T : QueryBase
        {
            return new ViewLink<T>(query) { Text = text };
        }

        public static ViewLink<T> AsViewLink<T>(this T query, string text, string cssClass)
            where T : QueryBase
        {
            return new ViewLink<T>(query) { Text = text, CssClass = cssClass };
        }
        
        public static string Route(this QueryBase query)
        {
            return query.GetType().GetField("Route").GetValue(query) as string;            
        }

        public static ViewLink AsExternal(this ViewLink viewLink)
        {            
            viewLink.IsExternal = true;
            return viewLink;
        }

        public static string MakeAbsolute(this QueryBase query, bool isSecure)
        {
            return MakeAbsolute(query.ToString(), isSecure);
        }

        public static string MakeAbsolute(this QueryBase query, bool isSecure, string overrideUrl, int overridePort)
        {
            return MakeAbsolute(query.ToString(), isSecure, overrideUrl, overridePort);
        }

        public static string MakeAbsolute(string relativePath, bool isSecure, string url, int port)
        {
            var urlString = relativePath;
            if (url != null)
            {
                var ub = new UriBuilder { Host = url, Port = port };
                var uri = new Uri(ub.Uri, urlString);
                urlString = uri.ToUrl();
            }
            return urlString;
        }

        public static string MakeAbsolute(string relativePath, bool isSecure)
        {
            var urlString = relativePath;
            var baseUri = GetBaseUri(isSecure);
            if (baseUri != null)
            {
                var uri = new Uri(baseUri, urlString);
                urlString = uri.ToUrl();
            }
            return urlString;
        }

        public static string MakeAbsolute(this AdTrakWebImage image, bool isSecure, string overrideUrl, int overridePort)
        {
            return MakeAbsolute(image.Url, isSecure, overrideUrl, overridePort);
        }

        public static string GetAbsoluteUrl(this AdTrakWebImage image)
        {
            return image.GetAbsoluteUrl(false);
        }

        public static string GetAbsoluteUrl(this AdTrakWebImage image, bool secure)
        {
            return MakeAbsolute(image.Url, secure);
        }

        private static Uri GetBaseUri(bool isSecure)
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return null;

            string host = HttpContext.Current.Request.Url.Host;

            if (!string.IsNullOrEmpty(host))
            {
                var ub = new UriBuilder { Host = host };
                if (isSecure)
                    ub.Scheme = Uri.UriSchemeHttps;

                // Set the port if it's not the default
                if (HttpContext.Current.Request.Url != null)
                {
                    if (HttpContext.Current.Request.Url.Port != 80 && HttpContext.Current.Request.Url.Port != 443)
                        ub.Port = HttpContext.Current.Request.Url.Port;
                }

                return ub.Uri;
            }
            return null;
        }
    }
}
