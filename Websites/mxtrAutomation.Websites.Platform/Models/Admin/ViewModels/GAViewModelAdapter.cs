using System;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Web.Common.UI;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;

namespace mxtrAutomation.Websites.Platform.Models.Admin.ViewModels
{

    public class GoogleAnalyticsViewData: MainLayoutViewModelBase
    {
        public string AccountObjectId { get; set; }
        public string CredentialFile { get; set; }
        public List<GoogleAnalyticsAccount> lstGAAccount { get; set; }
    }

    public class GoogleAnalyticsAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<GoogleAnalyticsProfile> lstGAProfile { get; set; }
    }
    public class GoogleAnalyticsProfile
    {
        public string ViewId { get; set; }
        public string WebsiteUrl { get; set; }
        public string Timezone { get; set; }
        public string UserName { get; set; }
        public string PropertyId { get; set; }
        public string ViewName { get; set; }
    }
}