using System;
using System.Web;
using mxtrAutomation.Common.Codebase;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Attributes
{
    public class SecureEndpointAttribute : EndpointFilterAttributeBase
    {
        //private static readonly bool UsingSecureUrl = ConfigurationManager.AppSettings["SecureRootUrl"].Contains("https");

        private static readonly bool UsingSecureUrl =
            ServiceLocator.Current.GetInstance<IEnvironment>().Environment == EnvironmentKind.Production;

        public bool IsSecureConnection { get; set; }

        public override string Filter(HttpContext context, QueryBase query)
        {
            Uri uri = context.Request.Url;
            //Uri tempUri;

            if (IsSecureConnection && !context.Request.IsLocal && UsingSecureUrl && uri.Scheme == "http")
            {
                //remove port
                //tempUri = new UriBuilder("https", uri.Host, uri.Port, uri.PathAndQuery).Uri;
                //return tempUri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped).ToString();

                return new UriBuilder("https", uri.Host, -1, uri.PathAndQuery).Uri.ToString();
                //return new UriBuilder("https", uri.Host, -1, uri.PathAndQuery).Uri.ToString();

                //original
                //return new UriBuilder("https", uri.Host, uri.Port, uri.PathAndQuery).Uri.ToString();
            }

            if (!IsSecureConnection && !context.Request.IsLocal && uri.Scheme == "https")
            {
                //remove port
                //tempUri = new UriBuilder("http", uri.Host, uri.Port, uri.PathAndQuery).Uri;
                //return tempUri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped).ToString();
                return new UriBuilder("http", uri.Host, -1, uri.PathAndQuery).Uri.ToString();
                
                //original
                //return new UriBuilder("http", uri.Host, uri.Port, uri.PathAndQuery).Uri.ToString();
            }

            return null;
        }
    }
}
