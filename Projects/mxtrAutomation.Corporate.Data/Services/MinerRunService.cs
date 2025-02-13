using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class MinerRunService : MongoRepository<MinerRunAuditTrail>, IMinerRunServiceInternal
    {
        public CreateNotificationReturn CreateMinerRunAuditTrail(MinerRunAuditTrailDataModel minerRunData)
        {
            MinerRunAuditTrail entry = new MinerRunAuditTrail()
            {
                AccountObjectID = minerRunData.AccountObjectID,
                MxtrAccountID = minerRunData.MxtrAccountID,
                MinerRunDetails = minerRunData.MinerRunDetails
            };

            using (MongoRepository<MinerRunAuditTrail> repo = new MongoRepository<MinerRunAuditTrail>())
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

        public CreateNotificationReturn UpdateMinerRunDetails(string accountObjectID, MinerRunMinerDetails minerRunMinerDetails)
        {
            using (MongoRepository<MinerRunAuditTrail> repo = new MongoRepository<MinerRunAuditTrail>())
            {
                try
                {
                    MinerRunAuditTrail entry = repo.Where(x => x.AccountObjectID == accountObjectID).FirstOrDefault();

                    entry.MinerRunDetails.Add(minerRunMinerDetails);

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = accountObjectID };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = accountObjectID };
                }
            }
        }

        public MinerRunAuditTrailDataModel GetMinerRunAuditTrailByAccountObjectID(string accountObjectID)
        {
            using (MongoRepository<MinerRunAuditTrail> repo = new MongoRepository<MinerRunAuditTrail>())
            {
                return repo
                    .Where(a => a.AccountObjectID == accountObjectID)
                    .Select(a => new MinerRunAuditTrailDataModel
                    {
                        AccountObjectID = a.AccountObjectID,
                        MxtrAccountID = a.MxtrAccountID,
                        MinerRunDetails = a.MinerRunDetails
                    }).FirstOrDefault();
            }
        }

        public MinerRunAuditTrailDataModel GetLastMinerRunAuditTrailByAccountObjectID(string accountObjectID)
        {
            using (MongoRepository<MinerRunAuditTrail> repo = new MongoRepository<MinerRunAuditTrail>())
            {
                List<MinerRunAuditTrailDataModel> runs = repo
                    .Where(a => a.AccountObjectID == accountObjectID)
                    .Select(a => new MinerRunAuditTrailDataModel
                    {
                        AccountObjectID = a.AccountObjectID,
                        MxtrAccountID = a.MxtrAccountID,
                        MinerRunDetails = a.MinerRunDetails
                    }).ToList();

                if (runs.Count == 0)
                    return new MinerRunAuditTrailDataModel();

                List<MinerRunMinerDetails> details = runs.SelectMany(a => a.MinerRunDetails).OrderByDescending(a => a.LastEndDateForDataCollection).ToList();
                runs[0].MinerRunDetails = details;
                return runs[0];
            }
        }

        public CreateNotificationReturn DeleteTrailLogandAddOneEntry(string minerType, DateTime lastEndDateForDataCollection, List<string> accountIds)
        {
            using (MongoRepository<MinerRunAuditTrail> repo = new MongoRepository<MinerRunAuditTrail>())
            {
                try
                {
                    var lstMinerRunAuditTrailLogs = repo.ToList();
                    if (accountIds.Count > 0)
                    {
                        lstMinerRunAuditTrailLogs = repo.Where(r => accountIds.Contains(r.AccountObjectID)).ToList();
                    }
                    if (lstMinerRunAuditTrailLogs.Count == 0)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "No Record found" };
                    }

                    DateTime dtLastEndDateForDataCollection = lastEndDateForDataCollection;
                    DateTime dtLastStartDateForDataCollection = lastEndDateForDataCollection.AddDays(-1);
                    foreach (var minerLog in lstMinerRunAuditTrailLogs)
                    {
                        minerLog.MinerRunDetails.RemoveAll(w => w.MinerName == minerType
                        && w.LastEndDateForDataCollection >= lastEndDateForDataCollection);
                        minerLog.MinerRunDetails.Add(new MinerRunMinerDetails()
                        {
                            IsFirstMinerRunComplete = true,
                            LastMinerRunTime = dtLastStartDateForDataCollection,
                            LastStartDateForDataCollection = dtLastStartDateForDataCollection,
                            LastEndDateForDataCollection = dtLastEndDateForDataCollection,
                            MinerName = minerType,
                            Summary = new List<MinerRunDataCollectionSummary>()
                        });
                    }
                    repo.Update(lstMinerRunAuditTrailLogs);
                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception ex)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        /// <summary>
        /// Item1: Lead Create Date
        /// Item2: Lead Score with Decay
        /// Item3: SharpSpring ID
        /// Item4: Lead Score
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public bool UpdateCRMLeadData(List<Tuple<DateTime?, int, long, int>> lstData, string accountObjectId)
        {
            List<long> lstLeadId = lstData.Select(x => x.Item3).ToList();
            List<CRMLead> lstCRMData = new List<CRMLead>();
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var data = repo.Where(x => lstLeadId.Contains(x.LeadID) && x.AccountObjectID == accountObjectId);
                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        var updatedData = lstData.Where(x => x.Item3 == item.LeadID).FirstOrDefault();
                        if (updatedData != null)
                        {
                            item.CreatedOnMXTR = item.CreateDate; // save old date in new column
                            item.CreateDate = Convert.ToDateTime(updatedData.Item1);
                            item.LeadScoreWeighted = updatedData.Item2;
                            item.LeadScore = updatedData.Item4;
                            lstCRMData.Add(item);
                        }
                    }
                    repo.Update(lstCRMData);
                }
            }
            return true;
        }

        public bool UpdateCRMLeadAnalyticsData(List<Tuple<DateTime?, int, long, int>> lstData, string accountObjectId, string mxtrAccountId)
        {
            List<CRMLeadAnalytics> lstCRMLeadAnalyticsData = new List<CRMLeadAnalytics>();
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var data = repo.Where(x => x.AccountObjectID == accountObjectId).ToList();
                if (data == null || data.Count == 0)
                {
                    foreach (var item in lstData)
                    {
                        lstCRMLeadAnalyticsData.Add(new CRMLeadAnalytics
                        {
                            AccountObjectID = accountObjectId,
                            MxtrAccountID = mxtrAccountId,
                            CreatedDate = Convert.ToDateTime(item.Item1),
                            LeadScoreWeighted = item.Item2,
                            LeadId = item.Item3,
                            LeadScore = item.Item4,
                            CreatedOnMXTR = DateTime.UtcNow,
                            CRMKind = CRMKind.ShawKPIMiner.ToString(),
                        });
                    }
                    repo.Add(lstCRMLeadAnalyticsData);
                }
            }

            return true;
        }
    }
}
