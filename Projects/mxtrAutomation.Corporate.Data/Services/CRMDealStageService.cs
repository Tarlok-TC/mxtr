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
    public class CRMDealStageService : MongoRepository<CRMDealStage>, ICRMDealStageServiceInternal
    {
        public CreateNotificationReturn CreateCRMDealStage(CRMDealStageDataModel crmDealStageDataModel)
        {
            CRMDealStage entry = new CRMDealStage()
            {
                AccountObjectID = crmDealStageDataModel.AccountObjectID,
                MxtrAccountID = crmDealStageDataModel.MxtrAccountID,
                CRMKind = crmDealStageDataModel.CRMKind,
                CreateDate = crmDealStageDataModel.CreateDate,
                LastUpdatedDate = crmDealStageDataModel.LastUpdatedDate,
                DealStageID = crmDealStageDataModel.DealStageID,
                DealStageName = crmDealStageDataModel.DealStageName,
                Description = crmDealStageDataModel.Description,
                DefaultProbability = crmDealStageDataModel.DefaultProbability,
                Weight = crmDealStageDataModel.Weight,
                IsEditable = crmDealStageDataModel.IsEditable
            };

            using (MongoRepository<CRMDealStage> repo = new MongoRepository<CRMDealStage>())
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

        public CreateNotificationReturn CreateBatchCRMDealStages(List<CRMDealStageDataModel> crmDealStageDataModels)
        {
            List<CRMDealStage> entries = crmDealStageDataModels
                .Select(s => new CRMDealStage()
                {
                    AccountObjectID = s.AccountObjectID,
                    MxtrAccountID = s.MxtrAccountID,
                    CRMKind = s.CRMKind,
                    CreateDate = s.CreateDate,
                    LastUpdatedDate = s.LastUpdatedDate,
                    DealStageID = s.DealStageID,
                    DealStageName = s.DealStageName,
                    Description = s.Description,
                    DefaultProbability = s.DefaultProbability,
                    Weight = s.Weight,
                    IsEditable = s.IsEditable
                }).ToList();

            using (MongoRepository<CRMDealStage> repo = new MongoRepository<CRMDealStage>())
            {
                try
                {
                    // Remove entries which already exist in database
                    entries.RemoveAll(x => repo.Any(y => y.DealStageID == x.DealStageID && y.AccountObjectID == x.AccountObjectID && y.CRMKind == x.CRMKind));

                    repo.Add(entries);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateDealStages(List<CRMDealStageDataModel> dealStageDatas, string accountObjectID, string crmKind)
        {
            List<long> dealStageIDs = dealStageDatas.Select(x => x.DealStageID).ToList();

            using (MongoRepository<CRMDealStage> repo = new MongoRepository<CRMDealStage>())
            {
                try
                {
                    IEnumerable<CRMDealStage> entries = repo
                        .Where(x => x.AccountObjectID == accountObjectID && x.CRMKind == crmKind)
                        .Where(x => dealStageIDs.Contains(x.DealStageID));

                    foreach (CRMDealStage entry in entries)
                    {
                        CRMDealStageDataModel dealStage = dealStageDatas.Where(d => d.DealStageID == entry.DealStageID).FirstOrDefault();

                        entry.LastUpdatedDate = dealStage.LastUpdatedDate;
                        entry.DealStageName = dealStage.DealStageName;
                        entry.Description = dealStage.Description;
                        entry.DefaultProbability = dealStage.DefaultProbability;
                        entry.Weight = dealStage.Weight;
                        entry.IsEditable = dealStage.IsEditable;

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
