using System;
using System.Xml.Linq;
using mxtrAutomation.Web.Common.Extensions;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Services
{
    public class SiteMapUrl
    {
        public WebQueryBase Query { get; private set; }
        public DateTime? LastModifiedDate { get; set; }
        public SiteMapUrlFrequencyKind? ChangeFrequency { get; set; }
        public double? Priority { get; set; }

        public SiteMapUrl(WebQueryBase query)
        {
            Query = query;
        }

        public string Location
        {
            get { return Query.MakeAbsolute(false); }
        }

        public override string ToString()
        {
            return ((XElement)this).ToString();
        }

        public static implicit operator XElement(SiteMapUrl siteMapUrl)
        {
            XElement xElement = new XElement("url", new XElement("loc", siteMapUrl.Location));

            if (siteMapUrl.LastModifiedDate.HasValue)
                xElement.Add(new XElement("lastmod", siteMapUrl.LastModifiedDate.Value.ToString("yyyy-MM-dd")));

            if (siteMapUrl.ChangeFrequency.HasValue)
                xElement.Add(new XElement("changefreq", siteMapUrl.ChangeFrequency.Value.ToString().ToLower()));

            if (siteMapUrl.Priority.HasValue)
                xElement.Add(new XElement("priority", siteMapUrl.Priority));

            return xElement;
        }
    }
}
