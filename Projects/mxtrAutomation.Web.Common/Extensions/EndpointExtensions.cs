using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class EndpointExtensions
    {
        public static IEnumerable<EndpointFilterAttributeBase> EndpointFilters(this QueryBase query)
        {
            return
                query
                    .GetType()
                    .GetCustomAttributes(true)
                    .OfType<EndpointFilterAttributeBase>();
        }

        public static IEnumerable<EndpointFilterAttributeBase> EndpointFilters(this ControllerBase controller, string methodName)
        {
            var controllerFilters =
                controller.GetType()
                    .GetCustomAttributes(true)
                    .OfType<EndpointFilterAttributeBase>();

            var methodFilters =
                controller.GetType()
                    .GetMethod(methodName)
                    .GetCustomAttributes(true)
                    .OfType<EndpointFilterAttributeBase>();

            return controllerFilters.Union(methodFilters).Distinct();
        }
    }
}
