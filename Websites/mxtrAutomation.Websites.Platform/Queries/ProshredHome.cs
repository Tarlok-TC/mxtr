using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class ProshredHomeWebQuery : WebQueryBase
    {
        public static readonly string Route = "/Proshred/Home";
    }
}
