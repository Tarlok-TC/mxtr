using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.Services
{
    public class SeoInfo
    {
        public IEnumerable<SiteMapUrl> SiteMapUrls { get; set; }

        public SeoInfo() { }

        public SeoInfo(SiteMapUrl singleSiteMapUrl)
        {
            SiteMapUrls = new[] { singleSiteMapUrl };
        }
    }
}
