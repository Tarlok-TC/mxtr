using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Websites.Platform.Queries;
using System.Web;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IRetailerViewModelAdapter
    {
        RetailerViewModel BuildRetailerViewModel(List<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails,
            List<CRMRestSearchResponseLogModel> searchResponsesDate, List<CRMLeadSummaryDataModel> leadsDate, List<CRMEmailJobDataModel> emailsDate, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<CRMLeadDataModel> retailerLeads, IEnumerable<GASearchDataModel> gaData);
    }
    public class RetailerViewModelAdapter : IRetailerViewModelAdapter
    {
        public RetailerViewModel BuildRetailerViewModel(List<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails,
            List<CRMRestSearchResponseLogModel> searchResponsesDate, List<CRMLeadSummaryDataModel> leadsDate, List<CRMEmailJobDataModel> emailsDate, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<CRMLeadDataModel> retailerLeads, IEnumerable<GASearchDataModel> gaData)
        {
            RetailerViewModel model = new RetailerViewModel();

            AddPageTitle(model);
            AddAttributes(model, startDate, endDate, accountObjectIDs);
            BuildDataTable(model, accounts, searchResponses, leads, emails, gaData);
            AddRetailerLeads(model, retailerLeads);
            //BuildScoreBoxes(model, searchResponses, leads, emails);
            AddData(model, searchResponsesDate, leadsDate, emailsDate);

            return model;
        }

        public void AddPageTitle(RetailerViewModel model)
        {
            model.PageTitle = "Retailer Detail";
        }
        public void AddAttributes(RetailerViewModel model, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.ShowWorkspaceFilter = false;
            model.ShowDateFilter = true;
            model.UpdateDataUrl = new RetailerWebQuery();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        public void BuildDataTable(RetailerViewModel model, List<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails, IEnumerable<GASearchDataModel> gaData)
        {
            //var data = accounts.Where(w => w.AccountType != "Group" && w.IsActive).FirstOrDefault();
            var data = accounts.Where(w => w.AccountType != "Group").FirstOrDefault();
            //var data = accounts.Where(w => w.AccountType != "Group").FirstOrDefault();
            if (data != null)
            {
                model.RetailerActivityReportViewData = new RetailerActivityReportViewData()
                {
                    AccountName = data.AccountName,
                    AccountObjectID = data.ObjectID,
                    LocatorPageviews = GetSearchResponseData(data.ObjectID, searchResponses, SearchResonseDataKind.LocatorPageviews),
                    //LPPageviews = GetSearchResponseData(data.ObjectID, searchResponses, SearchResonseDataKind.UrlClicks),
                    LPPageviews = GetGoogleAnalyticsData(data.ObjectID, gaData, SearchResonseDataKind.UrlClicks),
                    DirectionsClicks = GetGoogleAnalyticsData(data.ObjectID, gaData, SearchResonseDataKind.DirectionsClicks),
                    MapClicks = GetSearchResponseData(data.ObjectID, searchResponses, SearchResonseDataKind.MapClicks),
                    MoreInfoClicks = GetGoogleAnalyticsData(data.ObjectID, gaData, SearchResonseDataKind.MoreInfoClicks),
                    FormSubmissions = 2,
                    Contacts = GetContactsCount(data.ObjectID, leads),
                    EmailsSent = GetEmailsCount(data.ObjectID, emails),
                    PhoneClicks = GetGoogleAnalyticsData(data.ObjectID, gaData, SearchResonseDataKind.PhoneClicks),
                    WebsiteClicks = GetGoogleAnalyticsData(data.ObjectID, gaData, SearchResonseDataKind.WebsiteClicks),
                    LogoClicks = GetGoogleAnalyticsData(data.ObjectID, gaData, SearchResonseDataKind.LogoClicks),
                    TotalClicks = GetSearchResponseData(data.ObjectID, searchResponses, SearchResonseDataKind.TotalClicks)
                };
            }
        }

        private void AddRetailerLeads(RetailerViewModel model, List<CRMLeadDataModel> retailerLeads)
        {
            model.RetailerLeads = retailerLeads.Select(l => new LeadViewData
            {
                ObjectID = l.ObjectID,
                AccountObjectID = l.AccountObjectID,
                CreateDate = l.CreateDate,
                LeadID = l.LeadID,
                OwnerID = l.OwnerID,
                CampaignID = l.CampaignID,
                // CampaignName = campaigns.Where(c => c.CampaignID == l.CampaignID).Select(c => c.CampaignName).FirstOrDefault() ?? "Unassigned",
                LeadStatus = l.LeadStatus,
                LeadScore = l.LeadScore,
                IsActive = l.IsActive,
                FirstName = l.FirstName ?? string.Empty,
                LastName = l.LastName ?? string.Empty,
                EmailAddress = l.EmailAddress ?? string.Empty,
                CompanyName = l.CompanyName ?? string.Empty,
                City = l.City ?? string.Empty,
                State = l.State ?? string.Empty,
                EventsCount = l.Events.Count(),
                //EventsViewData = l.Events.Select(e => new EventViewData
                //{
                //    CreateTimestamp = e.CreateTimestamp,
                //}).ToList(),                
                EventLastTouch = GetLastTouch(l),
                //EventLastTouch = l.Events.OrderByDescending(t => t.CreateTimestamp).FirstOrDefault()?.CreateTimestamp.ToString("yyyy-MM-dd") ?? string.Empty,
            }).Where(l => l.CreateDate >= Convert.ToDateTime(model.StartDate) && l.CreateDate <= Convert.ToDateTime(model.EndDate))
          .OrderBy(l => l.CreateDate).ToList();
        }
        private static string GetLastTouch(CRMLeadDataModel leadData)
        {
            var data = leadData.Events.OrderByDescending(t => t.CreateTimestamp).FirstOrDefault();
            if (data == null)
            {
                return string.Empty;
            }
            return data.CreateTimestamp.ToString("yyyy-MM-dd") ?? string.Empty;
        }

        public void AddData(RetailerViewModel model, List<CRMRestSearchResponseLogModel> searchResponsesDate, List<CRMLeadSummaryDataModel> leadsDate, List<CRMEmailJobDataModel> emailsDate)
        {
            model.LeadsData = leadsDate.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewLeadsSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    LeadCount = x.LeadCount
                }).ToList();

            model.SearchData = searchResponsesDate.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewSearchSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    LocatorPageviews = x.LocatorPageviews,
                    LPPageviews = x.UrlClicks,
                    TotalClicks = x.UrlClicks + x.DirectionsClicks + x.MapClicks + x.MoreInfoClicks + x.EmailClicks
                }).ToList();

            model.EmailData = emailsDate.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewEmailSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    EmailSends = x.SendCount
                }).ToList();

        }

        public void BuildScoreBoxes(RetailerViewModel model, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails)
        {
            //model.TotalPageviewsLocator = model.RetailerActivityReportViewData.LocatorPageviews;
            //// model.TopAccountPageviewsLocator = model.RetailerActivityReportViewData.OrderByDescending(c => c.LocatorPageviews).Select(c => c.AccountName).FirstOrDefault();

            //model.TotalPageviewsLP = model.RetailerActivityReportViewData.LPPageviews;
            //// model.TopAccountPageviewsLP = model.RetailerActivityReportViewData.OrderByDescending(c => c.UrlClicks).Select(c => c.AccountName).FirstOrDefault();

            //model.TotalLeads = model.RetailerActivityReportViewData.Contacts;
            // model.AverageConversionRate = GetAverageConversionRate(model.RetailerActivityReportViewData.Sum(c => c.LPPageviews), model.RetailerActivityReportViewData.Sum(c => c.Contacts));
            //model.TotalPageviewsLocator = model.RetailerActivityReportViewData.Sum(c => c.LocatorPageviews);
            //model.TopAccountPageviewsLocator = model.RetailerActivityReportViewData.OrderByDescending(c => c.LocatorPageviews).Select(c => c.AccountName).FirstOrDefault();

            //model.TotalPageviewsLP = model.RetailerActivityReportViewData.Sum(c => c.LPPageviews);
            //model.TopAccountPageviewsLP = model.RetailerActivityReportViewData.OrderByDescending(c => c.UrlClicks).Select(c => c.AccountName).FirstOrDefault();

            //model.TotalLeads = model.RetailerActivityReportViewData.Sum(c => c.Contacts);
            //model.AverageConversionRate = GetAverageConversionRate(model.RetailerActivityReportViewData.Sum(c => c.LPPageviews), model.RetailerActivityReportViewData.Sum(c => c.Contacts));
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

            //GASearchDataModel search = searchResponses.Where(r => r.AccountObjectID == accountObjectID).FirstOrDefault();
            var search = searchResponses.Where(r => r.AccountObjectID == accountObjectID).ToList();

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
}