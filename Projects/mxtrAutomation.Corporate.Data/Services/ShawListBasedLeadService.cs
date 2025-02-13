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
    public class ShawListBasedLeadService : MongoRepository<Account>, IShawListBasedLeadServiceInternal
    {
        public CreateNotificationReturn AddUpdateShawListBasedData(ShawListBasedDataModel data)
        {

            ShawListBasedLeadAnalytics entry = new ShawListBasedLeadAnalytics()
            {
                AccountObjectID = data.AccountObjectID,
                MxtrAccountID = data.MxtrAccountID,
                Name = data.Name,
                SharpSpringListID = data.SharpSpringListID,
                MemberCount = data.MemberCount,
                //MemberCountForToday = data.MemberCountForToday,
                RemovedCount = data.RemovedCount,
                Description = data.Description,
                CRMKind = data.CRMKind,
                CreatedOnMXTR = data.CreatedOnMXTR,
                CreateTimestamp = data.CreateTimestamp,
            };

            using (MongoRepository<ShawListBasedLeadAnalytics> repo = new MongoRepository<ShawListBasedLeadAnalytics>())
            {
                try
                {
                    var shawListData = repo.Where(x => x.SharpSpringListID == entry.SharpSpringListID && x.AccountObjectID == entry.AccountObjectID && x.CRMKind == entry.CRMKind).ToList().FirstOrDefault(y => y.CreatedOnMXTR.Date == data.CreatedOnMXTR.Date);

                    if (shawListData == null)
                    {
                        //get last record of ShawList for calculating MemberCount diff                    
                        GetMemberCountForToday(data, entry, repo, data.SharpSpringListID);
                        repo.Add(entry);
                    }
                    else
                    {
                        //get last record of ShawList for calculating MemberCount diff                    
                        GetMemberCountForToday(data, entry, repo, shawListData.SharpSpringListID);
                        entry.Id = shawListData.Id;
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

        public ShawListBasedDataModel GetShawListBasedData(string accountObjectId, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<ShawListBasedLeadAnalytics> repo = new MongoRepository<ShawListBasedLeadAnalytics>())
            {
                //var data = repo.Where(w => w.AccountObjectID == accountObjectId && w.CreatedOnMXTR >= startDate &&
                // w.CreatedOnMXTR <= endDate).OrderByDescending(o => o.CreatedOnMXTR).FirstOrDefault();
                var data = repo.Where(w => w.AccountObjectID == accountObjectId ).ToList();
                var filterData = data.Where(w => w.CreatedOnMXTR.Date >= startDate.Date && w.CreatedOnMXTR.Date <= endDate.Date).OrderByDescending(o => o.CreatedOnMXTR).FirstOrDefault();
                return AdaptData(filterData);
            }
        }

        private static void GetMemberCountForToday(ShawListBasedDataModel data, ShawListBasedLeadAnalytics entry, MongoRepository<ShawListBasedLeadAnalytics> repo, long sharpSpringListID)
        {
            var lastRecord = repo.Where(x => x.SharpSpringListID == sharpSpringListID && x.AccountObjectID == data.AccountObjectID).OrderByDescending(o => o.CreateTimestamp).FirstOrDefault();
            int memberCountForToday = 0;
            if (lastRecord != null)
            {
                memberCountForToday = (String.IsNullOrEmpty(data.MemberCount) ? 0 : Convert.ToInt32(data.MemberCount)) - (String.IsNullOrEmpty(lastRecord.MemberCount) ? 0 : Convert.ToInt32(lastRecord.MemberCount));
                entry.MemberCountForToday = memberCountForToday;
            }
        }

        private ShawListBasedDataModel AdaptData(ShawListBasedLeadAnalytics data)
        {
            if (data == null)
            {
                return new ShawListBasedDataModel();
            }
            return new ShawListBasedDataModel()
            {
                Id = data.Id,
                AccountObjectID = data.AccountObjectID,
                MxtrAccountID = data.MxtrAccountID,
                Name = data.Name,
                MemberCount = data.MemberCount,
                MemberCountForToday = data.MemberCountForToday,
                RemovedCount = data.RemovedCount,
                SharpSpringListID = data.SharpSpringListID,
                Description = data.Description,
                CRMKind = data.CRMKind,
                CreatedOnMXTR = data.CreatedOnMXTR,
                CreateTimestamp = data.CreateTimestamp,
            };
        }
    }
}
