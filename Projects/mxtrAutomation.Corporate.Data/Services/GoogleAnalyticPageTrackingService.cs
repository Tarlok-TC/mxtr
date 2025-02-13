using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class GoogleAnalyticPageTrackingService : MongoRepository<GAPageTracking>, IGoogleAnalyticPageTrackingServiceInternal
    {
        public CreateNotificationReturn CreateBatchGoogleAnalyticsPageTracking(List<GAPageTrackingDataModel> pagesTracking, string accountObjectID)
        {
            using (MongoRepository<GAPageTracking> repo = new MongoRepository<GAPageTracking>())
            {
                var entries = pagesTracking
                .Select(c => new GAPageTracking()
                {
                    AccountObjectID = accountObjectID,
                    MxtrAccountID = Guid.NewGuid().ToString(),
                    CRMKind = CRMKind.GoogleAnalytics.ToString(),
                    PagePath = c.pagePath,
                    PageTitle = c.pageTitle,
                    PageViews = c.pageviews,
                    CreateDate = c.date,
                    Events = c.Events.Select(e => new GAEventTracking()
                    {
                        EventLabel = e.eventLabel,
                        EventCategory = e.eventCategory,
                        EventAction = e.eventAction,
                        PagePath = e.pagePath,
                        Total = e.totalEvents
                    }).ToList()
                }).ToList();

                try
                {
                    repo.Add(entries);

                    // Save Data in new table
                    MappingOfLocationId(repo);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateGoogleAnalyticsPageTracking(List<GAPageTrackingDataModel> pagesTracking, string accountObjectID)
        {
            using (MongoRepository<GAPageTracking> repo = new MongoRepository<GAPageTracking>())
            {
                try
                {
                    foreach (var track in pagesTracking)
                    {
                        var entry = repo.Where(x => x.AccountObjectID == accountObjectID && x.PagePath == track.pagePath && x.CreateDate == track.date).FirstOrDefault();
                        entry.PageViews = track.pageviews;
                        entry.Events = track.Events.Select(e => new GAEventTracking()
                        {
                            EventLabel = e.eventLabel,
                            PagePath = e.pagePath,
                            Total = e.totalEvents
                        }).ToList();
                        repo.Update(entry);
                    }

                    // Save Data in new table
                    MappingOfLocationId(repo);

                    return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        private void MappingOfLocationId(MongoRepository<GAPageTracking> repo)
        {
            List<GAPageTracking> filterGAData = GetFilteredGAData(repo);

            using (MongoRepository<Account> accountRepo = new MongoRepository<Account>())
            {
                foreach (var item in filterGAData)
                {
                    string pagePath = item.PagePath;
                    if (pagePath.Contains("/"))
                    {
                        string locationId = pagePath.Substring(pagePath.LastIndexOf("/") + 1);
                        int bullseyeLocationId = IsValidLocation(locationId);
                        if (bullseyeLocationId > 0)
                        {
                            var accountData = accountRepo.Where(x => x.BullseyeLocationId == bullseyeLocationId && x.IsActive).AsEnumerable();
                            if (accountData != null && accountData.Count() > 0)
                            {
                                foreach (var accountItem in accountData)
                                {
                                    using (MongoRepository<GASearchData> gaSearchDataRepo = new MongoRepository<GASearchData>())
                                    {
                                        var gaSearchEntry = gaSearchDataRepo.Where(x => x.AccountObjectID == accountItem.Id && x.CreateDate == item.CreateDate).FirstOrDefault();
                                        if (gaSearchEntry == null)
                                        {
                                            gaSearchEntry = new GASearchData()
                                            {
                                                AccountObjectID = accountItem.Id,
                                                MxtrAccountID = accountItem.MxtrAccountID,
                                                CRMKind = CRMKind.GoogleAnalytics.ToString(),
                                                BullseyeLocationId = bullseyeLocationId,
                                                CreateDate = item.CreateDate,
                                                LastUpdatedDate = DateTime.Now,
                                                LandingPageviews = item.PageViews,
                                                LogoClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Logo)),
                                                WebsiteClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Website)),
                                                MoreInfoClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.MoreInfo)),
                                                DirectionsClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Direction)),
                                                PhoneClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Phone)),
                                            };
                                            gaSearchDataRepo.Add(gaSearchEntry);
                                        }
                                        else
                                        {
                                            gaSearchEntry.LastUpdatedDate = DateTime.Now;
                                            gaSearchEntry.LandingPageviews = item.PageViews;
                                            gaSearchEntry.LogoClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Logo));
                                            gaSearchEntry.WebsiteClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Website));
                                            gaSearchEntry.MoreInfoClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.MoreInfo));
                                            gaSearchEntry.DirectionsClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Direction));
                                            gaSearchEntry.PhoneClicks = GetGAClicks(item.Events, EnumExtensions.GetStringValue(GAEventAction.Phone));
                                            gaSearchDataRepo.Update(gaSearchEntry);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<GAPageTracking> GetCampaignsByAccountObjectIDs(List<string> accountObjectIDs)
        {
            using (MongoRepository<GAPageTracking> repo = new MongoRepository<GAPageTracking>())
            {
                return repo
                    .Where(c => accountObjectIDs.Contains(c.AccountObjectID))
                    .Select(c => new GAPageTracking
                    {
                        AccountObjectID = c.AccountObjectID,
                        MxtrAccountID = c.MxtrAccountID,
                        CRMKind = c.CRMKind,
                        PagePath = c.PagePath,
                        PageTitle = c.PageTitle,
                        PageViews = c.PageViews,
                        CreateDate = c.CreateDate,
                        Events = c.Events
                    }).ToList();
            }
        }

        private static List<GAPageTracking> GetFilteredGAData(MongoRepository<GAPageTracking> repo)
        {
            var gaPageTrackData = repo.Select(x => new GAPageTracking
            {
                PagePath = RemoveQueryString(x.PagePath),
                PageTitle = x.PageTitle,
                AccountObjectID = x.AccountObjectID,
                PageViews = x.PageViews,
                CreateDate = x.CreateDate,
                Events = x.Events,
            }).ToList();


            return gaPageTrackData.GroupBy(x => new { x.PagePath, x.CreateDate })
                                            .Select(g => new GAPageTracking
                                            {
                                                PagePath = g.First().PagePath,
                                                PageTitle = g.First().PageTitle,
                                                AccountObjectID = g.First().AccountObjectID,
                                                CreateDate = g.First().CreateDate,
                                                PageViews = g.Sum(n => n.PageViews),
                                                Events = g.SelectMany(l => l.Events).GroupBy(b => b.EventAction)
                                                            .Select(gg => new GAEventTracking
                                                            {
                                                                EventAction = gg.Key,
                                                                EventCategory = gg.First().EventCategory,
                                                                EventLabel = gg.First().EventLabel,
                                                                Total = gg.Sum(b => b.Total)
                                                            }).ToList()
                                            }).ToList();
        }

        private static string RemoveQueryString(string pagePath) // Remove querystring value to get actual LocationId
        {
            string[] separateURL = pagePath.Split('?');
            if (separateURL.Length > 0)
            {
                return separateURL[0];
            }
            return pagePath;
        }

        private static int GetGAClicks(List<GAEventTracking> gaEventTracking, string strActionType)
        {
            var result = gaEventTracking.Where(x => x.EventAction == strActionType).FirstOrDefault();
            return result != null ? result.Total : 0;
        }

        private int IsValidLocation(string locationId)
        {
            int bullsEyeLocationId = 0;
            try
            {
                int.TryParse(locationId, out bullsEyeLocationId);
            }
            catch (Exception)
            {
            }
            return bullsEyeLocationId;
        }
    }
}
