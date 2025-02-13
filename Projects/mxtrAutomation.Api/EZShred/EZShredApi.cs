using EasyHttp.Http;
using mxtrAutomation.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace mxtrAutomation.Api.EZShred
{
    public class EZShredApi : IEZShredApi
    {
        private readonly string _baseUrl;
        private string _eZShredIP;
        private string _eZShredPort;
        public string EZShredIP
        {
            get { return _eZShredIP; }
            set { _eZShredIP = value; }
        }
        public string EZShredPort
        {
            get { return _eZShredPort; }
            set { _eZShredPort = value; }
        }
        public EZShredApi(string argBaseUrl)
        {
            _baseUrl = argBaseUrl;
        }

        public string GetEZShredData(EZShredDataEnum ezSharedDataType, string timestamp = "")
        {
            string result = string.Empty;
            switch (ezSharedDataType)
            {
                case EZShredDataEnum.BuildingTypes:
                    result = GetDataFromAPI("GetBuildingTypes");
                    break;
                case EZShredDataEnum.CustomerTypes:
                    result = GetDataFromAPI("GetCustomerTypes");
                    break;
                case EZShredDataEnum.Routes:
                    result = GetDataFromAPI("GetRoutes");
                    break;
                case EZShredDataEnum.Salesmen:
                    result = GetDataFromAPI("GetSalesmen");
                    break;
                case EZShredDataEnum.SalesTaxRegions:
                    result = GetDataFromAPI("GetSalesTaxRegions");
                    break;
                case EZShredDataEnum.ServiceItems:
                    result = GetDataFromAPI("GetServiceItems");
                    break;
                case EZShredDataEnum.ServiceTypes:
                    result = GetDataFromAPI("GetServiceTypes");
                    break;
                case EZShredDataEnum.CustomerList:
                    result = string.IsNullOrEmpty(timestamp) ? GetDataFromAPI("GetCustomerList") : GetDataFromAPI("GetCustomerList", timestamp);
                    break;
                case EZShredDataEnum.BuildingList:
                    result = string.IsNullOrEmpty(timestamp) ? GetDataFromAPI("GetBuildingList") : GetDataFromAPI("GetBuildingList", timestamp);
                    break;
                case EZShredDataEnum.FrequencyTypes:
                    result = GetDataFromAPI("GetFrequency");
                    break;
                case EZShredDataEnum.ReferralSourceTypes:
                    result = GetDataFromAPI("GetReferralSource");
                    break;
                case EZShredDataEnum.TermsTypes:
                    result = GetDataFromAPI("GetTerms");
                    break;
                case EZShredDataEnum.InvoiceTypes:
                    result = GetDataFromAPI("GetInvoiceTypes");
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return result;
        }

        public CustomerInfoResult GetEZShredCustomerInfo(string userId, string customerId)
        {
            try
            {
                string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
                //url = "http://52.91.2.172:2000/api/ezshred/getdata?port=4561"; // remove after testing
                string data = @"{""Request"":""{\""Request\"":\""GetCustomerInfo\"",\""UserId\"":\""" + userId + "\\\",\\\"CustomerId\\\":\\\"" + customerId + "\\\"}\"}";
                //string SampleData = @"{""Request"":""{\""Request\"":\""GetCustomerInfo\"",\""UserId\"":\""3\"",\""CustomerId\"":\""1078\""}""}";
                //JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetCustomerInfo", "UserID": UserID, "CustomerID": customerID }) 
                string responseText = ExecutePostRestCall(url, data, "application/x-www-form-urlencoded");
                //string successResponseSample = @"{""Success"":true,""FailureInformation"":"""",""Result"":""{\""Request\"": \""GetCustomerInfo\"", \r\n\""status\"": \""OK\"",\r\n\""CustomerID\"": \""1078\"",\r\n\""tblCustomers\"": [\r\n{\""CustomerID\"":\""1078\"",\""Company\"":\""Pierro, Connor & Associates\"",\""Attention\"":\""Accounts Payable\"",\""Street\"":\""43 British American Boulevard\"",\""City\"":\""Albany\"",\""State\"":\""NY\"",\""Zip\"":\""12110\"",\""Contact\"":\""Accounts Payable-Nicole Krawowski\"",\""Phone\"":\""5184592100\"",\""Fax\"":\""\"",\""Notes\"":\""#employees=10\"",\""PurchaseOrder\"":\""\"",\""PurchaseOrderExpire\"":\""\"",\""ARCustomerCode\"":\""19004293 Pierro, Connor & Associates\"",\""CustomerTypeID\"":\""8\"",\""ReferralSourceID\"":\""177\"",\""InvoiceTypeID\"":\""23\"",\""EmailAddress\"":\""nkrasowski@pierrolaw.com\"",\""EmailInvoice\"":\""False\"",\""EmailCOD\"":\""False\"",\""CertificateDestruction\"":\""False\"",\""AllowZeroInvoices\"":\""False\"",\""CreditHold\"":\""False\"",\""PaidInFull\"":\""False\"",\""InvoiceNote\"":\""\"",\""TermID\"":\""1\"",\""ezTimestamp\"":\""2016-11-29 12:53:20 PM\"",\""operation\"":\""Up\"",\""userID\"":\""3\""}\r\n] }""}";

                EZShredResponse objCustomerInfoResponse = null;
                CustomerInfoResult objCustomerInfoResultDetail = null;

                if (!string.IsNullOrEmpty(responseText))
                {
                    objCustomerInfoResponse = JsonConvert.DeserializeObject<EZShredResponse>(responseText);
                    objCustomerInfoResultDetail = JsonConvert.DeserializeObject<CustomerInfoResult>(objCustomerInfoResponse.Result);
                }
                return objCustomerInfoResultDetail;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public BuildingInfoResult GetEZShredBuildingInfo(string userId, string buildingId)
        {
            try
            {
                //string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
                string url = "http://52.91.2.172:2000/api/ezshred/getdata?port=" + _eZShredPort; // remove after testing
                string data = @"{""Request"":""{\""Request\"":\""GetBuildingInfo\"",\""UserId\"":\""" + userId + "\\\",\\\"BuildingId\\\":\\\"" + buildingId + "\\\"}\"}";
                string responseText = ExecutePostRestCall(url, data, "application/x-www-form-urlencoded");

                EZShredResponse objBuildingInfoResponse = null;
                BuildingInfoResult objBuildingInfoResult = null;

                if (!string.IsNullOrEmpty(responseText))
                {
                    objBuildingInfoResponse = JsonConvert.DeserializeObject<EZShredResponse>(responseText);
                    objBuildingInfoResult = JsonConvert.DeserializeObject<BuildingInfoResult>(objBuildingInfoResponse.Result);
                }
                return objBuildingInfoResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetNextAvailableDate(int userId, int buildingId)
        {
            try
            {
                string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
                //url = "http://52.91.2.172:2000/api/ezshred/getdata?port=4561"; // remove after testing  
                string data = @"{""Request"":""{\""Request\"":\""GetNextServiceDate\"",\""UserId\"":\""" + userId + "\\\",\\\"BuildingID\\\":\\\"" + buildingId + "\\\"}\"}";
                // request sample
                //{ 'Request': 'GetNextServiceDate', 'userID':5, 'BuildingID':43}
                //success Sample
                //{"Success":true,"FailureInformation":"","Result":"{\"Request\": \"GetNextServiceDate\", \r\n\"status\": \"OK\",\r\n\"NextServiceDate\": \"2017-10-26\"}"}
                string responseText = ExecutePostRestCall(url, data, "application/x-www-form-urlencoded");

                EZShredResponse objGetNextServiceDateResponse = null;
                NextServiceDateInfo objNextServiceDateInfoResult = null;
                if (!string.IsNullOrEmpty(responseText))
                {
                    objGetNextServiceDateResponse = JsonConvert.DeserializeObject<EZShredResponse>(responseText);
                    objNextServiceDateInfoResult = JsonConvert.DeserializeObject<NextServiceDateInfo>(objGetNextServiceDateResponse.Result);
                }
                if (objNextServiceDateInfoResult != null && objNextServiceDateInfoResult.status.ToLower() == "ok")
                {
                    return objNextServiceDateInfoResult.NextServiceDate;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public ZIPDataModel GetAvailableDates(string userId, string zipCode)
        {
            try
            {
                string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
                string data = @"{""Request"":""{\""Request\"":\""GetNextDatesByZip\"",\""UserId\"":\""" + userId + "\\\",\\\"zip\\\":\\\"" + zipCode + "\\\",\\\"upToDate\\\":\\\"" + DateTime.Now.AddDays(30) + "\\\"}\"}";
                string responseText = ExecutePostRestCall(url, data, "application/x-www-form-urlencoded");
                EZShredResponse objGetAvailableResponse = null;
                ZIPDataModel objAvailableZIPInfoResult = null;
                if (!string.IsNullOrEmpty(responseText))
                {
                    objGetAvailableResponse = JsonConvert.DeserializeObject<EZShredResponse>(responseText);
                    objAvailableZIPInfoResult = JsonConvert.DeserializeObject<ZIPDataModel>(objGetAvailableResponse.Result);
                }
                return objAvailableZIPInfoResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetNextDayRoute(int userId, string nextDate)
        {
            try
            {
                //string url = "http://52.91.2.172:2000/api/ezshred/getdata?port=" + _eZShredPort; // remove after testing
                string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
                string data = @"{""Request"":""{\""Request\"":\""TicketsForTheDay\"",\""UserId\"":\""" + userId.ToString() + "\\\",\\\"date\\\":\\\"" + nextDate + "\\\"}\"}";
                string responseText = ExecutePostRestCall(url, data, "application/x-www-form-urlencoded");
                //Remove below code after testing
                //string responseText = @"{ ""Success"":true,""FailureInformation"":"""",""Result"":""{\""Request\"": \""TicketsForTheDay\"", \r\n\""status\"": \""OK\"",\r\n\""TicketsForTheDay\"": [{\""BuildingID\"": \""2732\"",\""Ticket\"": \""909336500\"",\""Amount\"": \""45\"",\""Email\"": \""mary@mountcarmel.com\""},{\""BuildingID\"": \""2739\"",\""Ticket\"": \""909336496\"",\""Amount\"": \""398.08\"",\""Email\"": \""\""},{\""BuildingID\"": \""2733\"",\""Ticket\"": \""909336499\"",\""Amount\"": \""96\"",\""Email\"": \""jsamson@ccf.org\""},{\""BuildingID\"": \""2906\"",\""Ticket\"": \""102533654\"",\""Amount\"": \""120\"",\""Email\"": \""jaons@roguein.com\""}]}""}";
                return responseText;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private string GetDataFromAPI(string methodName, string timeStamp = "")
        {
            string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
            try
            {
                string data = @"{""Request"":""{\""Request\"":\""" + methodName + @"\""}""}";
                if (!string.IsNullOrEmpty(timeStamp))
                {
                    data = @"{""Request"":""{\""Request\"":\""" + methodName + @"\"",\""after\"":\""" + timeStamp + @"\""}""}";
                }
                string responseText = ExecutePostRestCall(url, data, "application/x-www-form-urlencoded");
                return responseText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: no response text received from server." + ex.Message);
                return string.Empty;
            }
        }

        public AddUpdateCustomerResult CreateEZShredCustomer(CustomerDataModel customer)
        {
            //string customerDataJson = JsonConvert.SerializeObject(customer);
            string customerDataJson = CreateCustomerAPIRequestString(customer);
            return CreateUpdateEZShredCustomer(customerDataJson);
            //---Success Response Sample
            //string SuccessResponseSample = @"{ ""Success"":true,""FailureInformation"":"""",""Result"":""{\""Request\"": \""AddCustomer\"", \r\n\""status\"": \""OK\"",\r\n\""CustomerID\"": \""2636\""}""}";

        }
        public AddUpdateCustomerResult UpdateEZShredCustomer(CustomerDataModel customer)
        {
            //string customerDataJson = JsonConvert.SerializeObject(customer);
            string customerDataJson = UpdateCustomerAPIRequestString(customer);
            return CreateUpdateEZShredCustomer(customerDataJson);
            //---Success Response Sample
            //string SuccessResponseSample = @"{""Success"":true,""FailureInformation"":"""",""Result"":""{\""Request\"": \""UpdateCustomer\"", \r\n\""status\"": \""OK\""}""}";

        }
        public AddUpdateBuildingResult AddBuilding(BuildingDataModel building)
        {
            string buildingDataJson = CreateBuildingAPIRequestString(building);
            return CreateUpdateEZShredBuilding(buildingDataJson);
        }
        public AddUpdateBuildingResult UpdateBuilding(BuildingDataModel building)
        {
            string buildingDataJson = UpdateBuildingAPIRequestString(building);
            return CreateUpdateEZShredBuilding(buildingDataJson);
        }
        public AddUpdateCustomerResult CreateUpdateEZShredCustomerWithRequestString(string jsonRequestString)
        {
            return CreateUpdateEZShredCustomer(jsonRequestString);
        }
        public AddUpdateBuildingResult CreateUpdateEZShredBuildingWithRequestString(string jsonRequestString)
        {
            return CreateUpdateEZShredBuilding(jsonRequestString);
        }
        private AddUpdateBuildingResult CreateUpdateEZShredBuilding(string buildingDataJson)
        {
            try
            {
                AddUpdateBuildingResponse objBuildingResponse = null;
                AddUpdateBuildingResult objBuildingResult = null;

                string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
                //url = "http://52.91.2.172:2000/api/ezshred/getdata"; //Remove after testing 
                string responseText = ExecutePostRestCall(url, buildingDataJson, "application/x-www-form-urlencoded");
                if (!string.IsNullOrEmpty(responseText))
                {
                    objBuildingResponse = JsonConvert.DeserializeObject<AddUpdateBuildingResponse>(responseText);
                    objBuildingResult = JsonConvert.DeserializeObject<AddUpdateBuildingResult>(objBuildingResponse.Result);
                }
                return objBuildingResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private AddUpdateCustomerResult CreateUpdateEZShredCustomer(string customerDataJson)
        {
            try
            {
                AddUpdateCustomerResponse objCustomerResponse = null;
                AddUpdateCustomerResult objCustomerResult = null;

                string url = "http://" + _eZShredIP + ":" + _eZShredPort + "/api/ezshred/getdata";
                //url = "http://52.91.2.172:2000/api/ezshred/getdata"; //Remove after testing 
                string responseText = ExecutePostRestCall(url, customerDataJson, "application/x-www-form-urlencoded");
                if (!string.IsNullOrEmpty(responseText))
                {
                    objCustomerResponse = JsonConvert.DeserializeObject<AddUpdateCustomerResponse>(responseText);
                    objCustomerResult = JsonConvert.DeserializeObject<AddUpdateCustomerResult>(objCustomerResponse.Result);
                }
                return objCustomerResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //private string CreateCustomerDummyData()
        //{
        //    return @"{""Request"":""{\""Request\"":\""AddCustomer\"",\""UserId\"": \""18\"",\""tblCustomers\"": [{\""Company\"": \""SF Test\"",\""Attention\"": \""account\"",\""Street\"": \""5010 State\"",\""City\"": \""Schenectady\"",\""State\"": \""NY\"",\""Zip\"": \""23879\"",\""Contact\"": \""Jhon Tries\"",\""Phone\"": \""456789\"",\""Fax\"": \""08765\"",\""CustomerTypeID\"": \""10\"",\""InvoiceTypeID\"": \""23\"",\""EmailAddress\"": \""Anjyr1@gmail.com\"",\""EmailInvoice\"": \""true\"",\""EmailCOD\"": \""true\"",\""ReferralsourceID\"": \""200\"",\""TermID\"": \""1\"",\""Notes\"": \""#Employee=12\""}]""}";
        //}
        //private string UpdateCustomerDummyData()
        //{
        //    return @"{""Request"":""{\""Request\"":\""UpdateCustomer\"",\""UserId\"":\""18\"",\""CustomerID\"":\""2636\"",\""tblCustomers\"": [{\""Company\"": \""SF Test 2\"",\""Attention\"": \""account\"",\""Street\"": \""5010 State \"",\""City\"": \""Schenectady\"",\""State\"": \""NY\"",\""Zip\"": \""23879\"",\""Contact\"": \""Jhon Tries\"",\""Phone\"": \""456789\"",\""Fax\"": \""08765\"",\""CustomerTypeID\"": \""10\"",\""InvoiceTypeID\"": \""23\"",\""EmailAddress\"": \""Anjyr2@gmail.com\"",\""EmailInvoice\"": \""true\"",\""EmailCOD\"": \""true\"",\""ReferralsourceID\"": \""200\"",\""TermID\"": \""1\"",\""Notes\"": \""#Employee=12\""}]""}";
        //}
        //private string AddBuildingDummyData()
        //{
        //    return @"{""Request"":""{\""Request\"": \""AddBuilding\"",\""UserId\"":\""18\"",\""tblBuilding\"": [{\""BuildingTypeID\"": \""1\"",\""CustomerID\"": \""2636\"",\""CompanyName\"": \""Test Anjyr 1\"",\""Street\"": \""40 Steuben Street\"",\""City\"": \""Albany\"",\""State\"": \""NY\"",\""Zip\"": \""3456\"",\""Phone1\"": \""345678\"",\""Phone2\"": \""4567\"",\""SalesmanID\"": \""342\"",\""Directions\"": \""\"",\""RoutineInstructions\"": \""\"",\""SiteContact1\"": \""Jhon Doe\"",\""SiteContact2\"": \""Micky jec\"",\""RouteID\"": \""113\"",\""Stop\"": \""99\"",\""ScheduleFrequency\"": \""O\"",\""ServiceTypeID\"": \""XX\"",\""SalesTaxRegionID\"": \""1\"", \""TimeTaken\"": \""15\""}]""}";
        //}
        private string CreateCustomerAPIRequestString(CustomerDataModel objEZShredDataModel)
        {
            return @"{""Request"":""{\""Request\"":\""AddCustomer\"",\""UserId\"": \""" + objEZShredDataModel.UserId + "\\\",\\\"tblCustomers\\\": [{\\\"Company\\\": \\\"" + objEZShredDataModel.CustomerData[0].Company + "\\\",\\\"Attention\\\": \\\"" + objEZShredDataModel.CustomerData[0].Attention + "\\\",\\\"Street\\\": \\\"" + objEZShredDataModel.CustomerData[0].Street + "\\\",\\\"City\\\": \\\"" + objEZShredDataModel.CustomerData[0].City + "\\\",\\\"State\\\": \\\"" + objEZShredDataModel.CustomerData[0].State + "\\\",\\\"Zip\\\": \\\"" + objEZShredDataModel.CustomerData[0].Zip + "\\\",\\\"Contact\\\": \\\"" + objEZShredDataModel.CustomerData[0].Contact + "\\\",\\\"Phone\\\": \\\"" + objEZShredDataModel.CustomerData[0].Phone + "\\\",\\\"Fax\\\": \\\"" + objEZShredDataModel.CustomerData[0].Fax + "\\\",\\\"CustomerTypeID\\\": \\\"" + objEZShredDataModel.CustomerData[0].CustomerTypeID + "\\\",\\\"InvoiceTypeID\\\": \\\"" + objEZShredDataModel.CustomerData[0].InvoiceTypeID + "\\\",\\\"EmailAddress\\\": \\\"" + objEZShredDataModel.CustomerData[0].EmailAddress + "\\\",\\\"EmailInvoice\\\": \\\"" + objEZShredDataModel.CustomerData[0].EmailInvoice + "\\\",\\\"EmailCOD\\\": \\\"" + objEZShredDataModel.CustomerData[0].EmailCOD + "\\\",\\\"ReferralsourceID\\\": \\\"" + objEZShredDataModel.CustomerData[0].ReferralsourceID + "\\\",\\\"TermID\\\": \\\"" + objEZShredDataModel.CustomerData[0].TermID + "\\\",\\\"Notes\\\": \\\"" + objEZShredDataModel.CustomerData[0].Notes + "\\\"}]\"}";
        }
        private string UpdateCustomerAPIRequestString(CustomerDataModel objEZShredDataModel)
        {
            return @"{""Request"":""{\""Request\"":\""UpdateCustomer\"",\""UserId\"": \""" + objEZShredDataModel.UserId + "\\\",\\\"CustomerID\\\":\\\"" + objEZShredDataModel.CustomerData[0].CustomerId + "\\\" ,\\\"tblCustomers\\\": [{\\\"Company\\\": \\\"" + objEZShredDataModel.CustomerData[0].Company + "\\\",\\\"Attention\\\": \\\"" + objEZShredDataModel.CustomerData[0].Attention + "\\\",\\\"Street\\\": \\\"" + objEZShredDataModel.CustomerData[0].Street + "\\\",\\\"City\\\": \\\"" + objEZShredDataModel.CustomerData[0].City + "\\\",\\\"State\\\": \\\"" + objEZShredDataModel.CustomerData[0].State + "\\\",\\\"Zip\\\": \\\"" + objEZShredDataModel.CustomerData[0].Zip + "\\\",\\\"Contact\\\": \\\"" + objEZShredDataModel.CustomerData[0].Contact + "\\\",\\\"Phone\\\": \\\"" + objEZShredDataModel.CustomerData[0].Phone + "\\\",\\\"Fax\\\": \\\"" + objEZShredDataModel.CustomerData[0].Fax + "\\\",\\\"CustomerTypeID\\\": \\\"" + objEZShredDataModel.CustomerData[0].CustomerTypeID + "\\\",\\\"InvoiceTypeID\\\": \\\"" + objEZShredDataModel.CustomerData[0].InvoiceTypeID + "\\\",\\\"EmailAddress\\\": \\\"" + objEZShredDataModel.CustomerData[0].EmailAddress + "\\\",\\\"EmailInvoice\\\": \\\"" + objEZShredDataModel.CustomerData[0].EmailInvoice + "\\\",\\\"EmailCOD\\\": \\\"" + objEZShredDataModel.CustomerData[0].EmailCOD + "\\\",\\\"ReferralsourceID\\\": \\\"" + objEZShredDataModel.CustomerData[0].ReferralsourceID + "\\\",\\\"TermID\\\": \\\"" + objEZShredDataModel.CustomerData[0].TermID + "\\\",\\\"Notes\\\": \\\"" + objEZShredDataModel.CustomerData[0].Notes + "\\\"}]\"}";
        }
        private string CreateBuildingAPIRequestString(BuildingDataModel objEZShredDataModel)
        {
            return @"{""Request"":""{\""Request\"":\""AddBuilding\"",\""UserId\"": \""" + objEZShredDataModel.UserId + "\\\",\\\"tblBuilding\\\": [{\\\"BuildingTypeID\\\": \\\"" + objEZShredDataModel.BuildingData[0].BuildingTypeID + "\\\",\\\"CustomerID\\\": \\\"" + objEZShredDataModel.BuildingData[0].CustomerID + "\\\",\\\"CompanyName\\\": \\\"" + objEZShredDataModel.BuildingData[0].CompanyName + "\\\",\\\"Street\\\": \\\"" + objEZShredDataModel.BuildingData[0].Street + "\\\",\\\"City\\\": \\\"" + objEZShredDataModel.BuildingData[0].City + "\\\",\\\"State\\\": \\\"" + objEZShredDataModel.BuildingData[0].State + "\\\",\\\"Zip\\\": \\\"" + objEZShredDataModel.BuildingData[0].Zip + "\\\",\\\"Phone1\\\": \\\"" + objEZShredDataModel.BuildingData[0].Phone1 + "\\\",\\\"Phone2\\\": \\\"" + objEZShredDataModel.BuildingData[0].Phone2 + "\\\",\\\"SalesmanID\\\": \\\"" + objEZShredDataModel.BuildingData[0].SalesmanID + "\\\",\\\"Directions\\\": \\\"" + objEZShredDataModel.BuildingData[0].Directions + "\\\",\\\"RoutineInstructions\\\": \\\"" + objEZShredDataModel.BuildingData[0].RoutineInstructions + "\\\",\\\"SiteContact1\\\": \\\"" + objEZShredDataModel.BuildingData[0].SiteContact1 + "\\\",\\\"SiteContact2\\\": \\\"" + objEZShredDataModel.BuildingData[0].SiteContact2 + "\\\",\\\"RouteID\\\": \\\"" + objEZShredDataModel.BuildingData[0].RouteID + "\\\",\\\"Stop\\\": \\\"" + objEZShredDataModel.BuildingData[0].Stop + "\\\",\\\"ScheduleFrequency\\\": \\\"" + objEZShredDataModel.BuildingData[0].ScheduleFrequency + "\\\",\\\"ServiceTypeID\\\": \\\"" + objEZShredDataModel.BuildingData[0].ServiceTypeID + "\\\",\\\"SalesTaxRegionID\\\": \\\"" + objEZShredDataModel.BuildingData[0].SalesTaxRegionID + "\\\",\\\"TimeTaken\\\": \\\"" + objEZShredDataModel.BuildingData[0].TimeTaken + "\\\"}]\"}";
        }
        private string UpdateBuildingAPIRequestString(BuildingDataModel objEZShredDataModel)
        {
            return @"{""Request"":""{\""Request\"":\""UpdateBuilding\"", \""UserId\"":\""" + objEZShredDataModel.UserId + "\\\",\\\"BuildingID\\\": \\\"" + objEZShredDataModel.BuildingData[0].BuildingID + "\\\",\\\"tblBuilding\\\": [{\\\"BuildingTypeID\\\":\\\"" + objEZShredDataModel.BuildingData[0].BuildingTypeID + "\\\",\\\"CustomerID\\\": \\\"" + objEZShredDataModel.BuildingData[0].CustomerID + "\\\",\\\"CompanyName\\\": \\\"" + objEZShredDataModel.BuildingData[0].CompanyName + "\\\",\\\"Street\\\": \\\"" + objEZShredDataModel.BuildingData[0].Street + "\\\",\\\"City\\\":\\\"" + objEZShredDataModel.BuildingData[0].City + "\\\",\\\"State\\\":\\\"" + objEZShredDataModel.BuildingData[0].State + "\\\",\\\"Zip\\\":\\\"" + objEZShredDataModel.BuildingData[0].Zip + "\\\",\\\"Phone1\\\":\\\"" + objEZShredDataModel.BuildingData[0].Phone1 + "\\\",\\\"Phone2\\\":\\\"" + objEZShredDataModel.BuildingData[0].Phone2 + "\\\",\\\"SalesmanID\\\":\\\"" + objEZShredDataModel.BuildingData[0].SalesmanID + "\\\",\\\"Directions\\\":\\\"" + objEZShredDataModel.BuildingData[0].Directions + "\\\",\\\"RoutineInstructions\\\":\\\"" + objEZShredDataModel.BuildingData[0].RoutineInstructions + "\\\",\\\"SiteContact1\\\":\\\"" + objEZShredDataModel.BuildingData[0].SiteContact1 + "\\\",\\\"SiteContact2\\\":\\\"" + objEZShredDataModel.BuildingData[0].SiteContact2 + "\\\",\\\"RouteID\\\":\\\"0\\\",\\\"Stop\\\": \\\"99\\\",\\\"ScheduleFrequency\\\":\\\"" + objEZShredDataModel.BuildingData[0].ScheduleFrequency + "\\\",\\\"ServiceTypeID\\\":\\\"" + objEZShredDataModel.BuildingData[0].ServiceTypeID + "\\\",\\\"SalesTaxRegionID\\\":\\\"0\\\",\\\"TimeTaken\\\":\\\"15\\\"}]\"}";
        }

        private static string GetFinalisedData(string responseText, string fieldName)
        {
            try
            {
                //JObject o = new JObject();

                //if (string.IsNullOrEmpty(responseText))
                //{
                //    Console.WriteLine("Error: no response text received from server.");
                //}

                //o = JObject.Parse(responseText);

                var jsonData = (JObject)JsonConvert.DeserializeObject(responseText);
                IList<string> keys = jsonData.Properties().Select(p => p.Name).ToList();
                string res = jsonData["Result"].Value<string>();

                var jsonDataChild = (dynamic)JsonConvert.DeserializeObject(res);
                string output = (((JObject)jsonDataChild).Property(fieldName)).Value.ToString();

                return output;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        private string ExecutePostRestCall(string url, object data, string contentType, object query = null)
        {
            var http = new HttpClient
            {
                Request = { Accept = HttpContentTypes.ApplicationJson }
            };

            HttpResponse response = null;
            try
            {
                response = http.Post(url, data, contentType);
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error: web exception - msg = {0}", ex.Message);
                return string.Empty;
            }

            var statcode = response.StatusCode;
            string statdesc = response.StatusDescription;

            if (statcode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Error: response code = {0}", statdesc);
                return string.Empty;
            }

            return response.RawText;
        }

    }
}
