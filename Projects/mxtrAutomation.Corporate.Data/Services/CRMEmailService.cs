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
    public class CRMEmailService : MongoRepository<CRMEmail>, ICRMEmailServiceInternal
    {
        public CreateNotificationReturn CreateCRMEmail(CRMEmailDataModel crmEmailDataModel)
        {
            CRMEmail entry = new CRMEmail()
            {
                AccountObjectID = crmEmailDataModel.AccountObjectID,
                MxtrAccountID = crmEmailDataModel.MxtrAccountID,
                CRMKind = crmEmailDataModel.CRMKind,
                CreateDate = crmEmailDataModel.CreateDate,
                LastUpdatedDate = crmEmailDataModel.LastUpdatedDate,
                EmailID = crmEmailDataModel.EmailID,
                Title = crmEmailDataModel.Title,
                Subject = crmEmailDataModel.Subject,
                Thumbnail = crmEmailDataModel.Thumbnail,
                CreateTimestamp = crmEmailDataModel.CreateTimestamp
            };

            using (MongoRepository<CRMEmail> repo = new MongoRepository<CRMEmail>())
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

        public CreateNotificationReturn CreateBatchCRMEmails(List<CRMEmailDataModel> crmEmailDataModels)
        {
            List<CRMEmail> entries = crmEmailDataModels
                .Select(c => new CRMEmail()
                {
                    AccountObjectID = c.AccountObjectID,
                    MxtrAccountID = c.MxtrAccountID,
                    CRMKind = c.CRMKind,
                    CreateDate = c.CreateDate,
                    LastUpdatedDate = c.LastUpdatedDate,
                    EmailID = c.EmailID,
                    Title = c.Title,
                    Subject = c.Subject,
                    Thumbnail = c.Thumbnail,
                    CreateTimestamp = c.CreateTimestamp
                }).ToList();

            using (MongoRepository<CRMEmail> repo = new MongoRepository<CRMEmail>())
            {
                try
                {
                    // Remove entries which already exist in database as we added lead to database manually using copy lead feature
                    entries.RemoveAll(x => repo.Any(y => y.EmailID == x.EmailID && y.AccountObjectID == x.AccountObjectID && y.CRMKind == x.CRMKind));

                    repo.Add(entries);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateEmail(CRMEmailDataModel emailData)
        {
            using (MongoRepository<CRMEmail> repo = new MongoRepository<CRMEmail>())
            {
                try
                {
                    CRMEmail entry = repo
                        .Where(x => x.AccountObjectID == emailData.AccountObjectID && x.EmailID == emailData.EmailID && x.CRMKind == emailData.CRMKind)
                        .FirstOrDefault();

                    entry.Title = emailData.Title;
                    entry.Subject = emailData.Subject;
                    entry.Thumbnail = emailData.Thumbnail;
                    entry.LastUpdatedDate = emailData.LastUpdatedDate;

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateEmails(List<CRMEmailDataModel> emailDatas, string accountObjectID, string crmKind)
        {
            List<long> emailIDs = emailDatas.Select(x => x.EmailID).ToList();

            using (MongoRepository<CRMEmail> repo = new MongoRepository<CRMEmail>())
            {
                try
                {
                    IEnumerable<CRMEmail> entries = repo
                        .Where(x => x.AccountObjectID == accountObjectID && x.CRMKind == crmKind)
                        .Where(x => emailIDs.Contains(x.EmailID));

                    foreach (CRMEmail entry in entries)
                    {
                        CRMEmailDataModel email = emailDatas.Where(c => c.EmailID == entry.EmailID).FirstOrDefault();

                        entry.LastUpdatedDate = email.LastUpdatedDate;
                        entry.Title = email.Title;
                        entry.Subject = email.Subject;
                        entry.Thumbnail = email.Thumbnail;

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

        public CreateNotificationReturn UpsertEmails(List<CRMEmailDataModel> emailDatas, string accountObjectID, string crmKind)
        {
            List<string> accountObjectIDs = new List<string>();
            accountObjectIDs.Add(accountObjectID);
            List<CRMEmailDataModel> existingEmails = GetEmailsByAccountObjectIDs(accountObjectIDs);
            List<long> emailIDs = existingEmails.Select(x => x.EmailID).ToList();
            List<CRMEmailDataModel> newEmails = emailDatas.Where(x => !emailIDs.Contains(x.EmailID)).ToList();

            //CreateNotificationReturn updates = UpdateEmails(emailDatas, accountObjectID, crmKind);
            CreateNotificationReturn inserts = CreateBatchCRMEmails(newEmails);

            //if (updates.Success && inserts.Success)
            if (inserts.Success)
            {
                return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
            }
            else
            {
                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }


        public List<CRMEmailDataModel> GetEmailsByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<CRMEmail> repo = new MongoRepository<CRMEmail>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Select(c => new CRMEmailDataModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        MxtrAccountID = c.MxtrAccountID,
                        CreateDate = c.CreateDate,
                        EmailID = c.EmailID,
                        Title = c.Title,
                        Subject = c.Subject,
                        Thumbnail = c.Thumbnail,
                        CreateTimestamp = c.CreateTimestamp
                    }).ToList();
            }
        }
        public IEnumerable<CRMEmailDataModel> GetEmailsByAccountObjectIDsAsEnumerable(List<string> accountObjectIDs)
        {
            using (MongoRepository<CRMEmail> repo = new MongoRepository<CRMEmail>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Select(c => new CRMEmailDataModel
                    {
                        AccountObjectID = c.AccountObjectID,
                        MxtrAccountID = c.MxtrAccountID,
                        CreateDate = c.CreateDate,
                        EmailID = c.EmailID,
                        Title = c.Title,
                        Subject = c.Subject,
                        Thumbnail = c.Thumbnail,
                        CreateTimestamp = c.CreateTimestamp
                    }).ToList();
            }
        }
    }
}
