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
    public interface ICRMEmailJobService
    {
        CreateNotificationReturn CreateCRMEmailJob(CRMEmailJobDataModel crmEmailJobDataModel);
        CreateNotificationReturn CreateBatchCRMEmailJobs(List<CRMEmailJobDataModel> crmEmailJobDataModels);

        CreateNotificationReturn UpdateEmailJobs(List<CRMEmailJobDataModel> emailDatas, string accountObjectID, string crmKind);

        List<CRMEmailJobDataModel> GetEmailJobsByAccountObjectIDs(List<string> accountObjectIDs);
        List<CRMEmailJobDataModel> GetEmailJobsByAccountObjectIDsForDateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        IEnumerable<CRMEmailJobDataModel> GetEmailJobsByAccountObjectIDsForDateRangeAsEnumerable(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<CRMEmailJobDataModel> GetEmailJobsGroupedByAccountObjectIDsForDateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<CRMEmailJobDataModel> GetEmailJobsGroupedByAccountObjectIDsForDateRange(IEnumerable<CRMEmailJobDataModel> emails);
        List<CRMEmailJobDataModel> GetEmailJobsGroupedByDate(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<CRMEmailJobDataModel> GetEmailJobsGroupedByDate(IEnumerable<CRMEmailJobDataModel> emails);
        IEnumerable<CRMEmailJobDataModel> GetEmailJobs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
    }

    public interface ICRMEmailJobServiceInternal : ICRMEmailJobService
    {
    }
}
