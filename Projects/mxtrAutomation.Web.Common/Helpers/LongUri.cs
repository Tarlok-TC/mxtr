using System;
using mxtrAutomation.Web.Common.Extensions;

namespace mxtrAutomation.Web.Common.Helpers
{
    public class LongUri
    {
        private readonly Uri _uri;

        public string AbsolutePath { get { return _uri.AbsolutePath; } }

        public string Query { get { return _uri.Query; } }

        public LongUri(string uriString)
        {
            _uri = new Uri(uriString);
        }

        public string ToUrl()
        {
            return _uri.ToUrl();
        }

        public string ToHost()
        {
            return _uri.ToHost();
        }
    }
}
