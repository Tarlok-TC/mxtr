using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Codebase;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewData;
using mxtrAutomation.Corporate.Data.DataModels;
using Ninject;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IMainLayoutViewModelAdapter
    {
        void AddData(MainLayoutViewModelBase model, string userAgent, mxtrUser user, mxtrAccount account, List<mxtrAccount> childAccounts);
    }

    public class MainLayoutViewModelAdapter : IMainLayoutViewModelAdapter
    {
        public void AddData(MainLayoutViewModelBase model, string userAgent, mxtrUser user, mxtrAccount account, List<mxtrAccount> childAccounts)
        {
            AddUserData(model, user);
            AddAccountData(model, account);
            AddBodyClass(model, userAgent);
            AddPageTitlePrefix(model);
            AddApplicationLogoURL(model, account);
            AddFavIcon(model, account);
            AddHomePageURL(model, account);
        }

        public void AddUserData(MainLayoutViewModelBase model, mxtrUser user)
        {
            model.FullName = user.FullName;
            model.MxtrUserID = user.MxtrUserID;
            model.Email = user.Email;
        }

        public void AddAccountData(MainLayoutViewModelBase model, mxtrAccount account)
        {
            model.AccountName = account.AccountName;
        }

        public void AddPageTitlePrefix(MainLayoutViewModelBase model)
        {
            if (EnvironmentBase.Current.Environment == EnvironmentKind.Development)
                model.PageTitle = model.PageTitle + " [[Staging]]";
        }

        public void AddBodyClass(MainLayoutViewModelBase model, string userAgent)
        {
            model.BodyClass = GetUserAgentClass(userAgent);
        }

        public static string GetUserAgentClass(string userAgent)
        {
            string browser = string.Empty;
            string os = string.Empty;

            // DETERMINE BROWSER - ONLY IE NEEDS VERSION
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

            return !browser.IsNullOrEmpty() && !os.IsNullOrEmpty() ? browser + " " + os : string.Empty;
        }
        public void AddApplicationLogoURL(MainLayoutViewModelBase model, mxtrAccount account)
        {
            model.ApplicationLogoURL = account.ApplicationLogoURL;
        }
        public void AddFavIcon(MainLayoutViewModelBase model, mxtrAccount account)
        {
            model.FavIconURL = account.FavIconURL;
        }
        public void AddHomePageURL(MainLayoutViewModelBase model, mxtrAccount account)
        {
            model.HomePageUrl = account.HomePageUrl;
        }
    }
}