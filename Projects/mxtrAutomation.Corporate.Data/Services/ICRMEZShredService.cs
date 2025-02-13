using mxtrAutomation.Api.EZShred;
using mxtrAutomation.Corporate.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ICRMEZShredService
    {
        bool AddUpdateData(EZShredDataModel data);
        IOrderedEnumerable<EZShredDataModel> GetEZShredDataByAccountObjectIDs(List<string> accountObjectIDs);
        IOrderedEnumerable<EZShredDataModel> GetEZShredDataByAccountObjectIDs_DateRange(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
        //List<Customers> GetAllCustomers(string AccountObjectId);
        EZShredDataModel GetCustomerAndBuildingInformations(string AccountObjectId, string CustomerID, string BuidlingId);
        //int GetBuildingIdAgainstCustomerId(string AccountObjectId, string CustomerId);
        List<ServiceItems> GetAllServiceItems(string AccountObjectId);
        List<ServiceTypes> GetAllServiceTypes(string AccountObjectId);
        List<Frequencys> GetAllFrequencys(string AccountObjectId);
        List<BuildingTypes> GetAllBuildingTypes(string AccountObjectId);
        List<InvoiceTypes> GetAllInvoiceTypes(string AccountObjectId);
        List<TermTypes> GetAllTermTypes(string AccountObjectId);
        List<ReferralSources> GetAllReferralSources(string AccountObjectId);
        List<CustomerTypes> GetAllCustomerTypes(string AccountObjectId);
        EZShredDataModel GetAllTypes(string AccountObjectId);
        List<SSField> GetAllSSFields(string AccountObjectId);
        int IsSSFieldExist(string AccountObjectId);
        //bool UpdateCustomerAndBuilding(EZShredDataModel data);
        //bool AddCustomerAndBuilding(EZShredDataModel data);
    }
    public interface ICRMEZShredServiceInternal : ICRMEZShredService
    {
    }
}
