using System;
using System.Net;
using System.Xml;

namespace mxtrAutomation.Common.Downloader
{
    public interface IDownloader
    {
        string GetString(Uri uri);
        string GetString(WebResponse response);
        DownloaderResponse GetResponse(Uri uri);
        DownloaderResponse GetPostResponse(Uri uri);
        DownloaderResponseStream GetResponseStream(Uri uri);
        XmlDocument GetXmlDocument(Uri uri);
        XmlTextReader GetXmlReader(Uri uri);
    }
}
