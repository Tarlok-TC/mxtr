using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class DashboardSummaryService : MongoRepository<DashboardSummary>, IDashboardSummaryServiceInternal
    {
        public DashboardSummaryDataModel GetDashboardSummaryByAccountObjectID(string accountObjectID, int year, int month)
        {
            using (MongoRepository<DashboardSummary> repo = new MongoRepository<DashboardSummary>())
            {
                return repo
                    .Where(s => s.AccountObjectID == accountObjectID && s.Year == year && s.Month == month)
                    .Select(s => new DashboardSummaryDataModel
                    {
                        ObjectID = s.Id,
                        AccountObjectID = s.AccountObjectID,
                        Year = s.Year,
                        Month = s.Month,
                        Daily = s.Daily.Select(d => new DashboardSummaryDetail
                        {
                            Date = d.Date,
                            ActivePotentialOpportunitiesCount = d.ActivePotentialOpportunitiesCount,
                            ActivePotentialOpportunitiesAmount = d.ActivePotentialOpportunitiesAmount,
                            TotalActivePotentialOpportunitiesAmount = d.TotalActivePotentialOpportunitiesAmount,
                            WonOpportunitiesCount = d.WonOpportunitiesCount,
                            WonOpportunitiesAmount = d.WonOpportunitiesAmount,
                            ClosedOpportunitiesCount = d.ClosedOpportunitiesCount,
                            ClosedOpportunitiesAmount = d.ClosedOpportunitiesAmount,
                            TotalLeads = d.TotalLeads,
                            NewLeads = d.NewLeads,
                            UpdatedLeads = d.UpdatedLeads,
                            TotalCampaigns = d.TotalCampaigns,
                            NewCampaigns = d.NewCampaigns,
                            UpdatedCampaigns = d.UpdatedCampaigns
                        }).ToList()
                    }).FirstOrDefault();
            }
        }

        public List<DashboardSummaryDataModel> GetDashboardSummaries(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<DashboardSummary> repo = new MongoRepository<DashboardSummary>())
            {
                return repo
                    .Where(s => accountObjectIDs.Contains(s.AccountObjectID) && (s.Month >= startDate.Month && s.Month <= endDate.Month) && (s.Year >= startDate.Year && s.Year <= endDate.Year))
                    .Select(s => new DashboardSummaryDataModel
                    {
                        ObjectID = s.Id,
                        AccountObjectID = s.AccountObjectID,
                        Year = s.Year,
                        Month = s.Month,
                        Daily = s.Daily
                            .Where(d => d.Date >= startDate && d.Date <= endDate)
                            .Select(d => new DashboardSummaryDetail
                            {
                                Date = d.Date,
                                ActivePotentialOpportunitiesCount = d.ActivePotentialOpportunitiesCount,
                                ActivePotentialOpportunitiesAmount = d.ActivePotentialOpportunitiesAmount,
                                TotalActivePotentialOpportunitiesAmount = d.TotalActivePotentialOpportunitiesAmount,
                                WonOpportunitiesCount = d.WonOpportunitiesCount,
                                WonOpportunitiesAmount = d.WonOpportunitiesAmount,
                                ClosedOpportunitiesCount = d.ClosedOpportunitiesCount,
                                ClosedOpportunitiesAmount = d.ClosedOpportunitiesAmount,
                                TotalLeads = d.TotalLeads,
                                NewLeads = d.NewLeads,
                                UpdatedLeads = d.UpdatedLeads,
                                TotalCampaigns = d.TotalCampaigns,
                                NewCampaigns = d.NewCampaigns,
                                UpdatedCampaigns = d.UpdatedCampaigns
                            }).ToList(),
                        CampaignPerformance = s.CampaignPerformance
                            .Select(p => new DashboardSummaryCampaignPerformance
                            {
                                CampaignID = p.CampaignID,
                                CampaignName = p.CampaignName,
                                OpportunityAmount = p.OpportunityAmount,
                                Leads = p.Leads,
                                Score = p.Score,
                                Rank = p.Rank
                            }).ToList()
                    }).ToList();
            }
        }
    }
}
