using System;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IRetailersViewModelAdapter
    {
        RetailersViewModel BuildRetailersViewModel(IEnumerable<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails,
            List<CRMRestSearchResponseLogModel> searchResponsesDate, List<CRMLeadSummaryDataModel> leadsDate, List<CRMEmailJobDataModel> emailsDate, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, IEnumerable<GASearchDataModel> gaData);
    }

    public class RetailersViewModelAdapter : IRetailersViewModelAdapter
    {
        public RetailersViewModel BuildRetailersViewModel(IEnumerable<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails,
            List<CRMRestSearchResponseLogModel> searchResponsesDate, List<CRMLeadSummaryDataModel> leadsDate, List<CRMEmailJobDataModel> emailsDate, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, IEnumerable<GASearchDataModel> gaData)
        {
            RetailersViewModel model = new RetailersViewModel();
            model.RetailersChartViewData = new RetailersChartViewData();

            AddPageTitle(model);
            AddAttributes(model, startDate, endDate, accountObjectIDs);
            BuildDataTable(model, accounts, searchResponses, leads, emails, gaData);
            BuildScoreBoxes(model);
            AddData(model, searchResponsesDate, leadsDate, emailsDate);

            return model;
        }

        public void AddAttributes(RetailersViewModel model, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.ShowWorkspaceFilter = true;
            model.ShowDateFilter = true;
            model.UpdateDataUrl = new RetailersWebQuery();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        public void AddPageTitle(RetailersViewModel model)
        {
            model.PageTitle = "Retailer Activity Report";
        }

        public void AddData(RetailersViewModel model, List<CRMRestSearchResponseLogModel> searchResponsesDate, List<CRMLeadSummaryDataModel> leadsDate, List<CRMEmailJobDataModel> emailsDate)
        {
            model.RetailersChartViewData.LeadsData = leadsDate.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewLeadsSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    LeadCount = x.LeadCount
                }).ToList();

            model.RetailersChartViewData.SearchData = searchResponsesDate.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewSearchSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    LocatorPageviews = x.LocatorPageviews,
                    LPPageviews = x.UrlClicks,
                    TotalClicks = x.UrlClicks + x.DirectionsClicks + x.MapClicks + x.MoreInfoClicks + x.EmailClicks
                }).ToList();

            model.RetailersChartViewData.EmailData = emailsDate.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewEmailSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    EmailSends = x.SendCount
                }).ToList();

        }

        public void BuildScoreBoxes(RetailersViewModel model)
        {
            model.RetailersChartViewData.TotalPageviewsLocator = model.RetailerActivityReportViewData.Sum(c => c.LocatorPageviews);
            model.RetailersChartViewData.TopAccountPageviewsLocator = model.RetailerActivityReportViewData.OrderByDescending(c => c.LocatorPageviews).Select(c => c.AccountName).FirstOrDefault();

            model.RetailersChartViewData.TotalPageviewsLP = model.RetailerActivityReportViewData.Sum(c => c.LPPageviews);
            model.RetailersChartViewData.TopAccountPageviewsLP = model.RetailerActivityReportViewData.OrderByDescending(c => c.LPPageviews).Select(c => c.AccountName).FirstOrDefault();

            model.RetailersChartViewData.TotalLeads = model.RetailerActivityReportViewData.Sum(c => c.Contacts);
            model.RetailersChartViewData.AverageConversionRate = GetAverageConversionRate(model.RetailerActivityReportViewData.Sum(c => c.LPPageviews), model.RetailerActivityReportViewData.Sum(c => c.Contacts));
        }

        public void BuildDataTable(RetailersViewModel model, IEnumerable<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails, IEnumerable<GASearchDataModel> gaData)
        {
            model.RetailerActivityReportViewData =
                accounts
                //.Where(w => w.AccountType != "Group" && w.AccountType.ToLower() != "organization" && w.IsActive)
                .Where(w => w.AccountType != "Group" && w.AccountType.ToLower() != "organization")
                //.Where(w => w.AccountType != "Group")
                .Select(r => new RetailerActivityReportViewData
                {
                    AccountName = r.AccountName,
                    AccountObjectID = r.ObjectID,
                    LocatorPageviews = GetSearchResponseData(r.ObjectID, searchResponses, SearchResonseDataKind.LocatorPageviews),
                    //LPPageviews = GetSearchResponseData(r.ObjectID, searchResponses, SearchResonseDataKind.UrlClicks),
                    LPPageviews = GetGoogleAnalyticsData(r.ObjectID, gaData, SearchResonseDataKind.UrlClicks),
                    MapClicks = GetSearchResponseData(r.ObjectID, searchResponses, SearchResonseDataKind.MapClicks),
                    FormSubmissions = 2,
                    Contacts = GetContactsCount(r.ObjectID, leads),
                    EmailsSent = GetEmailsCount(r.ObjectID, emails),
                    TotalClicks = GetSearchResponseData(r.ObjectID, searchResponses, SearchResonseDataKind.TotalClicks),
                    DirectionsClicks = GetGoogleAnalyticsData(r.ObjectID, gaData, SearchResonseDataKind.DirectionsClicks),
                    MoreInfoClicks = GetGoogleAnalyticsData(r.ObjectID, gaData, SearchResonseDataKind.MoreInfoClicks),
                    PhoneClicks = GetGoogleAnalyticsData(r.ObjectID, gaData, SearchResonseDataKind.PhoneClicks),
                    WebsiteClicks = GetGoogleAnalyticsData(r.ObjectID, gaData, SearchResonseDataKind.WebsiteClicks),
                    LogoClicks = GetGoogleAnalyticsData(r.ObjectID, gaData, SearchResonseDataKind.LogoClicks),
                    IsActive = r.IsActive,
                }).ToList();

            model.RetailersChartViewData.RetailerActivityReportViewDataMini = new RetailerActivityReportViewDataMini()
            {
                LocatorPageviews = model.RetailerActivityReportViewData.Sum(s => s.LocatorPageviews),
                LPPageviews = model.RetailerActivityReportViewData.Sum(s => s.LPPageviews),
                Contacts = model.RetailerActivityReportViewData.Sum(s => s.Contacts),
                EmailsSent = model.RetailerActivityReportViewData.Sum(s => s.EmailsSent),
                TotalClicks = model.RetailerActivityReportViewData.Sum(s => s.TotalClicks),
                DirectionsClicks = model.RetailerActivityReportViewData.Sum(s => s.DirectionsClicks),
                MapClicks = model.RetailerActivityReportViewData.Sum(s => s.MapClicks),
                MoreInfoClicks = model.RetailerActivityReportViewData.Sum(s => s.MoreInfoClicks),
                PhoneClicks = model.RetailerActivityReportViewData.Sum(s => s.PhoneClicks),
                LogoClicks = model.RetailerActivityReportViewData.Sum(s => s.LogoClicks),
                WebsiteClicks = model.RetailerActivityReportViewData.Sum(s => s.WebsiteClicks),
            };
        }

        private int GetSearchResponseData(string accountObjectID, List<CRMRestSearchResponseLogModel> searchResponses, SearchResonseDataKind dataPoint)
        {
            if (searchResponses == null)
                return 0;

            if (searchResponses.Count == 0)
                return 0;

            CRMRestSearchResponseLogModel search = searchResponses.Where(r => r.AccountObjectID == accountObjectID).FirstOrDefault();

            if (search == null)
                return 0;

            if (dataPoint == SearchResonseDataKind.LocatorPageviews)
                return search.LocatorPageviews;

            if (dataPoint == SearchResonseDataKind.UrlClicks)
                return search.UrlClicks;

            if (dataPoint == SearchResonseDataKind.DirectionsClicks)
                return search.DirectionsClicks;

            if (dataPoint == SearchResonseDataKind.MapClicks)
                return search.MapClicks;

            if (dataPoint == SearchResonseDataKind.MoreInfoClicks)
                return search.MoreInfoClicks;

            if (dataPoint == SearchResonseDataKind.EmailClicks)
                return search.EmailClicks;

            if (dataPoint == SearchResonseDataKind.TotalClicks)
                return search.UrlClicks + search.DirectionsClicks + search.MapClicks + search.MoreInfoClicks + search.EmailClicks;

            return 0;
        }

        private int GetGoogleAnalyticsData(string accountObjectID, IEnumerable<GASearchDataModel> searchResponses, SearchResonseDataKind dataPoint)
        {
            if (searchResponses == null)
                return 0;

            if (searchResponses.Count() == 0)
                return 0;

            var search = searchResponses.Where(r => r.AccountObjectID == accountObjectID).ToList();

            //GASearchDataModel search = searchResponses.Where(r => r.AccountObjectID == accountObjectID);

            if (search == null)
                return 0;

            if (dataPoint == SearchResonseDataKind.UrlClicks)
                return search.Sum(x => x.LandingPageviews);

            if (dataPoint == SearchResonseDataKind.DirectionsClicks)
                return search.Sum(x => x.DirectionsClicks);

            if (dataPoint == SearchResonseDataKind.MoreInfoClicks)
                return search.Sum(x => x.MoreInfoClicks);

            if (dataPoint == SearchResonseDataKind.PhoneClicks)
                return search.Sum(x => x.PhoneClicks);

            if (dataPoint == SearchResonseDataKind.WebsiteClicks)
                return search.Sum(x => x.WebsiteClicks);

            if (dataPoint == SearchResonseDataKind.LogoClicks)
                return search.Sum(x => x.LogoClicks);

            return 0;
        }

        private int GetAverageConversionRate(int lpCount, long leadCount)
        {
            if (lpCount == 0)
                return 0;

            return Convert.ToInt32(leadCount) / lpCount * 100;
        }

        private int GetContactsCount(string accountObjectID, List<CRMLeadSummaryDataModel> leads)
        {
            CRMLeadSummaryDataModel lead = leads.Where(l => l.AccountObjectID == accountObjectID).FirstOrDefault();

            if (lead == null)
                return 0;

            return Convert.ToInt32(lead.LeadCount);
        }

        private int GetEmailsCount(string accountObjectID, List<CRMEmailJobDataModel> emails)
        {
            List<CRMEmailJobDataModel> emailsSubset = emails.Where(l => l.AccountObjectID == accountObjectID).ToList();

            if (emailsSubset.Count() == 0)
                return 0;

            return emailsSubset.Sum(x => x.SendCount);
        }
    }

    public enum SearchResonseDataKind
    {
        LocatorPageviews,
        UrlClicks,
        DirectionsClicks,
        MapClicks,
        MoreInfoClicks,
        EmailClicks,
        LogoClicks,
        PhoneClicks,
        WebsiteClicks,
        TotalClicks
    }
}
