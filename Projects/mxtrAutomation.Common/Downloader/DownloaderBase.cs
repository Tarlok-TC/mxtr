using System;
using System.IO;
using System.Net;
using System.Xml;

namespace mxtrAutomation.Common.Downloader
{
    public abstract class DownloaderBase : IDownloader
    {
        /// <summary>
        /// Loads the web resource and returns the result as a string.
        /// </summary>
        /// <exception cref="WebException"></exception>
        /// <param name="uri"></param>
        /// <returns></returns>
        public virtual string GetString(Uri uri)
        {
            var response = GetResponse(uri);
            return response.ResponseString;
        }

        public virtual string GetString(WebResponse response)
        {
            string result;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Private method because I want to be sure to dispose of the response and stream.  
        /// According to the docs you don't have to call close on the response and stream but it doesn't hurt.
        /// Users of this function should wrap the response in a using statement to ensure the connection
        /// is closed.</remarks>
        /// <exception cref="WebException">
        ///     this is thrown if the HttpStatus code is 
        ///     returned as anything other then 200 and the response is null
        /// </exception>
        /// <exception cref="NullReferenceException">
        ///     this is thrown if the HttpStatus code is 200 and the response is null
        /// </exception>
        /// <param name="uri"></param>
        /// <returns></returns>
        public abstract DownloaderResponse GetResponse(Uri uri);

        public abstract DownloaderResponseStream GetResponseStream(Uri uri);

        public virtual DownloaderResponse GetPostResponse(Uri uri)
        {
            return GetResponse(uri);
        }

        /// <summary>
        /// Loads a stream into an XML document.  
        /// </summary>
        /// <exception cref="System.Xml.XmlException" />
        /// <exception cref="WebException" />
        /// <param name="uri"></param>
        /// <returns></returns>
        public virtual XmlDocument GetXmlDocument(Uri uri)
        {
            var doc = new XmlDocument();

            var resp = GetResponse(uri);
            doc.LoadXml(resp.ResponseString);
            return doc;
        }

        public virtual XmlTextReader GetXmlReader(Uri uri)
        {
            return new XmlTextReader(uri.ToUrl());
        }
    }

    public static class UriExtensions
    {
        public static string ToUrl(this Uri url)
        {
            if (url != null)
            {
                return string.Concat(url.Scheme, Uri.SchemeDelimiter, url.Authority, url.PathAndQuery, url.Fragment);
            }
            return null;
        }
    }
}
