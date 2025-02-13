using System;
using System.Net;

namespace mxtrAutomation.Common.Downloader
{
    [Serializable]
    public class DownloaderResponse
    {
        public DownloaderResponse(HttpStatusCode httpStatusCode, string response)
        {
            StatusCode = httpStatusCode;
            ResponseString = response;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string ResponseString { get; set; }
    }
}
