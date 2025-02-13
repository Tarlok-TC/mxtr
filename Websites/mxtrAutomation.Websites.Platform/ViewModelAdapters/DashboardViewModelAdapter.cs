using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Dashboard.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Dashboard.ViewData;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Common.Attributes;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Enums;
using mxtrAutomation.Common.Dto;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IDashboardViewModelAdapter
    {
        DashboardViewModel BuildDashboardViewModel(List<DashboardSummaryDataModel> dashboardSummaries, List<CRMLeadDataModel> leads, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<mxtrAccount> accounts);
    }

    public class DashboardViewModelAdapter : IDashboardViewModelAdapter
    {
        public DashboardViewModel BuildDashboardViewModel(List<DashboardSummaryDataModel> dashboardSummaries, List<CRMLeadDataModel> leads, DateTime startDate, DateTime endDate, List<string> accountObjectIDs, List<mxtrAccount> accounts)
        {
            DashboardViewModel model = new DashboardViewModel();        

            AddPageTitle(model);

            model.HasData = false;
            AddParameters(model, startDate, endDate, accountObjectIDs);

            //if (dashboardSummaries.Count() > 0)
            //{
                //AddParameters(model, startDate, endDate, accountObjectIDs);
                //AddScoreboxData(model, dashboardSummaries);
                //AddChartData(model, dashboardSummaries);
                //AddCampaignPerformanceData(model, dashboardSummaries);
                //AddAccountSummaries(model, dashboardSummaries, accounts);
                //model.HasData = true;
            //}

            return model;
        }

        public void AddPageTitle(DashboardViewModel model)
        {
            model.PageTitle = "Dashboard";
            model.MainPageHeader = "Dashboard";
            model.SubPageHeader = "View your at-a-glance summaries and trends.";
            model.ShowWorkspaceFilter = true;
            model.ShowDateFilter = true;
            model.CallbackFunction = "updateDashboard";
        }

        public void AddParameters(DashboardViewModel model, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.RefreshUrl = new DashboardWebQuery();
            model.CurrentAccountIDs = String.Join(",", accountObjectIDs);
        }

        public void AddScoreboxData(DashboardViewModel model, List<DashboardSummaryDataModel> dashboardSummaries)
        {
            List<DashboardSummaryDetail> details =
                dashboardSummaries.SelectMany(d => d.Daily).ToList();

            DateTime lastDate = details.OrderByDescending(d => d.Date).Select(d => d.Date).First();
            DateTime firstDate = details.OrderBy(d => d.Date).Select(d => d.Date).First();

            int lastLeads = details.Where(d => d.Date == lastDate).Sum(d => d.TotalLeads);
            int lastCampaigns = details.Where(d => d.Date == lastDate).Sum(d => d.TotalCampaigns);
            double lastOpenOpportunityValue = details.Where(d => d.Date == lastDate).Sum(d => d.TotalActivePotentialOpportunitiesAmount);

            int firstLeads = details.Where(d => d.Date == firstDate).Sum(d => d.TotalLeads);
            int firstCampaigns = details.Where(d => d.Date == firstDate).Sum(d => d.TotalCampaigns);
            double firstOpenOpportunityValue = details.Where(d => d.Date == firstDate).Sum(d => d.TotalActivePotentialOpportunitiesAmount);

            model.TotalLeads = lastLeads;
            model.TotalCampaigns = lastCampaigns;
            model.OpenOpportunityValue = lastOpenOpportunityValue;
            model.ScoreBoxDataDate = lastDate.ToShortDateString();

            model.TotalCampaignsDelta = (double)(lastCampaigns - firstCampaigns) / (double)firstCampaigns;
            model.TotalCampaignsDeltaArrowCss = model.TotalCampaignsDelta > 0 ? "fa fa-arrow-up" : (model.TotalCampaignsDelta == 0 ? "fa fa-arrows-h" : "fa fa-arrow-down");

            model.TotalLeadsDelta = (double)(lastLeads - firstLeads) / (double)firstLeads;
            model.TotalLeadsDeltaArrowCss = model.TotalLeadsDelta > 0 ? "fa fa-arrow-up" : (model.TotalLeadsDelta == 0 ? "fa fa-arrows-h" : "fa fa-arrow-down");

            model.OpenOpportunityValueDelta = (lastOpenOpportunityValue - firstOpenOpportunityValue) / firstOpenOpportunityValue;
            model.OpenOpportunityValueDeltaArrowCss = model.OpenOpportunityValueDelta > 0 ? "fa fa-arrow-up" : (model.OpenOpportunityValueDelta == 0 ? "fa fa-arrows-h" : "fa fa-arrow-down");
        }

        public void AddChartData(DashboardViewModel model, List<DashboardSummaryDataModel> dashboardSummaries)
        {
            List<DashboardSummaryDetail> details =
                dashboardSummaries.SelectMany(d => d.Daily).OrderBy(d => d.Date).ToList();

            var t = from d in details
                    group d by new { d.Date } into g
                    select new
                    {
                        DataDate = g.Key.Date,
                        TotalLeads = g.Sum(d => d.TotalLeads),
                        TotalCampaigns = g.Sum(d => d.TotalCampaigns),
                        TotalOpenOpportunityValue = g.Sum(d => d.TotalActivePotentialOpportunitiesAmount)
                    };


            model.LeadsChartData = new LineChartDataModel
            {
                labels = t.Select(d => d.DataDate.ToShortDateString()).ToList(),
                datasets = new List<ChartDatasetDataModel>() { 
                    new ChartDatasetDataModel
                    {
                        label = "Total Leads",
                        data = t.Select(d => d.TotalLeads).ToList(),
                        yAxisID = "y-axis-1",
                        borderColor = "rgba(4,159,219,0.5)",
                        backgroundColor = "rgba(4,159,219,0.5)"
                    }
                }
            };

            model.OpenOpportunityValueChartData = new LineChartDataModel
            {
                labels = t.Select(d => d.DataDate.ToShortDateString()).ToList(),
                datasets = new List<ChartDatasetDataModel>() { 
                    new ChartDatasetDataModel
                    {
                        label = "Total Open Opportunities Value",
                        data = t.Select(d => Convert.ToInt32(d.TotalOpenOpportunityValue)).ToList(),
                        yAxisID = "y-axis-1",
                        borderColor = "rgba(38, 185, 154, 0.5)",
                        backgroundColor = "rgba(38, 185, 154, 0.5)"
                    }
                }
            };
        }

        public void AddCampaignPerformanceData(DashboardViewModel model, List<DashboardSummaryDataModel> dashboardSummaries)
        {
            List<CampaignPerformanceViewData> campaignPerformances = new List<CampaignPerformanceViewData>();
            foreach (DashboardSummaryDataModel ds in dashboardSummaries)
            {
                List<CampaignPerformanceViewData> cp = ds.CampaignPerformance
                    .Select(p => new CampaignPerformanceViewData
                    {
                        AccountName = ds.AccountObjectID,
                        CampaignName = p.CampaignName,
                        Width = (int)p.Score,
                        Leads = (int)p.Leads,
                        OpportunityAmount = (int)p.OpportunityAmount,
                        Score = p.Score
                    }).ToList();

                campaignPerformances.AddRange(cp);
            }

            campaignPerformances = campaignPerformances.OrderByDescending(d => d.Score).Take(5).ToList();

            model.CampaignPerformances = campaignPerformances;

            model.CampaignsChartData = new LineChartDataModel
            {
                labels = campaignPerformances.Select(d => d.CampaignName).ToList(),
                //labels = campaignPerformances.Select(d => string.Format("{0}{1}{2}", d.CampaignName, Environment.NewLine, d.AccountName)).ToList(),
                datasets = new List<ChartDatasetDataModel>() { 
                    new ChartDatasetDataModel
                    {
                        label = "Leads",
                        data = campaignPerformances.Select(d => d.Leads).ToList(),
                        yAxisID = "y-axis-1",
                        borderColor = "rgba(4,159,219,0.5)",
                        backgroundColor = "rgba(4,159,219,0.5)"
                    },
                    new ChartDatasetDataModel
                    {
                        label = "Opportunity Amount",
                        data = campaignPerformances.Select(d => d.OpportunityAmount).ToList(),
                        yAxisID = "y-axis-2",
                        borderColor = "rgba(38, 185, 154, 0.5)",
                        backgroundColor = "rgba(38, 185, 154, 0.5)"
                    }
                }
            };
        }

        public void AddAccountSummaries(DashboardViewModel model, List<DashboardSummaryDataModel> dashboardSummaries, List<mxtrAccount> accounts)
        {
            List<AccountSummaryViewData> accountSummaries = accounts
                .Select(a => new AccountSummaryViewData
                {
                    AccountName = a.AccountName,
                    TotalActivePotentialOpportunitiesAmount = dashboardSummaries.Where(d => d.AccountObjectID == a.ObjectID).SelectMany(d => d.Daily).OrderByDescending(d => d.Date).Select(d => d.TotalActivePotentialOpportunitiesAmount).FirstOrDefault(),
                    TotalLeads = dashboardSummaries.Where(d => d.AccountObjectID == a.ObjectID).SelectMany(d => d.Daily).OrderByDescending(d => d.Date).Select(d => d.TotalLeads).FirstOrDefault(),
                    TotalCampaigns = dashboardSummaries.Where(d => d.AccountObjectID == a.ObjectID).SelectMany(d => d.Daily).OrderByDescending(d => d.Date).Select(d => d.TotalCampaigns).FirstOrDefault(),
                    LeadsUrl = new LeadsWebQuery { AccountObjectID = a.ObjectID }
                }).ToList();

            model.AccountSummaries = accountSummaries;
        }
    }
}
