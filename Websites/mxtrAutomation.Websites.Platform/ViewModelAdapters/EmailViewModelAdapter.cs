using System;
using mxtrAutomation.Websites.Platform.Models.Email.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Email.ViewData;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IEmailViewModelAdapter
    {
        EmailViewModel BuildEmailViewModel(IEnumerable<mxtrAccount> accounts, IEnumerable<CRMEmailJobDataModel> emailJobs, DateTime startDate, DateTime endDate, List<string> accountObjectIDs);
    }

    public class EmailViewModelAdapter : IEmailViewModelAdapter
    {
        public EmailViewModel BuildEmailViewModel(IEnumerable<mxtrAccount> accounts, IEnumerable<CRMEmailJobDataModel> emailJobs, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            EmailViewModel model = new EmailViewModel();
            model.EmailChartViewData = new EmailChartViewData();
            AddPageTitle(model);
            AddAttributes(model, startDate, endDate, accountObjectIDs);
            AddEmailJobStatsByDate(model, emailJobs);
            AddEmailActivity(model, emailJobs, accounts);
            AddTotals(model);
            return model;
        }

        public void AddPageTitle(EmailViewModel model)
        {
            model.PageTitle = "Email";
            model.ShowWorkspaceFilter = true;
            model.ShowDateFilter = true;
        }

        public void AddAttributes(EmailViewModel model, DateTime startDate, DateTime endDate, List<string> accountObjectIDs)
        {
            model.StartDate = startDate.ToShortDateString();
            model.EndDate = endDate.ToShortDateString();
            model.CallbackFunction = "updatePageFromWorkspace";
            model.UpdateDataUrl = new EmailWebQuery();
            model.CurrentAccountIDs = string.Join(",", accountObjectIDs);
        }

        public void AddTotals(EmailViewModel model)
        {
            int totalSend = 0;
            int totalOpens = 0;
            int totalClicks = 0;
            foreach (var item in model.EmailChartViewData.EmailJobStatsViewData)
            {
                totalSend += item.Sends;
                totalOpens += item.Opens;
                totalClicks += item.Clicks;
            }
            model.EmailChartViewData.TotalEmailSends = totalSend;
            model.EmailChartViewData.TotalEmailOpens = totalOpens;
            model.EmailChartViewData.TotalEmailClicks = totalClicks;
            model.EmailChartViewData.OverallOpenRate = GetRates(totalOpens, totalSend) * 100;
            model.EmailChartViewData.OverallClickRate = GetRates(totalClicks, totalOpens) * 100;  
        }

        public void AddEmailJobStatsByDate(EmailViewModel model, IEnumerable<CRMEmailJobDataModel> emailJobs)
        {
            var flattenedEmailJobStats = emailJobs.Select(x => new
            {
                DataDate = x.CreateTimestamp.Date,
                Sends = x.Events.Where(e => e.EventType == "sends").Count(),
                Opens = x.Events.Where(e => e.EventType == "opens").Count(),
                Clicks = x.Events.Where(e => e.EventType == "clicks").Count()
            }).Where(w => w.DataDate >= Convert.ToDateTime(model.StartDate) &&
            w.DataDate <= Convert.ToDateTime(model.EndDate)).OrderBy(o => o.DataDate);

            model.EmailChartViewData.EmailJobStatsViewData =
                flattenedEmailJobStats.GroupBy(l => l.DataDate)
                .Select(group => new EmailJobStatsViewData
                {
                    DataDate = group.Key.ToShortDateString(),
                    Sends = group.Sum(c => c.Sends),
                    Opens = group.Sum(c => c.Opens),
                    Clicks = group.Sum(c => c.Clicks),
                    OpenRate = GetRates(group.Sum(c => c.Opens), group.Sum(c => c.Sends)) * 100,
                    ClickRate = GetRates(group.Sum(c => c.Clicks), group.Sum(c => c.Opens)) * 100
                });
        }

        public void AddEmailActivity(EmailViewModel model, IEnumerable<CRMEmailJobDataModel> emailJobs, IEnumerable<mxtrAccount> accounts)
        {
            var flattenedEmailJobStats = emailJobs.Select(x => new
            {
                AccountObjectID = x.AccountObjectID,
                Sends = x.Events.Where(e => e.EventType == "sends").Count(),
                Opens = x.Events.Where(e => e.EventType == "opens").Count(),
                Clicks = x.Events.Where(e => e.EventType == "clicks").Count(),
            });

            model.EmailActivityViewData =
                flattenedEmailJobStats.GroupBy(l => l.AccountObjectID)
                .Select(group => new EmailActivityViewData
                {
                    AccountObjectID = group.Key,
                    AccountName = GetAccountName(group.Key, accounts),
                    Sends = group.Sum(c => c.Sends),
                    Opens = group.Sum(c => c.Opens),
                    Clicks = group.Sum(c => c.Clicks),
                    OpenRate = GetRates(group.Sum(c => c.Opens), group.Sum(c => c.Sends)) * 100,
                    ClickRate = GetRates(group.Sum(c => c.Clicks), group.Sum(c => c.Opens)) * 100
                }).ToList();

            model.EmailChartViewData.EmailActivityViewDataMini = new EmailActivityViewDataMini()
            {
                Sends = model.EmailActivityViewData.Sum(c => c.Sends),
                Opens = model.EmailActivityViewData.Sum(f => f.Opens),
                Clicks = model.EmailActivityViewData.Sum(f => f.Clicks),
                OpenRate = model.EmailActivityViewData.Sum(f => f.OpenRate),
                ClickRate = model.EmailActivityViewData.Sum(f => f.ClickRate),
            };
        }

        public void AddEmailActivityBackup(EmailViewModel model, List<CRMEmailJobDataModel> emailJobs, List<CRMEmailDataModel> emails, List<mxtrAccount> accounts)
        {
            var flattenedEmailJobStats = emailJobs.Select(x => new
            {
                Sends = x.Events.Where(e => e.EventType == "sends").Count(),
                Opens = x.Events.Where(e => e.EventType == "opens").Count(),
                Clicks = x.Events.Where(e => e.EventType == "clicks").Count(),
                EmailID = x.Events.Select(e => e.EmailID).FirstOrDefault()
            });

            List<EmailJobStatsViewData> emailJobsGroupedByEmailID =
                flattenedEmailJobStats.GroupBy(l => l.EmailID)
                .Select(group => new EmailJobStatsViewData
                {
                    EmailID = group.Key,
                    Sends = group.Sum(c => c.Sends),
                    Opens = group.Sum(c => c.Opens),
                    Clicks = group.Sum(c => c.Clicks),
                    OpenRate = GetRates(group.Sum(c => c.Opens), group.Sum(c => c.Sends)) * 100,
                    ClickRate = GetRates(group.Sum(c => c.Clicks), group.Sum(c => c.Opens)) * 100
                }).ToList();

            model.EmailActivityViewData = emails.Select(x => new EmailActivityViewData
            {
                AccountObjectID = x.AccountObjectID,
                AccountName = GetAccountName(x.AccountObjectID, accounts),
                EmailID = x.EmailID,
                EmailTitle = x.Title,
                Sends = emailJobsGroupedByEmailID.Where(e => e.EmailID == x.EmailID).Select(e => e.Sends).FirstOrDefault(),
                Opens = emailJobsGroupedByEmailID.Where(e => e.EmailID == x.EmailID).Select(e => e.Opens).FirstOrDefault(),
                Clicks = emailJobsGroupedByEmailID.Where(e => e.EmailID == x.EmailID).Select(e => e.Clicks).FirstOrDefault(),
                OpenRate = emailJobsGroupedByEmailID.Where(e => e.EmailID == x.EmailID).Select(e => e.OpenRate).FirstOrDefault(),
                ClickRate = emailJobsGroupedByEmailID.Where(e => e.EmailID == x.EmailID).Select(e => e.ClickRate).FirstOrDefault()
            }).ToList();
        }

        protected double GetRates(int num, int den)
        {
            if ((num == 0) || (den == 0))
                return 0;

            double rate = Convert.ToDouble(num) / Convert.ToDouble(den);
            if (rate > 1)
                return 1;

            return rate;
        }

        private string GetAccountName(string accountObjectID, IEnumerable<mxtrAccount> accounts)
        {
            return accounts.Where(a => a.ObjectID == accountObjectID).Select(a => a.AccountName).FirstOrDefault();
        }
    }
}
