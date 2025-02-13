using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class CRMRestSearchResponseLogService : MongoRepository<CRMRestSearchResponseLog>, ICRMRestSearchResponseLogServiceInternal
    {
        public CreateNotificationReturn CreateCRMRestSearchResponseLog(CRMRestSearchResponseLogModel crmRestSearchResponseLogModel)
        {
            CRMRestSearchResponseLog entry = new CRMRestSearchResponseLog()
            {
                AccountObjectID = crmRestSearchResponseLogModel.AccountObjectID,
                MxtrAccountID = crmRestSearchResponseLogModel.MxtrAccountID,
                CRMKind = crmRestSearchResponseLogModel.CRMKind,
                CreateDate = crmRestSearchResponseLogModel.CreateDate,
                LastUpdatedDate = crmRestSearchResponseLogModel.LastUpdatedDate,
                AccountName = crmRestSearchResponseLogModel.AccountName,
                LocatorPageviews = crmRestSearchResponseLogModel.LocatorPageviews,
                UrlClicks = crmRestSearchResponseLogModel.UrlClicks,
                EmailClicks = crmRestSearchResponseLogModel.EmailClicks,
                MoreInfoClicks = crmRestSearchResponseLogModel.MoreInfoClicks,
                MapClicks = crmRestSearchResponseLogModel.MapClicks,
                DirectionsClicks = crmRestSearchResponseLogModel.DirectionsClicks
            };

            using (MongoRepository<CRMRestSearchResponseLog> repo = new MongoRepository<CRMRestSearchResponseLog>())
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

        public CreateNotificationReturn CreateBatchCRMRestSearchResponseLogs(List<CRMRestSearchResponseLogModel> crmRestSearchResponseLogModels)
        {
            List<CRMRestSearchResponseLog> entries = crmRestSearchResponseLogModels
                .Select(c => new CRMRestSearchResponseLog()
                {
                    AccountObjectID = c.AccountObjectID,
                    MxtrAccountID = c.MxtrAccountID,
                    CRMKind = c.CRMKind,
                    CreateDate = c.CreateDate,
                    LastUpdatedDate = c.LastUpdatedDate,
                    AccountName = c.AccountName,
                    LocatorPageviews = c.LocatorPageviews,
                    UrlClicks = c.UrlClicks,
                    EmailClicks = c.EmailClicks,
                    MoreInfoClicks = c.MoreInfoClicks,
                    MapClicks = c.MapClicks,
                    DirectionsClicks = c.DirectionsClicks
                }).ToList();

            using (MongoRepository<CRMRestSearchResponseLog> repo = new MongoRepository<CRMRestSearchResponseLog>())
            {
                try
                {
                    repo.Add(entries);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateRestSearchResponseLog(List<CRMRestSearchResponseLogModel> logDatas, string accountObjectID, string crmKind)
        {
            List<string> accountNames = logDatas.Select(x => x.AccountName).ToList();

            using (MongoRepository<CRMRestSearchResponseLog> repo = new MongoRepository<CRMRestSearchResponseLog>())
            {
                try
                {
                    IEnumerable<CRMRestSearchResponseLog> entries = repo
                        .Where(x => x.AccountObjectID == accountObjectID && x.CRMKind == crmKind)
                        .Where(x => accountNames.Contains(x.AccountName));

                    foreach (CRMRestSearchResponseLog entry in entries)
                    {
                        CRMRestSearchResponseLogModel account = logDatas.Where(c => c.AccountName == entry.AccountName).FirstOrDefault();

                        entry.LastUpdatedDate = account.LastUpdatedDate;
                        entry.LocatorPageviews = account.LocatorPageviews;
                        entry.UrlClicks = account.UrlClicks;
                        entry.EmailClicks = account.EmailClicks;
                        entry.MoreInfoClicks = account.MoreInfoClicks;
                        entry.MapClicks = account.MapClicks;
                        entry.DirectionsClicks = account.DirectionsClicks;

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

        public CreateNotificationReturn UpdateRestSearchResponseLog(CRMRestSearchResponseLogModel logData, CRMKind crmKind)
        {
            using (MongoRepository<CRMRestSearchResponseLog> repo = new MongoRepository<CRMRestSearchResponseLog>())
            {
                try
                {
                    CRMRestSearchResponseLog entry = repo.Where(x => x.AccountObjectID == logData.AccountObjectID && x.CRMKind == crmKind.ToString()).FirstOrDefault();
                    if (entry!=null)
                    {
                        entry.LastUpdatedDate = logData.LastUpdatedDate;
                        entry.LocatorPageviews = logData.LocatorPageviews;
                        entry.UrlClicks = logData.UrlClicks;
                        entry.EmailClicks = logData.EmailClicks;
                        entry.MoreInfoClicks = logData.MoreInfoClicks;
                        entry.MapClicks = logData.MapClicks;
                        entry.DirectionsClicks = logData.DirectionsClicks;

                        repo.Update(entry);
                    }
                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception ex)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public List<CRMRestSearchResponseLogModel> GetRestSearchResponseLogByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<CRMRestSearchResponseLog> repo = new MongoRepository<CRMRestSearchResponseLog>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Select(c => new CRMRestSearchResponseLogModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        AccountName = c.AccountName,
                        LocatorPageviews = c.LocatorPageviews,
                        UrlClicks = c.UrlClicks,
                        EmailClicks = c.EmailClicks,
                        MoreInfoClicks = c.MoreInfoClicks,
                        MapClicks = c.MapClicks,
                        DirectionsClicks = c.DirectionsClicks,
                        CreateDate = c.CreateDate
                    }).ToList();
            }
        }

        public IEnumerable<CRMRestSearchResponseLogModel> GetRestSearchResponseLogByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMRestSearchResponseLog> repo = new MongoRepository<CRMRestSearchResponseLog>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID) && (c.CreateDate >= startDate && c.CreateDate <= endDate))
                    .Select(c => new CRMRestSearchResponseLogModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        AccountName = c.AccountName,
                        LocatorPageviews = c.LocatorPageviews,
                        UrlClicks = c.UrlClicks,
                        EmailClicks = c.EmailClicks,
                        MoreInfoClicks = c.MoreInfoClicks,
                        MapClicks = c.MapClicks,
                        DirectionsClicks = c.DirectionsClicks,
                        CreateDate = c.CreateDate
                    }).ToList().OrderBy(o => o.CreateDate);
            }
        }

        public List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            List<CRMRestSearchResponseLogModel> logs =
                GetRestSearchResponseLogByAccountObjectIDs(accountObjectIDs).Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate).OrderBy(o => o.CreateDate).ToList();

            return logs.GroupBy(l => l.AccountObjectID)
                .Select(group => new CRMRestSearchResponseLogModel
                {
                    AccountObjectID = group.Key,
                    AccountName = group.Select(c => c.AccountName).First(),
                    LocatorPageviews = group.Sum(c => c.LocatorPageviews),
                    UrlClicks = group.Sum(c => c.UrlClicks),
                    EmailClicks = group.Sum(c => c.EmailClicks),
                    MoreInfoClicks = group.Sum(c => c.MoreInfoClicks),
                    MapClicks = group.Sum(c => c.MapClicks),
                    DirectionsClicks = group.Sum(c => c.DirectionsClicks)
                }).ToList();
        }

        public List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByAccountObjectIDs(IEnumerable<CRMRestSearchResponseLogModel> logs)
        {
            return logs.GroupBy(l => l.AccountObjectID)
                .Select(group => new CRMRestSearchResponseLogModel
                {
                    AccountObjectID = group.Key,
                    AccountName = group.Select(c => c.AccountName).First(),
                    LocatorPageviews = group.Sum(c => c.LocatorPageviews),
                    UrlClicks = group.Sum(c => c.UrlClicks),
                    EmailClicks = group.Sum(c => c.EmailClicks),
                    MoreInfoClicks = group.Sum(c => c.MoreInfoClicks),
                    MapClicks = group.Sum(c => c.MapClicks),
                    DirectionsClicks = group.Sum(c => c.DirectionsClicks)
                }).ToList();
        }

        public List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByDate(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            List<CRMRestSearchResponseLogModel> logs =
                GetRestSearchResponseLogByAccountObjectIDs(accountObjectIDs).Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate).OrderBy(o => o.CreateDate).ToList();

            return logs.GroupBy(l => l.CreateDate)
                .Select(group => new CRMRestSearchResponseLogModel
                {
                    CreateDate = group.Key,
                    LocatorPageviews = group.Sum(c => c.LocatorPageviews),
                    UrlClicks = group.Sum(c => c.UrlClicks),
                    EmailClicks = group.Sum(c => c.EmailClicks),
                    MoreInfoClicks = group.Sum(c => c.MoreInfoClicks),
                    MapClicks = group.Sum(c => c.MapClicks),
                    DirectionsClicks = group.Sum(c => c.DirectionsClicks)
                }).ToList();
        }

        public List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByDate(IEnumerable<CRMRestSearchResponseLogModel> logs)
        {
            return logs.GroupBy(l => l.CreateDate)
                .Select(group => new CRMRestSearchResponseLogModel
                {
                    CreateDate = group.Key,
                    LocatorPageviews = group.Sum(c => c.LocatorPageviews),
                    UrlClicks = group.Sum(c => c.UrlClicks),
                    EmailClicks = group.Sum(c => c.EmailClicks),
                    MoreInfoClicks = group.Sum(c => c.MoreInfoClicks),
                    MapClicks = group.Sum(c => c.MapClicks),
                    DirectionsClicks = group.Sum(c => c.DirectionsClicks)
                }).ToList();
        }

    }
}
