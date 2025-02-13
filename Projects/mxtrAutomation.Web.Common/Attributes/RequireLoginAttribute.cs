using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Web.Common.Authentication;
using mxtrAutomation.Web.Common.Queries;
using WebMatrix.WebData;

namespace mxtrAutomation.Web.Common.Attributes
{
    public enum SimpleMembershipRoles
    {
        User,
        Admin,
        SuperAdmin
    }

    public class RequireLoginAttribute : EndpointFilterAttributeBase
    {
        public bool IsLoginRequired { get; set; }
        public bool IsSimpleMembership { get; set; }
        public bool RedirectBack { get; set; }

        public override string Filter(HttpContext context, QueryBase query)
        {
            Uri uri = context.Request.Url;

            string loginPath = ConfigurationManager.AppSettings["LoginPath"] ?? String.Empty;
            string returnUrl = new UriBuilder(uri.Scheme, uri.Host, uri.Port, loginPath).Uri.ToString();

            if (RedirectBack)
            {
                returnUrl += String.Format("?redirecturl={0}", Uri.EscapeDataString(uri.ToString()));
            }

            if (IsLoginRequired)
            {
                if (IsSimpleMembership)
                {
                    if (!WebSecurity.IsAuthenticated)
                    {
                        return returnUrl;
                    }

                    WebSecurity.RequireRoles(SimpleMembershipRoles.SuperAdmin.ToString().ToLower());
                }
                else
                {
                    if (context.User == null || !context.User.Identity.IsAuthenticated)
                        return returnUrl;

                    //FormsAuthenticationTicket curTicket = getAuthTicket(HttpContext.Current.Request);

                    //if (curTicket == null)
                    //    return new UriBuilder(uri.Scheme, uri.Host, uri.Port).Uri.ToString();
                }
            }

            return null;
        }

#if false
        private FormsAuthenticationTicket getAuthTicket(HttpRequest request)
        {
            FormsAuthenticationTicket retVal = null;
            HttpCookie curCookie = request.Cookies[FormsAuthentication.FormsCookieName];

            if (curCookie != null && !String.IsNullOrWhiteSpace(curCookie.Value))
            {
                retVal = FormsAuthentication.Decrypt(curCookie.Value);
            }

            return retVal;
        }
#endif
    }
}
