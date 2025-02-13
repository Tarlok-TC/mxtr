using mxtrAutomation.Corporate.Data.DataModels;
using System.Collections.Generic;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ICRMEZShredBuildingService
    {
        bool AddUpdateBuildingData(EZShredBuildingDataModel buildingData);
        bool AddUpdateBuildingData(List<EZShredBuildingDataModel> lstBuildingData, string accountObjectId, string mxtrAccountId);
        bool InsertUpdateBuilding();
        int GetBuildingCountInEZShredTable();
        IEnumerable<EZShredBuildingDataModel> GetAllBuildingByAccountObjectId(string accountObjectId);
        int GetBuildingIdAgainstCustomerId(string accountObjectId, string customerId);
        int GetBuildingCountAgaistCustomerId(string accountObjectId, int customerId);
        int GetCustomerByBuildingId(string accountObjectId, string buildingId);
        IEnumerable<EZShredBuildingDataModel> SearchBuilding(string accountObjectId, string searchText);
        IEnumerable<EZShredBuildingDataModelMini> GetBuildingsAgainstCustomerId(string accountObjectId, string customerId);
        List<CustomerSearchResult> GetBuildingsByCustomerId(string accountObjectId, string customerId);
        bool DeleteDuplicateBuildingRecords();
        bool UpdateOpportunityIDByBuildingId(string accountObjectId, string buildingId, long opportunityId);
    }
    public interface ICRMEZShredBuildingServiceInternal : ICRMEZShredBuildingService
    {
    }
}
