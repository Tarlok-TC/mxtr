using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Web.Common.Authentication;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Web.Common.Extensions;

namespace mxtrAutomation.Web.Common.HttpModules
{
    public class UrlFilterHttpModule : IHttpModule
    {
        public void ProcessRequest(HttpContext context)
        {
            string url = context.Request.Url.ToString();

            IEnumerable<QueryBase> webQueries =
                RouteTable.Routes.OfType<WebQueryRouteBase>().Select(r => r.Query);

            QueryBase query =
                webQueries.FirstOrDefault(q => q.IsMatch(new Uri(url))) ??
                webQueries.Where(q => q is IHave301Redirects).FirstOrDefault(q => q.MatchesDepricatedRoute(new Uri(url)));

            string redirectUrl =
                query.Coalesce(q => q.EndpointFilters().Select(f => f.Filter(context, query)).FirstOrDefault(s => !s.IsNullOrEmpty()));

            if (redirectUrl != null)
            {
                if (context.Request.Form.HasKeys())
                {
                    if (context.Request.Url.Query.IsNullOrEmpty())
                        redirectUrl += "?" + context.Request.Form;
                    else
                        redirectUrl += "&" + context.Request.Form;
                }

                context.Response.Redirect(redirectUrl);
            }
        }

        private void ApplicationAuthenticateRequest(object source, EventArgs e)
        {
            mxtrAutomationHttpApplication application = source as mxtrAutomationHttpApplication;
            if (application != null)
            {
                application.Application_AuthenticateRequest(source, e);

                ProcessRequest(application.Context);
            }

        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += ApplicationAuthenticateRequest;
        }

        public void Dispose() { }
    }
}
