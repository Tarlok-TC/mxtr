using System;
using System.Collections.Specialized;
using System.Configuration;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;

namespace mxtrAutomation.Common.Utils
{
    public static class ConfigManager
    {
        private static readonly Lazy<IConfigManager> LazyConfig =
            new Lazy<IConfigManager>(() => ServiceLocator.Current.GetInstance<IConfigManager>());
       
        public static NameValueCollection AppSettings
        {
            get { return LazyConfig.Value.Coalesce(x => x.AppSettings, new NameValueCollection()); }
        }

        public static ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return LazyConfig.Value.Coalesce(x => x.ConnectionStrings, new ConnectionStringSettingsCollection()); }
        }
    }
}
