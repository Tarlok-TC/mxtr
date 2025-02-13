using System.Collections.Specialized;
using System.Configuration;

namespace mxtrAutomation.Common.Utils
{
    public interface IConfigManager
    {
        NameValueCollection AppSettings { get; }
        ConnectionStringSettingsCollection ConnectionStrings { get; }
    }
}
