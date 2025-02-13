using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Common.Attributes;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Enums;
using mxtrAutomation.Common.Dto;


namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ILeadsViewModelAdapter
    {
        LeadsViewModel BuildLeadsViewModel(IEnumerable<CRMLeadDataModel> leads, IEnumerable<CRMCampaignDataModel> campaigns, List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
    }

    public class LeadsViewModelAdapter : ILeadsViewModelAdapter
    {
        public LeadsViewModel BuildLeadsViewModel(IEnumerable<CRMLeadDataModel> leads, IEnumerable<CRMCampaignDataModel> campaigns, List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            LeadsViewModel model = new LeadsViewModel();
            AddAttributes(model, startDate, endDate, accountObjectIDs);
            AddPageTitle(model);
            AddLeadsData(model, leads, campaigns, accountObjectIDs);

            return model;
        }

        public void AddAttributes(LeadsViewModel model, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.UpdateDataUrl = new LeadsWebQuery();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        public void AddPageTitle(LeadsViewModel model)
        {
            model.PageTitle = "Leads";
            model.MainPageHeader = "Leads";
            model.SubPageHeader = "View leads.";
            model.ShowWorkspaceFilter = true;
            model.ShowDateFilter = true;
            model.CallbackFunction = "updatePageFromWorkspace";
        }

        public void AddLeadsData(LeadsViewModel model, IEnumerable<CRMLeadDataModel> leads, IEnumerable<CRMCampaignDataModel> campaigns, List<string> accountObjectIDs)
        {
            model.AccountObjectIDs = accountObjectIDs;
            model.CurrentAccountIDs = String.Join(",", accountObjectIDs);

            model.Leads = leads.Select(l => new LeadViewData
            {
                ObjectID = l.ObjectID,
                AccountObjectID = l.AccountObjectID,
                CreateDate = l.CreateDate,
                LeadID = l.LeadID,
                OwnerID = l.OwnerID,
                CampaignID = l.CampaignID,
                CampaignName = campaigns.Where(c => c.CampaignID == l.CampaignID).Select(c => c.CampaignName).FirstOrDefault() ?? "Unassigned",
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
                LeadParentAccount = l.LeadParentAccount,
                //EventLastTouch = l.Events.OrderByDescending(t => t.CreateTimestamp).FirstOrDefault()?.CreateTimestamp.ToString("yyyy-MM-dd") ?? string.Empty,
            }).Where(l => l.CreateDate >= Convert.ToDateTime(model.StartDate) && l.CreateDate <= Convert.ToDateTime(model.EndDate))
            .OrderBy(l => l.CreateDate);

            model.LeadsChartViewData = leads.Select(l => new LeadsChartViewData
            {
                CreateDate = l.CreateDate,
            }).Where(l => l.CreateDate >= Convert.ToDateTime(model.StartDate) && l.CreateDate <= Convert.ToDateTime(model.EndDate))
            .OrderBy(l => l.CreateDate);
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
    }
}
