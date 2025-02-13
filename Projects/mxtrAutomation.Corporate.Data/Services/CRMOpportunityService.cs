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
    public class CRMOpportunityService : MongoRepository<CRMOpportunity>, ICRMOpportunityServiceInternal
    {
        public CreateNotificationReturn CreateCRMOpportunity(CRMOpportunityDataModel crmOpportunityDataModel)
        {
            CRMOpportunity entry = new CRMOpportunity()
            {
                AccountObjectID = crmOpportunityDataModel.AccountObjectID,
                MxtrAccountID = crmOpportunityDataModel.MxtrAccountID,
                CRMKind = crmOpportunityDataModel.CRMKind,
                CreateDate = crmOpportunityDataModel.CreateDate,
                LastUpdatedDate = crmOpportunityDataModel.LastUpdatedDate,
                OpportunityID = crmOpportunityDataModel.OpportunityID,
                OwnerID = crmOpportunityDataModel.OwnerID,
                PrimaryLeadID = crmOpportunityDataModel.PrimaryLeadID,
                DealStageID = crmOpportunityDataModel.DealStageID,
                AccountID = crmOpportunityDataModel.AccountID,
                CampaignID = crmOpportunityDataModel.CampaignID,
                OpportunityName = crmOpportunityDataModel.OpportunityName,
                Probability = crmOpportunityDataModel.Probability,
                Amount = crmOpportunityDataModel.Amount,
                IsClosed = crmOpportunityDataModel.IsClosed,
                IsWon = crmOpportunityDataModel.IsWon,
                IsActive = crmOpportunityDataModel.IsActive,
                CloseDate = crmOpportunityDataModel.CloseDate
            };

            using (MongoRepository<CRMOpportunity> repo = new MongoRepository<CRMOpportunity>())
            {
                try
                {
                    repo.Add(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn CreateBatchCRMOpportunities(List<CRMOpportunityDataModel> crmOpportunityDataModels)
        {
            List<CRMOpportunity> entries = crmOpportunityDataModels
                .Select(o => new CRMOpportunity()
                {
                    AccountObjectID = o.AccountObjectID,
                    MxtrAccountID = o.MxtrAccountID,
                    CRMKind = o.CRMKind,
                    CreateDate = o.CreateDate,
                    LastUpdatedDate = o.LastUpdatedDate,
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

            using (MongoRepository<CRMOpportunity> repo = new MongoRepository<CRMOpportunity>())
            {
                try
                {
                    // Remove entries which already exist in database
                    entries.RemoveAll(x => repo.Any(y => y.OpportunityID == x.OpportunityID && y.AccountObjectID == x.AccountObjectID && y.CRMKind == x.CRMKind));

                    repo.Add(entries);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateOpportunities(List<CRMOpportunityDataModel> opportunitiesDatas, string accountObjectID, string crmKind)
        {
            List<long> opportunityIDs = opportunitiesDatas.Select(x => x.OpportunityID).ToList();

            using (MongoRepository<CRMOpportunity> repo = new MongoRepository<CRMOpportunity>())
            {
                try
                {
                    IEnumerable<CRMOpportunity> entries = repo
                        .Where(x => x.AccountObjectID == accountObjectID && x.CRMKind == crmKind)
                        .Where(x => opportunityIDs.Contains(x.OpportunityID));

                    foreach (CRMOpportunity entry in entries)
                    {
                        CRMOpportunityDataModel opportunity = opportunitiesDatas.Where(o => o.OpportunityID == entry.OpportunityID).FirstOrDefault();

                        entry.LastUpdatedDate = opportunity.LastUpdatedDate;
                        entry.OwnerID = opportunity.OwnerID;
                        entry.PrimaryLeadID = opportunity.PrimaryLeadID;
                        entry.DealStageID = opportunity.DealStageID;
                        entry.AccountID = opportunity.AccountID;
                        entry.CampaignID = opportunity.CampaignID;
                        entry.OpportunityName = opportunity.OpportunityName;
                        entry.Probability = opportunity.Probability;
                        entry.Amount = opportunity.Amount;
                        entry.IsClosed = opportunity.IsClosed;
                        entry.IsWon = opportunity.IsWon;
                        entry.IsActive = opportunity.IsActive;
                        entry.CloseDate = opportunity.CloseDate;

                        repo.Update(entry);
                    }

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

    }
}
