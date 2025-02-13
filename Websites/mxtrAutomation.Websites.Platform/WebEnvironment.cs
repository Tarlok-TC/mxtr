using System.Configuration;
using mxtrAutomation.Common.Codebase;

namespace mxtrAutomation.Websites.Platform
{
    public class WebEnvironment : EnvironmentBase
    {
        public WebEnvironment()
        {
            Environment = ConfigurationManager.AppSettings["Environment"] == "Production" ? EnvironmentKind.Production : EnvironmentKind.Development;
        }
    }
}