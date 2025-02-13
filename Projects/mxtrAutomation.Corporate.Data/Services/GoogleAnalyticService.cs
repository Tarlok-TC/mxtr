using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class GoogleAnalyticService : MongoRepository<GASearchData>, IGoogleAnalyticServiceInternal
    {
        public IEnumerable<GASearchDataModel> GetGoogleAnalyticData(List<string> accountObjectIDs, DateTime startDate, DateTime endDate)
        {
            using (MongoRepository<GASearchData> repo = new MongoRepository<GASearchData>())
            {
                return repo
                    .Where(l => accountObjectIDs.Contains(l.AccountObjectID) && (l.CreateDate >= startDate && l.CreateDate <= endDate))
                    .Select(l => new GASearchDataModel
                    {
                        ObjectID = l.Id,
                        AccountObjectID = l.AccountObjectID,
                        MxtrAccountID = l.MxtrAccountID,
                        CRMKind = l.CRMKind,
                        CreateDate = l.CreateDate,
                        LastUpdatedDate = l.LastUpdatedDate,
                        BullseyeLocationId = l.BullseyeLocationId,
                        LandingPageviews = l.LandingPageviews,
                        LogoClicks = l.LogoClicks,
                        WebsiteClicks = l.WebsiteClicks,
                        MoreInfoClicks = l.MoreInfoClicks,
                        DirectionsClicks = l.DirectionsClicks,
                        PhoneClicks = l.PhoneClicks,
                    }).ToList().OrderBy(o => o.CreateDate);
            }
        }
    }
}
