using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System.IO;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class CRMLeadService : MongoRepository<CRMLead>, ICRMLeadServiceInternal
    {
        public CreateNotificationReturn CreateCRMLead(CRMLeadDataModel crmLeadDataModel)
        {
            CRMLead entry = new CRMLead()
            {
                AccountObjectID = crmLeadDataModel.AccountObjectID,
                MxtrAccountID = crmLeadDataModel.MxtrAccountID,
                CRMKind = crmLeadDataModel.CRMKind,
                CreateDate = crmLeadDataModel.CreateDate,
                LastUpdatedDate = crmLeadDataModel.LastUpdatedDate,
                LeadID = crmLeadDataModel.LeadID,
                AccountID = crmLeadDataModel.AccountID,
                OwnerID = crmLeadDataModel.OwnerID,
                CampaignID = crmLeadDataModel.CampaignID,
                LeadStatus = crmLeadDataModel.LeadStatus,
                LeadScore = crmLeadDataModel.LeadScore,
                IsActive = crmLeadDataModel.IsActive,
                FirstName = crmLeadDataModel.FirstName,
                LastName = crmLeadDataModel.LastName,
                EmailAddress = crmLeadDataModel.EmailAddress,
                CompanyName = crmLeadDataModel.CompanyName,
                Title = crmLeadDataModel.Title,
                Street = crmLeadDataModel.Street,
                City = crmLeadDataModel.City,
                Country = crmLeadDataModel.Country,
                State = crmLeadDataModel.State,
                ZipCode = crmLeadDataModel.ZipCode,
                Website = crmLeadDataModel.Website,
                PhoneNumber = crmLeadDataModel.PhoneNumber,
                MobilePhoneNumber = crmLeadDataModel.MobilePhoneNumber,
                FaxNumber = crmLeadDataModel.FaxNumber,
                Description = crmLeadDataModel.Description,
                Industry = crmLeadDataModel.Industry,
                IsUnsubscribed = crmLeadDataModel.IsUnsubscribed,
                ClonedLeadID = crmLeadDataModel.ClonedLeadID,
                CopiedToParent = crmLeadDataModel.CopiedToParent,
                OriginalLeadID = crmLeadDataModel.OriginalLeadID,
                Events = crmLeadDataModel.Events
            };

            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                try
                {
                    var lead = repo.FirstOrDefault(x => x.LeadID == entry.LeadID && x.AccountObjectID == entry.AccountObjectID && x.CRMKind == entry.CRMKind);
                    if (lead == null)
                    {
                        repo.Add(entry);
                    }
                    else
                    {
                        entry.Id = lead.Id;
                        entry.CreateDate = lead.CreateDate;
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

        public CreateNotificationReturn CreateBatchCRMLeads(List<CRMLeadDataModel> crmLeadDataModels)
        {
            List<CRMLead> entries = crmLeadDataModels
                .Select(l => new CRMLead()
                {
                    AccountObjectID = l.AccountObjectID,
                    MxtrAccountID = l.MxtrAccountID,
                    CRMKind = l.CRMKind,
                    CreateDate = l.CreateDate,
                    LastUpdatedDate = l.LastUpdatedDate,
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
                    Events = l.Events
                }).ToList();

            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                try
                {
                    // Remove entries which already exist in database as we added lead to database manually using copy lead feature
                    entries.RemoveAll(x => repo.Any(y => y.LeadID == x.LeadID && y.AccountObjectID == x.AccountObjectID && y.CRMKind == x.CRMKind));

                    repo.Add(entries);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateLeads(List<CRMLeadDataModel> leadDatas, string accountObjectID, string crmKind)
        {
            List<long> leadIDs = leadDatas.Select(x => x.LeadID).ToList();

            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                try
                {
                    List<CRMLead> entries = repo
                        //.Where(x => x.AccountObjectID == accountObjectID && x.CRMKind == crmKind)
                        .Where(x => leadIDs.Contains(x.LeadID)).ToList();

                    foreach (CRMLead entry in entries)
                    {
                        CRMLeadDataModel lead = leadDatas.Where(l => l.LeadID == entry.LeadID).FirstOrDefault();

                        #region Add new events in lead data
                        var leadEvents = repo.Where(l => l.LeadID == entry.LeadID).FirstOrDefault();
                        var leadExistingEvents = leadEvents.Events.Select(e => new CRMLeadEventDataModel
                        {
                            CreateTimestamp = e.CreateTimestamp,
                            EventID = e.EventID,
                            LeadID = e.LeadID,
                            EventName = e.EventName,
                            WhatID = e.WhatID,
                            WhatType = e.WhatType,
                            LeadAccountName = e.LeadAccountName,
                            EventData = e.EventData.Select(ed => new CRMLeadEventEventData
                            {
                                Name = ed.Name,
                                Value = ed.Value
                            }).ToList(),
                        }).ToList();

                        if (leadExistingEvents != null && leadExistingEvents.Count > 0)
                        {
                            lead.Events.AddRange(leadExistingEvents);
                            lead.Events = lead.Events.GroupBy(x => x.EventID).Select(x => x.First()).OrderBy(o => o.CreateTimestamp).ToList();
                        }
                        #endregion

                        entry.LastUpdatedDate = lead.LastUpdatedDate;
                        entry.OwnerID = lead.OwnerID;
                        entry.LeadScore = lead.LeadScore;
                        entry.LeadStatus = lead.LeadStatus;
                        entry.IsActive = lead.IsActive;
                        entry.FirstName = lead.FirstName;
                        entry.LastName = lead.LastName;
                        entry.EmailAddress = lead.EmailAddress;
                        entry.CompanyName = lead.CompanyName;
                        entry.Title = lead.Title;
                        entry.Street = lead.Street;
                        entry.City = lead.City;
                        entry.Country = lead.Country;
                        entry.State = lead.State;
                        entry.ZipCode = lead.ZipCode;
                        entry.Website = lead.Website;
                        entry.PhoneNumber = lead.PhoneNumber;
                        entry.MobilePhoneNumber = lead.MobilePhoneNumber;
                        entry.FaxNumber = lead.FaxNumber;
                        entry.Description = lead.Description;
                        entry.Industry = lead.Industry;
                        entry.IsUnsubscribed = lead.IsUnsubscribed;
                        entry.Events = lead.Events;

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

        public CreateNotificationReturn DeleteLead(long leadId)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                try
                {
                    CRMLead lead = repo.Where(w => w.LeadID == leadId).FirstOrDefault();
                    if (lead != null)
                    {
                        lead.IsActive = false;
                        repo.Update(lead);
                        return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                    }
                    else
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                    }
                }
                catch (Exception ex)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn DeleteLead(string leadId)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                try
                {
                    CRMLead lead = repo.Where(w => w.Id == leadId).FirstOrDefault();
                    if (lead != null)
                    {
                        repo.Delete(lead);
                        return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                    }
                    else
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                    }
                }
                catch (Exception ex)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        private IEnumerable<CRMLeadDataModel> GetLeadsByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                return repo
                    .Where(l => accountObjectIDs.Contains(l.AccountObjectID))
                    .Select(l => new CRMLeadDataModel
                    {
                        ObjectID = l.Id,
                        AccountObjectID = l.AccountObjectID,
                        MxtrAccountID = l.MxtrAccountID,
                        CRMKind = l.CRMKind,
                        CreateDate = l.CreateDate,
                        LastUpdatedDate = l.LastUpdatedDate,
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
                        Events = l.Events
                    });
            }
        }
        public IEnumerable<CRMLeadDataModel> GetLeadsByAccountObjectIDs_DateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                return repo.Where(l => accountObjectIDs.Contains(l.AccountObjectID) && (l.CreateDate >= startDate && l.CreateDate <= endDate) && l.IsActive)
                     .Select(l => new CRMLeadDataModel
                     {
                         ObjectID = l.Id,
                         AccountObjectID = l.AccountObjectID,
                         MxtrAccountID = l.MxtrAccountID,
                         CRMKind = l.CRMKind,
                         CreateDate = l.CreateDate,
                         LastUpdatedDate = l.LastUpdatedDate,
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
                         Events = l.Events,
                     }).ToList();
            }
        }

        public IEnumerable<CRMLeadDataModel> GetLeadsByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var leads = repo.Where(l => accountObjectIDs.Contains(l.AccountObjectID) && (l.CreateDate >= startDate && l.CreateDate <= endDate))
                    .Select(l => new CRMLeadDataModel
                    {
                        ObjectID = l.Id,
                        AccountObjectID = l.AccountObjectID,
                        MxtrAccountID = l.MxtrAccountID,
                        CRMKind = l.CRMKind,
                        CreateDate = l.CreateDate,
                        LastUpdatedDate = l.LastUpdatedDate,
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
                        Events = l.Events,
                        OriginalLeadID = l.OriginalLeadID,
                        ClonedLeadID = l.ClonedLeadID,
                        CopiedToParent = l.CopiedToParent,
                    }).ToList();

                var lstLeadsClonedFrom = leads.Where(w => w.ClonedLeadID > 0 && w.CopiedToParent == false).ToList();
                //var lstLeadsClonedFrom = leads.Where(w => w.ClonedLeadID > 0 ).ToList();
                var lstLeadsClonedFromId = lstLeadsClonedFrom.Select(x => x.AccountObjectID).ToList();

                //var clonedLeads = repo.Where(l => accountObjectIDs.Contains(l.AccountObjectID)
                //  && l.OriginalLeadID != null && l.ClonedLeadID > 0).ToList();

                if (leads != null && leads.Count > 0)
                {
                    using (MongoRepository<Account> repoAccount = new MongoRepository<Account>())
                    {
                        //List<string> lst = leads.Select(l => l.AccountObjectID).ToList();
                        //List<Account> accounts = repoAccount.Where(r => lst.Contains(r.Id)).ToList();
                        List<Account> accounts = repoAccount.Where(l => accountObjectIDs.Contains(l.Id)).ToList();
                        if (accounts != null && accounts.Count > 0)
                        {
                            foreach (var lead in leads)
                            {
                                var accountData = accounts.Where(w => w.Id == lead.AccountObjectID).FirstOrDefault(); ;
                                if (accountData != null)
                                {
                                    lead.LeadParentAccount = accountData.AccountName;
                                }
                            }
                        }
                        List<Account> accountsCloned = repoAccount.Where(l => lstLeadsClonedFromId.Contains(l.Id)).ToList();
                        foreach (var item in lstLeadsClonedFrom)
                        {
                            var leaddata = leads.FirstOrDefault(x => x.ObjectID == item.ObjectID);
                            var accountClonedLeadData = accountsCloned.Where(w => w.Id == item.AccountObjectID).FirstOrDefault();
                            if (accountClonedLeadData != null)
                            {
                                leaddata.CopiedToDealerAccount = accountClonedLeadData.AccountName;
                            }
                        }

                    };
                }

                return leads.ToList().OrderBy(o => o.CreateDate);
            }
        }
        public IEnumerable<CRMLeadDataModel> GetLeadsCreatedDateByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var leads = repo.Where(l => accountObjectIDs.Contains(l.AccountObjectID) && (l.CreateDate >= startDate && l.CreateDate <= endDate))
                    .Select(l => new CRMLeadDataModel
                    {
                        ObjectID = l.Id,
                        AccountObjectID = l.AccountObjectID,
                        MxtrAccountID = l.MxtrAccountID,                      
                        CreateDate = l.CreateDate,
                    });
                return leads.ToList().OrderBy(o => o.CreateDate);
            }
        }
        public CRMLeadDataModel GetLeadsByObjectID(string objectID)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var lead = repo
                    .Where(l => l.Id == objectID)
                    .Select(l => new CRMLeadDataModel
                    {
                        AccountObjectID = l.AccountObjectID,
                        MxtrAccountID = l.MxtrAccountID,
                        CRMKind = l.CRMKind,
                        CreateDate = l.CreateDate,
                        LastUpdatedDate = l.LastUpdatedDate,
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
                        ClonedLeadID = l.ClonedLeadID,
                        CopiedToParent = l.CopiedToParent,
                        OriginalLeadID = l.OriginalLeadID,
                        Events = l.Events
                    }).FirstOrDefault();
                using (MongoRepository<Account> repoAccount = new MongoRepository<Account>())
                {
                    var account = repoAccount.Where(r => r.Id == lead.AccountObjectID).FirstOrDefault();
                    if (account != null)
                    {
                        lead.Events.ForEach(x => x.LeadAccountName = account.AccountName);
                        lead.Events.ForEach(x => x.CopiedToParent = lead.CopiedToParent);

                    }
                };
                return lead;
            }
        }

        public IQueryable<CRMLeadDataModel> GetLeadsByAccountObjectIDsAsQueryable(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var leads = repo.Where(l => accountObjectIDs.Contains(l.AccountObjectID) && (l.CreateDate >= startDate && l.CreateDate <= endDate))
                    .Select(l => new CRMLeadDataModel
                    {
                        ObjectID = l.Id,
                        AccountObjectID = l.AccountObjectID,
                        MxtrAccountID = l.MxtrAccountID,
                        CRMKind = l.CRMKind,
                        CreateDate = l.CreateDate,
                        LastUpdatedDate = l.LastUpdatedDate,
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
                        Events = l.Events,
                    });

                if (leads != null && leads.Count() > 0)
                {
                    using (MongoRepository<Account> repoAccount = new MongoRepository<Account>())
                    {
                        //List<string> lst = leads.Select(l => l.AccountObjectID).ToList();
                        //List<Account> accounts = repoAccount.Where(r => lst.Contains(r.Id)).ToList();
                        List<Account> accounts = repoAccount.Where(l => accountObjectIDs.Contains(l.Id)).ToList();
                        if (accounts != null && accounts.Count > 0)
                        {
                            foreach (var lead in leads)
                            {
                                var accountData = accounts.Where(w => w.Id == lead.AccountObjectID).FirstOrDefault();
                                if (accountData != null)
                                {
                                    lead.LeadParentAccount = accountData.AccountName;
                                }
                            }
                        }
                    };
                }

                return leads.OrderBy(o => o.CreateDate);
            }
        }
        public List<CRMLeadDataModel> GetLeadsByLeadIds(List<long> leadIds)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var leads = repo.Where(l => leadIds.Contains(l.LeadID)).Select(l => new CRMLeadDataModel
                {
                    ObjectID = l.Id,
                    AccountObjectID = l.AccountObjectID,
                    MxtrAccountID = l.MxtrAccountID,
                    CRMKind = l.CRMKind,
                    CreateDate = l.CreateDate,
                    LastUpdatedDate = l.LastUpdatedDate,
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
                    Events = l.Events
                }).ToList();
                return leads;
            }
        }

        public IEnumerable<CRMLeadDataModel> GetLeadEvents(IEnumerable<string> objectIDs)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                List<CRMLeadDataModel> leads = repo
                    .Where(l => objectIDs.Contains(l.Id))
                    .Select(l => new CRMLeadDataModel
                    {
                        Events = l.Events,
                        AccountObjectID = l.AccountObjectID,
                    }).ToList();
                if (leads.Count() > 0)
                {
                    using (MongoRepository<Account> repoAccount = new MongoRepository<Entities.Account>())
                    {
                        List<string> lst = leads.Select(l => l.AccountObjectID).ToList();
                        List<Account> accounts = repoAccount.Where(r => lst.Contains(r.Id)).ToList();
                        foreach (var lead in leads)
                        {
                            var accountData = accounts.Where(w => w.Id == lead.AccountObjectID).FirstOrDefault();
                            if (accountData != null)
                            {
                                lead.Events.ForEach(x => x.LeadAccountName = accountData.AccountName);
                                lead.Events.ForEach(x => x.CopiedToParent = lead.CopiedToParent);
                            }
                        }
                    }
                }
                return leads;

            }
        }
        public List<string> GetLeadsIdByClonedObjectId(string objectId)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                IEnumerable<CRMLeadDataModel> leads = repo.Where(r => r.OriginalLeadID == objectId)
                      .Select(r => new CRMLeadDataModel
                      {
                          ObjectID = r.Id
                      });
                return leads.Select(x => x.ObjectID).ToList();
            }
        }

        public List<string> GetLeadsIdByCloneLeadID(long clonedLeadID)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                IEnumerable<CRMLeadDataModel> leads = repo.Where(r => r.LeadID == clonedLeadID)
                      .Select(r => new CRMLeadDataModel
                      {
                          ObjectID = r.Id
                      });
                return leads.Select(x => x.ObjectID).ToList();
            }
        }

        public Dictionary<string, Tuple<string, bool, long>> GetAccountByCloneLeadID(long leadID)
        {
            Dictionary<string, Tuple<string, bool, long>> dicAccount = new Dictionary<string, Tuple<string, bool, long>>();
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                IEnumerable<CRMLeadDataModel> leads = repo.Where(r => r.ClonedLeadID == leadID)
                      .Select(r => new CRMLeadDataModel
                      {
                          AccountObjectID = r.AccountObjectID,
                          IsActive = r.IsActive,
                          LeadID = r.LeadID,

                      });
                if (leads.Count() > 0)
                {
                    using (MongoRepository<Account> repoAccount = new MongoRepository<Account>())
                    {
                        List<string> lst = leads.Select(l => l.AccountObjectID).ToList();
                        List<Account> accounts = repoAccount.Where(r => lst.Contains(r.Id)).ToList();
                        //List<Account> accounts = repoAccount.Where(r => leads.Select(l => l.AccountObjectID).ToList().Contains(r.Id)).ToList();
                        //assign leadStatus
                        foreach (var account in accounts)
                        {
                            var leadData = leads.Where(w => w.AccountObjectID == account.Id).FirstOrDefault();
                            if (leadData != null)
                            {
                                dicAccount.Add(account.Id, new Tuple<string, bool, long>(account.AccountName, leadData.IsActive, leadData.LeadID));
                            }
                        }
                    }
                }
                return dicAccount;
            }
        }


        public List<CRMLeadSummaryDataModel> GetLeadCountsByAccountObjectIDs(List<string> accountObjectIDs, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<CRMLeadDataModel> leads = new List<CRMLeadDataModel>();
            if (startDate == null && endDate == null)
            {
                leads = GetLeadsByAccountObjectIDs(accountObjectIDs);
            }
            else
            {
                leads = GetLeadsByAccountObjectIDs(accountObjectIDs).Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate).OrderBy(o => o.CreateDate).ToList();
            }

            return leads.GroupBy(l => l.AccountObjectID)
                .Select(group => new CRMLeadSummaryDataModel
                {
                    AccountObjectID = group.Key,
                    LeadCount = group.Count()
                }).ToList();
        }
        public List<CRMLeadSummaryDataModel> GetLeadCountsByAccountObjectIDs(IEnumerable<CRMLeadDataModel> leads)
        {
            return leads.GroupBy(l => l.AccountObjectID)
                .Select(group => new CRMLeadSummaryDataModel
                {
                    AccountObjectID = group.Key,
                    LeadCount = group.Count()
                }).ToList();
        }

        public List<CRMLeadSummaryDataModel> GetLeadCountsByDate(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            List<CRMLeadDataModel> leads = GetLeadsByAccountObjectIDs(accountObjectIDs).Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate).OrderBy(o => o.CreateDate).ToList();

            return leads.GroupBy(l => l.CreateDate)
                .Select(group => new CRMLeadSummaryDataModel
                {
                    CreateDate = group.Key,
                    LeadCount = group.Count()
                }).ToList();
        }

        public List<CRMLeadSummaryDataModel> GetLeadCountsByDate(IEnumerable<CRMLeadDataModel> leads)
        {
            return leads.GroupBy(l => l.CreateDate)
                .Select(group => new CRMLeadSummaryDataModel
                {
                    CreateDate = group.Key,
                    LeadCount = group.Count()
                }).ToList();
        }

        public List<CRMLeadDataModel> GetLeadsByAccountObjectIDsForDateRange(List<string> accountObjectIDs, DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null && endDate == null)
            {
                return GetLeadsByAccountObjectIDs(accountObjectIDs).ToList();
            }
            return GetLeadsByAccountObjectIDs(accountObjectIDs).Where(l => l.CreateDate >= startDate && l.CreateDate <= endDate).ToList();
        }

        public int GetLeadScores(List<long> leadid)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var result = repo.Where(l => leadid.Contains(l.LeadID)).ToList();
                return result.Sum(x => x.LeadScore);
            }
        }

        public int GetTotalLeads(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                return repo.AsQueryable()
                   .Where(l => accountObjectIDs.Contains(l.AccountObjectID) && l.IsActive && l.CreateDate >= startDate && l.CreateDate <= endDate)
                   .Count();
            }
        }

        public List<GroupLeadsDataModel> GetGroupLeads(List<string> accountObjectIDs, List<mxtrAccount> accounts, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var Leads = repo.AsQueryable()
                    .Where(l => accountObjectIDs.Contains(l.AccountObjectID) && l.IsActive && l.CreateDate >= startDate && l.CreateDate <= endDate)
                    .Select(l => new
                    {
                        AccountObjectID = l.AccountObjectID
                    }).ToList();

                return Leads.Join(accounts,
                      lead => lead.AccountObjectID,
                      account => account.ObjectID,
                      (lead, account) => new { AccountObjectID = lead.AccountObjectID, State = account.State })
                      .GroupBy(l => l.State)
                      .Select(group => new GroupLeadsDataModel()
                      {
                          State = group.Key,
                          TotalLeads = group.Count()
                      }).ToList();
            }
        }
        //--------Shaw KPI --------------
        public double GetAveragePassToDealerDays(string mxtrAccountObjectId)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                //---mxtrAccountObjectId = "592fc8cab61cf21f34adaef4";
                //var clonedLeads = repo.Where(l => l.AccountObjectID == mxtrAccountObjectId
                //&& l.OriginalLeadID != null && l.ClonedLeadID > 0).ToList();
                var allLead = repo.Where(l => l.AccountObjectID == mxtrAccountObjectId).ToList();

                var allLeadIds = allLead.Where(l => l.AccountObjectID == mxtrAccountObjectId).Select(x => x.LeadID).ToList();

                var clonedLeads = repo.Where(l => allLeadIds.Contains(l.ClonedLeadID) && l.ClonedLeadID > 0).ToList();

                var clonedLeadIds = clonedLeads.Select(x => x.OriginalLeadID).ToList();

                var originalLeads = allLead.Where(w => clonedLeadIds.Contains(w.Id)).ToList();

                List<double> days = new List<double>();
                foreach (var item in originalLeads)
                {
                    DateTime copiedDate = clonedLeads.LastOrDefault(w => w.OriginalLeadID == item.Id).CreateDate;
                    double daydiff = (item.CreateDate - copiedDate).TotalDays;
                    if (daydiff > 0)
                    {
                        days.Add(daydiff);
                    }
                }
                if (days.Count == 0)
                    return 0;

                return days.Sum() / days.Count;
            }
        }
        public double GetAverageCreateToSaleDate(List<long> leadIds)
        {
            //---leadIds.Add(528439592963);
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var lstleads = repo.Where(l => leadIds.Contains(l.LeadID)).ToList();
                using (MongoRepository<CRMOpportunity> repoOpportunity = new MongoRepository<CRMOpportunity>())
                {
                    var leadsWonOpportunity = repoOpportunity.Where(l => leadIds.Contains(l.PrimaryLeadID) && l.IsWon).ToList();
                    List<double> days = new List<double>();
                    foreach (var item in lstleads)
                    {
                        var leadOpportunity = leadsWonOpportunity.Where(w => w.PrimaryLeadID == item.LeadID).ToList();
                        foreach (var opportunity in leadOpportunity)
                        {
                            DateTime? wonDate = opportunity.CloseDate;
                            double daydiff = wonDate == null ? 0 : (item.CreateDate - Convert.ToDateTime(wonDate)).TotalDays;
                            if (daydiff > 0)
                            {
                                days.Add(daydiff);
                            }
                        }
                    }
                    if (days.Count == 0)
                        return 0;

                    return days.Sum() / days.Count;
                }
            }
        }
        public int GetCopiedLeads(string mxtrAccountObjectId)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var clonedLeads = repo.Where(l => l.AccountObjectID == mxtrAccountObjectId
                && l.OriginalLeadID != null && l.ClonedLeadID > 0).ToList();

                return clonedLeads.Count;
            }
        }
        public int GetCopiedLeads(List<string> dealersAccountIds, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                if (startDate == null && endDate == null)
                {
                    var clonedLeads = repo.Where(l => dealersAccountIds.Contains(l.AccountObjectID)
                    && l.OriginalLeadID != null && l.ClonedLeadID > 0).ToList();
                    return clonedLeads.Count;
                }
                else
                {
                    var clonedLeads = repo.Where(l => dealersAccountIds.Contains(l.AccountObjectID)
                    && l.OriginalLeadID != null && l.ClonedLeadID > 0 && l.CreateDate >= startDate && l.CreateDate <= endDate).ToList();
                    return clonedLeads.Count;
                }
            }
        }
        public Dictionary<string, int> GetCopiedLeadsToDealer(List<string> dealersAccountIds, DateTime? startDate = null, DateTime? endDate = null)
        {
            Dictionary<string, int> dicCopiedLead = new Dictionary<string, int>();
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                List<CRMLead> clonedLeads = new List<CRMLead>();
                if (startDate == null && endDate == null)
                {
                    clonedLeads = repo.Where(l => dealersAccountIds.Contains(l.AccountObjectID)
                   && l.OriginalLeadID != null && l.ClonedLeadID > 0).ToList();
                }
                else
                {
                    clonedLeads = repo.Where(l => dealersAccountIds.Contains(l.AccountObjectID)
                   && l.OriginalLeadID != null && l.ClonedLeadID > 0 && l.CreateDate >= startDate && l.CreateDate <= endDate).ToList();
                }
                foreach (var dealer in dealersAccountIds)
                {
                    var record = clonedLeads.Where(f => f.AccountObjectID == dealer).ToList();
                    dicCopiedLead.Add(dealer, record.Count);
                }
            }
            return dicCopiedLead;
        }
        public double GetAverageLeadTimeInDealers(List<long> leadIds)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var lstleads = repo.Where(l => leadIds.Contains(l.LeadID)).ToList();
                using (MongoRepository<CRMOpportunity> repoOpportunity = new MongoRepository<CRMOpportunity>())
                {
                    //TODO: Need to check if we need open as well
                    /* Lead time for converted leads = Date oppurtinity was closed - First Touch date at dealer level 
                     * (First touch at the dealer level is when they recieved lead from Shaw, which is created dated at dealer instance)
                     * Lead time for open leads  = Current Date - First Touch date at dealer level Correct */
                    List<CRMOpportunity> leadsClosedOpportunity = repoOpportunity.Where(l => leadIds.Contains(l.PrimaryLeadID) && l.IsClosed).ToList();
                    List<double> days = new List<double>();
                    foreach (var item in lstleads)
                    {
                        var leadOpportunity = leadsClosedOpportunity.Where(w => w.PrimaryLeadID == item.LeadID).ToList();
                        foreach (var opportunity in leadOpportunity)
                        {
                            DateTime? closeDate = opportunity.CloseDate;
                            double daydiff = closeDate == null ? 0 : (Convert.ToDateTime(closeDate) - item.CreateDate).TotalDays;
                            if (daydiff > 0)
                            {
                                days.Add(daydiff);
                            }
                        }
                    }
                    if (days.Count == 0)
                        return 0;

                    return days.Sum() / days.Count;
                }
            }
        }
        public Dictionary<string, double> GetAverageLeadTimeInDealers(List<string> dealerAccountIds, IEnumerable<CRMLeadDataModel> leadAccountData)
        {
            //get leads under dealer
            Dictionary<string, double> dicAverageLeadTimeInDealers = new Dictionary<string, double>();
            var lstLeadIds = leadAccountData.ToList().Select(x => new { x.LeadID, x.AccountObjectID }).ToList();

            foreach (var accountId in dealerAccountIds)
            {
                //List<string> lstAccounts = new List<string>();
                //lstAccounts.Add(accountId);
                //List<long> leadIds = GetLeadsByAccountObjectIDsForDateRange(lstAccounts, startDate, endDate).Select(x => x.LeadID).ToList();
                List<long> leadIds = lstLeadIds.Where(x => x.AccountObjectID == accountId).Select(y => y.LeadID).ToList();
                using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
                {
                    var lstleads = repo.Where(l => leadIds.Contains(l.LeadID)).ToList();
                    using (MongoRepository<CRMOpportunity> repoOpportunity = new MongoRepository<CRMOpportunity>())
                    {
                        //TODO: Need to check if we need open as well
                        /* Lead time for converted leads = Date oppurtinity was closed - First Touch date at dealer level 
                         * (First touch at the dealer level is when they recieved lead from Shaw, which is created dated at dealer instance)
                         * Lead time for open leads  = Current Date - First Touch date at dealer level Correct */
                        List<CRMOpportunity> leadsClosedOpportunity = repoOpportunity.Where(l => leadIds.Contains(l.PrimaryLeadID) && l.IsClosed).ToList();
                        List<double> days = new List<double>();
                        foreach (var item in lstleads)
                        {
                            var leadOpportunity = leadsClosedOpportunity.Where(w => w.PrimaryLeadID == item.LeadID).ToList();
                            foreach (var opportunity in leadOpportunity)
                            {
                                DateTime? closeDate = opportunity.CloseDate;
                                double daydiff = closeDate == null ? 0 : (Convert.ToDateTime(closeDate) - item.CreateDate).TotalDays;
                                if (daydiff > 0)
                                {
                                    days.Add(daydiff);
                                }
                            }
                        }
                        if (days.Count == 0)
                        {
                            dicAverageLeadTimeInDealers.Add(accountId, 0);
                        }
                        else
                        {
                            double avg = (double)days.Sum() / (double)days.Count;
                            dicAverageLeadTimeInDealers.Add(accountId, avg);
                        }
                    }
                }
            }
            return dicAverageLeadTimeInDealers;
        }
        public double GetConversionRateInDealer(List<long> leadIds)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var lstleads = repo.Where(l => leadIds.Contains(l.LeadID)).ToList();
                using (MongoRepository<CRMOpportunity> repoOpportunity = new MongoRepository<CRMOpportunity>())
                {
                    List<CRMOpportunity> leadsWinClosedOpportunity = repoOpportunity.Where(l => leadIds.Contains(l.PrimaryLeadID) && (l.IsClosed || l.IsWon)).ToList();
                    if (lstleads.Count == 0 || leadsWinClosedOpportunity.Count == 0)
                    {
                        return 0;
                    }
                    return Math.Round(((double)leadsWinClosedOpportunity.Count / (double)lstleads.Count) * 100, 2);
                }
            }
        }
        public int GetWonLeadCount(List<long> leadIds)
        {
            using (MongoRepository<CRMOpportunity> repoOpportunity = new MongoRepository<CRMOpportunity>())
            {
                var leadsWonOpportunity = repoOpportunity.Where(l => leadIds.Contains(l.PrimaryLeadID) && l.IsWon).ToList();
                return leadsWonOpportunity.Count;
            }
        }
        public Dictionary<string, int> GetTotalLeadsWithObject(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                var data = repo.Where(l => accountObjectIDs.Contains(l.AccountObjectID) && l.IsActive &&
                        l.CreateDate >= startDate && l.CreateDate <= endDate).ToList();
                Dictionary<string, int> dicLeadCount = new Dictionary<string, int>();
                foreach (var accountObjectID in accountObjectIDs)
                {
                    int count = data.Where(l => l.AccountObjectID == accountObjectID).Count();
                    dicLeadCount.Add(accountObjectID, count);
                }
                return dicLeadCount;
            }
        }
        public Dictionary<string, double> GetPassOffPercentage(List<string> dealersAccountIds, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<CRMLead> repo = new MongoRepository<CRMLead>())
            {
                Dictionary<string, double> dicPassOffPercentage = new Dictionary<string, double>();
                var allLeads = repo.Where(l => dealersAccountIds.Contains(l.AccountObjectID)
                       && l.CreateDate >= startDate && l.CreateDate <= endDate).ToList();
                int totalCorporateLeads = allLeads.Count;
                foreach (var accountId in dealersAccountIds)
                {
                    int leadsInDealer = allLeads.Where(x => x.AccountObjectID == accountId).ToList().Count;
                    double passOffPercent = ((double)leadsInDealer / (double)totalCorporateLeads) * 100;
                    dicPassOffPercentage.Add(accountId, Math.Round(passOffPercent, 2));
                }
                return dicPassOffPercentage;
            }
        }

    }
}
