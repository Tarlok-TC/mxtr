using mxtrAutomation.Common.Codebase;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IPublicLayoutViewModelAdapter
    {
        void AddData(PublicLayoutViewModelBase model, string userAgent, mxtrAccount account);
    }

    public class PublicLayoutViewModelAdapter : IPublicLayoutViewModelAdapter
    {
        public void AddData(PublicLayoutViewModelBase model, string userAgent, mxtrAccount account)
        {
            AddBodyClass(model, userAgent);
            AddPageTitlePrefix(model);
            AddBrandingLogo(model, account);
        }
        public void AddBrandingLogo(PublicLayoutViewModelBase model, mxtrAccount account)
        {
            model.BrandingLogoURL = account.BrandingLogoURL;
        }

        public void AddPageTitlePrefix(PublicLayoutViewModelBase model)
        {
            if (EnvironmentBase.Current.Environment == EnvironmentKind.Development)
                model.PageTitle = model.PageTitle + " [[Staging]]";
        }

        public void AddBodyClass(PublicLayoutViewModelBase model, string userAgent)
        {
            model.BodyClass = GetUserAgentClass(userAgent);
        }

        public static string GetUserAgentClass(string userAgent)
        {
            string browser = string.Empty;
            string os = string.Empty;

            // DETERMINE BROWSER - ONLY IE NEEDS VERSION
            if (userAgent != null)
            {
                if (userAgent.Contains("MSIE 6")) { browser = "IE6"; }
                if (userAgent.Contains("MSIE 7")) { browser = "IE7"; }
                if (userAgent.Contains("MSIE 8")) { browser = "IE8"; }
                if (userAgent.Contains("MSIE 9")) { browser = "IE9"; }
                if (userAgent.Contains("MSIE 10")) { browser = "IE10"; }
                if (userAgent.Contains("Trident/7.0")) { browser = "IE11"; }
                if (userAgent.Contains("Firefox")) { browser = "firefox"; }
                if (userAgent.Contains("Safari")) { browser = "safari"; }
                if (userAgent.Contains("Chrome")) { browser = "chrome"; }
                if (userAgent.Contains("Opera")) { browser = "opera"; }

                //// DETERMINE OS
                if (userAgent.Contains("Windows")) { os = "win"; }
                if (userAgent.Contains("Macintosh")) { os = "mac"; }
                if (userAgent.Contains("iPad")) { os = "ipad"; }
                if (userAgent.Contains("iPhone")) { os = "iphone"; }
            }
            return !browser.IsNullOrEmpty() && !os.IsNullOrEmpty() ? browser + " " + os : string.Empty;
        }
    }
}