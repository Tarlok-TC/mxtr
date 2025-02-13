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
    public class CRMCampaignService : MongoRepository<CRMCampaign>, ICRMCampaignServiceInternal
    {
        public CreateNotificationReturn CreateCRMCampaign(CRMCampaignDataModel crmCampaignDataModel)
        {
            CRMCampaign entry = new CRMCampaign()
            {
                AccountObjectID = crmCampaignDataModel.AccountObjectID,
                MxtrAccountID = crmCampaignDataModel.MxtrAccountID,
                CRMKind = crmCampaignDataModel.CRMKind,
                CreateDate = crmCampaignDataModel.CreateDate,
                LastUpdatedDate = crmCampaignDataModel.LastUpdatedDate,
                CampaignID = crmCampaignDataModel.CampaignID,
                CampaignName = crmCampaignDataModel.CampaignName,
                CampaignType = crmCampaignDataModel.CampaignType,
                CampaignAlias = crmCampaignDataModel.CampaignAlias,
                CampaignOrigin = crmCampaignDataModel.CampaignOrigin,
                Quantity = crmCampaignDataModel.Quantity,
                Price = crmCampaignDataModel.Price,
                Goal = crmCampaignDataModel.Goal,
                OtherCosts = crmCampaignDataModel.OtherCosts,
                StartDate = crmCampaignDataModel.StartDate,
                EndDate = crmCampaignDataModel.EndDate,
                IsActive = crmCampaignDataModel.IsActive
            };

            using (MongoRepository<CRMCampaign> repo = new MongoRepository<CRMCampaign>())
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

        public CreateNotificationReturn CreateBatchCRMCampaigns(List<CRMCampaignDataModel> crmCampaignDataModels)
        {
            List<CRMCampaign> entries = crmCampaignDataModels
                .Select(c => new CRMCampaign()
                {
                    AccountObjectID = c.AccountObjectID,
                    MxtrAccountID = c.MxtrAccountID,
                    CRMKind = c.CRMKind,
                    CreateDate = c.CreateDate,
                    LastUpdatedDate = c.LastUpdatedDate,
                    CampaignID = c.CampaignID,
                    CampaignName = c.CampaignName,
                    CampaignType = c.CampaignType,
                    CampaignAlias = c.CampaignAlias,
                    CampaignOrigin = c.CampaignOrigin,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    Goal = c.Goal,
                    OtherCosts = c.OtherCosts,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive
                }).ToList();

            using (MongoRepository<CRMCampaign> repo = new MongoRepository<CRMCampaign>())
            {
                try
                {
                    // Remove entries which already exist in database
                    entries.RemoveAll(x => repo.Any(y => y.CampaignID == x.CampaignID && y.AccountObjectID == x.AccountObjectID && y.CRMKind == x.CRMKind));

                    repo.Add(entries);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateCampaign(CRMCampaignDataModel campaignData)
        {
            using (MongoRepository<CRMCampaign> repo = new MongoRepository<CRMCampaign>())
            {
                try
                {
                    CRMCampaign entry = repo
                        .Where(x => x.AccountObjectID == campaignData.AccountObjectID && x.CampaignID == campaignData.CampaignID && x.CRMKind == campaignData.CRMKind)
                        .FirstOrDefault();

                    entry.CampaignName = campaignData.CampaignName;
                    entry.CampaignType = campaignData.CampaignType;
                    entry.CampaignAlias = campaignData.CampaignAlias;
                    entry.CampaignOrigin = campaignData.CampaignOrigin;
                    entry.Quantity = campaignData.Quantity;
                    entry.Price = campaignData.Price;
                    entry.Goal = campaignData.Goal;
                    entry.OtherCosts = campaignData.OtherCosts;
                    entry.StartDate = campaignData.StartDate;
                    entry.EndDate = campaignData.EndDate;
                    entry.IsActive = campaignData.IsActive;

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateCampaigns(List<CRMCampaignDataModel> campaignDatas, string accountObjectID, string crmKind)
        {
            List<long> campaignIDs = campaignDatas.Select(x => x.CampaignID).ToList();

            using (MongoRepository<CRMCampaign> repo = new MongoRepository<CRMCampaign>())
            {
                try
                {
                    IEnumerable<CRMCampaign> entries = repo
                        .Where(x => x.AccountObjectID == accountObjectID && x.CRMKind == crmKind)
                        .Where(x => campaignIDs.Contains(x.CampaignID));

                    foreach (CRMCampaign entry in entries)
                    {
                        CRMCampaignDataModel campaign = campaignDatas.Where(c => c.CampaignID == entry.CampaignID).FirstOrDefault();

                        entry.LastUpdatedDate = campaign.LastUpdatedDate;
                        entry.CampaignName = campaign.CampaignName;
                        entry.CampaignType = campaign.CampaignType;
                        entry.CampaignAlias = campaign.CampaignAlias;
                        entry.CampaignOrigin = campaign.CampaignOrigin;
                        entry.Quantity = campaign.Quantity;
                        entry.Price = campaign.Price;
                        entry.Goal = campaign.Goal;
                        entry.OtherCosts = campaign.OtherCosts;
                        entry.StartDate = campaign.StartDate;
                        entry.EndDate = campaign.EndDate;
                        entry.IsActive = campaign.IsActive;

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

        public List<CRMCampaignDataModel> GetCampaignsByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<CRMCampaign> repo = new MongoRepository<CRMCampaign>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Select(c => new CRMCampaignDataModel
                    {
                        CampaignID = c.CampaignID,
                        CampaignName = c.CampaignName,
                        CampaignType = c.CampaignType,
                        CampaignAlias = c.CampaignAlias,
                        CampaignOrigin = c.CampaignOrigin,
                        Quantity = c.Quantity,
                        Price = c.Price,
                        Goal = c.Goal,
                        OtherCosts = c.OtherCosts,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        IsActive = c.IsActive
                    }).ToList();
            }
        }

        public IEnumerable<CRMCampaignDataModel> GetCampaignsByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMCampaign> repo = new MongoRepository<CRMCampaign>())
            {
                return repo
                    // .Where(c => accountObjectIDs.Contains(c.AccountObjectID) && (c.CreateDate >= startDate && c.CreateDate <= endDate))
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID) && (c.StartDate >= startDate && c.EndDate <= endDate))
                    //.Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Select(c => new CRMCampaignDataModel
                    {
                        CampaignID = c.CampaignID,
                        CampaignName = c.CampaignName,
                        CampaignType = c.CampaignType,
                        CampaignAlias = c.CampaignAlias,
                        CampaignOrigin = c.CampaignOrigin,
                        Quantity = c.Quantity,
                        Price = c.Price,
                        Goal = c.Goal,
                        OtherCosts = c.OtherCosts,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        IsActive = c.IsActive
                    });
            }
        }

    }
}
