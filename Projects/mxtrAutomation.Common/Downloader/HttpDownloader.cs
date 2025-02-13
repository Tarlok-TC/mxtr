using System;
using System.IO;
using System.Net;
using System.Text;

namespace mxtrAutomation.Common.Downloader
{
    /// <summary>
    /// Just loads the results of a REST GET query and loads it into a generic XML Document for further processing.
    /// This class currently does no error handling at all
    /// </summary>
    /// <remarks>There is information that can be gleaned when a query fails by looking at either an error document,
    /// or examining the HTTP Header codes.  The Error Document should just be returned to the caller, but error codes
    /// could be placed into a more helpful exception object, and then passed up the chain.
    /// </remarks>
    public class HttpDownloader : DownloaderBase
    {

        public override DownloaderResponseStream GetResponseStream(Uri uri)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.KeepAlive = false;
            req.Proxy = null;

            HttpWebResponse response;
            DownloaderResponseStream downloaderResponseStream;

            try
            {
                response = (HttpWebResponse)req.GetResponse();

                downloaderResponseStream = new DownloaderResponseStream(response.StatusCode, response.GetResponseStream());
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    throw;

                response = (HttpWebResponse)ex.Response;

                downloaderResponseStream = new DownloaderResponseStream(response.StatusCode, response.GetResponseStream());
            }

            if (response == null || downloaderResponseStream == null)
                throw new NullReferenceException("Response was null with status code of 200");

            return downloaderResponseStream;
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
        public override DownloaderResponse GetResponse(Uri uri)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.KeepAlive = false;
            req.Proxy = null;

            HttpWebResponse response;
            DownloaderResponse downloaderResponse;
            string result;
            try
            {
                response = (HttpWebResponse)req.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                downloaderResponse = new DownloaderResponse(response.StatusCode, result);
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    throw;

                response = (HttpWebResponse)ex.Response;

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                downloaderResponse = new DownloaderResponse(response.StatusCode, result);
            }

            if (response == null || downloaderResponse == null)
                throw new NullReferenceException("Response was null with status code of 200");

            return downloaderResponse;
        }

        public override DownloaderResponse GetPostResponse(Uri uri)
        {
            byte[] encodedData = Encoding.UTF8.GetBytes(uri.Query);

            WebRequest request = WebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = encodedData.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(encodedData, 0, encodedData.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            string result = string.Empty;

            if (responseStream != null)
            {
                using (StreamReader sr = new StreamReader(responseStream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return new DownloaderResponse(response.StatusCode, result);
        }
    }
}
