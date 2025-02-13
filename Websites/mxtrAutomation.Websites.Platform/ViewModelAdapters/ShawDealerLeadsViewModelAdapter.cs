using System;
using System.Linq;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Corporate.Data.DataModels;


namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IShawDealerLeadsViewModelAdapter
    {
        ShawDealerLeadsViewModel BuildShawDealerLeadsViewModel(IEnumerable<CRMLeadDataModel> leads, List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
    }

    public class ShawDealerLeadsViewModelAdapter : IShawDealerLeadsViewModelAdapter
    {
        public ShawDealerLeadsViewModel BuildShawDealerLeadsViewModel(IEnumerable<CRMLeadDataModel> leads, List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            ShawDealerLeadsViewModel model = new ShawDealerLeadsViewModel();
            AddAttributes(model, startDate, endDate, accountObjectIDs);
            AddPageTitle(model);
            AddLeadsData(model, leads, accountObjectIDs);

            return model;
        }

        public void AddAttributes(ShawDealerLeadsViewModel model, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.UpdateDataUrl = new ShawLeadsWebQuery();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        public void AddPageTitle(ShawDealerLeadsViewModel model)
        {
            model.PageTitle = "Corporate Leads";
            model.MainPageHeader = "Corporate Leads";
            model.SubPageHeader = "View leads.";
            model.ShowWorkspaceFilter = true;
            model.ShowDateFilter = true;
            //model.CallbackFunction = "updatePageFromWorkspace";
        }

        public void AddLeadsData(ShawDealerLeadsViewModel model, IEnumerable<CRMLeadDataModel> leads, List<string> accountObjectIDs)
        {
            model.AccountObjectIDs = accountObjectIDs;
            model.CurrentAccountIDs = String.Join(",", accountObjectIDs);

            //model.Leads = leads.Select(l => new ShawLeadViewData
            //{
            //    ObjectID = l.ObjectID,
            //    AccountObjectID = l.AccountObjectID,
            //    CreateDate = l.CreateDate,
            //    LeadID = l.LeadID,
            //    OwnerID = l.OwnerID,
            //    CampaignID = l.CampaignID,
            //    CampaignName = campaigns.Where(c => c.CampaignID == l.CampaignID).Select(c => c.CampaignName).FirstOrDefault() ?? "Unassigned",
            //    LeadStatus = l.LeadStatus,
            //    LeadScore = l.LeadScore,
            //    IsActive = l.IsActive,
            //    FirstName = l.FirstName ?? string.Empty,
            //    LastName = l.LastName ?? string.Empty,
            //    EmailAddress = l.EmailAddress ?? string.Empty,
            //    CompanyName = l.CompanyName ?? string.Empty,
            //    City = l.City ?? string.Empty,
            //    State = l.State ?? string.Empty,
            //   // EventsCount = l.Events.Count(),                            
            //    EventLastTouch = GetLastTouch(l),
            //    LeadParentAccount = l.LeadParentAccount,
            //}).Where(l => l.CreateDate >= Convert.ToDateTime(model.StartDate) && l.CreateDate <= Convert.ToDateTime(model.EndDate))
            //.OrderBy(l => l.CreateDate);

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
