using System.Web;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Attributes
{
    public class CanonicalAttribute : EndpointFilterAttributeBase
    {
        public override string Filter(HttpContext context, QueryBase query)
        {
            if (context != null && query != null)
            {
                string expectedUrl = query.ToString();

                if (StripQueryString(context.Request.Url.AbsolutePath) != StripQueryString(expectedUrl))
                    return expectedUrl;
            }

            return null;
        }

        protected string StripQueryString(string url)
        {
            int idx = url.IndexOf('?');

            return idx > -1 ? url.Substring(0, idx) : url;
        }
    }
}
