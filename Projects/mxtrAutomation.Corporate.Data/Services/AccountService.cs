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
    public class AccountService : MongoRepository<Account>, IAccountServiceInternal
    {
        public CreateNotificationReturn CreateAccount(mxtrAccount accountData)
        {
            Account entry = new Account()
            {
                MxtrAccountID = accountData.MxtrAccountID,
                ParentMxtrAccountID = accountData.ParentMxtrAccountID,
                ParentAccountObjectID = accountData.ParentAccountObjectID,
                AccountName = accountData.AccountName,
                StreetAddress = accountData.StreetAddress,
                Suite = accountData.Suite,
                City = accountData.City,
                State = accountData.State,
                ZipCode = accountData.ZipCode,
                Country = accountData.Country,
                Phone = accountData.Phone,
                AccountType = accountData.AccountType,
                SharpspringSecretKey = accountData.SharpspringSecretKey,
                SharpspringAccountID = accountData.SharpspringAccountID,
                BullseyeAdminApiKey = accountData.BullseyeAdminApiKey,
                BullseyeClientId = accountData.BullseyeClientId,
                BullseyeSearchApiKey = accountData.BullseyeSearchApiKey,
                BullseyeLocationId = accountData.BullseyeLocationId,
                BullseyeThirdPartyId = accountData.BullseyeThirdPartyId,
                GoogleAnalyticsReportingViewId = accountData.GoogleAnalyticsReportingViewId,
                GoogleAnalyticsTimeZoneName = accountData.GoogleAnalyticsTimeZoneName,
                GoogleServiceAccountCredentialFile = accountData.GoogleServiceAccountCredentialFile,
                GoogleServiceAccountEmail = accountData.GoogleServiceAccountEmail,
                GAProfileName = accountData.GAProfileName,
                GAWebsiteUrl = accountData.GAWebsiteUrl,
                WebsiteUrl = accountData.WebsiteUrl,
                CreateDate = accountData.CreateDate,
                IsActive = accountData.IsActive,
                EZShredIP = accountData.EZShredIP,
                EZShredPort = accountData.EZShredPort,
                KlipfolioSSOSecretKey = accountData.KlipfolioSSOSecretKey,
                KlipfolioCompanyID = accountData.KlipfolioCompanyID,
                StoreId = accountData.StoreId,
                DealerId = accountData.DealerId,
                SharpSpringShawFunnelListID = accountData.SharpSpringShawFunnelListID,
                OpportunityPipeLine = new Entities.OpportunityPipeLine
                {
                    Lead = accountData.Lead,
                    ContactMade = accountData.ContactMade,
                    ProposalSent = accountData.ProposalSent,
                    WonNotScheduled = accountData.WonNotScheduled,
                    Closed = accountData.Closed,
                },
                Coordinates = new Coordinates
                {
                    Latitude = accountData.Latitude,
                    Longitude = accountData.Longitude,
                },
            };

            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    int accountCount = 0;
                    if (!string.IsNullOrEmpty(entry.BullseyeThirdPartyId))
                    {
                        accountCount = repo.Where(w => w.AccountName.ToLower() == entry.AccountName.ToLower() && w.BullseyeThirdPartyId == entry.BullseyeThirdPartyId && w.IsActive).ToList().Count();
                    }
                    else
                    {
                        //we should put a check if account exists with same account name earlier 
                        accountCount = repo.Where(w => w.AccountName.ToLower() == entry.AccountName.ToLower() && w.IsActive).ToList().Count();
                    }
                    //int accountCount = repo.Where(w => w.AccountName.ToLower() == entry.AccountName.ToLower()).ToList().Count();
                    if (accountCount > 0)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Account with this name already exists" };
                    }

                    repo.Add(entry);

                    //Set domain name for newly created account
                    if (entry.AccountType == AccountKind.Organization.ToString())
                    {
                        string domainName = Regex.Replace(entry.AccountName, @"[^0-9a-zA-Z]+", "-").ToLower();
                        int domainNamecount = repo.Where(w => w.DomainName.ToLower() == domainName.ToLower() && w.Id != entry.Id).ToList().Count();
                        if (domainNamecount > 0)
                        {
                            domainName = domainName + entry.Id;
                        }
                        entry.DomainName = domainName;
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

        public CreateNotificationReturn UpdateAccount(mxtrAccount accountData)
        {
            return updateAccountProfile(accountData);
        }
        public mxtrAccount UpdateAccountFromBullsEye(mxtrAccount accountData)
        {
            return updateAccountProfileFromBullsEye(accountData);
        }

        public CreateNotificationReturn AddUpdateDomainName(string accountObjectId, string domainName)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    int domainNamecount = repo.Where(w => w.DomainName.ToLower() == domainName.ToLower() && w.Id != accountObjectId).ToList().Count();
                    if (domainNamecount > 0)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Domain already exists" };
                    }
                    Account entry = repo.Where(x => x.Id == accountObjectId).FirstOrDefault();
                    if (entry == null)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Account not found" };
                    }
                    entry.DomainName = domainName.ToLower();
                    repo.Update(entry);
                    return new CreateNotificationReturn { Success = true, ObjectID = accountObjectId };
                }
                catch (Exception ex)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = ex.Message };
                }
            }
        }

        /// <summary>
        /// Set domain Name
        /// </summary>
        /// <param name="dicAccounts">Having account Id and domain name</param>
        /// <returns></returns>
        public CreateNotificationReturn AddUpdateDomainName(Dictionary<string, string> dicAccounts)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    List<Account> lstAccounts = new List<Account>();
                    foreach (var account in dicAccounts)
                    {
                        Account entry = repo.Where(x => x.Id == account.Key).FirstOrDefault();
                        entry.DomainName = account.Value.ToLower();
                        lstAccounts.Add(entry);
                    }
                    repo.Update(lstAccounts);
                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception ex)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = ex.Message };
                }
            }
        }

        public CreateNotificationReturn CreateAccountAttributes(mxtrAccount accountData)
        {
            return updateAccountAttributes(accountData);
        }

        public CreateNotificationReturn UpdateAccountAttributes(mxtrAccount accountData)
        {
            return updateAccountAttributes(accountData);
        }

        public mxtrAccount GetAccountByAccountObjectID(string accountObjectID)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                //Account account = repo.Where(a => a.Id == accountObjectID && a.IsActive).FirstOrDefault();                
                //return AdaptAccount(account);
                return repo
                    .Where(a => a.Id == accountObjectID && a.IsActive)
                    //.Where(a => a.Id == accountObjectID)
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        DomainName = a.DomainName,
                        ApplicationLogoURL = a.ApplicationLogoURL,
                        BrandingLogoURL = a.BrandingLogoURL,
                        FavIconURL = a.FavIconURL,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        StoreId = a.StoreId,
                        HomePageUrl = a.HomePageUrl,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                    }).FirstOrDefault();
            }
        }

        public mxtrAccount GetBrandingLogoURL(string subDomainName)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Where(a => a.DomainName.ToLower() == subDomainName.ToLower() && a.IsActive)
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        BrandingLogoURL = a.BrandingLogoURL
                    }).FirstOrDefault();
            }
        }
        public mxtrAccount GetAccountByMxtrAccountId(string mxtrAccountId)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                //Account account = repo.Where(a => a.MxtrAccountID == mxtrAccountId).FirstOrDefault();
                //return AdaptAccount(account);
                return repo
                    .Where(a => a.MxtrAccountID == mxtrAccountId)
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                    }).FirstOrDefault();
            }
        }
        public List<mxtrAccount> GetAccountsByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Where(a => accountObjectIDs.Contains(a.Id) && a.IsActive)
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                        Latitude = a.Coordinates == null ? 0 : a.Coordinates.Latitude,
                        Longitude = a.Coordinates == null ? 0 : a.Coordinates.Longitude,
                    }).ToList();
            }
        }

        private List<mxtrAccount> GetAccountsByAccountObjectIDsCoordinates(List<string> accountObjectIDs)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Where(a => accountObjectIDs.Contains(a.Id) && a.IsActive)
                    .Select(a => new mxtrAccount
                    {
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountType = a.AccountType,
                        AccountName = a.AccountName,
                        Latitude = a.Coordinates == null ? 0 : a.Coordinates.Latitude,
                        Longitude = a.Coordinates == null ? 0 : a.Coordinates.Longitude,
                        ObjectID = a.Id,
                        City = a.City,
                        CreateDate = a.CreateDate,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        MxtrAccountID = a.MxtrAccountID,
                    }).ToList();
            }
        }
        public List<mxtrAccount> GetAllAccountsByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Where(a => accountObjectIDs.Contains(a.Id))
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                    }).ToList();
            }
        }
        public IEnumerable<mxtrAccount> GetAccountsByAccountObjectIDsAsIEnumberable(List<string> accountObjectIDs)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Where(a => accountObjectIDs.Contains(a.Id) && a.AccountType != "Group")
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                    });
            }
        }

        public mxtrAccount GetAccountByAccountName(string accountName)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                //Account account = repo.Where(a => a.AccountName == accountName).FirstOrDefault();
                //return AdaptAccount(account);
                return repo
                    .Where(a => a.AccountName == accountName)
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                    }).FirstOrDefault();
            }
        }

        public mxtrAccount GetAccountMiniByAccountObjectId(string accountObjectId)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Where(a => a.Id == accountObjectId)
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                    }).FirstOrDefault();
            }
        }

        public mxtrAccount GetAccountByDealerId(string dealerId, string parentAccountObjectId)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                //Account account = repo.Where(a => a.DealerId == dealerId).FirstOrDefault();
                //return AdaptAccount(account);
                dealerId = dealerId.Trim().ToUpper();
                var rec = repo.Where(w => w.ParentAccountObjectID == parentAccountObjectId && w.DealerId != null).ToList();
                var a = rec.FirstOrDefault(w => w.DealerId.ToUpper().Trim() == dealerId);
                if (a == null)
                {
                    return null;
                }
                return new mxtrAccount()
                {
                    ObjectID = a.Id,
                    MxtrAccountID = a.MxtrAccountID,
                    ParentMxtrAccountID = a.ParentMxtrAccountID,
                    ParentAccountObjectID = a.ParentAccountObjectID,
                    AccountName = a.AccountName,
                    StreetAddress = a.StreetAddress,
                    Suite = a.Suite,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode,
                    Country = a.Country,
                    Phone = a.Phone,
                    AccountType = a.AccountType,
                    SharpspringSecretKey = a.SharpspringSecretKey,
                    SharpspringAccountID = a.SharpspringAccountID,
                    BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                    BullseyeClientId = a.BullseyeClientId,
                    BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                    BullseyeLocationId = a.BullseyeLocationId,
                    BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                    GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                    GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                    GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                    GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                    GAProfileName = a.GAProfileName,
                    GAWebsiteUrl = a.GAWebsiteUrl,
                    WebsiteUrl = a.WebsiteUrl,
                    CreateDate = a.CreateDate,
                    IsActive = a.IsActive,
                    EZShredIP = a.EZShredIP,
                    EZShredPort = a.EZShredPort,
                    KlipfolioCompanyID = a.KlipfolioCompanyID,
                    KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                    StoreId = a.StoreId,
                    DealerId = a.DealerId,
                    SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                    Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                    ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                    ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                    WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                    Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                };

                //return repo
                //    .Where(a => a.DealerId.ToUpper() == dealerId && a.ParentAccountObjectID == parentAccountObjectId)
                //    .Select(a => new mxtrAccount
                //    {
                //        ObjectID = a.Id,
                //        MxtrAccountID = a.MxtrAccountID,
                //        ParentMxtrAccountID = a.ParentMxtrAccountID,
                //        ParentAccountObjectID = a.ParentAccountObjectID,
                //        AccountName = a.AccountName,
                //        StreetAddress = a.StreetAddress,
                //        Suite = a.Suite,
                //        City = a.City,
                //        State = a.State,
                //        ZipCode = a.ZipCode,
                //        Country = a.Country,
                //        Phone = a.Phone,
                //        AccountType = a.AccountType,
                //        SharpspringSecretKey = a.SharpspringSecretKey,
                //        SharpspringAccountID = a.SharpspringAccountID,
                //        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                //        BullseyeClientId = a.BullseyeClientId,
                //        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                //        BullseyeLocationId = a.BullseyeLocationId,
                //        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                //        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                //        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                //        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                //        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                //        GAProfileName = a.GAProfileName,
                //        GAWebsiteUrl = a.GAWebsiteUrl,
                //        WebsiteUrl = a.WebsiteUrl,
                //        CreateDate = a.CreateDate,
                //        IsActive = a.IsActive,
                //        EZShredIP = a.EZShredIP,
                //        EZShredPort = a.EZShredPort,
                //        KlipfolioCompanyID = a.KlipfolioCompanyID,
                //        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                //        StoreId = a.StoreId,
                //        DealerId = a.DealerId,
                //    }).FirstOrDefault();
            }
        }

        public IEnumerable<mxtrAccount> GetAccountsByParentAccountObjectID(string accountObjectID)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Where(a => a.ParentAccountObjectID == accountObjectID)
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                    });
            }
        }

        public IEnumerable<mxtrAccount> GetAccountHeirarchy(string accountObjectID)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                ICollection<string> accountIDs = GetChildAccounts(accountObjectID, repo);

                return repo
                    .Where(a => accountIDs.Contains(a.Id) && a.IsActive)
                    //.Where(a => accountIDs.Contains(a.Id))
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                    });
            }
        }

        public IEnumerable<mxtrAccount> GetAllAccounts()
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo
                    .Select(a => new mxtrAccount
                    {
                        ObjectID = a.Id,
                        MxtrAccountID = a.MxtrAccountID,
                        ParentMxtrAccountID = a.ParentMxtrAccountID,
                        ParentAccountObjectID = a.ParentAccountObjectID,
                        AccountName = a.AccountName,
                        StreetAddress = a.StreetAddress,
                        Suite = a.Suite,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        AccountType = a.AccountType,
                        SharpspringSecretKey = a.SharpspringSecretKey,
                        SharpspringAccountID = a.SharpspringAccountID,
                        BullseyeClientId = a.BullseyeClientId,
                        BullseyeSearchApiKey = a.BullseyeSearchApiKey,
                        BullseyeAdminApiKey = a.BullseyeAdminApiKey,
                        BullseyeLocationId = a.BullseyeLocationId,
                        BullseyeThirdPartyId = a.BullseyeThirdPartyId,
                        GoogleAnalyticsReportingViewId = a.GoogleAnalyticsReportingViewId,
                        GoogleServiceAccountEmail = a.GoogleServiceAccountEmail,
                        GoogleServiceAccountCredentialFile = a.GoogleServiceAccountCredentialFile,
                        GoogleAnalyticsTimeZoneName = a.GoogleAnalyticsTimeZoneName,
                        GAProfileName = a.GAProfileName,
                        GAWebsiteUrl = a.GAWebsiteUrl,
                        WebsiteUrl = a.WebsiteUrl,
                        CreateDate = a.CreateDate,
                        IsActive = a.IsActive,
                        EZShredIP = a.EZShredIP,
                        EZShredPort = a.EZShredPort,
                        KlipfolioCompanyID = a.KlipfolioCompanyID,
                        KlipfolioSSOSecretKey = a.KlipfolioSSOSecretKey,
                        StoreId = a.StoreId,
                        DealerId = a.DealerId,
                        SharpSpringShawFunnelListID = a.SharpSpringShawFunnelListID,
                        Lead = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Lead,
                        ContactMade = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ContactMade,
                        ProposalSent = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.ProposalSent,
                        WonNotScheduled = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.WonNotScheduled,
                        Closed = a.OpportunityPipeLine == null ? "" : a.OpportunityPipeLine.Closed,
                        Latitude = a.Coordinates == null ? 0 : a.Coordinates.Latitude,
                        Longitude = a.Coordinates == null ? 0 : a.Coordinates.Longitude
                    });
            }
        }
        public mxtrAccount GetAccountByBullseyeLocationID(int bullseyeLocationID)
        {
            return GetAllAccounts().Where(a => a.BullseyeLocationId == bullseyeLocationID).FirstOrDefault();
        }
        public mxtrAccount GetAccountByBullseyeThirdPartyId(string thirdPartyId)
        {
            return GetAllAccounts().Where(a => a.BullseyeThirdPartyId == thirdPartyId).FirstOrDefault();
        }
        public IEnumerable<mxtrAccount> GetAllAccountsWithBullseye()
        {
            return GetAllAccounts().Where(a => !a.BullseyeAdminApiKey.IsNullOrEmpty() && !a.BullseyeSearchApiKey.IsNullOrEmpty() && a.IsActive);
        }

        public IEnumerable<mxtrAccount> GetAllAccountsWithEZShred()
        {
            return GetAllAccounts().Where(a => !a.EZShredIP.IsNullOrEmpty() && !a.EZShredPort.IsNullOrEmpty() && a.EZShredIP != "0" && a.EZShredPort != "0");
        }

        public IEnumerable<mxtrAccount> GetAllAccountsWithSharpspring()
        {
            return GetAllAccounts().Where(a => !a.SharpspringAccountID.IsNullOrEmpty() && !a.SharpspringSecretKey.IsNullOrEmpty() && a.IsActive);
        }

        public IEnumerable<mxtrAccount> GetAllAccountsWithGoogle()
        {
            return GetAllAccounts().Where(a => !a.GoogleAnalyticsReportingViewId.IsNullOrEmpty() && !a.GoogleServiceAccountEmail.IsNullOrEmpty() && !a.GoogleServiceAccountCredentialFile.IsNullOrEmpty() && a.IsActive);
        }

        public IEnumerable<mxtrAccount> GetAllAccountsWithOrganization()
        {
            return GetAllAccounts().Where(a => (a.AccountType == AccountKind.Organization.ToString() || a.AccountType.ToLower() == "reseller") && a.IsActive);
        }

        public IEnumerable<mxtrAccount> GetAllAccountWithShawFunnelListId()
        {
            return GetAllAccounts().Where(a => !a.SharpspringAccountID.IsNullOrEmpty() && !a.SharpspringSecretKey.IsNullOrEmpty() && a.SharpSpringShawFunnelListID > 0 && a.IsActive);
        }

        public List<string> GetFlattenedChildAccountObjectIDs(string accountObjectID)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return GetChildAccounts(accountObjectID, repo).ToList();
            }
        }

        public List<string> GetFlattenedChildAccountObjectIDsWithGroupClients(string accountObjectID)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return GetChildAccountsWithGroupClients(accountObjectID, repo).ToList();
            }
        }

        public List<string> GetAllFlattenedChildAccountObjectIDs(string accountObjectID)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return GetAllChildAccounts(accountObjectID, repo).ToList();
            }
        }

        public List<mxtrAccount> GetFlattenedChildAccounts(string accountObjectID)
        {
            List<string> accountObjectIDs = new List<string>();
            accountObjectIDs.Add(accountObjectID);
            List<string> childAccountObjectIDs = GetFlattenedChildAccountObjectIDs(accountObjectID);
            accountObjectIDs.AddRange(childAccountObjectIDs);
            return GetAccountsByAccountObjectIDs(accountObjectIDs);
        }
        public List<mxtrAccount> GetFlattenedChildAccountsCoordinates(string accountObjectID)
        {
            List<string> accountObjectIDs = new List<string>();
            accountObjectIDs.Add(accountObjectID);
            List<string> childAccountObjectIDs = GetFlattenedChildAccountObjectIDsWithGroupClients(accountObjectID);
            accountObjectIDs.AddRange(childAccountObjectIDs);
            return GetAccountsByAccountObjectIDsCoordinates(accountObjectIDs);
        }

        public List<mxtrAccount> GetAccountsCoordinates(List<string> accountObjectIDs)
        {
            return GetAccountsByAccountObjectIDsCoordinates(accountObjectIDs);
        }

        /// <summary>
        /// Client having Sharpspring key and accountId
        /// </summary>
        /// <param name="accountObjectID"></param>
        /// <returns></returns>
        public IEnumerable<mxtrAccount> GetFlattenedChildAccounts_Client(string accountObjectID)
        {
            List<string> accountObjectIDs = new List<string>();
            //accountObjectIDs.Add(accountObjectID);
            List<string> childAccountObjectIDs = GetFlattenedChildAccountObjectIDs(accountObjectID);
            accountObjectIDs.AddRange(childAccountObjectIDs);
            //parent not required only clients under it required
            accountObjectIDs.Remove(accountObjectID);
            return GetAccountsByAccountObjectIDs(accountObjectIDs).Where(
                    a => (a.AccountType == AccountKind.Client.ToString() || a.AccountType == AccountKind.Organization.ToString())
                    && !string.IsNullOrEmpty(a.SharpspringSecretKey)
                    && !string.IsNullOrEmpty(a.SharpspringAccountID)
                ).OrderBy(o => o.AccountName);
        }

        public List<EZShredAccountDataModel> GetFlattenedChildClientAccounts(string accountObjectID)
        {
            List<string> accountObjectIDs = new List<string>();
            //accountObjectIDs.Add(accountObjectID);
            List<string> childAccountObjectIDs = GetFlattenedChildAccountObjectIDs(accountObjectID);
            accountObjectIDs.AddRange(childAccountObjectIDs);
            //parent not required only clients under it required
            accountObjectIDs.Remove(accountObjectID);
            return GetAccountsByAccountObjectIDs(accountObjectIDs)
                .Where(a => (a.AccountType == AccountKind.Client.ToString() || a.AccountType == AccountKind.Organization.ToString()) && !string.IsNullOrEmpty(a.AccountName)
                && !string.IsNullOrEmpty(a.EZShredPort) && !string.IsNullOrEmpty(a.EZShredIP))
                .OrderBy(o => o.AccountName).Select(x => new EZShredAccountDataModel
                {
                    AccountName = x.AccountName,
                    AccountObjectId = x.ObjectID,
                }).ToList();
        }

        public List<mxtrAccount> GetFlattenedAccountsForMoving(string loggedInAccountObjectID, string editingAccountObjectID)
        {
            List<string> allAccountObjectIDs = new List<string>();
            allAccountObjectIDs.Add(loggedInAccountObjectID);
            List<string> allChildAccountObjectIDs = GetFlattenedChildAccountObjectIDs(loggedInAccountObjectID);
            allAccountObjectIDs.AddRange(allChildAccountObjectIDs);

            List<string> allEditingAccountObjectIDs = new List<string>();
            allEditingAccountObjectIDs.Add(editingAccountObjectID);
            List<string> allEditingChildAccountObjectIDs = GetFlattenedChildAccountObjectIDs(editingAccountObjectID);
            allEditingAccountObjectIDs.AddRange(allEditingChildAccountObjectIDs);

            List<string> filteredAccountObjectIDs = allAccountObjectIDs.Except(allEditingAccountObjectIDs).ToList();
            filteredAccountObjectIDs.Add(loggedInAccountObjectID); //add logged in account back in
            return GetAccountsByAccountObjectIDs(filteredAccountObjectIDs);
        }

        public int GetTotalRetailers(List<string> accountObjectIDs)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                return repo.AsQueryable()
                    .Where(a => accountObjectIDs.Contains(a.Id) && a.IsActive && a.AccountType == AccountKind.Client.ToString()).Count();

            }
        }

        public bool IsAccountActive(string mxtrAccountId)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                var account = repo.Where(a => a.MxtrAccountID == mxtrAccountId).FirstOrDefault();
                return account.IsActive;
            }
        }

        public CreateNotificationReturn updateApplicationLogo(string accountObjectId, string ApplicationLogoURL)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    Account entry = repo.Where(x => x.Id == accountObjectId).FirstOrDefault();
                    if (entry == null)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Account not found" };
                    }
                    entry.ApplicationLogoURL = ApplicationLogoURL;

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = accountObjectId };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = accountObjectId };
                }
            }
        }
        public CreateNotificationReturn updateBrandingLogo(string accountObjectId, string BrandingLogoURL)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    Account entry = repo.Where(x => x.Id == accountObjectId).FirstOrDefault();

                    if (entry == null)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Account not found" };
                    }
                    entry.BrandingLogoURL = BrandingLogoURL;

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = accountObjectId };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = accountObjectId };
                }
            }
        }
        public CreateNotificationReturn updateFavIcon(string accountObjectId, string FavIconURL)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    Account entry = repo.Where(x => x.Id == accountObjectId).FirstOrDefault();
                    if (entry == null)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Account not found" };
                    }
                    entry.FavIconURL = FavIconURL;

                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = accountObjectId };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = accountObjectId };
                }
            }
        }
        public List<EZShredAccountDataModel> GetEzshredByAccountObjectIds(List<string> accountObjectIDs, mxtrUser mxtrUser)
        {
            List<mxtrAccount> listMxtrAccount = new List<mxtrAccount>();
            List<EZShredAccountDataModel> EZShredAccountDataModel = new List<EZShredAccountDataModel>();
            listMxtrAccount = GetAccountsByAccountObjectIDs(accountObjectIDs);
            foreach (var item in mxtrUser.EZShredAccountMappings)
            {
                var result = (from a in listMxtrAccount where a.ObjectID == item.AccountObjectId select a).FirstOrDefault();
                EZShredAccountDataModel.Add(new EZShredAccountDataModel
                {
                    AccountName = result.AccountName,
                    EZShredPort = result.EZShredPort,
                    EZShredIP = result.EZShredIP,
                    EZShredId = item.EZShredId,
                    AccountObjectId = item.AccountObjectId
                });
            }
            return EZShredAccountDataModel;
        }

        public CreateNotificationReturn AddUpdateHomePageUrl(string accountObjectId, string homePageUrl)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    Account entry = repo.Where(x => x.Id == accountObjectId).FirstOrDefault();
                    if (entry == null)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Account not found" };
                    }
                    entry.HomePageUrl = homePageUrl;
                    repo.Update(entry);
                    return new CreateNotificationReturn { Success = true, ObjectID = accountObjectId };
                }
                catch (Exception ex)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = ex.Message };
                }
            }
        }

        public string GetHomePageUrl(string accountObjectId)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    Account entry = repo.FirstOrDefault(x => x.Id == accountObjectId);
                    if (entry != null)
                    {
                        return entry.HomePageUrl;
                    }
                }
                catch (Exception ex)
                {
                    return String.Empty;
                }
            }
            return String.Empty;
        }

        public bool SetOpportunityPipeLine()
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                var account = repo.Where(a => !String.IsNullOrEmpty(a.SharpspringAccountID) && !String.IsNullOrEmpty(a.SharpspringSecretKey) && a.IsActive).ToList();
                List<Account> lstMxtrAccount = new List<Account>();
                foreach (var item in account)
                {
                    if (item.AccountName.ToLower() == "syracuse")
                    {
                        item.OpportunityPipeLine = new OpportunityPipeLine
                        {
                            Lead = "Lead - " + item.AccountName,
                            ContactMade = "Contact Made - " + item.AccountName,
                            ProposalSent = "Proposal Sent - " + item.AccountName,
                            WonNotScheduled = "Won/Not Scheduled - " + item.AccountName,
                        };
                    }
                    else
                    {
                        item.OpportunityPipeLine = new OpportunityPipeLine
                        {
                            Lead = "Lead",
                            ContactMade = "Contact Made",
                            ProposalSent = "Proposal Sent",
                            WonNotScheduled = "Won/Not Scheduled",
                        };
                    }
                    lstMxtrAccount.Add(item);
                }
                repo.Update(lstMxtrAccount);
            }
            return true;
        }

        public List<Tuple<DateTime, int>> GetParticipatingdealerdata(string accountObjectID, DateTime endDate, int numberOfDays)
        {
            try
            {
                List<string> accountObjectIDs = new List<string>();
                // accountObjectIDs.Add(accountObjectID);
                List<string> childAccountObjectIDs = GetFlattenedChildAccountObjectIDsWithGroupClients(accountObjectID);
                accountObjectIDs.AddRange(childAccountObjectIDs);
                //we need dealer accounts 
                accountObjectIDs.Remove(accountObjectID);
                using (MongoRepository<Account> repo = new MongoRepository<Account>())
                {
                    var dealerLeads = repo.Where(a => accountObjectIDs.Contains(a.Id) && a.IsActive).ToList();
                    var data = dealerLeads.Select(a => new
                    {
                        ObjectID = a.Id,
                        CreateDate = DateTime.ParseExact(a.CreateDate, "M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                    }).ToList();

                    DateTime startDate = endDate.AddDays(-numberOfDays);
                    data = data.Where(a => Convert.ToDateTime(a.CreateDate) >= startDate && Convert.ToDateTime(a.CreateDate) <= endDate).ToList();

                    var participatingDealerGrouped = data.GroupBy(n => n.CreateDate.Date).Select
                        (group => new
                        {
                            CreatedDate = group.Key,
                            //Notices = group.ToList(),
                            Count = group.Count()
                        }).ToList();

                    List<Tuple<DateTime, int>> lsttup = new List<Tuple<DateTime, int>>();
                    foreach (var item in participatingDealerGrouped)
                    {
                        lsttup.Add(new Tuple<DateTime, int>(item.CreatedDate, item.Count));
                    }

                    return lsttup;
                }
            }
            catch (Exception ex)
            {
                return new List<Tuple<DateTime, int>>();
            }
        }

        #region Private Methods

        private CreateNotificationReturn updateAccountAttributes(mxtrAccount accountData)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    Account entry = repo.Where(x => x.Id == accountData.ObjectID).FirstOrDefault();

                    entry.SharpspringSecretKey = accountData.SharpspringSecretKey;
                    entry.SharpspringAccountID = accountData.SharpspringAccountID;
                    entry.BullseyeClientId = accountData.BullseyeClientId;
                    entry.BullseyeAdminApiKey = accountData.BullseyeAdminApiKey;
                    entry.BullseyeSearchApiKey = accountData.BullseyeSearchApiKey;
                    entry.BullseyeLocationId = accountData.BullseyeLocationId;
                    entry.BullseyeThirdPartyId = accountData.BullseyeThirdPartyId;
                    entry.WebsiteUrl = accountData.WebsiteUrl;
                    entry.GoogleAnalyticsReportingViewId = accountData.GoogleAnalyticsReportingViewId;
                    entry.GoogleAnalyticsTimeZoneName = accountData.GoogleAnalyticsTimeZoneName;
                    entry.GoogleServiceAccountCredentialFile = accountData.GoogleServiceAccountCredentialFile;
                    entry.GoogleServiceAccountEmail = accountData.GoogleServiceAccountEmail;
                    entry.GAProfileName = accountData.GAProfileName;
                    entry.GAWebsiteUrl = accountData.GAWebsiteUrl;
                    entry.EZShredIP = accountData.EZShredIP;
                    entry.EZShredPort = accountData.EZShredPort;
                    entry.KlipfolioCompanyID = accountData.KlipfolioCompanyID;
                    entry.KlipfolioSSOSecretKey = accountData.KlipfolioSSOSecretKey;
                    entry.DealerId = accountData.DealerId;
                    entry.OpportunityPipeLine = new OpportunityPipeLine()
                    {
                        WonNotScheduled = accountData.WonNotScheduled,
                        Closed = accountData.Closed,
                        ContactMade = accountData.ContactMade,
                        Lead = accountData.Lead,
                        ProposalSent = accountData.ProposalSent,
                    };
                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = accountData.ObjectID };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = accountData.ObjectID };
                }
            }
        }

        private CreateNotificationReturn updateAccountProfile(mxtrAccount accountData)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    Account entry = repo.Where(x => x.Id == accountData.ObjectID).FirstOrDefault();

                    //we should put a check if account exists with same account name earlier 
                    int accountCount = repo.Where(w => w.AccountName.ToLower() == accountData.AccountName.ToLower() && w.Id != accountData.ObjectID && w.IsActive).ToList().Count();
                    if (accountCount > 0)
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = "Account with this name already exists" };
                    }
                    //check if Account type is changed 
                    if (entry.AccountType != accountData.AccountType)
                    {
                        //check if it is set to Organization
                        if (accountData.AccountType == AccountKind.Organization.ToString())
                        {
                            //set domain name 
                            string domainName = Regex.Replace(accountData.AccountName, @"[^0-9a-zA-Z]+", "-").ToLower();
                            int domainNamecount = repo.Where(w => w.DomainName.ToLower() == domainName.ToLower() && w.Id != entry.Id).ToList().Count();
                            if (domainNamecount > 0)
                            {
                                domainName = domainName + entry.Id;
                            }
                            entry.DomainName = domainName;
                        }
                        //check if old Account type was organization then remove domain name
                        if (entry.AccountType == AccountKind.Organization.ToString())
                        {
                            entry.DomainName = string.Empty;
                        }
                    }
                    entry.MxtrAccountID = accountData.MxtrAccountID;
                    entry.ParentMxtrAccountID = accountData.ParentMxtrAccountID;
                    entry.ParentAccountObjectID = accountData.ParentAccountObjectID;
                    entry.AccountName = accountData.AccountName;
                    entry.StreetAddress = accountData.StreetAddress;
                    entry.Suite = accountData.Suite;
                    entry.City = accountData.City;
                    entry.State = accountData.State;
                    entry.ZipCode = accountData.ZipCode;
                    entry.Country = accountData.Country;
                    entry.Phone = accountData.Phone;
                    entry.AccountType = accountData.AccountType;
                    entry.IsActive = accountData.IsActive;
                    entry.StoreId = accountData.StoreId;
                    // entry.BullseyeThirdPartyId = accountData.BullseyeThirdPartyId;
                    repo.Update(entry);

                    return new CreateNotificationReturn { Success = true, ObjectID = accountData.ObjectID };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = accountData.ObjectID };
                }
            }
        }
        private mxtrAccount updateAccountProfileFromBullsEye(mxtrAccount accountData)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    // Account entry = repo.Where(x => x.BullseyeLocationId == accountData.BullseyeLocationId).FirstOrDefault();
                    Account entry = repo.Where(x => x.BullseyeThirdPartyId == accountData.BullseyeThirdPartyId).FirstOrDefault();
                    if (entry == null)
                    {
                        return new mxtrAccount();
                    }
                    entry.AccountName = accountData.AccountName;
                    entry.StreetAddress = accountData.StreetAddress;
                    entry.Suite = accountData.Suite;
                    entry.City = accountData.City;
                    entry.State = accountData.State;
                    entry.ZipCode = accountData.ZipCode;
                    entry.Country = accountData.Country;
                    entry.Phone = accountData.Phone;
                    entry.IsActive = accountData.IsActive;
                    entry.StoreId = accountData.StoreId;
                    entry.BullseyeThirdPartyId = accountData.BullseyeThirdPartyId;

                    repo.Update(entry);

                    mxtrAccount account = AdaptAccount(entry);

                    return account;
                }
                catch (Exception e)
                {
                    return new mxtrAccount();
                }
            }
        }

        private mxtrAccount AdaptAccount(Account account)
        {
            if (account == null)
            {
                return new mxtrAccount();
            }
            return new mxtrAccount()
            {
                ObjectID = account.Id,
                MxtrAccountID = account.MxtrAccountID,
                ParentMxtrAccountID = account.ParentMxtrAccountID,
                ParentAccountObjectID = account.ParentAccountObjectID,
                AccountName = account.AccountName,
                StreetAddress = account.StreetAddress,
                Suite = account.Suite,
                City = account.City,
                State = account.State,
                ZipCode = account.ZipCode,
                Country = account.Country,
                Phone = account.Phone,
                AccountType = account.AccountType,
                SharpspringSecretKey = account.SharpspringSecretKey,
                SharpspringAccountID = account.SharpspringAccountID,
                BullseyeAdminApiKey = account.BullseyeAdminApiKey,
                BullseyeClientId = account.BullseyeClientId,
                BullseyeSearchApiKey = account.BullseyeSearchApiKey,
                BullseyeLocationId = account.BullseyeLocationId,
                BullseyeThirdPartyId = account.BullseyeThirdPartyId,
                GoogleAnalyticsReportingViewId = account.GoogleAnalyticsReportingViewId,
                GoogleAnalyticsTimeZoneName = account.GoogleAnalyticsTimeZoneName,
                GoogleServiceAccountCredentialFile = account.GoogleServiceAccountCredentialFile,
                GoogleServiceAccountEmail = account.GoogleServiceAccountEmail,
                GAProfileName = account.GAProfileName,
                GAWebsiteUrl = account.GAWebsiteUrl,
                WebsiteUrl = account.WebsiteUrl,
                CreateDate = account.CreateDate,
                IsActive = account.IsActive,
                StoreId = account.StoreId,
                DealerId = account.DealerId,
                SharpSpringShawFunnelListID = account.SharpSpringShawFunnelListID,
                Lead = account.OpportunityPipeLine == null ? "" : account.OpportunityPipeLine.Lead,
                ContactMade = account.OpportunityPipeLine == null ? "" : account.OpportunityPipeLine.ContactMade,
                ProposalSent = account.OpportunityPipeLine == null ? "" : account.OpportunityPipeLine.ProposalSent,
                Closed = account.OpportunityPipeLine == null ? "" : account.OpportunityPipeLine.Closed,
                WonNotScheduled = account.OpportunityPipeLine == null ? "" : account.OpportunityPipeLine.WonNotScheduled,
            };
        }

        protected ICollection<string> GetChildAccounts(string accountObjectID, MongoRepository<Account> repo)
        {
            List<string> finalAccountIDs = new List<string>();
            List<string> workingAccountIDs = new List<string> { accountObjectID };

            while (workingAccountIDs.Count > 0)
            {
                List<string> childAccountIDs = repo
                        .Where(a => workingAccountIDs.Contains(a.ParentAccountObjectID) && a.IsActive)
                        .Select(a => a.Id)
                        .ToList();

                finalAccountIDs.AddRange(workingAccountIDs);
                workingAccountIDs = childAccountIDs;
            }

            return finalAccountIDs;
        }

        protected ICollection<string> GetChildAccountsWithGroupClients(string accountObjectID, MongoRepository<Account> repo)
        {
            List<string> finalAccountIDs = new List<string>();
            List<string> workingAccountIDs = new List<string> { accountObjectID };

            while (workingAccountIDs.Count > 0)
            {
                List<string> childAccountIDs = repo
                        .Where(a => workingAccountIDs.Contains(a.ParentAccountObjectID) && a.IsActive && a.AccountType != AccountKind.Group.ToString())
                        .Select(a => a.Id)
                        .ToList();

                finalAccountIDs.AddRange(workingAccountIDs);
                workingAccountIDs = childAccountIDs;
            }

            // Grouping start
            workingAccountIDs = new List<string> { accountObjectID };
            List<mxtrAccount> accounts = repo
                      .Where(a => workingAccountIDs.Contains(a.ParentAccountObjectID) && a.IsActive && a.AccountType == AccountKind.Group.ToString())
                      .Select(a => new mxtrAccount
                      {
                          ObjectID = a.Id,
                          AccountType = a.AccountType,
                          ParentAccountObjectID = a.ParentAccountObjectID,
                      }).ToList();
            List<string> groupAccountIDs = new List<string>();
            finalAccountIDs.AddRange(GetParentAccountOfLead(accounts, groupAccountIDs));
            // Grouping end

            finalAccountIDs = finalAccountIDs.Distinct().ToList();
            return finalAccountIDs;
        }

        private List<string> GetParentAccountOfLead(List<mxtrAccount> parentAccount, List<string> filterAccount)
        {
            List<mxtrAccount> result = new List<mxtrAccount>();
            foreach (var item in parentAccount)
            {
                if (item.AccountType == AccountKind.Group.ToString())
                {
                    result = GetFlattenedChildAccounts_Client(item.ObjectID).ToList();
                    //call the function recursively until we get parent
                    GetParentAccountOfLead(result, filterAccount);
                }
                else
                {
                    filterAccount.Add(item.ObjectID);
                }
            }

            return filterAccount;
        }

        public CreateNotificationReturn AssignCoordinates(string accountObjectId, double latitude, double longitude)
        {
            using (MongoRepository<Account> repo = new MongoRepository<Account>())
            {
                try
                {
                    var account = repo.FirstOrDefault(x => x.Id == accountObjectId);
                    if (account != null)
                    {
                        account.Coordinates = new Coordinates
                        {
                            Latitude = latitude,
                            Longitude = longitude,
                        };
                        repo.Update(account);
                        return new CreateNotificationReturn { Success = true, ObjectID = accountObjectId };
                    }
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        protected ICollection<string> GetAllChildAccounts(string accountObjectID, MongoRepository<Account> repo)
        {
            List<string> finalAccountIDs = new List<string>();
            List<string> workingAccountIDs = new List<string> { accountObjectID };

            while (workingAccountIDs.Count > 0)
            {
                List<string> childAccountIDs = repo
                        .Where(a => workingAccountIDs.Contains(a.ParentAccountObjectID))
                        .Select(a => a.Id)
                        .ToList();

                finalAccountIDs.AddRange(workingAccountIDs);
                workingAccountIDs = childAccountIDs;
            }

            return finalAccountIDs;
        }

        #endregion

    }
}
