using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Common.Adapter;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Sharpspring.Adapter
{
    public static class SharpspringApiToDataModelAdapter
    {
        public static List<CRMLeadDataModel> AdaptLeads(mxtrAccount account, List<SharpspringLeadDataModel> leads, List<CRMLeadEventDataModel> leadEvents, DateTime date)
        {
            DateTime tempDate = date; //date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return leads
                    .Select(l => new CRMLeadDataModel()
                    {
                        AccountObjectID = account.ObjectID,
                        MxtrAccountID = account.MxtrAccountID,
                        CRMKind = CRMKind.Sharpspring.ToString(),
                        CreateDate = createDate,
                        LastUpdatedDate = lastModifiedDate,
                        LeadID = l.LeadID,
                        AccountID = l.AccountID,
                        OwnerID = l.OwnerID,
                        CampaignID = l.CampaignID,
                        LeadStatus = l.LeadStatus,
                        LeadScore = l.LeadScore,
                        IsActive = l.IsActive,
                        FirstName = l.FirstName,
                        LastName = l.LastName,
                        EmailAddress = l.EmailAddress,
                        CompanyName = l.CompanyName,
                        Title = l.Title,
                        Street = l.Street,
                        City = l.City,
                        Country = l.Country,
                        State = l.State,
                        ZipCode = l.ZipCode,
                        Website = l.Website,
                        PhoneNumber = l.PhoneNumber,
                        MobilePhoneNumber = l.MobilePhoneNumber,
                        FaxNumber = l.FaxNumber,
                        Description = l.Description,
                        Industry = l.Industry,
                        IsUnsubscribed = l.IsUnsubscribed,
                        Events = leadEvents.Where(e => e.LeadID == l.LeadID).OrderBy(e => e.CreateTimestamp).ToList()
                    }).ToList();
        }

        public static List<CRMLeadDataModel> AdaptLeads(mxtrAccount account, List<CRMLeadDataModel> leads, List<CRMLeadEventDataModel> leadEvents, DateTime date)
        {
            DateTime tempDate = date; //date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return leads
                    .Select(l => new CRMLeadDataModel()
                    {
                        AccountObjectID = account.ObjectID,
                        MxtrAccountID = account.MxtrAccountID,
                        CRMKind = CRMKind.Sharpspring.ToString(),
                        CreateDate = createDate,
                        LastUpdatedDate = lastModifiedDate,
                        LeadID = l.LeadID,
                        AccountID = l.AccountID,
                        OwnerID = l.OwnerID,
                        CampaignID = l.CampaignID,
                        LeadStatus = l.LeadStatus,
                        LeadScore = l.LeadScore,
                        IsActive = l.IsActive,
                        FirstName = l.FirstName,
                        LastName = l.LastName,
                        EmailAddress = l.EmailAddress,
                        CompanyName = l.CompanyName,
                        Title = l.Title,
                        Street = l.Street,
                        City = l.City,
                        Country = l.Country,
                        State = l.State,
                        ZipCode = l.ZipCode,
                        Website = l.Website,
                        PhoneNumber = l.PhoneNumber,
                        MobilePhoneNumber = l.MobilePhoneNumber,
                        FaxNumber = l.FaxNumber,
                        Description = l.Description,
                        Industry = l.Industry,
                        IsUnsubscribed = l.IsUnsubscribed,
                        Events = leadEvents.Where(e => e.LeadID == l.LeadID).OrderBy(e => e.CreateTimestamp).ToList()
                    }).ToList();
        }

        public static List<CRMCampaignDataModel> AdaptCampaigns(mxtrAccount account, List<SharpspringCampaignDataModel> campaigns, DateTime date)
        {
            DateTime tempDate = date; //date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return campaigns
                    .Select(c => new CRMCampaignDataModel()
                    {
                        AccountObjectID = account.ObjectID,
                        MxtrAccountID = account.MxtrAccountID,
                        CRMKind = CRMKind.Sharpspring.ToString(),
                        CreateDate = createDate,
                        LastUpdatedDate = lastModifiedDate,
                        CampaignID = c.CampaignID,
                        CampaignName = c.CampaignName,
                        CampaignType = c.CampaignType,
                        CampaignAlias = c.CampaignAlias,
                        CampaignOrigin = c.CampaignOrigin,
                        Quantity = c.Quantity,
                        Price = c.Price,
                        Goal = c.Goal,
                        OtherCosts = c.OtherCosts,
                        StartDate = new DateTime(c.StartDate.Year, c.StartDate.Month, c.StartDate.Day, 0, 0, 0, DateTimeKind.Utc),
                        EndDate = new DateTime(c.EndDate.Year, c.EndDate.Month, c.EndDate.Day, 0, 0, 0, DateTimeKind.Utc),
                        IsActive = c.IsActive
                    }).ToList();
        }

        public static List<CRMDealStageDataModel> AdaptDealStages(mxtrAccount account, List<SharpspringDealStageDataModel> dealStages, DateTime date)
        {
            DateTime tempDate = date; //date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return dealStages
                    .Select(d => new CRMDealStageDataModel()
                    {
                        AccountObjectID = account.ObjectID,
                        MxtrAccountID = account.MxtrAccountID,
                        CRMKind = CRMKind.Sharpspring.ToString(),
                        CreateDate = createDate,
                        LastUpdatedDate = lastModifiedDate,
                        DealStageID = d.DealStageID,
                        DealStageName = d.DealStageName,
                        Description = d.Description,
                        DefaultProbability = d.DefaultProbability,
                        Weight = d.Weight,
                        IsEditable = d.IsEditable
                    }).ToList();
        }

        public static List<CRMOpportunityDataModel> AdaptOpportunities(mxtrAccount account, List<SharpspringOpportunityDataModel> opportunities, DateTime date)
        {
            DateTime tempDate = date; //date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return opportunities
                    .Select(o => new CRMOpportunityDataModel()
                    {
                        AccountObjectID = account.ObjectID,
                        MxtrAccountID = account.MxtrAccountID,
                        CRMKind = CRMKind.Sharpspring.ToString(),
                        CreateDate = createDate,
                        LastUpdatedDate = lastModifiedDate,
                        OpportunityID = o.OpportunityID,
                        OwnerID = o.OwnerID,
                        PrimaryLeadID = o.PrimaryLeadID,
                        DealStageID = o.DealStageID,
                        AccountID = o.AccountID,
                        CampaignID = o.CampaignID,
                        OpportunityName = o.OpportunityName,
                        Probability = o.Probability,
                        Amount = o.Amount,
                        IsClosed = o.IsClosed,
                        IsWon = o.IsWon,
                        IsActive = o.IsActive,
                        CloseDate = o.CloseDate
                    }).ToList();
        }

        public static List<CRMEmailJobDataModel> AdaptEmailJobs(mxtrAccount account, List<SharpspringEmailJobDataModel> jobs, List<CRMEmailEventDataModel> emailEvents, DateTime date)
        {
            DateTime tempDate = date; //date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return jobs
                    .Select(c => new CRMEmailJobDataModel()
                    {
                        AccountObjectID = account.ObjectID,
                        MxtrAccountID = account.MxtrAccountID,
                        CRMKind = CRMKind.Sharpspring.ToString(),
                        CreateDate = createDate,
                        LastUpdatedDate = lastModifiedDate,
                        EmailJobID = c.EmailJobID,
                        IsList = c.IsList,
                        IsActive = c.IsActive,
                        RecipientID = c.RecipientID,
                        SendCount = c.SendCount,
                        CreateTimestamp = c.CreateTimestamp,
                        Events = emailEvents.Where(e => e.EmailJobID == c.EmailJobID).OrderBy(e => e.CreateDate).ToList()
                    }).ToList();
        }

        public static List<CRMEmailDataModel> AdaptEmails(mxtrAccount account, List<SharpspringEmailDataModel> emails, DateTime date)
        {
            DateTime tempDate = date;// date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return emails.Select(c => new CRMEmailDataModel()
            {
                AccountObjectID = account.ObjectID,
                MxtrAccountID = account.MxtrAccountID,
                CRMKind = CRMKind.Sharpspring.ToString(),
                CreateDate = createDate,
                LastUpdatedDate = lastModifiedDate,
                EmailID = c.EmailID,
                Subject = c.Subject,
                Title = c.Title,
                CreateTimestamp = c.CreateTimestamp,
                Thumbnail = c.Thumbnail
            }).ToList();
        }
    }
}
