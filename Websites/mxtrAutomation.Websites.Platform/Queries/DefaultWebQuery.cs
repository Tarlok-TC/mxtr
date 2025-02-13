using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    public class DefaultWebQuery : WebQueryBase
    {
        public static readonly string Route = "/default.aspx";

        public override string ToString()
        {
            return base.ToString().Replace("default.aspx/", string.Empty);
        }
    }
}