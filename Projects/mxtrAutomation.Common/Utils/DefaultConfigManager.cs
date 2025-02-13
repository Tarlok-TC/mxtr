using System.Collections.Specialized;
using System.Configuration;

namespace mxtrAutomation.Common.Utils
{
    public class DefaultConfigManager : IConfigManager
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ConfigurationManager.ConnectionStrings; }
        }
    }
}
