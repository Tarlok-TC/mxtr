using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Websites.Platform.Models.Dealer.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IDealerPerformanceViewModelAdapter
    {
        DealerPerformanceViewModel BuildDealerPerformanceViewModel(DateTime startDate, DateTime endDate, List<mxtrAccount> accounts, List<string> accountObjectIDs, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService);
    }
    public class DealerPerformanceViewModelAdapter : IDealerPerformanceViewModelAdapter
    {
        public DealerPerformanceViewModel BuildDealerPerformanceViewModel(DateTime startDate, DateTime endDate, List<mxtrAccount> accounts, List<string> accountObjectIDs, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService)
        {
            DealerPerformanceViewModel model = new DealerPerformanceViewModel();
            AddAttribute(startDate, endDate, accountObjectIDs, model);
            AddData(startDate, endDate, accounts, accountObjectIDs, model, leadService, dbShawLeadDetailService);
            return model;
        }

        private static void AddAttribute(DateTime startDate, DateTime endDate, List<string> accountObjectIDs, DealerPerformanceViewModel model)
        {
            model.PageTitle = "Dealer Performance";
            model.ShowDateFilter = true;
            model.ShowWorkspaceFilter = true;
            model.CallbackFunction = "updatePageFromWorkspace";
            model.UpdateDataUrl = new DealerPerformanceWebQuery();
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        private void AddData(DateTime startDate, DateTime endDate, List<mxtrAccount> accounts, List<string> accountObjectIDs, DealerPerformanceViewModel model, ICRMLeadService leadService, IShawLeadDetailService dbShawLeadDetailService)
        {
            accountObjectIDs = accountObjectIDs.Distinct().ToList();
            model.LeadsCountInDealerFunnel = string.Format("{0:n0}", leadService.GetCopiedLeads(accountObjectIDs, startDate, endDate));//leadService.GetCopiedLeads(accountObjectIDs, startDate, endDate);
            var leadsData = leadService.GetLeadsByAccountObjectIDs_DateRange(accountObjectIDs, startDate, endDate);//leadService.GetLeadsByAccountObjectIDsForDateRange(accountObjectIDs, startDate, endDate);
            List<long> leadIds = leadsData.Select(x => x.LeadID).ToList();
            model.AverageLeadTimeDealer = Math.Round(leadService.GetAverageLeadTimeInDealers(leadIds), 0);
            model.ConversionRateDealer = Math.Round(leadService.GetConversionRateInDealer(leadIds), 0);
            var coldHotWarmLeadCount = dbShawLeadDetailService.GetColdHotWarmLeadCountWithObjectId(accountObjectIDs, startDate, endDate);
            var leadCount = leadService.GetTotalLeadsWithObject(accountObjectIDs, startDate, endDate);
            var copiedLeadCount = leadService.GetCopiedLeadsToDealer(accountObjectIDs, startDate, endDate);
            var passOff = leadService.GetPassOffPercentage(accountObjectIDs, startDate, endDate);
            var avgTimeInFunnel = leadService.GetAverageLeadTimeInDealers(accountObjectIDs, leadsData);
            model.DealerData = new List<DealerPerformanceViewData>();
            foreach (var item in accounts)
            {
                model.DealerData.Add(new DealerPerformanceViewData()
                {
                    AccountName = item.AccountName,
                    AccountObjectID = item.ObjectID,
                    ColdLeadsCount = coldHotWarmLeadCount.FirstOrDefault(t => t.Item1 == item.ObjectID) == null ? 0 : coldHotWarmLeadCount.FirstOrDefault(t => t.Item1 == item.ObjectID).Item2,
                    WarmLeadsCount = coldHotWarmLeadCount.FirstOrDefault(t => t.Item1 == item.ObjectID) == null ? 0 : coldHotWarmLeadCount.FirstOrDefault(t => t.Item1 == item.ObjectID).Item3,
                    HotLeadsCount = coldHotWarmLeadCount.FirstOrDefault(t => t.Item1 == item.ObjectID) == null ? 0 : coldHotWarmLeadCount.FirstOrDefault(t => t.Item1 == item.ObjectID).Item4,
                    LeadsCount = Convert.ToString(leadCount.FirstOrDefault(t => t.Key == item.ObjectID).Value),
                    HandedOffLeads = copiedLeadCount.FirstOrDefault(t => t.Key == item.ObjectID).Value,
                    PassOff = Math.Round(passOff.FirstOrDefault(t => t.Key == item.ObjectID).Value, 2),
                    AverageTimeInFunnel = Math.Round(avgTimeInFunnel.FirstOrDefault(t => t.Key == item.ObjectID).Value, 2),
                });
            }
            int passOffCount = passOff.Count == 0 ? 1 : passOff.Count;
            int avgTimeInFunnelCount = avgTimeInFunnel.Count == 0 ? 1 : avgTimeInFunnel.Count;
            model.FooterData = new DatatableFooter
            {
                TotalLeads = leadCount.Select(x => x.Value).Sum(),
                TotalColdLead = coldHotWarmLeadCount.Select(x => x.Item2).Sum(),
                TotalWarmLead = coldHotWarmLeadCount.Select(x => x.Item3).Sum(),
                TotalHotLead = coldHotWarmLeadCount.Select(x => x.Item4).Sum(),
                //Math.Round(passOff.Select(x => x.Value).Sum(), 2),
                // Changed below as per hangout discussion
                TotalPassOf = Math.Round((passOff.Select(x => x.Value).Sum() / passOffCount), 2),
                //MXTR-544
                //TotalAvgTimeFunnel = Math.Round(avgTimeInFunnel.Select(x => x.Value).Sum(), 2),
                TotalAvgTimeFunnel = Math.Round((avgTimeInFunnel.Select(x => x.Value).Sum() / avgTimeInFunnelCount), 2),
                TotalHandedOffLeads = copiedLeadCount.Select(x => x.Value).Sum(),
            };
        }
    }
}