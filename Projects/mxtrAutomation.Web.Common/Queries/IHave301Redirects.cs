using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.Queries
{
    public interface IHave301Redirects
    {
        IEnumerable<string> DeprecatedRoutes { get; }
    }
}
