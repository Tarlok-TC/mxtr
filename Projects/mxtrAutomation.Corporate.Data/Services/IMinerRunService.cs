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
    public interface IMinerRunService
    {
        CreateNotificationReturn CreateMinerRunAuditTrail(MinerRunAuditTrailDataModel minerRunData);
        CreateNotificationReturn UpdateMinerRunDetails(string accountObjectID, MinerRunMinerDetails minerRunMinerDetails);
        MinerRunAuditTrailDataModel GetMinerRunAuditTrailByAccountObjectID(string accountObjectID);
        MinerRunAuditTrailDataModel GetLastMinerRunAuditTrailByAccountObjectID(string accountObjectID);
        CreateNotificationReturn DeleteTrailLogandAddOneEntry(string minerType, DateTime lastEndDateForDataCollection, List<string> accountIds);
        bool UpdateCRMLeadData(List<Tuple<DateTime?, int, long, int>> lstData, string accountObjectId);
        bool UpdateCRMLeadAnalyticsData(List<Tuple<DateTime?, int, long, int>> lstData, string accountObjectId, string mxtrAccountId);
    }

    public interface IMinerRunServiceInternal : IMinerRunService
    {
    }
}
