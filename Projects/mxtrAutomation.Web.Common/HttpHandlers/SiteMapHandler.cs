using System;
using System.IO;
using System.Web;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Web.Common.Services;

namespace mxtrAutomation.Web.Common.HttpHandlers
{
    public class SiteMapHandler : IHttpHandler
    {
        public const string SiteMapPath = "sitemap.xml";

        protected ISeoService SeoService { get; set; }
        protected ICacheService CacheService { get; set; }

        public SiteMapHandler()
        {
            SeoService = ServiceLocator.Current.GetInstance<ISeoService>();

            if (SeoService == null)
                throw new InvalidOperationException("ISeoService creation failed in call to service locator.");

            CacheService = ServiceLocator.Current.GetInstance<ICacheService>();

            if (CacheService == null)
                throw new InvalidOperationException("ICacheService creation failed in call to service locator.");
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Items["_handled"] = true;

            string siteMapXml = CacheService.GetFromCache<string>(SiteMapPath);

            if (siteMapXml == null)
            {
                siteMapXml = SeoService.BuildSiteMapXml();

                CacheService.AddToAbsoluteCache(SiteMapPath, siteMapXml, DateTime.Now.AddHours(8));
            }

            using (StreamWriter outputWriter = new StreamWriter(context.Response.OutputStream))
            {
                outputWriter.Write(siteMapXml);
            }

            context.Response.ContentType = "text/xml";
            context.Response.CacheControl = "public";
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
        }

        /// <summary>
        /// IsReusable implementation returns true since this class is stateless
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
    }
}
