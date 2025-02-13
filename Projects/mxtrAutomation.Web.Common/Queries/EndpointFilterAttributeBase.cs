using System;
using System.Web;

namespace mxtrAutomation.Web.Common.Queries
{
    public abstract class EndpointFilterAttributeBase : Attribute
    {
        /// <summary>
        /// Called by the query routing engine when an endpoint is requested.  Allows endpoint filters
        /// to perform a redirect.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>True if handled here</returns>
        public abstract string Filter(HttpContext context, QueryBase query);
    }
}
