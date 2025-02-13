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
    public interface IShawLeadDetailService
    {
        CreateNotificationReturn AddLeadAnalyticData(LeadAnalyticDataModel data);
        bool AddLeadAnalyticData(List<LeadAnalyticDataModel> lstdata);
        bool AddLeadAnalyticData(List<LeadAnalyticDataModel> lstdata, DateTime whichDate);
        bool AddLeadAnalyticData(LeadAnalyticDataModel data, DateTime whichDate);
        List<LeadAnalyticDataModel> GetLeadAnalyticData(string accountObjectId);
        List<LeadAnalyticDataModel> GetLeadAnalyticData(List<string> accountObjectIds);
        List<Tuple<DateTime, int, int, int>> GetColdHotWarmLeadCount(string accountObjectId, DateTime startDate, DateTime endDate);
        List<Tuple<DateTime, int, int, int>> GetColdHotWarmLeadCount(List<string> accountObjectIds, DateTime startDate, DateTime endDate);
        List<Tuple<string, int, int, int>> GetColdHotWarmLeadCountWithObjectId(List<string> accountObjectIds, DateTime? startDate, DateTime? endDate);
        List<LeadAnalyticDataModel> GetLeadAnalyticDataByDateRange(List<string> accountObjectIds, DateTime startDate, DateTime endDate);       
        Tuple<double, int, int, int> GetLeadScores(List<long> leadid, DateTime startDate, DateTime endDate);
        int CheckLeadCount(long leadId, string accountObjectId, CRMKind minerKind);
        DateTime? GetLastRunDate(string accountObjectId, CRMKind minerKind);
        bool UpdateSSCreateDate();
    }
    public interface IShawLeadDetailServiceInternal : IShawLeadDetailService
    {
    }
}
