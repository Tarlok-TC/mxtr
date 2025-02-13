using mxtrAutomation.Api.EZShred;
using mxtrAutomation.Common.Enums;

namespace mxtrAutomation.Api.Services
{
    public interface IEZShredService
    {
        string GetEZShredData(EZShredDataEnum ezSharedDataType, string timeStamp = "");
        CustomerInfoResult GetEZShredCustomerInfo(string userId, string customerId);
        BuildingInfoResult GetEZShredBuildingInfo(string userId, string buildingId);
        void SetConnectionTokens(string eZShredIP, string eZShredPort);
        AddUpdateCustomerResult CreateEZShredCustomer(CustomerDataModel customer);
        AddUpdateCustomerResult UpdateEZShredCustomer(CustomerDataModel customer);
        AddUpdateBuildingResult AddBuilding(BuildingDataModel building);
        AddUpdateBuildingResult UpdateBuilding(BuildingDataModel building);
        AddUpdateCustomerResult CreateUpdateEZShredCustomerWithRequestString(string jsonRequestString);
        AddUpdateBuildingResult CreateUpdateEZShredBuildingWithRequestString(string jsonRequestString);
        ZIPDataModel GetAvailableDates(string userId, string zipCode);
        string GetNextAvailableDate(int userId, int buildingId);
        string GetNextDayRoute(int userId, string nextDate);
    }
    public class EZShredService : IEZShredService
    {
        private readonly IEZShredApi _ezshredApi;
        public EZShredService(IEZShredApi argEZShredApi)
        {
            _ezshredApi = argEZShredApi;
        }
        public void SetConnectionTokens(string eZShredIP, string eZShredPort)
        {
            _ezshredApi.EZShredIP = eZShredIP;
            _ezshredApi.EZShredPort = eZShredPort;
        }
        public string GetEZShredData(EZShredDataEnum ezSharedDataType, string timeStamp = "")
        {
            return _ezshredApi.GetEZShredData(ezSharedDataType, timeStamp);
        }
        public CustomerInfoResult GetEZShredCustomerInfo(string userId, string customerId)
        {
            return _ezshredApi.GetEZShredCustomerInfo(userId, customerId);
        }
        public BuildingInfoResult GetEZShredBuildingInfo(string userId, string buildingId)
        {
            return _ezshredApi.GetEZShredBuildingInfo(userId, buildingId);
        }
        public AddUpdateCustomerResult CreateEZShredCustomer(CustomerDataModel customer)
        {
            return _ezshredApi.CreateEZShredCustomer(customer);
        }
        public AddUpdateCustomerResult UpdateEZShredCustomer(CustomerDataModel customer)
        {
            return _ezshredApi.UpdateEZShredCustomer(customer);
        }

        public AddUpdateBuildingResult AddBuilding(BuildingDataModel building)
        {
            return _ezshredApi.AddBuilding(building);
        }
        public AddUpdateBuildingResult UpdateBuilding(BuildingDataModel building)
        {
            return _ezshredApi.UpdateBuilding(building);
        }
        public AddUpdateCustomerResult CreateUpdateEZShredCustomerWithRequestString(string jsonRequestString)
        {
            return _ezshredApi.CreateUpdateEZShredCustomerWithRequestString(jsonRequestString);
        }
        public AddUpdateBuildingResult CreateUpdateEZShredBuildingWithRequestString(string jsonRequestString)
        {
            return _ezshredApi.CreateUpdateEZShredBuildingWithRequestString(jsonRequestString);
        }
        public ZIPDataModel GetAvailableDates(string userId, string zipCode)
        {
            return _ezshredApi.GetAvailableDates(userId, zipCode);
        }
        public string GetNextAvailableDate(int userId, int buildingId)
        {
            return _ezshredApi.GetNextAvailableDate(userId, buildingId);
        }
        public string GetNextDayRoute(int userId, string nextDate)
        {
            return _ezshredApi.GetNextDayRoute(userId, nextDate);
        }
    }
}
