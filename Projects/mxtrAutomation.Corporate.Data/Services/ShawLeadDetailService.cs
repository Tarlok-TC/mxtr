using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.Enums;
using System.Text.RegularExpressions;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class ShawLeadDetailService : MongoRepository<Account>, IShawLeadDetailServiceInternal
    {
        public CreateNotificationReturn AddLeadAnalyticData(LeadAnalyticDataModel data)
        {

            CRMLeadAnalytics entry = new CRMLeadAnalytics()
            {
                AccountObjectID = data.AccountObjectID,
                LeadId = data.LeadId,
                LeadScore = data.LeadScore,
                LeadScoreForToday = data.LeadScoreForToday,
                MxtrAccountID = data.MxtrAccountID,
                CRMKind = data.CRMKind,
                CreatedOnMXTR = data.CreatedDate,
                CreatedDate = data.CreatedDate,
            };

            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                try
                {
                    var lead = repo.FirstOrDefault(x => x.LeadId == entry.LeadId && x.AccountObjectID == entry.AccountObjectID && x.CRMKind == entry.CRMKind);
                    if (lead == null)
                    {
                        repo.Add(entry);
                    }
                    else
                    {
                        entry.Id = lead.Id;
                        repo.Update(entry);
                    }

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }


        }

        public bool AddLeadAnalyticData(List<LeadAnalyticDataModel> lstdata)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                List<CRMLeadAnalytics> lstEntryCreate = new List<CRMLeadAnalytics>();
                List<CRMLeadAnalytics> lstEntryUpdate = new List<CRMLeadAnalytics>();
                foreach (var data in lstdata)
                {
                    var lead = repo.FirstOrDefault(x => x.LeadId == data.LeadId && x.AccountObjectID == data.AccountObjectID && x.CRMKind == data.CRMKind);
                    if (lead == null)
                    {
                        //add new leads
                        lstEntryCreate.Add(new CRMLeadAnalytics()
                        {
                            AccountObjectID = data.AccountObjectID,
                            LeadId = data.LeadId,
                            LeadScore = data.LeadScore,
                            LeadScoreForToday = data.LeadScoreForToday,
                            MxtrAccountID = data.MxtrAccountID,
                            CRMKind = data.CRMKind,
                            CreatedOnMXTR = data.CreatedOnMXTR,
                            CreatedDate = data.CreatedDate,
                        });
                    }
                    else
                    {
                        //update existing leads
                        lstEntryUpdate.Add(new CRMLeadAnalytics()
                        {
                            Id = lead.Id,
                            AccountObjectID = data.AccountObjectID,
                            LeadId = data.LeadId,
                            LeadScore = data.LeadScore,
                            LeadScoreForToday = 0,//need to set
                            MxtrAccountID = data.MxtrAccountID,
                            CRMKind = data.CRMKind,
                            //--CreatedOnMXTR = data.CreatedOnMXTR,
                            //--CreatedDate = data.CreatedDate,
                        });
                    }

                }
                try
                {
                    repo.Add(lstEntryCreate);
                    repo.Update(lstEntryUpdate);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool AddLeadAnalyticData(List<LeadAnalyticDataModel> lstdata, DateTime whichDate)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                List<CRMLeadAnalytics> lstEntryCreate = new List<CRMLeadAnalytics>();
                List<CRMLeadAnalytics> lstEntryUpdate = new List<CRMLeadAnalytics>();
                foreach (var data in lstdata)
                {
                    var lead = repo.FirstOrDefault(x => x.LeadId == data.LeadId && x.AccountObjectID == data.AccountObjectID && x.CRMKind == data.CRMKind && x.CreatedDate == whichDate);

                    if (lead == null)
                    {
                        //get last record of that lead for calculating lead score diff                    
                        var Lastleadrecord = repo.Where(x => x.LeadId == data.LeadId && x.AccountObjectID == data.AccountObjectID).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                        int? leadScoreForToday = null;
                        if (Lastleadrecord != null)
                        {
                            leadScoreForToday = data.LeadScore - Lastleadrecord.LeadScore;
                        }

                        //add new leads
                        lstEntryCreate.Add(new CRMLeadAnalytics()
                        {
                            AccountObjectID = data.AccountObjectID,
                            LeadId = data.LeadId,
                            LeadScore = data.LeadScore,
                            LeadScoreForToday = leadScoreForToday,
                            MxtrAccountID = data.MxtrAccountID,
                            CRMKind = data.CRMKind,
                            CreatedOnMXTR = data.CreatedOnMXTR,
                            CreatedDate = data.CreatedDate,
                        });
                    }
                    else
                    {
                        //get last record of that lead for calculating lead score diff                    
                        var Lastleadrecord = repo.Where(x => x.LeadId == lead.LeadId && x.AccountObjectID == data.AccountObjectID).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                        int leadScoreForToday = 0;
                        if (Lastleadrecord != null)
                        {
                            leadScoreForToday = data.LeadScore - Lastleadrecord.LeadScore;
                        }

                        //update existing leads
                        lstEntryUpdate.Add(new CRMLeadAnalytics()
                        {
                            Id = lead.Id,
                            LeadScore = data.LeadScore,
                            LeadScoreForToday = leadScoreForToday,
                            CreatedOnMXTR = data.CreatedOnMXTR,
                        });
                    }
                }

                if (lstEntryCreate.Count > 0)
                    repo.Add(lstEntryCreate);
                if (lstEntryUpdate.Count > 0)
                    repo.Update(lstEntryUpdate);
                return true;
            }
        }

        public bool AddLeadAnalyticData(LeadAnalyticDataModel data, DateTime whichDate)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var lead = repo.FirstOrDefault(x => x.AccountObjectID == data.AccountObjectID && x.CRMKind == data.CRMKind && x.CreatedDate == whichDate);
                if (lead == null)
                {
                    repo.Add(new CRMLeadAnalytics()
                    {
                        AccountObjectID = data.AccountObjectID,
                        LeadId = data.LeadId,
                        LeadScore = data.LeadScore,
                        LeadScoreForToday = 0,
                        MxtrAccountID = data.MxtrAccountID,
                        CRMKind = data.CRMKind,
                        CreatedOnMXTR = data.CreatedOnMXTR,
                        CreatedDate = data.CreatedDate,
                    });
                }
                else
                {
                    lead.CreatedOnMXTR = data.CreatedOnMXTR;
                    repo.Update(lead);
                }
                return true;
            }
        }

        public List<LeadAnalyticDataModel> GetLeadAnalyticData(string accountObjectId)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var data = repo.Where(w => w.AccountObjectID == accountObjectId);
                return AdaptData(data);
            }
        }

        public List<LeadAnalyticDataModel> GetLeadAnalyticData(List<string> accountObjectIds)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var data = repo.Where(w => accountObjectIds.Contains(w.AccountObjectID));
                return AdaptData(data);
            }
        }
        /// <summary>
        /// Return list of tuple Item1=AccountObjectIds, Item2=Cold, Item3=Warm and Item4=Warm
        /// </summary>
        /// <param name="accountObjectIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>List of tuple Item1=AccountObjectIds, Item2=Cold, Item3=Warm and Item4=Warm</returns>
        public List<Tuple<string, int, int, int>> GetColdHotWarmLeadCountWithObjectId(List<string> accountObjectIds, DateTime? startDate, DateTime? endDate)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                List<Tuple<string, int, int, int>> lstFinalData = new List<Tuple<string, int, int, int>>();
                foreach (var accountObjectId in accountObjectIds)
                {
                    List<CRMLeadAnalytics> records = new List<CRMLeadAnalytics>();
                    if (startDate == null && endDate == null)
                    {
                        records = repo.Where(x => x.AccountObjectID == accountObjectId).OrderBy(o => o.CreatedDate).ToList();
                    }
                    else
                    {
                        records = repo.Where(x => x.AccountObjectID == accountObjectId && x.CreatedDate >= startDate && x.CreatedDate <= endDate).OrderBy(o => o.CreatedDate).ToList();
                    }
                    int cold = records.Where(x => x.LeadScoreForToday <= 99).ToList().Count;
                    int warm = records.Where(x => x.LeadScoreForToday > 99 && x.LeadScoreForToday <= 149).ToList().Count;
                    int hot = records.Where(x => x.LeadScoreForToday > 149).ToList().Count;
                    lstFinalData.Add(new Tuple<string, int, int, int>(accountObjectId, cold, warm, hot));
                }
                return lstFinalData;
            }
        }
        public List<Tuple<DateTime, int, int, int>> GetColdHotWarmLeadCount(string accountObjectId, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var records = repo.Where(x => x.AccountObjectID == accountObjectId && x.CreatedDate >= startDate && x.CreatedDate <= endDate).OrderBy(o => o.CreatedDate).ToList();
                List<DateTime> dateArray = records.DistinctBy(x => x.CreatedDate.Date).ToList().Select(x => x.CreatedDate.Date).ToList();
                List<Tuple<DateTime, int, int, int>> lstFinalData = new List<Tuple<DateTime, int, int, int>>();
                foreach (var date in dateArray)
                {
                    //int cold = records.Where(x => x.LeadScoreForToday <= 99 && x.CreatedDate.Date == date).ToList().Count;
                    //int warm = records.Where(x => x.LeadScoreForToday > 99 && x.LeadScoreForToday <= 149 && x.CreatedDate.Date == date).ToList().Count;
                    //int hot = records.Where(x => x.LeadScoreForToday > 149 && x.CreatedDate.Date == date).ToList().Count;
                    int cold = records.Where(x => x.LeadScore <= 99 && x.CreatedDate.Date == date).ToList().Count;
                    int warm = records.Where(x => x.LeadScore > 99 && x.LeadScore <= 149 && x.CreatedDate.Date == date).ToList().Count;
                    int hot = records.Where(x => x.LeadScore > 149 && x.CreatedDate.Date == date).ToList().Count;
                    lstFinalData.Add(new Tuple<DateTime, int, int, int>(date, cold, warm, hot));
                }
                return lstFinalData;
            }

        }
        public List<Tuple<DateTime, int, int, int>> GetColdHotWarmLeadCount(List<string> accountObjectIds, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var allRecods = repo.Where(x => accountObjectIds.Contains(x.AccountObjectID) && x.CreatedDate <= endDate).OrderByDescending(o => o.CreatedDate).ToList();
                var records = allRecods.Where(x => accountObjectIds.Contains(x.AccountObjectID) && x.CreatedDate >= startDate && x.CreatedDate <= endDate).OrderByDescending(o => o.CreatedDate).ToList();
                //records.ForEach(x =>
                //{
                //    if (x.LeadScoreForToday == null)
                //    {
                //        x.LeadScoreForToday = x.LeadScore;
                //    }
                //});


                var allcold = allRecods.Where(x => x.LeadScore <= 99).ToList();
                var allwarm = allRecods.Where(x => x.LeadScore > 99 && x.LeadScore <= 149).ToList();
                var allhot = allRecods.Where(x => x.LeadScore > 149).ToList();


                List<DateTime> dateArray = records.DistinctBy(x => x.CreatedDate.Date).ToList().Select(x => x.CreatedDate.Date).ToList();
                List<Tuple<DateTime, int, int, int>> lstFinalData = new List<Tuple<DateTime, int, int, int>>();
                //foreach (var date in dateArray)
                //{
                //    int cold = records.Where(x => x.LeadScoreForToday <= 99 && x.CreatedDate.Date == date).ToList().Count;
                //    int warm = records.Where(x => x.LeadScoreForToday > 99 && x.LeadScoreForToday <= 149 && x.CreatedDate.Date == date).ToList().Count;
                //    int hot = records.Where(x => x.LeadScoreForToday > 149 && x.CreatedDate.Date == date).ToList().Count;
                //    lstFinalData.Add(new Tuple<DateTime, int, int, int>(date, cold, warm, hot));
                //}
                int cold = allcold.Count();
                int warm = allwarm.Count();
                int hot = allhot.Count();
                foreach (var date in dateArray)
                {
                    //cold += records.Where(x => x.LeadScoreForToday <= 99 && x.CreatedDate.Date == date).ToList().Count;
                    //warm += records.Where(x => x.LeadScoreForToday > 99 && x.LeadScoreForToday <= 149 && x.CreatedDate.Date == date).ToList().Count;
                    //hot += records.Where(x => x.LeadScoreForToday > 149 && x.CreatedDate.Date == date).ToList().Count;
                    //lstFinalData.Add(new Tuple<DateTime, int, int, int>(date, cold, warm, hot));
                    cold -= allcold.Where(x => x.CreatedDate.Date == date).ToList().Count;
                    warm -= allwarm.Where(x => x.CreatedDate.Date == date).ToList().Count;
                    hot -= allhot.Where(x => x.CreatedDate.Date == date).ToList().Count;
                    lstFinalData.Add(new Tuple<DateTime, int, int, int>(date, cold, warm, hot));
                }
                return lstFinalData;
            }

        }

        public List<LeadAnalyticDataModel> GetLeadAnalyticDataByDateRange(List<string> accountObjectIds, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var data = repo.Where(w => accountObjectIds.Contains(w.AccountObjectID) && w.CreatedOnMXTR >= startDate && w.CreatedOnMXTR <= endDate);
                return AdaptData(data);
            }
        }

        public Tuple<double, int, int, int> GetLeadScores(List<long> leadid, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var result = repo.Where(l => leadid.Contains(l.LeadId)).ToList();
                result = result.Where(x => x.CreatedOnMXTR >= startDate && x.CreatedOnMXTR <= endDate).ToList();
                result.ForEach(x =>
                {
                    if (x.LeadScoreForToday == null)
                    {
                        x.LeadScoreForToday = x.LeadScore;
                    }
                });
                result = result.Where(x => x.LeadScoreForToday != 0).ToList();
                if (result.Count == 0)
                {
                    return new Tuple<double, int, int, int>(0, 0, 0, 0);
                }
                var leadScoreMin = result.Min(x => x.LeadScoreForToday);
                var leadScoreMax = result.Max(x => x.LeadScoreForToday);
                // double avg = result.Sum(x => Convert.ToInt32(x.LeadScoreForToday)) / result.Count;
                double avg = result.Sum(x => Convert.ToInt32(x.LeadScoreForToday));
                return new Tuple<double, int, int, int>(avg, result.Count, leadScoreMin == null ? 0 : Convert.ToInt32(leadScoreMin), leadScoreMax == null ? 0 : Convert.ToInt32(leadScoreMax));
            }
        }

        public int CheckLeadCount(long leadId, string accountObjectId, CRMKind minerKind)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                return repo.Where(x => x.LeadId == leadId && x.AccountObjectID == accountObjectId && x.CRMKind == minerKind.ToString()).ToList().Count;
            }
        }

        public DateTime? GetLastRunDate(string accountObjectId, CRMKind minerKind)
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                var lastRundate = repo.LastOrDefault(x => x.AccountObjectID == accountObjectId && x.CRMKind == minerKind.ToString());
                if (lastRundate != null)
                    //return lastRundate.CreatedOnMXTR;
                    return lastRundate.CreatedDate;
                return null;
            }
        }

        public bool UpdateSSCreateDate()
        {
            using (MongoRepository<CRMLeadAnalytics> repo = new MongoRepository<CRMLeadAnalytics>())
            {
                bool isUpdate = false;
                var data = repo.Where(x => x.LeadId > 0).ToList();
                foreach (var item in data)
                {
                    if (item.UpdatedDate == null)
                    {
                        isUpdate = true;
                        item.UpdatedDate = item.CreatedDate;
                    }
                }
                if (isUpdate)
                {
                    repo.Update(data);
                }

                data = repo.Where(x => x.LeadId > 0).ToList();
                var crmLeadAnalytics = data.GroupBy(x => new { x.LeadId }).Select(m => new CRMLeadAnalytics
                {
                    LeadId = m.First().LeadId
                }).ToList();

                List<long> lstLeadIds = crmLeadAnalytics.Select(x => x.LeadId).ToList();
                Dictionary<long, DateTime> dicLeadsUpdate = new Dictionary<long, DateTime>();
                using (MongoRepository<CRMLead> repoCRMLead = new MongoRepository<CRMLead>())
                {
                    var crmLeadData = repoCRMLead.Where(x => lstLeadIds.Contains(x.LeadID)).ToList();
                    foreach (var item in crmLeadData)
                    {
                        if (item != null && item.Events != null && item.Events.Count > 0)
                        {
                            dicLeadsUpdate.Add(item.LeadID, item.Events[0].CreateTimestamp);
                        }
                    }
                }


                List<CRMLeadAnalytics> lstCRMLeadAnalytics = new List<CRMLeadAnalytics>();
                foreach (var item in dicLeadsUpdate)
                {
                    var leadAnalyticsData = data.Where(x => x.LeadId == item.Key);
                    leadAnalyticsData.ForEach(x => x.CreatedDate = item.Value);
                    lstCRMLeadAnalytics.AddRange(leadAnalyticsData);
                }

                if (lstCRMLeadAnalytics.Count > 0)
                {
                    repo.Update(lstCRMLeadAnalytics);
                }

                return true;
            }
        }

        private List<LeadAnalyticDataModel> AdaptData(IEnumerable<CRMLeadAnalytics> data)
        {
            return data.Select(x => new LeadAnalyticDataModel()
            {
                AccountObjectID = x.AccountObjectID,
                LeadId = x.LeadId,
                LeadScore = x.LeadScore,
                LeadScoreForToday = x.LeadScoreForToday,
                MxtrAccountID = x.MxtrAccountID,
                CRMKind = x.CRMKind,
                CreatedOnMXTR = x.CreatedOnMXTR,
                CreatedDate = x.CreatedDate,
            }).ToList();
        }
    }
}
