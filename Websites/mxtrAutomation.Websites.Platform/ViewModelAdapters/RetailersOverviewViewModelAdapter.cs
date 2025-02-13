using System;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IRetailersOverviewViewModelAdapter
    {
        RetailersOverviewViewModel BuildRetailersOverviewViewModel(List<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails, DateTime startDate, DateTime endDate, List<string> accountObjectIDs);
    }

    public class RetailersOverviewViewModelAdapter : IRetailersOverviewViewModelAdapter
    {
        public RetailersOverviewViewModel BuildRetailersOverviewViewModel(List<mxtrAccount> accounts, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            RetailersOverviewViewModel model = new RetailersOverviewViewModel();        

            AddPageTitle(model);
            AddScoreBoxes(model, accounts);
            AddData(model, searchResponses, leads, emails);
            return model;
        }

        public void AddPageTitle(RetailersOverviewViewModel model)
        {
            model.PageTitle = "Retailers Overview";
        }

        public void AddScoreBoxes(RetailersOverviewViewModel model, List<mxtrAccount> accounts)
        {
            model.TotalGroups = accounts.Count(a => a.AccountType == AccountKind.Group.ToString());
            model.TotalRetailers = accounts.Count(a => a.AccountType == AccountKind.Client.ToString());
        }

        public void AddData(RetailersOverviewViewModel model, List<CRMRestSearchResponseLogModel> searchResponses, List<CRMLeadSummaryDataModel> leads, List<CRMEmailJobDataModel> emails)
        {
            model.LeadsData = leads.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewLeadsSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    LeadCount = x.LeadCount
                }).ToList();

            model.SearchData = searchResponses.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewSearchSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    LocatorPageviews = x.LocatorPageviews,
                    LPPageviews = x.UrlClicks,
                    TotalClicks = x.UrlClicks + x.DirectionsClicks + x.MapClicks + x.MoreInfoClicks + x.EmailClicks
                }).ToList();

            model.EmailData = emails.OrderBy(x => x.CreateDate)
                .Select(x => new RetailerOverviewEmailSummaryViewData
                {
                    CreateDate = x.CreateDate.ToShortDateString(),
                    EmailSends = x.SendCount
                }).ToList();

        }
    }
}
