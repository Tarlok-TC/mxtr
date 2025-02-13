using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Common.Ioc;
using System;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System.Linq;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Helpers;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using GoogleMaps.LocationServices;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Api.Services;
using System.Data;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class ManageMinerViewModel : MainLayoutViewModelBase
    {
        //  public IEnumerable<SelectListItem> Clients { get; set; }
        public List<Dealer> Clients { get; set; }
        public int BuildingCount { get; set; }
        public int CustomerCount { get; set; }
    }
    public class Dealer
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public interface IManageMinerViewModelAdapter
    {
        ManageMinerViewModel BuildManageMinerViewModel(IEnumerable<mxtrAccount> client);
    }
    public class ManageMinerViewModelAdapter : IManageMinerViewModelAdapter
    {
        public ManageMinerViewModel BuildManageMinerViewModel(IEnumerable<mxtrAccount> client)
        {
            ManageMinerViewModel model = new ManageMinerViewModel();
            model.PageTitle = "Manage Miner";
            AddClient(model, client);
            return model;
        }
        private void AddClient(ManageMinerViewModel model, IEnumerable<mxtrAccount> client)
        {
            //model.Clients = client.Where(w => !string.IsNullOrEmpty(w.AccountName)).Select(x => new SelectListItem
            //{
            //    Value = x.ObjectID,
            //    Text = x.AccountName + " " + x.ZipCode + " " + x.City + " " + x.State,
            //    Selected = false,
            //});
            model.Clients = client.Where(w => !string.IsNullOrEmpty(w.AccountName)).Select(x => new Dealer
            {
                ID = x.ObjectID,
                Name = x.AccountName + " " + x.ZipCode + " " + x.City + " " + x.State,
            }).ToList();
        }
    }
    public class ManageMinerController : MainLayoutControllerBase
    {
        private readonly IManageMinerViewModelAdapter _viewModelAdapter;
        private static IMinerRunService _dbMinerRunService;
        private readonly IAccountService _accountService;
        private readonly ICRMEZShredBuildingService _buildingService;
        private readonly ICRMEZShredCustomerService _customerService;
        private readonly IEZShredLeadMappingService _dbEZShredLeadMappingService;
        private readonly IShawLeadDetailService _dbShawLeadDetailService;
        private readonly ISharpspringService _apiSharpspringService;

        public ManageMinerController(IManageMinerViewModelAdapter viewModelAdapter, IAccountService accountService, ISharpspringService apiSharpspringService, ICRMEZShredBuildingService buildingService, ICRMEZShredCustomerService customerService, IEZShredLeadMappingService dbEZShredLeadMappingService, IShawLeadDetailService dbShawLeadDetailService)
        {
            _viewModelAdapter = viewModelAdapter;
            _dbMinerRunService = ServiceLocator.Current.GetInstance<IMinerRunService>();
            _accountService = accountService;
            _buildingService = buildingService;
            _customerService = customerService;
            _dbEZShredLeadMappingService = dbEZShredLeadMappingService;
            _dbShawLeadDetailService = dbShawLeadDetailService;
            _apiSharpspringService = apiSharpspringService;
        }

        public ActionResult ViewPage(ManageMinerWebQuery query)
        {
            IEnumerable<mxtrAccount> client = _accountService.GetFlattenedChildAccounts_Client(User.MxtrAccountObjectID);
            int customer = _customerService.GetCustomerCountInEZShredTable();
            int building = _buildingService.GetBuildingCountInEZShredTable();
            ManageMinerViewModel model = _viewModelAdapter.BuildManageMinerViewModel(client);
            model.BuildingCount = building;
            model.CustomerCount = customer;
            return View(ViewKind.ManageMiner, model, query);
        }
        public ActionResult DeleteAuditTraillog(ManageMinerDeleteSharpspringLogWebQuery query)
        {

            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                DateTime lastEndDateForDataCollection = DateTime.SpecifyKind(query.LastEndDateForDataCollection, DateTimeKind.Utc);
                if (lastEndDateForDataCollection != DateTime.MinValue)
                {
                    System.Collections.Generic.List<string> accountObjectIDs = new System.Collections.Generic.List<string>();
                    string minerType = GetMinerTypeName(Convert.ToInt32(query.MinerType));
                    if (!string.IsNullOrEmpty(query.AccountObjectIDs))
                    {
                        accountObjectIDs = query.AccountObjectIDs.Split(',').ToList();
                    }
                    notification = _dbMinerRunService.DeleteTrailLogandAddOneEntry(minerType, lastEndDateForDataCollection, accountObjectIDs);
                }
                else
                {
                    return Json(new { Success = notification.Success, Message = "Invalid date" });
                }
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult DeleteDuplicateBuilding(DeleteDuplicateBuildingWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                notification.Success = _buildingService.DeleteDuplicateBuildingRecords();
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult DeleteDuplicateCustomer(DeleteDuplicateCustomerWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                notification.Success = _customerService.DeleteDuplicateCustomerRecords();
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult SetDomainName(AssignDomainWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                IEnumerable<mxtrAccount> organizationAccounts = _accountService.GetAllAccountsWithOrganization();
                Dictionary<string, string> dicAccounts = new Dictionary<string, string>();
                foreach (var account in organizationAccounts)
                {
                    string domainName = AccountHelpers.CreateDomainName(account.AccountName);
                    if (!dicAccounts.Any(a => a.Value == domainName))
                    {
                        dicAccounts.Add(account.ObjectID, domainName);
                    }
                    else
                    {
                        dicAccounts.Add(account.ObjectID, domainName + "-" + account.ObjectID);
                    }
                }
                notification = _accountService.AddUpdateDomainName(dicAccounts);
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult InsertUpdateCustomers(InsertUpdateCustomerWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                notification.Success = _customerService.InsertUpdateCustomer();
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult InsertUpdateBuildings(InsertUpdateBuildingWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                notification.Success = _buildingService.InsertUpdateBuilding();
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult AddUpdateCustomerDataFromJson(AddUpdateCustomerDataFromJsonWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            if (string.IsNullOrEmpty(query.AccountObjectId) || string.IsNullOrEmpty(query.MxtrObjectId) || string.IsNullOrEmpty(query.FileName))
            {
                return Json(new { Success = notification.Success, Message = "Please fill all textboxes" });
            }
            try
            {
                //"CustomerNYC"
                notification.Success = AddCustomerDataFromJson(query.AccountObjectId, query.MxtrObjectId, query.FileName);
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult AddUpdateBuildingDataFromJson(AddUpdateBuildingDataFromJsonWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            if (string.IsNullOrEmpty(query.AccountObjectId) || string.IsNullOrEmpty(query.MxtrObjectId) || string.IsNullOrEmpty(query.FileName))
            {
                return Json(new { Success = notification.Success, Message = "Please fill all textboxes" });
            }
            try
            {
                //"BuildingNYC"
                notification.Success = AddBuildingDataFromJson(query.AccountObjectId, query.MxtrObjectId, query.FileName);
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult HandleOldBuildingData(HandleOldBuildingDataWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                notification.Success = _dbEZShredLeadMappingService.HandleOldBuildingData();
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult SetOpportunityPipeLines(SetOpportunityPipeLineWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            try
            {
                notification.Success = _accountService.SetOpportunityPipeLine();
            }
            catch (Exception ex)
            {
                result = "error " + ex.Message;
            }
            if (string.IsNullOrEmpty(result))
            {
                result = notification.ObjectID;
            }
            return Json(new { Success = notification.Success, Message = result });
        }

        private string GetMinerTypeName(int minerType)
        {
            string minerName = string.Empty;
            switch (minerType)
            {
                case -1:
                    minerName = Corporate.Data.Enums.CRMKind.None.ToString();
                    break;
                case 0:
                    minerName = Corporate.Data.Enums.CRMKind.Sharpspring.ToString();
                    break;
                case 1:
                    minerName = Corporate.Data.Enums.CRMKind.Bullseye.ToString();
                    break;
                case 2:
                    minerName = Corporate.Data.Enums.CRMKind.GoogleAnalytics.ToString();
                    break;
                case 3:
                    minerName = Corporate.Data.Enums.CRMKind.EZShred.ToString();
                    break;
                case 4:
                    minerName = Corporate.Data.Enums.CRMKind.EZShredCustomerLists.ToString();
                    break;
                case 5:
                    minerName = Corporate.Data.Enums.CRMKind.EZShredBuildingLists.ToString();
                    break;
                case 6:
                    minerName = Corporate.Data.Enums.CRMKind.EZShredServiceLists.ToString();
                    break;
                case 7:
                    minerName = Corporate.Data.Enums.CRMKind.EZShredMiscLists.ToString();
                    break;
                default:
                    break;
            }
            return minerName;
        }

        private bool AddBuildingDataFromJson(string accountObjectId, string mxtrAccountObjectId, string fileName)
        {
            try
            {
                string textBuilding;
                var fileStreamBuilding = new FileStream(Server.MapPath("~\\Scripts\\ProShred\\Data\\" + fileName + ".txt"), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStreamBuilding, Encoding.UTF8))
                {
                    textBuilding = streamReader.ReadToEnd();
                }
                RootObjectBuilding objBuildingInfoResponse = JsonConvert.DeserializeObject<RootObjectBuilding>(textBuilding);
                List<TblBuilding> tblBuilding = objBuildingInfoResponse.tblBuilding;
                List<EZShredBuildingDataModel> buildings = tblBuilding.Select(x => new EZShredBuildingDataModel()
                {
                    AccountObjectId = accountObjectId,
                    MxtrAccountId = mxtrAccountObjectId,
                    CompanyName = x.CompanyName,
                    BuildingID = x.BuildingID,
                    CustomerID = x.CustomerID,
                    Street = x.Street,
                    City = x.City,
                    State = x.State,
                    Zip = x.Zip,
                    ScheduleFrequency = x.ScheduleFrequency,
                    ServiceTypeID = x.ServiceTypeID,
                    Notes = x.Notes,
                    Directions = x.Directions,
                    RoutineInstructions = x.RoutineInstructions,
                    SiteContact1 = x.SiteContact1,
                    SiteContact2 = x.SiteContact2,
                    Phone1 = x.Phone1,
                    Phone2 = x.Phone2,
                    SalesTaxRegionID = x.SalesTaxRegionID,
                    //Suite = x.Suite,
                    SalesmanID = x.SalesmanID,
                    BuildingTypeID = x.BuildingTypeID,
                    UserID = x.userID,
                    //CompanyCountryCode = x.CompanyCountryCode,
                    EndDate = x.EndDate,
                    EZTimestamp = x.ezTimestamp,
                    LastServiceDate = x.LastServiceDate,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    NextServiceDate = x.NextServiceDate,
                    Operation = x.operation,
                    RouteID = x.RouteID,
                    ScheduleDescription = x.ScheduleDescription,
                    ScheduleDOWfri = x.ScheduleDOWfri,
                    ScheduleDOWmon = x.ScheduleDOWmon,
                    ScheduleDOWsat = x.ScheduleDOWsat,
                    ScheduleDOWsun = x.ScheduleDOWsun,
                    ScheduleDOWthu = x.ScheduleDOWthu,
                    ScheduleDOWtue = x.ScheduleDOWtue,
                    ScheduleDOWwed = x.ScheduleDOWwed,
                    ScheduleWeek1 = x.ScheduleWeek1,
                    ScheduleWeek2 = x.ScheduleWeek2,
                    ScheduleWeek3 = x.ScheduleWeek3,
                    ScheduleWeek4 = x.ScheduleWeek4,
                    ScheduleWeek5 = x.ScheduleWeek5,
                    StartDate = x.StartDate,
                    Stop = x.Stop,
                    TimeTaken = x.TimeTaken
                }).ToList();
                _buildingService.AddUpdateBuildingData(buildings, accountObjectId, mxtrAccountObjectId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool AddCustomerDataFromJson(string accountObjectId, string mxtrAccountObjectId, string fileName)
        {
            try
            {
                string textCustomer;
                var fileStream = new FileStream(Server.MapPath("~\\Scripts\\ProShred\\Data\\" + fileName + ".txt"), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    textCustomer = streamReader.ReadToEnd();
                }
                RootObject objCustomerInfoResponse = JsonConvert.DeserializeObject<RootObject>(textCustomer);
                List<TblCustomer> tblCustomers = objCustomerInfoResponse.tblCustomers;
                List<EZShredCustomerDataModel> lstCustomers = tblCustomers.Select(x => new EZShredCustomerDataModel()
                {
                    AccountObjectId = accountObjectId,
                    MxtrAccountId = mxtrAccountObjectId,
                    AllowZeroInvoices = x.AllowZeroInvoices,
                    ARCustomerCode = x.ARCustomerCode,
                    Attention = x.Attention,
                    //BillingContact = x.BillingContact,
                    //BillingContactExtension = x.BillingContactExtension,
                    //BillingContactPhone = x.BillingContactPhone,
                    //BillingContactSameAsMainContact = x.BillingContactSameAsMainContact,
                    //BillingCountryCode = x.BillingCountryCode,
                    CertificateDestruction = x.CertificateDestruction,
                    City = x.City,
                    Company = x.Company,
                    Contact = x.Contact,
                    CreditHold = x.CreditHold,
                    CustomerID = x.CustomerID,
                    CustomerTypeID = x.CustomerTypeID,
                    //DataSource = x.DataSource,
                    EmailAddress = x.EmailAddress,
                    EmailCOD = x.EmailCOD,
                    EmailInvoice = x.EmailInvoice,
                    EZTimestamp = x.ezTimestamp,
                    Fax = x.Fax,
                    InvoiceNote = x.InvoiceNote,
                    InvoiceTypeID = x.InvoiceTypeID,
                    //LeadID = x.LeadID,
                    //Mobile = x.Mobile,
                    Notes = x.Notes,
                    //NumberOfBoxes = x.NumberOfBoxes,
                    // NumberOfEmployees = x.NumberOfEmployees,
                    Operation = x.operation,
                    //OpportunityID = x.OpportunityID,
                    PaidInFull = x.PaidInFull,
                    Phone = x.Phone,
                    //PipelineStatus = x.PipelineStatus,
                    PurchaseOrder = x.PurchaseOrder,
                    PurchaseOrderExpire = x.PurchaseOrderExpire,
                    ReferralSourceID = x.ReferralSourceID,
                    //ServicesProfessionalType = x.ServicesProfessionalType,
                    State = x.State,
                    Street = x.Street,
                    //Suite = x.Suite,
                    //TaxExempt = x.TaxExempt,
                    TermID = x.TermID,
                    //TravelTourismType = x.TravelTourismType,
                    UserID = x.userID,
                    Zip = x.Zip
                }).ToList();
                _customerService.AddUpdateCustomerData(lstCustomers, accountObjectId, mxtrAccountObjectId);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult AssignCoordinatesToAccount(AssignCoordinatesToAccountWebQuery query)
        {
            try
            {
                var accounts = _accountService.GetAllAccounts().Where(x => x.IsActive && x.Latitude == 0 && x.Longitude == 0).ToList();
                foreach (var item in accounts)
                {
                    var address = item.StreetAddress + ", " + item.Country + " " + item.ZipCode;
                    var locationService = new GoogleLocationService();
                    var point = locationService.GetLatLongFromAddress(address);
                    if (point != null)
                    {
                        var latitude = point.Latitude;
                        var longitude = point.Longitude;
                        _accountService.AssignCoordinates(item.ObjectID, latitude, longitude);
                    }
                    System.Threading.Thread.Sleep(500);
                }
                return Json(new { Success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        public ActionResult AddLeadAnalyticalData(LeadAnalyticalWebQuery query)
        {
            try
            {
                _dbShawLeadDetailService.AddLeadAnalyticData(GetDataToAdd());
                return Json(new { Success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        private List<LeadAnalyticDataModel> GetDataToAdd()
        {
            List<LeadAnalyticDataModel> lstData = new List<LeadAnalyticDataModel>();
            Random numRandom = new Random();
            for (int i = 0; i < 10; i++)
            {
                lstData.Add(new LeadAnalyticDataModel()
                {
                    AccountObjectID = "592fc8cab61cf21f34adaef4",
                    MxtrAccountID = "cf660c27-5d99-4586-9d0e-5a4e77f6f391",
                    LeadId = 528439592963,
                    LeadScore = numRandom.Next(200),
                    CreatedDate = System.DateTime.Now.AddDays(i),
                    CRMKind = CRMKind.Sharpspring.ToString(),
                    CreatedOnMXTR = System.DateTime.Now,
                });
            }
            return lstData;
        }
        public ActionResult SetSubscribeLeadUpdates(SubscribeLeadUpdatesWebQuery query)
        {
            try
            {
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(new System.Uri(query.SubscribeUrl).Query);
                var AccountObjectID = queryDictionary["AccountObjectID"].ToString();
                mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(AccountObjectID);
                _apiSharpspringService.SetConnectionTokens(mxtrAccount.SharpspringSecretKey, mxtrAccount.SharpspringAccountID);
                string leadUpdateResponse = _apiSharpspringService.SubscribeToLeadUpdates(query.SubscribeUrl);
                return Json(new { Success = true, Message = leadUpdateResponse });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
        public ActionResult SSCreateDateFix(SSCreateDateFixWebQuery query)
        {
            try
            {
                //_dbShawLeadDetailService.UpdateSSCreateDate();

                var folderPath = Server.MapPath("~\\Data\\ShawLeads\\");
                foreach (string filePath in Directory.EnumerateFiles(folderPath, "*.csv"))
                {
                    using (StreamReader rdr = new StreamReader(filePath))
                    {
                        int counter = 0;
                        string strLine = rdr.ReadLine();
                        int indexOfLeadCreatedDate = 0, indexOfLeadScoreWithDecay = 0, indexOfLeadId = 0, indexOfLeadScore = 0;
                        // Item1:Lead Create Date, Item2:Lead Score with Decay, Item3:Lead Id, Item4:Lead Score
                        List<Tuple<DateTime?, int, long, int>> lstLeadsData = new List<Tuple<DateTime?, int, long, int>>();
                        string accountObjectId = Path.GetFileNameWithoutExtension(filePath);
                        accountObjectId = accountObjectId.Split('_')[0];

                        while (strLine != null)
                        {
                            if (counter == 0)
                            {
                                GetHeaderColumnIndex(strLine, ref indexOfLeadCreatedDate, ref indexOfLeadScoreWithDecay, ref indexOfLeadId, ref indexOfLeadScore);
                            }
                            else
                            {
                                GetLeadsDataFromCSV(folderPath, filePath, counter, strLine, indexOfLeadCreatedDate, indexOfLeadScoreWithDecay, indexOfLeadId, indexOfLeadScore, "CRMLeadError", lstLeadsData);
                            }
                            counter++;
                            strLine = rdr.ReadLine(); //Next
                        }

                        if (lstLeadsData.Count > 0)
                        {
                            // Update data in CRMLead
                            _dbMinerRunService.UpdateCRMLeadData(lstLeadsData, accountObjectId);
                        }
                    }
                };

                return Json(new { Success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("UpdateCRMLeadAnalyticsFromCSV")]
        public ActionResult UpdateCRMLeadAnalyticsFromCSV()
        {
            try
            {
                string msg = String.Empty;
                var folderPath = Server.MapPath("~\\Data\\ShawLeads\\");
                foreach (string filePath in Directory.EnumerateFiles(folderPath, "*.csv"))
                {
                    string accountObjectId = Path.GetFileNameWithoutExtension(filePath);
                    accountObjectId = accountObjectId.Split('_')[0];
                    var account = _accountService.GetAccountMiniByAccountObjectId(accountObjectId);
                    if (account != null)
                    {
                        using (StreamReader rdr = new StreamReader(filePath))
                        {
                            int counter = 0;
                            string strLine = rdr.ReadLine();
                            int indexOfLeadCreatedDate = 0, indexOfLeadScoreWithDecay = 0, indexOfLeadId = 0, indexOfLeadScore = 0;
                            // Item1:Lead Create Date, Item2:Lead Score with Decay, Item3:Lead Id, Item4:Lead Score
                            List<Tuple<DateTime?, int, long, int>> lstLeadsData = new List<Tuple<DateTime?, int, long, int>>();

                            while (strLine != null)
                            {
                                if (counter == 0)
                                {
                                    GetHeaderColumnIndex(strLine, ref indexOfLeadCreatedDate, ref indexOfLeadScoreWithDecay, ref indexOfLeadId, ref indexOfLeadScore);
                                }
                                else
                                {
                                    GetLeadsDataFromCSV(folderPath, filePath, counter, strLine, indexOfLeadCreatedDate, indexOfLeadScoreWithDecay, indexOfLeadId, indexOfLeadScore, "CRMLeadAnalyticsError", lstLeadsData);
                                }
                                counter++;
                                strLine = rdr.ReadLine(); //Next
                            }

                            if (lstLeadsData.Count > 0)
                            {
                                // Update data in CRMLeadAnalytics
                                _dbMinerRunService.UpdateCRMLeadAnalyticsData(lstLeadsData, account.ObjectID, account.MxtrAccountID);
                            }
                        }
                    }
                    else
                    {
                        msg += "Account Id: " + accountObjectId + " does not exist." + Environment.NewLine;
                    }
                };

                return Json(new { Success = true, Message = msg });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("PageActivator")]
        public ActionResult PageActivator()
        {
            return Json(new { Success = true, Message = "" });
        }

        private static void GetLeadsDataFromCSV(string folderPath, string filePath, int counter, string strLine, int indexOfLeadCreatedDate, int indexOfLeadScoreWithDecay, int indexOfLeadId, int indexOfLeadScore, string errorFileName, List<Tuple<DateTime?, int, long, int>> lstLeadsData)
        {
            try
            {
                //var CSV = System.Text.RegularExpressions.Regex.Split(strLine, ",", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(new StringReader(strLine))
                {
                    Delimiters = new string[] { "," }
                };
                string[] CSV = parser.ReadFields();

                DateTime? leadCreatedDate = null;
                int leadScoreWithDecay = 0;
                long leadId = 0;
                int leadScore = 0;
                int innerCount = 0;
                foreach (var item in CSV)
                {
                    if (innerCount == indexOfLeadCreatedDate)
                    {
                        leadCreatedDate = DateTime.Parse(item);
                    }
                    else if (innerCount == indexOfLeadScoreWithDecay)
                    {
                        leadScoreWithDecay = String.IsNullOrEmpty(item) ? 0 : int.Parse(item);
                    }
                    else if (innerCount == indexOfLeadId)
                    {
                        long.TryParse(item, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out leadId);
                        //leadId = String.IsNullOrEmpty(item) ? 0 : long.Parse(item);
                    }
                    else if (innerCount == indexOfLeadScore)
                    {
                        leadScore = String.IsNullOrEmpty(item) ? 0 : int.Parse(item);
                    }
                    innerCount++;
                }
                lstLeadsData.Add(new Tuple<DateTime?, int, long, int>(leadCreatedDate, leadScoreWithDecay, leadId, leadScore));
            }
            catch (Exception ex)
            {
                WriteErrorLog(folderPath, filePath, counter, errorFileName, ex);
            }
        }

        private static void GetHeaderColumnIndex(string strLine, ref int indexOfLeadCreatedDate, ref int indexOfLeadScoreWithDecay, ref int indexOfLeadId, ref int indexOfLeadScore)
        {
            //var data = strLine.Split(','); // Get header row
            var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(new StringReader(strLine))
            {
                Delimiters = new string[] { "," }
            };
            string[] data = parser.ReadFields(); // Get header row
            int innerCount = 0;
            foreach (var item in data)
            {
                if (item.IndexOf("Lead Create Date") >= 0)
                {
                    indexOfLeadCreatedDate = innerCount;
                }
                else if (item.IndexOf("Lead Score with Decay") >= 0)
                {
                    indexOfLeadScoreWithDecay = innerCount;
                }
                else if (item.IndexOf("SharpSpring ID") >= 0)
                {
                    indexOfLeadId = innerCount;
                }
                else if (item.IndexOf("Lead Score") >= 0)
                {
                    indexOfLeadScore = innerCount;
                }
                innerCount++;
            }
        }

        private static void WriteErrorLog(string folderPath, string filePath, int counter, string fileName, Exception ex)
        {
            var lineNumber = counter + 1;
            FileStream fs = new FileStream(folderPath + "/" + fileName + ".txt", FileMode.OpenOrCreate);
            StreamWriter str = new StreamWriter(fs);
            str.BaseStream.Seek(0, SeekOrigin.End);
            str.WriteLine(DateTime.Now.ToLongTimeString() + " " +
                          DateTime.Now.ToLongDateString());
            str.WriteLine("File Name and Path=> " + filePath);
            str.WriteLine("Line Number => " + lineNumber);
            str.WriteLine("Message=> " + ex.Message);
            str.WriteLine("---------------------------------");
            str.WriteLine(System.Environment.NewLine);
            str.Flush();
            str.Close();
            fs.Close();
        }
    }

    public class TblCustomer
    {
        public string CustomerID { get; set; }
        public string Company { get; set; }
        public string Attention { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Notes { get; set; }
        public string PurchaseOrder { get; set; }
        public string PurchaseOrderExpire { get; set; }
        public string ARCustomerCode { get; set; }
        public string CustomerTypeID { get; set; }
        public string ReferralSourceID { get; set; }
        public string InvoiceTypeID { get; set; }
        public string EmailAddress { get; set; }
        public string EmailInvoice { get; set; }
        public string EmailCOD { get; set; }
        public string CertificateDestruction { get; set; }
        public string AllowZeroInvoices { get; set; }
        public string CreditHold { get; set; }
        public string PaidInFull { get; set; }
        public string InvoiceNote { get; set; }
        public string TermID { get; set; }
        public string ezTimestamp { get; set; }
        public string operation { get; set; }
        public string userID { get; set; }
    }

    public class RootObject
    {
        public string Request { get; set; }
        public string status { get; set; }
        public List<TblCustomer> tblCustomers { get; set; }
    }

    public class TblBuilding
    {
        public string BuildingID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string SiteContact1 { get; set; }
        public string Phone1 { get; set; }
        public string SiteContact2 { get; set; }
        public string Phone2 { get; set; }
        public string SalesmanID { get; set; }
        public string Directions { get; set; }
        public string RoutineInstructions { get; set; }
        public string ScheduleDescription { get; set; }
        public string ScheduleFrequency { get; set; }
        public string ScheduleWeek1 { get; set; }
        public string ScheduleWeek2 { get; set; }
        public string ScheduleWeek3 { get; set; }
        public string ScheduleWeek4 { get; set; }
        public string ScheduleWeek5 { get; set; }
        public string ScheduleDOWsun { get; set; }
        public string ScheduleDOWmon { get; set; }
        public string ScheduleDOWtue { get; set; }
        public string ScheduleDOWwed { get; set; }
        public string ScheduleDOWthu { get; set; }
        public string ScheduleDOWfri { get; set; }
        public string ScheduleDOWsat { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ServiceTypeID { get; set; }
        public string RouteID { get; set; }
        public string Stop { get; set; }
        public string TimeTaken { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Notes { get; set; }
        public string BuildingTypeID { get; set; }
        public string SalesTaxRegionID { get; set; }
        public string LastServiceDate { get; set; }
        public string NextServiceDate { get; set; }
        public string ezTimestamp { get; set; }
        public string operation { get; set; }
        public string userID { get; set; }
    }

    public class RootObjectBuilding
    {
        public string Request { get; set; }
        public string status { get; set; }
        public List<TblBuilding> tblBuilding { get; set; }
    }
}
