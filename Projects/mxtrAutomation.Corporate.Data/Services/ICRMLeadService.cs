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
    public interface ICRMLeadService
    {
        CreateNotificationReturn CreateCRMLead(CRMLeadDataModel crmLeadDataModel);
        CreateNotificationReturn CreateBatchCRMLeads(List<CRMLeadDataModel> crmLeadDataModels);
        CreateNotificationReturn UpdateLeads(List<CRMLeadDataModel> leadDatas, string accountObjectID, string crmKind);
        //IEnumerable<CRMLeadDataModel> GetLeadsByAccountObjectIDs(List<string> accountObjectIDs);
        IEnumerable<CRMLeadDataModel> GetLeadsByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        IEnumerable<CRMLeadDataModel> GetLeadsCreatedDateByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        IQueryable<CRMLeadDataModel> GetLeadsByAccountObjectIDsAsQueryable(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        CRMLeadDataModel GetLeadsByObjectID(string objectID);
        List<CRMLeadSummaryDataModel> GetLeadCountsByAccountObjectIDs(List<string> accountObjectIDs, DateTime? startDate, DateTime? endDate);
        List<CRMLeadSummaryDataModel> GetLeadCountsByAccountObjectIDs(IEnumerable<CRMLeadDataModel> leads);
        List<CRMLeadSummaryDataModel> GetLeadCountsByDate(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<CRMLeadSummaryDataModel> GetLeadCountsByDate(IEnumerable<CRMLeadDataModel> leads);
        List<CRMLeadDataModel> GetLeadsByAccountObjectIDsForDateRange(List<string> accountObjectIDs, DateTime? startDate, DateTime? endDate);
        int GetTotalLeads(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        Dictionary<string, int> GetTotalLeadsWithObject(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        List<GroupLeadsDataModel> GetGroupLeads(List<string> accountObjectIDs, List<mxtrAccount> accounts, DateTime startDate, DateTime endDate);
        IEnumerable<CRMLeadDataModel> GetLeadEvents(IEnumerable<string> objectIDs);
        List<string> GetLeadsIdByClonedObjectId(string objectId);
        List<string> GetLeadsIdByCloneLeadID(long clonedLeadID);
        Dictionary<string, Tuple<string, bool, long>> GetAccountByCloneLeadID(long leadID);
        CreateNotificationReturn DeleteLead(long leadId);
        CreateNotificationReturn DeleteLead(string leadId);
        List<CRMLeadDataModel> GetLeadsByLeadIds(List<long> leadIds);
        int GetLeadScores(List<long> leadid);
        double GetAveragePassToDealerDays(string mxtrAccountd);
        double GetAverageCreateToSaleDate(List<long> leadIds);
        double GetAverageLeadTimeInDealers(List<long> leadIds);
        Dictionary<string, double> GetAverageLeadTimeInDealers(List<string> dealerAccountIds, IEnumerable<CRMLeadDataModel> leadAccountData);
        int GetCopiedLeads(string mxtrAccountd);
        int GetCopiedLeads(List<string> dealersAccountIds, DateTime? startDate = null, DateTime? endDate = null);
        Dictionary<string, int> GetCopiedLeadsToDealer(List<string> dealersAccountIds, DateTime? startDate = null, DateTime? endDate = null);
        int GetWonLeadCount(List<long> leadIds);
        double GetConversionRateInDealer(List<long> leadIds);
        Dictionary<string, double> GetPassOffPercentage(List<string> dealersAccountIds, DateTime startDate, DateTime endDate);
        IEnumerable<CRMLeadDataModel> GetLeadsByAccountObjectIDs_DateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
    }

    public interface ICRMLeadServiceInternal : ICRMLeadService
    {
    }
}
