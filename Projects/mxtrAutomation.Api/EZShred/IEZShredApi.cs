using mxtrAutomation.Common.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.EZShred
{

    public interface IEZShredApi
    {
        string EZShredIP { get; set; }
        string EZShredPort { get; set; }
        string GetEZShredData(EZShredDataEnum ezSharedDataType, string timestamp = "");
        CustomerInfoResult GetEZShredCustomerInfo(string userId, string customerId);
        BuildingInfoResult GetEZShredBuildingInfo(string userId, string buildingId);
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
}
