using System;
using System.Web;
using System.Text.RegularExpressions;
using mxtrAutomation.Web.Common.Enums;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        public static string RootUrl(this HttpRequestBase request)
        {
            return request.Url.Scheme + Uri.SchemeDelimiter + request.Url.Authority;
        }

        public static string RootUrl(this HttpRequest request)
        {
            return request.Url.Scheme + Uri.SchemeDelimiter + request.Url.Authority;
        }

        public static DeviceKind Device(this HttpRequestBase request)
        {
            if (!request.Browser.IsMobileDevice)
                return DeviceKind.Desktop;

            if (IsTablet(request.UserAgent, request.Browser.IsMobileDevice))
                return DeviceKind.Tablet;

            return DeviceKind.Phone;
        }

        public static DeviceKind Device(this HttpRequest request)
        {
            if(!request.Browser.IsMobileDevice)
                return DeviceKind.Desktop;

            if (IsTablet(request.UserAgent, request.Browser.IsMobileDevice))
                return DeviceKind.Tablet;

            return DeviceKind.Phone;
        }

        public static bool IsTablet(string userAgent, bool isMobile)
        {
            Regex r = new Regex("ipad|android|android 3.0|xoom|sch-i800|playbook|tablet|kindle|nexus");
            bool isTablet = r.IsMatch(userAgent) && isMobile;
            return isTablet;
        }
    }
}
