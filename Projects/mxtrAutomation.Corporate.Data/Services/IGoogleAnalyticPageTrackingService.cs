using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IGoogleAnalyticPageTrackingService
    {
        CreateNotificationReturn CreateBatchGoogleAnalyticsPageTracking(List<GAPageTrackingDataModel> pagesTracking, string accountObjectID);

        CreateNotificationReturn UpdateGoogleAnalyticsPageTracking(List<GAPageTrackingDataModel> pagesTracking, string accountObjectID);

        List<GAPageTracking> GetCampaignsByAccountObjectIDs(List<string> accountObjectIDs);
    }

    public interface IGoogleAnalyticPageTrackingServiceInternal : IGoogleAnalyticPageTrackingService {
    }
}
