using EasyHttp.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Bullseye
{
    public class BullseyeApi : IBullseyeApi
    {
        private readonly string _baseUrl;
        private int _clientId;
        private string _adminApiKey;
        private string _searchApiKey;

        public BullseyeApi(string argBaseUrl)
        {
            _baseUrl = argBaseUrl;
        }

        public int ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }

        public string AdminApiKey
        {
            get { return _adminApiKey; }
            set { _adminApiKey = value; }
        }

        public string SearchApiKey
        {
            get { return _searchApiKey; }
            set { _searchApiKey = value; }
        }

        public List<RestSearchResponseLogDataModel> GetSearchResponseLog(DateTime startDate, DateTime endDate, out int TotalResults)
        {
            JObject logDataJson = GetByDateRangeDataAdminKey("GetSearchResponseLog", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"));

            return ConvertToRestSearchResponseLogDataModel(logDataJson, out TotalResults);
        }
        
        private List<RestSearchResponseLogDataModel> ConvertToRestSearchResponseLogDataModel(JObject logDataJson, out int TotalResults)
        {
            TotalResults = 0;
            if (!logDataJson.HasValues)
                return new List<RestSearchResponseLogDataModel>();

            try
            {
                TotalResults = int.Parse(logDataJson["TotalResults"].ToString());

                var responseLogs =
                    from x in logDataJson["ResultList"].Children()
                    select new RestSearchResponseLogDataModel()
                    {
                        LocationID = (int)x["LocationID"],
                        LocationName = (string)x["LocationName"],
                        Viewed = (bool)x["Viewed"],
                        URLClicked = (bool)x["URLClicked"],
                        EmailClicked = (bool)x["EmailClicked"],
                        LocationResultsClicked = (bool)x["LocationResultsClicked"],
                        LocationMapClicked = (bool)x["LocationMapClicked"],
                        DirectionsClicked = (bool)x["DirectionsClicked"],
                        DateCreated = (DateTime)x["DateCreated"]
                    };

                return responseLogs.ToList();
            }
            catch (Exception e)
            {
                return new List<RestSearchResponseLogDataModel>();
            }
        }

        private JObject GetByDateRangeDataAdminKey(string methodName, string startDate, string endDate)
        {
            string url = BuildUrlDateRangeAdminKey(methodName, startDate, endDate);
            string responseText = ExecutePostRestCall(url);

            JObject o = new JObject();

            if (string.IsNullOrEmpty(responseText))
            {
                Console.WriteLine("Error: no response text received from server.");
                return o;
            }

            o = JObject.Parse(responseText);
            return o;
        }

        public List<BullseyeAccountDataModel> SearchBullseyeAccount(BullseyeAccountSearchModel search, out int TotalResults) {
            JObject accountsJson = GetByCriteria("DoSearch2", search);

            return ConvertToAccountDataModel(accountsJson, out TotalResults);
        }

        public BullseyeAccountDataModel GetBullseyeLocation(BullseyeAccountSearchModel search)
        {
            JObject accountJson = GetByCriteria("GetLocation", search);

            return ConvertToSingleAccountDataModel(accountJson);
        }

        private JObject GetByCriteria(string methodName, object search) {
            string url = BuildUrlCriteria(methodName, search);
            string responseText = ExecutePostRestCall(url);

            JObject o = new JObject();

            if (string.IsNullOrEmpty(responseText))
            {
                Console.WriteLine("Error: no response text received from server.");
                return o;
            }

            o = JObject.Parse(responseText);
            return o;
        }
        
        private List<BullseyeAccountDataModel> ConvertToAccountDataModel(JObject accountsJson, out int TotalResults)
        {
            TotalResults = 0;
            if (!accountsJson.HasValues)
                return new List<BullseyeAccountDataModel>();

            try
            {
                TotalResults = int.Parse(accountsJson["TotalResults"].ToString());

                var accounts =
                    from x in accountsJson["ResultList"].Children()
                    select new BullseyeAccountDataModel()
                    {
                        Id = (int?)x["Id"],
                        Name = (string)x["Name"],
                        URL = (string)x["URL"],
                        Address1 = (string)x["Address1"],
                        Address2 = (string)x["Address2"],
                        Address3 = (string)x["Address3"],
                        Address4 = (string)x["Address4"],
                        City = (string)x["City"],
                        State = (string)x["State"],
                        PostCode = (string)x["PostCode"],
                        EmailAddress = (string)x["EmailAddress"],
                        PhoneNumber = (string)x["PhoneNumber"],
                        FaxNumber = (string)x["FaxNumber"],
                        MobileNumber = (string)x["MobileNumber"],
                        ContactName = (string)x["ContactName"],
                        ContactPosition = (string)x["ContactPosition"],
                        ThirdPartyId = (string)x["ThirdPartyId"],
                        Distance = (double?)x["Distance"],
                        CategoryIds = (string)x["CategoryIds"],
                        CategoryNames = (string)x["CategoryNames"],
                        CountryCode = (string)x["CountryCode"],
                        Latitude = (decimal?)x["Latitude"],
                        Longitude = (decimal?)x["Longitude"],
                        GeoCodeStatusId = (int)x["GeoCodeStatusId"],
                        InternetLocation = (bool?)x["InternetLocation"],
                        BusinessHours = (string)x["BusinessHours"],
                        LocationTypeName = (string)x["LocationTypeName"],
                        TimeZone = (string)x["TimeZone"],
                        FacebookPageId = (string)x["FacebookPageId"],
                        ImageFileUrl = (string)x["ImageFileUrl"],
                        IsStoreLocator = (bool?)x["IsStoreLocator"],
                        IsLeadManager = (bool?)x["IsLeadManager"]
                    };

                return accounts.ToList();
            }
            catch (Exception e)
            {
                return new List<BullseyeAccountDataModel>();
            }
        }

        private BullseyeAccountDataModel ConvertToSingleAccountDataModel(JObject accountJson)
        {
            try
            {
                JObject x = accountJson;

                return new BullseyeAccountDataModel()
                    {
                        Id = (int?)x["Id"],
                        Name = (string)x["Name"],
                        URL = (string)x["URL"],
                        Address1 = (string)x["Address1"],
                        Address2 = (string)x["Address2"],
                        Address3 = (string)x["Address3"],
                        Address4 = (string)x["Address4"],
                        City = (string)x["City"],
                        State = (string)x["StateAbbr"],
                        PostCode = (string)x["PostCode"],
                        EmailAddress = (string)x["EmailAddress"],
                        PhoneNumber = (string)x["PhoneNumber"],
                        FaxNumber = (string)x["FaxNumber"],
                        MobileNumber = (string)x["MobileNumber"],
                        ContactName = (string)x["ContactName"],
                        ContactPosition = (string)x["ContactPosition"],
                        ThirdPartyId = (string)x["ThirdPartyId"],
                        CountryCode = (string)x["CountryId"],
                        Latitude = (decimal?)x["Latitude"],
                        Longitude = (decimal?)x["Longitude"],
                        GeoCodeStatusId = (int)x["GeoCodeStatusId"],
                        InternetLocation = (bool?)x["InternetLocation"],
                        LocationTypeName = (string)x["LocationTypeName"],
                        FacebookPageId = (string)x["FacebookPageId"],
                        ImageFileUrl = (string)x["ImageFileUrl"],
                        IsActive = (bool)x["Active"]
                };
            }
            catch (Exception e)
            {
                return new BullseyeAccountDataModel();
            }
        }

        private string BuildUrlDateRangeAdminKey(string methodName, string startDate, string endDate)
        {
            return string.Format("{0}{1}?ClientId={2}&ApiKey={3}&startDate={4}&endDate={5}", _baseUrl, methodName, _clientId, _adminApiKey, startDate, endDate);
        }

        private string BuildUrlDateRange(string methodName, string startDate, string endDate)
        {
            return string.Format("{0}{1}?ClientId={2}&ApiKey={3}&startDate={4}&endDate={5}", _baseUrl, methodName, _clientId, _searchApiKey, startDate, endDate);
        }

        private string BuildUrlCriteria(string methodName, object search)
        {
            var urlString = new StringBuilder(_baseUrl + methodName);
            urlString.AppendFormat("?ClientId={0}&ApiKey={1}", _clientId, _searchApiKey);

            foreach (var prop in search.GetType().GetProperties())
            {
                var value = prop.GetValue(search, null);
                if (value != null)
                    urlString.AppendFormat("&{0}={1}", prop.Name, value);
            }

            return urlString.ToString();
        }

        private string ExecutePostRestCall(string url)
        {
            var http = new HttpClient
            {
                Request = { Accept = HttpContentTypes.ApplicationJson }
            };

            HttpResponse response = null;
            try
            {
                response = http.Get(url);
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
