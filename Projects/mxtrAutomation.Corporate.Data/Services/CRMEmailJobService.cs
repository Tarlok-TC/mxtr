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
    public class CRMEmailJobService : MongoRepository<CRMEmailJob>, ICRMEmailJobServiceInternal
    {
        public CreateNotificationReturn CreateCRMEmailJob(CRMEmailJobDataModel crmEmailJobDataModel)
        {
            CRMEmailJob entry = new CRMEmailJob()
            {
                AccountObjectID = crmEmailJobDataModel.AccountObjectID,
                MxtrAccountID = crmEmailJobDataModel.MxtrAccountID,
                CRMKind = crmEmailJobDataModel.CRMKind,
                CreateDate = crmEmailJobDataModel.CreateDate,
                LastUpdatedDate = crmEmailJobDataModel.LastUpdatedDate,
                EmailJobID = crmEmailJobDataModel.EmailJobID,
                IsList = crmEmailJobDataModel.IsList,
                IsActive = crmEmailJobDataModel.IsActive,
                RecipientID = crmEmailJobDataModel.RecipientID,
                SendCount = crmEmailJobDataModel.SendCount,
                CreateTimestamp = crmEmailJobDataModel.CreateTimestamp,
                Events = crmEmailJobDataModel.Events
            };

            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
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

        public CreateNotificationReturn CreateBatchCRMEmailJobs(List<CRMEmailJobDataModel> crmEmailJobDataModels)
        {
            List<CRMEmailJob> entries = crmEmailJobDataModels
                .Select(c => new CRMEmailJob()
                {
                    AccountObjectID = c.AccountObjectID,
                    MxtrAccountID = c.MxtrAccountID,
                    CRMKind = c.CRMKind,
                    CreateDate = c.CreateDate,
                    LastUpdatedDate = c.LastUpdatedDate,
                    EmailJobID = c.EmailJobID,
                    IsList = c.IsList,
                    IsActive = c.IsActive,
                    RecipientID = c.RecipientID,
                    SendCount = c.SendCount,
                    CreateTimestamp = c.CreateTimestamp,
                    Events = c.Events
                }).ToList();

            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                try
                {
                    var emailJobs = entries.Where(x => repo.Any(y => y.EmailJobID == x.EmailJobID && y.AccountObjectID == x.AccountObjectID && y.CRMKind == x.CRMKind));
                    if (emailJobs != null && emailJobs.Count() > 0)
                    {
                        // update
                        foreach (var item in emailJobs)
                        {
                            UpdateEmailJob(item);
                        }
                    }

                    // Remove entries which already exist in database
                    entries.RemoveAll(x => repo.Any(y => y.EmailJobID == x.EmailJobID && y.AccountObjectID == x.AccountObjectID && y.CRMKind == x.CRMKind));

                    repo.Add(entries);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateEmailJob(CRMEmailJob emailJobData)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                try
                {
                    CRMEmailJob entry = repo
                        .Where(x => x.AccountObjectID == emailJobData.AccountObjectID && x.EmailJobID == emailJobData.EmailJobID && x.CRMKind == emailJobData.CRMKind)
                        .FirstOrDefault();

                    entry.IsList = emailJobData.IsList;
                    entry.IsActive = emailJobData.IsActive;
                    entry.RecipientID = emailJobData.RecipientID;
                    entry.SendCount = emailJobData.SendCount;
                    entry.CreateTimestamp = emailJobData.CreateTimestamp;
                    entry.Events = emailJobData.Events;

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateEmailJob(CRMEmailJobDataModel emailJobData)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                try
                {
                    CRMEmailJob entry = repo
                        .Where(x => x.AccountObjectID == emailJobData.AccountObjectID && x.EmailJobID == emailJobData.EmailJobID && x.CRMKind == emailJobData.CRMKind)
                        .FirstOrDefault();

                    entry.IsList = emailJobData.IsList;
                    entry.IsActive = emailJobData.IsActive;
                    entry.RecipientID = emailJobData.RecipientID;
                    entry.SendCount = emailJobData.SendCount;
                    entry.CreateTimestamp = emailJobData.CreateTimestamp;
                    entry.Events = emailJobData.Events;

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateEmailJobs(List<CRMEmailJobDataModel> emailJobDatas, string accountObjectID, string crmKind)
        {
            List<long> emailJobIDs = emailJobDatas.Select(x => x.EmailJobID).ToList();

            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                try
                {
                    IEnumerable<CRMEmailJob> entries = repo
                        .Where(x => x.AccountObjectID == accountObjectID && x.CRMKind == crmKind)
                        .Where(x => emailJobIDs.Contains(x.EmailJobID));

                    foreach (CRMEmailJob entry in entries)
                    {
                        CRMEmailJobDataModel emailJob = emailJobDatas.Where(c => c.EmailJobID == entry.EmailJobID).FirstOrDefault();

                        entry.LastUpdatedDate = emailJob.LastUpdatedDate;
                        entry.IsList = emailJob.IsList;
                        entry.IsActive = emailJob.IsActive;
                        entry.RecipientID = emailJob.RecipientID;
                        entry.SendCount = emailJob.SendCount;
                        entry.CreateTimestamp = emailJob.CreateTimestamp;
                        entry.Events = emailJob.Events;

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

        public List<CRMEmailJobDataModel> GetEmailJobsByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Select(c => new CRMEmailJobDataModel
                    {
                        EmailJobID = c.EmailJobID,
                        IsList = c.IsList,
                        IsActive = c.IsActive,
                        RecipientID = c.RecipientID,
                        SendCount = c.SendCount,
                        CreateTimestamp = c.CreateTimestamp,
                        Events = c.Events
                    }).ToList();
            }
        }

        public List<CRMEmailJobDataModel> GetEmailJobsByAccountObjectIDsForDateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate)
                    .Select(c => new CRMEmailJobDataModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        EmailJobID = c.EmailJobID,
                        IsList = c.IsList,
                        IsActive = c.IsActive,
                        RecipientID = c.RecipientID,
                        SendCount = c.SendCount,
                        CreateTimestamp = c.CreateTimestamp,
                        Events = c.Events
                    }).ToList();
            }
        }

        public IEnumerable<CRMEmailJobDataModel> GetEmailJobsByAccountObjectIDsForDateRangeAsEnumerable(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate)
                    .Select(c => new CRMEmailJobDataModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        EmailJobID = c.EmailJobID,
                        IsList = c.IsList,
                        IsActive = c.IsActive,
                        RecipientID = c.RecipientID,
                        SendCount = c.SendCount,
                        CreateTimestamp = c.CreateTimestamp,
                        Events = c.Events
                    });
            }
        }

        public List<CRMEmailJobDataModel> GetEmailJobsGroupedByAccountObjectIDsForDateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                List<CRMEmailJobDataModel> emails = repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate)
                    .OrderBy(o => o.CreateDate)
                    .Select(c => new CRMEmailJobDataModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        EmailJobID = c.EmailJobID,
                        IsList = c.IsList,
                        IsActive = c.IsActive,
                        RecipientID = c.RecipientID,
                        SendCount = c.SendCount,
                        CreateTimestamp = c.CreateTimestamp,
                        Events = c.Events
                    }).ToList();

                return emails.GroupBy(l => l.AccountObjectID)
                .Select(group => new CRMEmailJobDataModel
                {
                    AccountObjectID = group.Key,
                    SendCount = group.Sum(c => c.SendCount)
                }).ToList();
            }
        }

        public List<CRMEmailJobDataModel> GetEmailJobsGroupedByAccountObjectIDsForDateRange(IEnumerable<CRMEmailJobDataModel> emails)
        {
            return emails.GroupBy(l => l.AccountObjectID)
             .Select(group => new CRMEmailJobDataModel
             {
                 AccountObjectID = group.Key,
                 SendCount = group.Sum(c => c.SendCount)
             }).ToList();
        }

        public List<CRMEmailJobDataModel> GetEmailJobsGroupedByDate(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                List<CRMEmailJobDataModel> emails = repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate)
                    .OrderBy(o => o.CreateDate)
                    .Select(c => new CRMEmailJobDataModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        EmailJobID = c.EmailJobID,
                        IsList = c.IsList,
                        IsActive = c.IsActive,
                        RecipientID = c.RecipientID,
                        SendCount = c.SendCount,
                        CreateTimestamp = c.CreateTimestamp,
                        Events = c.Events
                    }).ToList();

                return emails.GroupBy(l => l.CreateTimestamp.Date)
                .Select(group => new CRMEmailJobDataModel
                {
                    CreateDate = group.Key,
                    SendCount = group.Sum(c => c.SendCount)
                }).ToList();
            }
        }

        public List<CRMEmailJobDataModel> GetEmailJobsGroupedByDate(IEnumerable<CRMEmailJobDataModel> emails)
        {
            return emails.GroupBy(l => l.CreateTimestamp.Date)
             .Select(group => new CRMEmailJobDataModel
             {
                 CreateDate = group.Key,
                 SendCount = group.Sum(c => c.SendCount)
             }).ToList();
        }

        public IEnumerable<CRMEmailJobDataModel> GetEmailJobs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMEmailJob> repo = new MongoRepository<CRMEmailJob>())
            {
                return repo
                       .Where(c => accountObjectIDs.Contains(c.AccountObjectID) && (c.CreateDate >= startDate && c.CreateDate <= endDate))
                       .Select(c => new CRMEmailJobDataModel
                       {
                           AccountObjectID = c.AccountObjectID,
                           EmailJobID = c.EmailJobID,
                           IsList = c.IsList,
                           IsActive = c.IsActive,
                           RecipientID = c.RecipientID,
                           SendCount = c.SendCount,
                           CreateTimestamp = c.CreateTimestamp,
                           Events = c.Events
                       }).ToList().OrderBy(o => o.CreateDate);
            }
        }

    }
}
