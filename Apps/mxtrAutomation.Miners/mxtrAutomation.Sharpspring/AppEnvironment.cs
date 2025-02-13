using System.Configuration;
using mxtrAutomation.Common.Codebase;

namespace mxtrAutomation.Sharpspring
{
    public class AppEnvironment : EnvironmentBase
    {
        public AppEnvironment()
        {
            Environment = ConfigurationManager.AppSettings["Environment"] == "Production" ? EnvironmentKind.Production : EnvironmentKind.Development;
        }
    }
}
