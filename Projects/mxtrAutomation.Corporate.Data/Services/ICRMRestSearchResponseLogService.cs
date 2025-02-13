using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ICRMRestSearchResponseLogService
    {
        CreateNotificationReturn CreateCRMRestSearchResponseLog(CRMRestSearchResponseLogModel crmRestSearchResponseLogModel);
        CreateNotificationReturn CreateBatchCRMRestSearchResponseLogs(List<CRMRestSearchResponseLogModel> crmRestSearchResponseLogModels);

        CreateNotificationReturn UpdateRestSearchResponseLog(List<CRMRestSearchResponseLogModel> logDatas, string accountObjectID, string crmKind);
        CreateNotificationReturn UpdateRestSearchResponseLog(CRMRestSearchResponseLogModel logData, CRMKind crmKind);
        List<CRMRestSearchResponseLogModel> GetRestSearchResponseLogByAccountObjectIDs(List<string> accountObjectIDs);
        IEnumerable<CRMRestSearchResponseLogModel> GetRestSearchResponseLogByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByAccountObjectIDs(IEnumerable<CRMRestSearchResponseLogModel> logs);
        List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByDate(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<CRMRestSearchResponseLogModel> GetSearchResponseSummariesByDate(IEnumerable<CRMRestSearchResponseLogModel> logs);
    }

    public interface ICRMRestSearchResponseLogServiceInternal : ICRMRestSearchResponseLogService
    {
    }
}
