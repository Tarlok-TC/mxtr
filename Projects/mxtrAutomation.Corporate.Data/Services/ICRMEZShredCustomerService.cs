using mxtrAutomation.Corporate.Data.DataModels;
using System.Collections.Generic;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ICRMEZShredCustomerService
    {
        bool AddUpdateCustomerData(EZShredCustomerDataModel customerData);
        bool AddUpdateCustomerData(List<EZShredCustomerDataModel> lstCustomerData, string accountObjectId, string mxtrAccountId);
        bool InsertUpdateCustomer();
        int GetCustomerCountInEZShredTable();
        IEnumerable<EZShredCustomerDataModel> GetAllCustomerByAccountObjectId(string accountObjectId);
        IEnumerable<EZShredCustomerDataModelMini> GetAllCustomerMiniByAccountObjectId(string accountObjectId);
        IEnumerable<EZShredCustomerDataModelMini> GetAllCustomerMiniByCustomerId(string accountObjectId, string customerId);
        IEnumerable<EZShredCustomerDataModelMini> SearchCustomer(string accountObjectId, string searchCompany);
        bool DeleteDuplicateCustomerRecords();
    }
    public interface ICRMEZShredCustomerServiceInternal : ICRMEZShredCustomerService
    {
    }
}
