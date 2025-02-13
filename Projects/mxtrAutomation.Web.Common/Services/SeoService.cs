using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;

namespace mxtrAutomation.Web.Common.Services
{
    public enum SiteMapUrlFrequencyKind
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }

    public class SeoService : ISeoServiceInternal
    {
        public string BuildSiteMapXml()
        {
            XNamespace dc = "http://www.sitemaps.org/schemas/sitemap/0.9";

            IEnumerable<XElement> siteMapUrls =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(typeof(Controller).IsAssignableFrom)
                    .Where(typeof(IHaveSeoInfo).IsAssignableFrom)
                    .Select(type => ServiceLocator.Current.GetInstance(type) as IHaveSeoInfo)
                    .Select(c => c.GetSeoInfo())
                    .SelectMany(i => i.SiteMapUrls)
                    .Select(u => (XElement)u);

            XElement siteMap =
                new XElement(dc + "urlset", siteMapUrls);

            return siteMap.ToXmlString(new XmlWriterSettings { Encoding = Encoding.UTF8, OmitXmlDeclaration = false, Indent = false });
        }
    }
}
