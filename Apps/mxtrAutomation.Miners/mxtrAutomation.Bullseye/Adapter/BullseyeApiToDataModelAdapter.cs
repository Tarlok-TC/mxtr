using mxtrAutomation.Api.Bullseye;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Bullseye.Adapter
{
    public static class BullseyeApiToDataModelAdapter
    {
        public static List<CRMRestSearchResponseLogModel> AdaptRestSearchResponseLog(List<RestSearchResponseLogDataModel> logs, DateTime date)
        {
            DateTime tempDate = date.AddDays(-1);
            DateTime createDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);

            DateTime lastModifiedDate = createDate;

            return (from l in logs group l by l.LocationID into g
             select new CRMRestSearchResponseLogModel()
             {
                 LocationID = g.Key,
                 CRMKind = CRMKind.Bullseye.ToString(),
                 AccountName = g.Select(x => x.LocationName).FirstOrDefault(),
                 CreateDate = createDate,
                 LastUpdatedDate = lastModifiedDate,
                 LocatorPageviews = g.Sum(x => (x.Viewed ? 1 : 0)),
                 UrlClicks = g.Sum(x => (x.URLClicked ? 1 : 0)),
                 EmailClicks = g.Sum(x => (x.EmailClicked ? 1 : 0)),
                 MoreInfoClicks = g.Sum(x => (x.LocationResultsClicked ? 1 : 0)),
                 MapClicks = g.Sum(x => (x.LocationMapClicked ? 1 : 0)),
                 DirectionsClicks = g.Sum(x => (x.DirectionsClicked ? 1 : 0)),
             }).ToList();
        }

        public static mxtrAccount AdaptAccount(BullseyeAccountDataModel account, mxtrAccount parentAccount) {
            
            DateTime createDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, DateTimeKind.Utc);

            return new mxtrAccount()
            {
                MxtrAccountID = Guid.NewGuid().ToString(),
                AccountName = account.Name,
                ParentAccountObjectID = parentAccount.ObjectID,
                StreetAddress = string.Format("{0} {1} {2} {3}", account.Address1, account.Address2, account.Address3, account.Address4),
                City = account.City,
                State = account.State,
                ZipCode = account.PostCode,
                Country = account.CountryCode,
                Phone = account.PhoneNumber,
                AccountType = AccountKind.Client.ToString(),
                WebsiteUrl = account.URL,
                CreateDate = createDate.ToString(),
                IsActive = true,
                BullseyeLocationId = account.Id ?? 0,
                BullseyeThirdPartyId=account.ThirdPartyId,
            };
        }

        public static mxtrAccount AdaptAccountForUpdate(BullseyeAccountDataModel account)
        {            
            return new mxtrAccount()
            {               
                AccountName = account.Name,                
                StreetAddress = string.Format("{0} {1} {2} {3}", account.Address1, account.Address2, account.Address3, account.Address4),
                City = account.City,
                State = account.State,
                ZipCode = account.PostCode,
                Country = account.CountryCode,
                Phone = account.PhoneNumber,                
                WebsiteUrl = account.URL,               
                IsActive = account.IsActive,
                BullseyeLocationId = account.Id ?? 0,
                BullseyeThirdPartyId = account.ThirdPartyId,
            };
        }
    }
}
