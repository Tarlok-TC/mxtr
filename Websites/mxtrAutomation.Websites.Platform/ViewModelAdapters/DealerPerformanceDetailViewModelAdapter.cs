using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Websites.Platform.Models.Dealer.ViewData;
using mxtrAutomation.Websites.Platform.Models.Dealer.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IDealerPerformanceDetailViewModelAdapter
    {
        DealerPerformanceDetailViewModel BuildDealerPerformanceDetailViewModel(string accountName, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService);
    }
    public class DealerPerformanceDetailViewModelAdapter : IDealerPerformanceDetailViewModelAdapter
    {
        public DealerPerformanceDetailViewModel BuildDealerPerformanceDetailViewModel(string accountName, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService)
        {
            DealerPerformanceDetailViewModel model = new DealerPerformanceDetailViewModel();
            AddAttribute(model, accountObjectIDs, startDate, endDate);
            AddData(model, accountName, startDate, endDate, accountObjectIDs, leadService, dbShawLeadDetailService);
            return model;
        }

        private static void AddAttribute(DealerPerformanceDetailViewModel model, List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            model.PageTitle = "Dealer Detail";
            model.ShowDateFilter = true;
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }
        private static void AddData(DealerPerformanceDetailViewModel model, string accountName, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService)
        {
            accountObjectIDs = accountObjectIDs.Distinct().ToList();
            List<DealerPerformanceDetailData> lstDealerPerformanceDetailData = new List<DealerPerformanceDetailData>();
            var lstDealerLeads = leadService.GetLeadsByAccountObjectIDs_DateRange(accountObjectIDs, startDate, endDate);
            lstDealerPerformanceDetailData = lstDealerLeads.Select(l => new DealerPerformanceDetailData
            {
                ObjectID = l.ObjectID,
                AccountObjectID = l.AccountObjectID,
                CreateDate = l.CreateDate,
                LeadID = l.LeadID,
                OwnerID = l.OwnerID,
                CampaignID = l.CampaignID,
                LeadStatus = l.LeadStatus,
                LeadScore = l.LeadScore,
                IsActive = l.IsActive,
                FirstName = l.FirstName ?? string.Empty,
                LastName = l.LastName ?? string.Empty,
                EmailAddress = l.EmailAddress ?? string.Empty,
                CompanyName = l.CompanyName ?? string.Empty,
                City = l.City ?? string.Empty,
                State = l.State ?? string.Empty,
                EventsCount = l.Events.Count()
            }).Where(l => l.CreateDate >= Convert.ToDateTime(startDate) && l.CreateDate <= Convert.ToDateTime(endDate)).OrderBy(l => l.CreateDate).ToList();
            model.DealerLeads = lstDealerPerformanceDetailData;
            var leadsData = lstDealerLeads; //leadService.GetLeadsByAccountObjectIDs_DateRange(accountObjectIDs, startDate, endDate);
            List<long> leadIds = leadsData.Select(x => x.LeadID).ToList();
            var coldHotWarmLeadCount = dbShawLeadDetailService.GetColdHotWarmLeadCountWithObjectId(accountObjectIDs, startDate, endDate);
            var leadCount = leadService.GetTotalLeadsWithObject(accountObjectIDs, startDate, endDate);
            var copiedLeadCount = leadService.GetCopiedLeadsToDealer(accountObjectIDs, startDate, endDate);
            var passOff = leadService.GetPassOffPercentage(accountObjectIDs, startDate, endDate);
            var avgTimeInFunnel = leadService.GetAverageLeadTimeInDealers(accountObjectIDs, leadsData);
            model.DealerDetail = new DealerPerformanceViewData()
            {
                AccountName = accountName,
                LeadsCount = string.Format("{0:n0}", leadCount.FirstOrDefault().Value), //leadCount.FirstOrDefault().Value,
                ColdLeadsCount = coldHotWarmLeadCount.FirstOrDefault().Item2,
                WarmLeadsCount = coldHotWarmLeadCount.FirstOrDefault().Item3,
                HotLeadsCount = coldHotWarmLeadCount.FirstOrDefault().Item4,
                HandedOffLeads = copiedLeadCount.FirstOrDefault().Value,
                // HandedOffLeads
                PassOff = passOff.FirstOrDefault().Value,
                AverageTimeInFunnel = Math.Round(avgTimeInFunnel.FirstOrDefault().Value, 0),
                ConversionRate = Math.Round(leadService.GetConversionRateInDealer(leadIds), 0),
            };
        }
    }
}