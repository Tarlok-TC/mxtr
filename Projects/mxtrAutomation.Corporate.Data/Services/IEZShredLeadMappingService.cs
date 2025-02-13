using mxtrAutomation.Api.EZShred;
using mxtrAutomation.Common.Enums;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IEZShredLeadMappingService
    {
        List<EZShredLeadMappingDataModel> GetCreateUpdateCustomerData(string accountObjectID);
        List<EZShredLeadMappingDataModel> GetCustomerDataFromEZShredLeadMapping(string accountObjectID);
        List<EZShredLeadMappingDataModel> GetEZShredLeadDataByCustomerId(string accountObjectId, int customerId);
        List<EZShredLeadMappingDataModel> GetCustomerDataFromEZShredLeadMapping(string accountObjectID, string searchText);
        EZShredLeadMappingDataModel GetEZShredLeadDataByAccountId(string accountObjectID);
        EZShredLeadMappingDataModel GetOpportunityIdByLeadId(string accountObjectID, long leadId);
        EZShredLeadMappingDataModel AddEZShredLeadMappingData(EZShredLeadMappingDataModel data);
        bool UpdateEZShredLeadMappingData(EZShredLeadMappingDataModel data);
        bool UpdateEZShredLeadMappingDataWithLeadID(EZShredLeadMappingDataModel data, string buildingSet);
        bool UpdateCustomerId(AddUpdateCustomerResult data, string Id);
        bool UpdateBuildingId(string leadMappingId, int buildingId, LeadBuildingSet whichBuilding);
        bool UpdateOpportunityId(long OpportunityId, string Id);
        bool UpdateRequestStatus(string Id, EZShredStatusKind status);
        bool UpdateRequestStatus(string Id, EZShredStatusKind status, LeadBuildingSet whichBuilding);
        EZShredLeadMappingDataModel GetEZShredLeadDataByLeadID(string accountObjectID, long leadId);
        bool AddLeadBuilding(EZShredLeadMappingDataModel data, LeadBuildingDataModel objLeadBuilding, string BuildingSet);
        List<EZShredLeadMappingDataModel> GetBuildingsDataFromEZShredLeadMapping(string accountObjectID, string searchText);
        bool HandleOldBuildingData();
        int GetBuildingCountByOpportunity(string accountObjectId, long opportunityId);
        List<EZShredLeadMappingDataModel> GetAllEZShredLeadDataByAccountId(string accountObjectID);
    }
    public interface IEZShredLeadMappingInternal : IEZShredLeadMappingService
    {
    }
}
