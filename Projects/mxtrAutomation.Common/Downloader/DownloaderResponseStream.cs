using System;
using System.IO;
using System.Net;

namespace mxtrAutomation.Common.Downloader
{
    [Serializable]
    public class DownloaderResponseStream
    {
        public DownloaderResponseStream(HttpStatusCode httpStatusCode, Stream response)
        {
            StatusCode = httpStatusCode;
            ResponseStream = response;
        }

        public HttpStatusCode StatusCode { get; set; }

        public Stream ResponseStream { get; set; }

        public string GetRawResponseString()
        {
            using (var sr = new StreamReader(ResponseStream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
