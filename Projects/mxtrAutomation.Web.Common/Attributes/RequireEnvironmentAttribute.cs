using System;
using System.Web;
using mxtrAutomation.Common.Codebase;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Attributes
{
    public class RequireEnvironmentAttribute : EndpointFilterAttributeBase
    {
        public EnvironmentKind Environment { get; set; }

        public override string Filter(HttpContext context, QueryBase query)
        {
            IEnvironment webEnvironment =
                ServiceLocator.Current.GetInstance<IEnvironment>();

            if (webEnvironment.Environment != Environment)
                return "/";

            return null;
        }
    }
}
