using System;
using System.IO;
using System.Net;
using System.Text;
using mxtrAutomation.Api.Services;
using Ninject;
using System.Reflection;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Common.Enums;
using RestSharp;
using mxtrAutomation.Corporate.Data.DataModels;
using System.Collections.Generic;
using Newtonsoft.Json;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Api.EZShred;
using System.Linq;
using System.Configuration;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.EZShred
{
    public class Program
    {
        private static IAccountService _dbAccountService;
        private static IEZShredService _apiEZShredService;
        private static ICRMEZShredService _dbCRMEZShredService;
        private static IErrorLogService _dbErrorLogService;
        private static IEZShredLeadMappingService _dbEZShredLeadMappingService;
        private static IMinerRunService _dbMinerRunService;
        private static IEZShredMinerLogService _dbEZShredMinerLogService;
        private static ICRMEZShredCustomerService _dbEZShredCustomerService;
        private static ICRMEZShredBuildingService _dbEZShredBuildingService;

        static void Main(string[] args)
        {
            try
            {
                AppStart();
                PrepareServices();
                ProcessEZShredForAllAccounts();
                Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                {
                    LogTime = DateTime.UtcNow,
                    Description = "Miner Setup Error",
                    LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                    ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                });
            }
        }



        private static void ProcessEZShredForAllAccounts()
        {
            ////set data for getting data
            //_apiEZShredService.SetConnectionTokens("72.1.203.91", "4550");
            ////get data from EZShred API
            //EZShredDataModel data = GetData_EZShred("72.1.203.91", "4550");

            bool isFirstTimeRun = false;
            string timestamp = "01/01/2010";
            var summary = new List<MinerRunDataCollectionSummary>();

            IEnumerable<mxtrAccount> accounts = _dbAccountService.GetAllAccountsWithEZShred();
            Console.WriteLine("Processing EZShred data for all {0} accounts...", accounts.Count());
            int flag = 0;
            foreach (var account in accounts)
            {
                if (account.EZShredIP == "0" && account.EZShredPort == "0")
                {
                    continue;
                }
                if (flag > 0)
                {
                    System.Threading.Thread.Sleep(2 * 1000 * 60);//set 2 Minutes
                }
                flag++;
                try
                {
                    if (!_dbEZShredMinerLogService.IsEZMinerRecordExist(account.ObjectID))
                    {
                        EZShredMinerLogDataModel objEZShredMinerLogDataModel = new EZShredMinerLogDataModel
                        {
                            AccountObjectId = account.ObjectID,
                            LocationName = account.AccountName,
                            Port = account.EZShredPort,
                        };
                        _dbEZShredMinerLogService.CreateEZMinerlog(objEZShredMinerLogDataModel);
                    }

                    MinerRunAuditTrailDataModel lastMinerRunAuditTrailDataModel = _dbMinerRunService.GetLastMinerRunAuditTrailByAccountObjectID(account.ObjectID);
                    DateTime entryStart = System.DateTime.UtcNow;
                    DateTime entryEnd = entryStart;

                    Console.WriteLine(string.Format("Processing data for {0} at time {1}", account.AccountName, DateTime.UtcNow));
                    //set IP and Port info for getting data
                    _apiEZShredService.SetConnectionTokens(account.EZShredIP, account.EZShredPort);
                    //Get Data from EZShredLeadMapping table to create/update customer
                    if (!string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["IsCreateUpdateOnEZShred"])) && Convert.ToBoolean(ConfigurationManager.AppSettings["IsCreateUpdateOnEZShred"]))
                    {
                        #region create/update customer/building
                        List<EZShredLeadMappingDataModel> lstEZShredLeadData = GetCreateUpdateCustomerData(account.ObjectID);
                        // Create  Customer
                        CreateCustomer(lstEZShredLeadData.Where(x => x.EZShredActionType == EZShredActionTypeKind.Create.ToString() && x.EZShredStatus == EZShredStatusKind.Failed.ToString()).ToList(), account);
                        //Update Customer
                        UpdateCustomer(lstEZShredLeadData.Where(x => x.EZShredActionType == EZShredActionTypeKind.Update.ToString() && x.EZShredStatus == EZShredStatusKind.Failed.ToString()).ToList(), account);
                        //again get data let pending
                        List<EZShredLeadMappingDataModel> lstEZShredLeadDataAferCreateUpdate = GetCreateUpdateCustomerData(account.ObjectID);
                        //check for building create pending
                        List<EZShredLeadMappingDataModel> buildingTobeCreated = lstEZShredLeadDataAferCreateUpdate.Where(x =>
                        (x.Building1.EZShredActionType == EZShredActionTypeKind.Create.ToString() && x.Building1.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                        (x.Building2.EZShredActionType == EZShredActionTypeKind.Create.ToString() && x.Building2.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                        (x.Building3.EZShredActionType == EZShredActionTypeKind.Create.ToString() && x.Building3.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                        (x.Building4.EZShredActionType == EZShredActionTypeKind.Create.ToString() && x.Building4.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                        (x.Building5.EZShredActionType == EZShredActionTypeKind.Create.ToString() && x.Building5.EZShredStatus == EZShredStatusKind.Failed.ToString())
                       ).ToList();

                        // create building 
                        CreateBuilding(buildingTobeCreated, account);

                        //check for building update pending
                        List<EZShredLeadMappingDataModel> buildingTobeUpdated = lstEZShredLeadDataAferCreateUpdate.Where(x =>
                         (x.Building1.EZShredActionType == EZShredActionTypeKind.Update.ToString() && x.Building1.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                         (x.Building2.EZShredActionType == EZShredActionTypeKind.Update.ToString() && x.Building2.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                         (x.Building3.EZShredActionType == EZShredActionTypeKind.Update.ToString() && x.Building3.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                         (x.Building4.EZShredActionType == EZShredActionTypeKind.Update.ToString() && x.Building4.EZShredStatus == EZShredStatusKind.Failed.ToString()) ||
                         (x.Building5.EZShredActionType == EZShredActionTypeKind.Update.ToString() && x.Building5.EZShredStatus == EZShredStatusKind.Failed.ToString())
                         ).ToList();
                        //update building
                        UpdateBuilding(buildingTobeUpdated, account);

                        #endregion
                    }

                    // Get EZShredCustomerLists data
                    #region  Get EZShredCustomerLists data
                    Console.WriteLine(string.Format("Getting EZShred Customer Lists data for {0}", account.AccountName));
                    isFirstTimeRun = IsFirstEZShredMinerRun(lastMinerRunAuditTrailDataModel, MinerKind.EZShredCustomerLists.ToString());
                    entryStart = EstablishStartDateForMinerRun(lastMinerRunAuditTrailDataModel, entryStart, MinerKind.EZShredCustomerLists.ToString());
                    timestamp = isFirstTimeRun == false ? entryStart.ToString() : timestamp;
                    double duration = (System.DateTime.UtcNow - entryStart).TotalMinutes;
                    if (isFirstTimeRun || duration >= Convert.ToInt32(ConfigurationManager.AppSettings["EZShredGetCustomerDataAfter"]))
                    {
                        try
                        {
                            Console.WriteLine(string.Format("Getting EZShred Customer Lists data from date {0}", timestamp));
                            EZShredDataModel customerLists = GetEZShredCustomerLists(account.ObjectID, account.MxtrAccountID, timestamp);
                            if (customerLists != null)
                            {
                                //add/update customer
                                //List<EZShredCustomerDataModel> lstCustomers = _dbEZShredCustomerService.GetCustomerDataModel(customerLists.Customers, account.ObjectID, account.MxtrAccountID);
                                //List<EZShredCustomerDataModel> lstCustomers = customerLists.Customer;
                                _dbEZShredCustomerService.AddUpdateCustomerData(customerLists.Customer, account.ObjectID, account.MxtrAccountID);
                                // add/update rest of the items like CustomerType , InvoiceType etc...
                                bool result = _dbCRMEZShredService.AddUpdateData(customerLists);
                                UpdateMinerLogs(MinerKind.EZShredCustomerLists.ToString(), isFirstTimeRun, summary, account, entryStart, entryEnd);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in getting customerlists data. Error detalis..." + ex.Message);
                            _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                            {
                                LogTime = DateTime.UtcNow,
                                Description = "Error in getting customerlists data for account: " + account.AccountName,
                                LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                                ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("Last run and current time difference(" + Math.Round(duration, 2) + ") is less than as set in app config(" + ConfigurationManager.AppSettings["EZShredGetCustomerDataAfter"] + ").");
                    }
                    #endregion
                    // Get EZShredBuildingLists data
                    #region Get EZShredBuildingLists data
                    Console.WriteLine(string.Format("Getting EZShred Building Lists data for {0}", account.AccountName));
                    isFirstTimeRun = IsFirstEZShredMinerRun(lastMinerRunAuditTrailDataModel, MinerKind.EZShredBuildingLists.ToString());
                    entryStart = EstablishStartDateForMinerRun(lastMinerRunAuditTrailDataModel, entryStart, MinerKind.EZShredBuildingLists.ToString());
                    timestamp = isFirstTimeRun == false ? entryStart.ToString() : timestamp;
                    duration = (System.DateTime.UtcNow - entryStart).TotalMinutes;
                    if (isFirstTimeRun || duration >= Convert.ToInt32(ConfigurationManager.AppSettings["EZShredGetBuildingDataAfter"]))
                    {
                        try
                        {
                            Console.WriteLine(string.Format("Getting EZShred Building Lists data from date {0}", timestamp));
                            EZShredDataModel buildingLists = GetEZShredBuildingLists(account.ObjectID, account.MxtrAccountID, timestamp);
                            if (buildingLists != null)
                            {
                                //add/update building
                                //List<EZShredBuildingDataModel> lstBuildings = buildingLists.Building;
                                _dbEZShredBuildingService.AddUpdateBuildingData(buildingLists.Building, account.ObjectID, account.MxtrAccountID);
                                // add/update rest of the items like BuildingType , Routes etc...
                                bool result = _dbCRMEZShredService.AddUpdateData(buildingLists);
                                UpdateMinerLogs(MinerKind.EZShredBuildingLists.ToString(), isFirstTimeRun, summary, account, entryStart, entryEnd);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in getting buildinglists data. Error detalis..." + ex.Message);
                            _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                            {
                                LogTime = DateTime.UtcNow,
                                Description = "Error in getting buildinglists data for account: " + account.AccountName,
                                LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                                ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("Last run and current time difference(" + Math.Round(duration, 2) + ") is less than as set in app config(" + ConfigurationManager.AppSettings["EZShredGetBuildingDataAfter"] + ").");
                    }
                    #endregion
                    // Get EZShredServiceLists data
                    #region Get EZShredServiceLists data
                    Console.WriteLine(string.Format("Getting EZShred Service Lists data for {0}", account.AccountName));
                    isFirstTimeRun = IsFirstEZShredMinerRun(lastMinerRunAuditTrailDataModel, MinerKind.EZShredServiceLists.ToString());
                    entryStart = EstablishStartDateForMinerRun(lastMinerRunAuditTrailDataModel, entryStart, MinerKind.EZShredServiceLists.ToString());
                    timestamp = isFirstTimeRun == false ? entryStart.ToString() : timestamp;
                    duration = (System.DateTime.UtcNow - entryStart).TotalMinutes;
                    if (isFirstTimeRun || duration >= Convert.ToInt32(ConfigurationManager.AppSettings["EZShredGetServiceDataAfter"]))
                    {
                        try
                        {
                            EZShredDataModel serviceLists = GetEZShredServiceLists(account.ObjectID, account.MxtrAccountID, timestamp);
                            if (serviceLists != null)
                            {
                                bool result = _dbCRMEZShredService.AddUpdateData(serviceLists);
                                UpdateMinerLogs(MinerKind.EZShredServiceLists.ToString(), isFirstTimeRun, summary, account, entryStart, entryEnd);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in getting servicelists data. Error detalis..." + ex.Message);
                            _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                            {
                                LogTime = DateTime.UtcNow,
                                Description = "Error in getting servicelists data for account: " + account.AccountName,
                                LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                                ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("Last run and current time difference(" + Math.Round(duration, 2) + ") is less than as set in app config(" + ConfigurationManager.AppSettings["EZShredGetServiceDataAfter"] + ").");
                    }
                    #endregion
                    // Get EZShredMiscLists data
                    #region Get EZShredMiscLists data
                    Console.WriteLine(string.Format("Getting EZShred Misc Lists data for {0}", account.AccountName));
                    isFirstTimeRun = IsFirstEZShredMinerRun(lastMinerRunAuditTrailDataModel, MinerKind.EZShredMiscLists.ToString());
                    entryStart = EstablishStartDateForMinerRun(lastMinerRunAuditTrailDataModel, entryStart, MinerKind.EZShredMiscLists.ToString());
                    timestamp = isFirstTimeRun == false ? entryStart.ToString() : timestamp;
                    duration = (System.DateTime.UtcNow - entryStart).TotalMinutes;
                    if (isFirstTimeRun || duration >= Convert.ToInt32(ConfigurationManager.AppSettings["EZShredGetMiscDataAfter"]))
                    {
                        try
                        {
                            EZShredDataModel miscLists = GetEZShredMiscLists(account.ObjectID, account.MxtrAccountID, timestamp);
                            if (miscLists != null)
                            {
                                bool result = _dbCRMEZShredService.AddUpdateData(miscLists);
                                UpdateMinerLogs(MinerKind.EZShredMiscLists.ToString(), isFirstTimeRun, summary, account, entryStart, entryEnd);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in getting misclists data. Error detalis..." + ex.Message);
                            _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                            {
                                LogTime = DateTime.UtcNow,
                                Description = "Error in getting misclists data for account: " + account.AccountName,
                                LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                                ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine("Last run and current time difference(" + Math.Round(duration, 2) + ") is less than as set in app config(" + ConfigurationManager.AppSettings["EZShredGetMiscDataAfter"] + ").");
                    }
                    #endregion
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error in getting data. Moving to next account..." + ex.Message);
                    _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                    {
                        LogTime = DateTime.UtcNow,
                        Description = "Error in processing account: " + account.AccountName,
                        LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                        ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                    });
                }
            }
        }

        private static void UpdateMinerLogs(string minerName, bool isFirstTimeRun, List<MinerRunDataCollectionSummary> summary, mxtrAccount account, DateTime entryStart, DateTime entryEnd)
        {
            MinerRunMinerDetails minerRunMinerDetails = new MinerRunMinerDetails()
            {
                MinerName = minerName,
                IsFirstMinerRunComplete = true,
                LastMinerRunTime = System.DateTime.Now,
                LastStartDateForDataCollection = new DateTime(entryStart.Year, entryStart.Month, entryStart.Day, entryStart.Hour, entryStart.Minute, entryStart.Second, DateTimeKind.Utc),
                LastEndDateForDataCollection = new DateTime(entryEnd.Year, entryEnd.Month, entryEnd.Day, entryEnd.Hour, entryEnd.Minute, entryEnd.Second, DateTimeKind.Utc), // Start and Last date must be same for EZShred miner
                Summary = summary
            };
            UpdateMinerRunAuditTrail(account, isFirstTimeRun, minerRunMinerDetails);
        }

        protected static bool IsFirstEZShredMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel, string minerName)
        {
            if (minerRunAuditTrailDataModel == null || minerRunAuditTrailDataModel.MinerRunDetails == null)
                return true;

            //no details for EZShred, so obviously it hasn't run, thus it's the first EZShred run
            if (!minerRunAuditTrailDataModel.MinerRunDetails.Any(x => x.MinerName == minerName))
                return true;

            //get all details for EZShred miner runs
            //if any have first miner run complete = true, then it has run, thus it's not the first EZShred run
            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
                .Where(x => x.MinerName == minerName)
                .ToList();

            if (minerRunMinerDetails.Any(x => x.IsFirstMinerRunComplete))
                return false;

            return true;
        }

        protected static DateTime EstablishStartDateForMinerRun(MinerRunAuditTrailDataModel minerRunAuditTrailDataModel, DateTime entryStart, string minerName)
        {
            DateTime now = entryStart;
            DateTime startDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
            DateTime endDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
            if (minerRunAuditTrailDataModel == null || minerRunAuditTrailDataModel.MinerRunDetails == null)
                return startDate;

            List<MinerRunMinerDetails> minerRunMinerDetails = minerRunAuditTrailDataModel.MinerRunDetails
               .Where(x => x.MinerName == minerName)
               .ToList();

            if (minerRunMinerDetails.Count > 0)
            {
                DateTime lastEndDateForCollection = minerRunMinerDetails.Max(x => x.LastEndDateForDataCollection);

                if (lastEndDateForCollection <= endDate)
                    startDate = new DateTime(lastEndDateForCollection.Year, lastEndDateForCollection.Month, lastEndDateForCollection.Day, lastEndDateForCollection.Hour, lastEndDateForCollection.Minute, lastEndDateForCollection.Second, DateTimeKind.Utc);
            }

            return startDate;
        }

        private static List<EZShredLeadMappingDataModel> GetCreateUpdateCustomerData(string accountObjectID)
        {
            return _dbEZShredLeadMappingService.GetCreateUpdateCustomerData(accountObjectID);
        }

        private static void CreateCustomer(List<EZShredLeadMappingDataModel> lstEZShredLeadData, mxtrAccount account)
        {
            Console.WriteLine("Creating EZShred customer...");
            CustomerDataModel customerData = new CustomerDataModel();

            if (lstEZShredLeadData != null && lstEZShredLeadData.Count > 0)
            {
                foreach (var leadData in lstEZShredLeadData)
                {
                    string creatingWhat = "Customer/Building";
                    string doingWhat = "Create";
                    try
                    {
                        int customerEZShredId = 0;
                        if (leadData.CustomerID == null)
                        {
                            creatingWhat = "Customer";
                            Console.WriteLine(string.Format("Creating EZShred customer {0} having leadMappingId {1} and lead Id {2}", leadData.UserID, leadData.Id, leadData.LeadID));
                            //string dummyCustomer = @"{'Request': null,'UserId': null,'CustomerData': [{'Company': null,'Attention': null,'Street': null,'City': null,'State': null,'Zip': null,'Contact': null,'Phone': null,'Fax': null,'CustomerTypeID': null,'InvoiceTypeID': null,'EmailAddress': null,'EmailInvoice': null,'EmailCOD': null,'ReferralsourceID': null,'TermID': null,'Notes': null,'CustomerId': null}]}";
                            customerData = JsonConvert.DeserializeObject<CustomerDataModel>(leadData.EZShredApiRequest);
                            //hit EZShred api to create user
                            AddUpdateCustomerResult objResult = _apiEZShredService.CreateEZShredCustomer(customerData);
                            if (objResult != null)
                            {
                                if (objResult.status.ToLower() == "ok")
                                {
                                    customerEZShredId = Convert.ToInt32(objResult.CustomerID);
                                    _dbEZShredLeadMappingService.UpdateCustomerId(objResult, leadData.Id);
                                    _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete);
                                    //Get ARCustomerCode of Newly Created Customer
                                    //--CustomerInfoResult objCustomerInfoResult = _apiEZShredService.GetEZShredCustomerInfo(customerData.UserId, objResult.CustomerID);
                                    //Add to Sharpspring lead and opportunity or keep in database
                                }
                            }
                        }
                        else
                        {
                            customerEZShredId = Convert.ToInt32(leadData.CustomerID);
                            //set status to complete as Customer is already created
                            if (customerEZShredId > 0)
                            {
                                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete);
                            }
                        }
                        //create building after creating customer
                        if (customerEZShredId > 0)
                        {
                            Console.WriteLine("Creating EZShred building");
                            creatingWhat = "Building";
                            //string dummy = @"{'Request': null,'UserId': null,'BuildingData': [{'BuildingTypeID': null,'CustomerID': null,'CompanyName': null,'Street': null,'City': null,'State': null,'Zip': null,'Phone1': null,'Phone2': null,'SalesmanID': null,'Directions': null,'RoutineInstructions': null,'SiteContact1': null,'SiteContact2': null,'RouteID': null,'Stop': null,'ScheduleFrequency': null,'ServiceTypeID': null,'SalesTaxRegionID': null,'TimeTaken': null}]}";                                        
                            //Handle multiple building case here
                            Dictionary<LeadBuildingSet, BuildingDataModel> dicBuildingData = ExtractCreateUpdateBuildingData(leadData, EZShredActionTypeKind.Create);
                            foreach (var dataItem in dicBuildingData)
                            {
                                var building = dataItem.Value;
                                LeadBuildingSet whichBuilding = dataItem.Key;

                                if (building.BuildingData != null)
                                {
                                    if (string.IsNullOrEmpty(building.BuildingData[0].BuildingID))
                                    {
                                        //set customer Id before creating building as customer id is required for building
                                        building.BuildingData[0].CustomerID = Convert.ToString(customerEZShredId);
                                        AddUpdateBuildingResult objBuildingResult = _apiEZShredService.AddBuilding(building);
                                        if (objBuildingResult != null)
                                        {
                                            if (objBuildingResult.status.ToLower() == "ok")
                                            {
                                                //set status from failed (customer search page status) to complete (from miner)
                                                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete, whichBuilding);
                                                //update building Id in the building set once building is created
                                                _dbEZShredLeadMappingService.UpdateBuildingId(leadData.Id, Convert.ToInt32(objBuildingResult.BuildingId), whichBuilding);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //update building 
                                        creatingWhat = "Building";
                                        doingWhat = "Update";
                                        AddUpdateBuildingResult objBuildingResult = _apiEZShredService.UpdateBuilding(building);
                                        if (objBuildingResult.status.ToLower() == "ok")
                                        {
                                            //set status from failed (customer search page status) to complete (from miner)
                                            _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete, whichBuilding);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in creating " + creatingWhat + " for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id = " + leadData.Id + ").Operation : " + doingWhat + ". Error detalis..." + ex.Message);
                        _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                        {
                            LogTime = DateTime.UtcNow,
                            Description = "Error in creating " + creatingWhat + " for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id=" + leadData.Id + "). Operation : " + doingWhat,
                            LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                            ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                        });
                    }
                }
            }
            else
            {
                Console.WriteLine("No record found for creating EZShred customer");
            }
        }
        private static void UpdateCustomer(List<EZShredLeadMappingDataModel> lstEZShredLeadData, mxtrAccount account)
        {
            Console.WriteLine("Updating EZShred customer...");
            CustomerDataModel customerData = new CustomerDataModel();
            if (lstEZShredLeadData != null && lstEZShredLeadData.Count > 0)
            {
                foreach (var leadData in lstEZShredLeadData)
                {
                    string creatingUpdatingWhat = "Customer/Building";
                    string doingWhat = "Create/Update";
                    try
                    {
                        creatingUpdatingWhat = "Customer";
                        doingWhat = "Update";
                        Console.WriteLine(string.Format("Updating EZShred customer {0} having leadMappingId {1} and lead Id {2}", leadData.UserID, leadData.Id, leadData.LeadID));
                        //string dummyCustomer = @"{'Request': null,'UserId': null,'CustomerData': [{'Company': null,'Attention': null,'Street': null,'City': null,'State': null,'Zip': null,'Contact': null,'Phone': null,'Fax': null,'CustomerTypeID': null,'InvoiceTypeID': null,'EmailAddress': null,'EmailInvoice': null,'EmailCOD': null,'ReferralsourceID': null,'TermID': null,'Notes': null,'CustomerId': null}]}";
                        customerData = JsonConvert.DeserializeObject<CustomerDataModel>(leadData.EZShredApiRequest);
                        //hit EZShred api to update user
                        AddUpdateCustomerResult objResult = _apiEZShredService.UpdateEZShredCustomer(customerData);
                        if (objResult.status.ToLower() == "ok")
                        {
                            //set status from failed (customer search page status) to complete (from miner)
                            _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete);
                        }
                        ////check if building exists
                        //if (_dbEZShredBuildingService.GetBuildingIdAgainstCustomerId(leadData.AccountObjectID, Convert.ToString(leadData.CustomerID)) > 0)
                        //{
                        //    creatingUpdatingWhat = "Building";
                        //    doingWhat = "Update";
                        //    //update building
                        //    BuildingDataModel buildingData = new BuildingDataModel();
                        //    buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(leadData.EZShredBuildingApiRequest);
                        //    AddUpdateBuildingResult objBuildingResult = _apiEZShredService.UpdateBuilding(buildingData);
                        //    if (objBuildingResult.status.ToLower() == "ok")
                        //    {
                        //        //set status from failed (customer search page status) to complete (from miner)
                        //        _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete);
                        //    }
                        //    else
                        //    {
                        //        _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Failed);
                        //    }
                        //}
                        //else
                        //{
                        //    creatingUpdatingWhat = "Building";
                        //    doingWhat = "Create";
                        //    //create building
                        //    BuildingDataModel buildingData = new BuildingDataModel();
                        //    //create building after creating customer
                        //    //string dummy = @"{'Request': null,'UserId': null,'BuildingData': [{'BuildingTypeID': null,'CustomerID': null,'CompanyName': null,'Street': null,'City': null,'State': null,'Zip': null,'Phone1': null,'Phone2': null,'SalesmanID': null,'Directions': null,'RoutineInstructions': null,'SiteContact1': null,'SiteContact2': null,'RouteID': null,'Stop': null,'ScheduleFrequency': null,'ServiceTypeID': null,'SalesTaxRegionID': null,'TimeTaken': null}]}";                                        
                        //    buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(leadData.EZShredBuildingApiRequest);

                        //    if (buildingData.BuildingData != null)
                        //    {

                        //        if (string.IsNullOrEmpty(buildingData.BuildingData[0].BuildingID))
                        //        {
                        //            //set customer Id before creating building as customer id is required for building
                        //            buildingData.BuildingData[0].CustomerID = Convert.ToString(leadData.CustomerID);
                        //            AddUpdateBuildingResult objBuildingResult = _apiEZShredService.AddBuilding(buildingData);
                        //            if (objBuildingResult.status.ToLower() == "ok")
                        //            {
                        //                //set status from failed (customer search page status) to complete (from miner)
                        //                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete);
                        //            }
                        //            else
                        //            {
                        //                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Failed);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            creatingUpdatingWhat = "Building";
                        //            doingWhat = "Update";
                        //            AddUpdateBuildingResult objBuildingResult = _apiEZShredService.UpdateBuilding(buildingData);
                        //            if (objBuildingResult.status.ToLower() == "ok")
                        //            {
                        //                //set status from failed (customer search page status) to complete (from miner)
                        //                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete);
                        //            }
                        //            else
                        //            {
                        //                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Failed);
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in " + doingWhat + " " + creatingUpdatingWhat + "  for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id=" + leadData.Id + "). Operation : " + doingWhat + ". Error detalis..." + ex.Message);
                        _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                        {
                            LogTime = DateTime.UtcNow,
                            Description = "Error in " + doingWhat + " " + creatingUpdatingWhat + " for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id=" + leadData.Id + "). Operation :" + doingWhat,
                            LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                            ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                        });
                    }
                }
            }
            else
            {
                Console.WriteLine("No record found for updating EZShred customer");
            }
        }
        private static void CreateBuilding(List<EZShredLeadMappingDataModel> lstEZShredLeadData, mxtrAccount account)
        {
            Console.WriteLine("Creating EZShred building...");
            if (lstEZShredLeadData != null && lstEZShredLeadData.Count > 0)
            {
                foreach (var leadData in lstEZShredLeadData)
                {
                    int customerEZShredId = Convert.ToInt32(leadData.CustomerID);
                    if (customerEZShredId > 0)
                    {
                        Dictionary<LeadBuildingSet, BuildingDataModel> dicBuildingData = ExtractCreateUpdateBuildingData(leadData, EZShredActionTypeKind.Create);
                        foreach (var dataItem in dicBuildingData)
                        {
                            string creatingWhat = "Building";
                            string doingWhat = "Create";
                            try
                            {
                                var building = dataItem.Value;
                                LeadBuildingSet whichBuilding = dataItem.Key;

                                if (building.BuildingData != null)
                                {
                                    if (string.IsNullOrEmpty(building.BuildingData[0].BuildingID))
                                    {
                                        //set customer Id before creating building as customer id is required for building
                                        building.BuildingData[0].CustomerID = Convert.ToString(customerEZShredId);
                                        AddUpdateBuildingResult objBuildingResult = _apiEZShredService.AddBuilding(building);
                                        if (objBuildingResult != null)
                                        {
                                            if (objBuildingResult.status.ToLower() == "ok")
                                            {
                                                //set status from failed (customer search page status) to complete (from miner)
                                                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete, whichBuilding);
                                                //update building Id in the building set once building is created
                                                _dbEZShredLeadMappingService.UpdateBuildingId(leadData.Id, Convert.ToInt32(objBuildingResult.BuildingId), whichBuilding);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //update building                                      
                                        doingWhat = "Update";
                                        AddUpdateBuildingResult objBuildingResult = _apiEZShredService.UpdateBuilding(building);
                                        if (objBuildingResult.status.ToLower() == "ok")
                                        {
                                            //set status from failed (customer search page status) to complete (from miner)
                                            _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete, whichBuilding);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error in " + doingWhat + " " + creatingWhat + " for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id = " + leadData.Id + ").Operation : " + doingWhat + ". Error detalis..." + ex.Message);
                                _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                                {
                                    LogTime = DateTime.UtcNow,
                                    Description = "Error in " + doingWhat + " " + creatingWhat + " for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id=" + leadData.Id + "). Operation : " + doingWhat,
                                    LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                                    ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No record found for creating EZShred building");
            }
        }
        private static void UpdateBuilding(List<EZShredLeadMappingDataModel> lstEZShredLeadData, mxtrAccount account)
        {
            Console.WriteLine("Updating EZShred building...");
            if (lstEZShredLeadData != null && lstEZShredLeadData.Count > 0)
            {
                foreach (var leadData in lstEZShredLeadData)
                {
                    Dictionary<LeadBuildingSet, BuildingDataModel> dicBuildingData = ExtractCreateUpdateBuildingData(leadData, EZShredActionTypeKind.Update);
                    foreach (var dataItem in dicBuildingData)
                    {
                        string creatingUpdatingWhat = "Building";
                        string doingWhat = "Update";
                        try
                        {
                            var building = dataItem.Value;
                            LeadBuildingSet whichBuilding = dataItem.Key;
                            //update building                           
                            AddUpdateBuildingResult objBuildingResult = _apiEZShredService.UpdateBuilding(building);
                            if (objBuildingResult.status.ToLower() == "ok")
                            {
                                //set status from failed (customer search page status) to complete (from miner)
                                _dbEZShredLeadMappingService.UpdateRequestStatus(leadData.Id, EZShredStatusKind.Complete, whichBuilding);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in " + doingWhat + " " + creatingUpdatingWhat + "  for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id=" + leadData.Id + "). Operation : " + doingWhat + ". Error detalis..." + ex.Message);
                            _dbErrorLogService.CreateErrorLog(new ErrorLogModel
                            {
                                LogTime = DateTime.UtcNow,
                                Description = "Error in " + doingWhat + " " + creatingUpdatingWhat + " for account:(" + account.AccountName + ") having account id " + leadData.AccountObjectID + " from leadmapping (id=" + leadData.Id + "). Operation :" + doingWhat,
                                LogType = ErrorKind.Miner + " " + CRMKind.EZShred.ToString(),
                                ErrorMessage = ex != null ? ex.Message + ex.StackTrace + ex.Source : null,
                            });
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No record found for updating EZShred building");
            }

        }

        private static Dictionary<LeadBuildingSet, BuildingDataModel> ExtractCreateUpdateBuildingData(EZShredLeadMappingDataModel leadData, EZShredActionTypeKind createorUpdate)
        {
            BuildingDataModel buildingData = new BuildingDataModel();

            Dictionary<LeadBuildingSet, BuildingDataModel> dicBuildingData = new Dictionary<LeadBuildingSet, BuildingDataModel>();

            //get data of all building i.e Building1,2 and so on
            try
            {
                //building 1
                if (!string.IsNullOrEmpty(leadData.Building1.EZShredBuildingApiRequest) && leadData.Building1.EZShredActionType == createorUpdate.ToString() && leadData.Building1.EZShredStatus == EZShredStatusKind.Failed.ToString())
                {
                    buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(leadData.Building1.EZShredBuildingApiRequest);
                    dicBuildingData.Add(LeadBuildingSet.Building1, buildingData);
                }

                //building 2
                if (!string.IsNullOrEmpty(leadData.Building2.EZShredBuildingApiRequest) && leadData.Building2.EZShredActionType == createorUpdate.ToString() && leadData.Building2.EZShredStatus == EZShredStatusKind.Failed.ToString())
                {
                    buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(leadData.Building2.EZShredBuildingApiRequest);
                    dicBuildingData.Add(LeadBuildingSet.Building2, buildingData);
                }

                //building 3
                if (!string.IsNullOrEmpty(leadData.Building3.EZShredBuildingApiRequest) && leadData.Building3.EZShredActionType == createorUpdate.ToString() && leadData.Building3.EZShredStatus == EZShredStatusKind.Failed.ToString())
                {
                    buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(leadData.Building3.EZShredBuildingApiRequest);
                    dicBuildingData.Add(LeadBuildingSet.Building3, buildingData);
                }

                //building 4
                if (!string.IsNullOrEmpty(leadData.Building4.EZShredBuildingApiRequest) && leadData.Building4.EZShredActionType == createorUpdate.ToString() && leadData.Building4.EZShredStatus == EZShredStatusKind.Failed.ToString())
                {
                    buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(leadData.Building4.EZShredBuildingApiRequest);
                    dicBuildingData.Add(LeadBuildingSet.Building4, buildingData);
                }

                //building 5
                if (!string.IsNullOrEmpty(leadData.Building5.EZShredBuildingApiRequest) && leadData.Building5.EZShredActionType == createorUpdate.ToString() && leadData.Building5.EZShredStatus == EZShredStatusKind.Failed.ToString())
                {
                    buildingData = JsonConvert.DeserializeObject<BuildingDataModel>(leadData.Building5.EZShredBuildingApiRequest);
                    dicBuildingData.Add(LeadBuildingSet.Building5, buildingData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Deserialize building data..." + ex.Message);
            }
            return dicBuildingData;
        }
        private static EZShredDataModel GetEZShredAllData(string accountObjectId, string mxtrAccountId, string lastRun)
        {
            var buildingTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.BuildingTypes, lastRun);
            DelayBetweenRequests();
            var customerTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.CustomerTypes, lastRun);
            DelayBetweenRequests();
            var routes = _apiEZShredService.GetEZShredData(EZShredDataEnum.Routes, lastRun);
            DelayBetweenRequests();
            var salesmen = _apiEZShredService.GetEZShredData(EZShredDataEnum.Salesmen, lastRun);
            DelayBetweenRequests();
            var salesTaxRegions = _apiEZShredService.GetEZShredData(EZShredDataEnum.SalesTaxRegions, lastRun);
            DelayBetweenRequests();
            var serviceItems = _apiEZShredService.GetEZShredData(EZShredDataEnum.ServiceItems, lastRun);
            DelayBetweenRequests();
            var serviceTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.ServiceTypes, lastRun);
            DelayBetweenRequests();
            var customerList = _apiEZShredService.GetEZShredData(EZShredDataEnum.CustomerList, lastRun);
            DelayBetweenRequests();
            var buildingList = _apiEZShredService.GetEZShredData(EZShredDataEnum.BuildingList, lastRun);
            DelayBetweenRequests();
            var frequencyTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.FrequencyTypes, lastRun);
            DelayBetweenRequests();
            var referralSourceTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.ReferralSourceTypes, lastRun);
            DelayBetweenRequests();
            var termsTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.TermsTypes, lastRun);
            DelayBetweenRequests();
            var invoiceTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.InvoiceTypes, lastRun);
            //---var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            EZShredDataModel data = new EZShredDataModel()
            {
                AccountObjectId = accountObjectId,
                MxtrAccountId = mxtrAccountId,
                BuildingTypes = JsonConvert.DeserializeObject<Result>(ResultSerlizer(buildingTypes)).tblBuildingType,
                CustomerTypes = JsonConvert.DeserializeObject<Result>(ResultSerlizer(customerTypes)).tblCustomerType,
                Routes = JsonConvert.DeserializeObject<Result>(ResultSerlizer(routes)).tblRoutes,
                Salesman = JsonConvert.DeserializeObject<Result>(ResultSerlizer(salesmen)).tblSalesman,
                SalesTaxRegions = JsonConvert.DeserializeObject<Result>(ResultSerlizer(salesTaxRegions)).tblSalesTaxRegion,
                ServiceItems = JsonConvert.DeserializeObject<Result>(ResultSerlizer(serviceItems)).tblServiceItems,
                ServiceTypes = JsonConvert.DeserializeObject<Result>(ResultSerlizer(serviceTypes)).tblServiceTypes,
                Customer = JsonConvert.DeserializeObject<Result>(ResultSerlizer(customerList)).tblCustomers,
                Building = JsonConvert.DeserializeObject<Result>(ResultSerlizer(buildingList)).tblBuilding,
                Frequencys = JsonConvert.DeserializeObject<Result>(ResultSerlizer(frequencyTypes)).Frequencies,
                ReferralSources = JsonConvert.DeserializeObject<Result>(ResultSerlizer(referralSourceTypes)).tblReferralSource,
                TermTypes = JsonConvert.DeserializeObject<Result>(ResultSerlizer(termsTypes)).tblTerms,
                InvoiceTypes = JsonConvert.DeserializeObject<Result>(ResultSerlizer(invoiceTypes)).tblInvoiceType
            };
            return data;
        }

        private static EZShredDataModel GetEZShredCustomerLists(string accountObjectId, string mxtrAccountId, string lastRun)
        {
            DateTime hitStartTime = DateTime.UtcNow;
            var customerLists = _apiEZShredService.GetEZShredData(EZShredDataEnum.CustomerList, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, customerLists, EZShredDataEnum.CustomerList.ToString(), !String.IsNullOrEmpty(customerLists) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var customerTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.CustomerTypes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, customerTypes, EZShredDataEnum.CustomerTypes.ToString(), !String.IsNullOrEmpty(customerTypes) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var invoiceTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.InvoiceTypes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, invoiceTypes, EZShredDataEnum.InvoiceTypes.ToString(), !String.IsNullOrEmpty(invoiceTypes) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var referralSourceTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.ReferralSourceTypes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, referralSourceTypes, EZShredDataEnum.ReferralSourceTypes.ToString(), !String.IsNullOrEmpty(referralSourceTypes) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var termsTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.TermsTypes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, termsTypes, EZShredDataEnum.TermsTypes.ToString(), !String.IsNullOrEmpty(termsTypes) ? true : false);

            EZShredDataModel data = new EZShredDataModel()
            {
                AccountObjectId = accountObjectId,
                MxtrAccountId = mxtrAccountId,
                //Customer = null, //Use Below
                Customer = DeserializeResultData(ResultSerlizer(customerLists)) != null ? DeserializeResultData(ResultSerlizer(customerLists)).tblCustomers : null,
                CustomerTypes = DeserializeResultData(ResultSerlizer(customerTypes)) != null ? DeserializeResultData(ResultSerlizer(customerTypes)).tblCustomerType : null,
                ReferralSources = DeserializeResultData(ResultSerlizer(referralSourceTypes)) != null ? DeserializeResultData(ResultSerlizer(referralSourceTypes)).tblReferralSource : null,
                TermTypes = DeserializeResultData(ResultSerlizer(termsTypes)) != null ? DeserializeResultData(ResultSerlizer(termsTypes)).tblTerms : null,
                InvoiceTypes = DeserializeResultData(ResultSerlizer(invoiceTypes)) != null ? DeserializeResultData(ResultSerlizer(invoiceTypes)).tblInvoiceType : null
            };
            return data;
        }

        private static EZShredDataModel GetEZShredBuildingLists(string accountObjectId, string mxtrAccountId, string lastRun)
        {
            DateTime hitStartTime = DateTime.UtcNow;
            var buildingLists = _apiEZShredService.GetEZShredData(EZShredDataEnum.BuildingList, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, buildingLists, EZShredDataEnum.BuildingList.ToString(), !String.IsNullOrEmpty(buildingLists) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var buildingTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.BuildingTypes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, buildingTypes, EZShredDataEnum.BuildingTypes.ToString(), !String.IsNullOrEmpty(buildingTypes) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var routes = _apiEZShredService.GetEZShredData(EZShredDataEnum.Routes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, routes, EZShredDataEnum.Routes.ToString(), !String.IsNullOrEmpty(routes) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var salesmen = _apiEZShredService.GetEZShredData(EZShredDataEnum.Salesmen, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, salesmen, EZShredDataEnum.Salesmen.ToString(), !String.IsNullOrEmpty(salesmen) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var salesTaxRegions = _apiEZShredService.GetEZShredData(EZShredDataEnum.SalesTaxRegions, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, salesTaxRegions, EZShredDataEnum.SalesTaxRegions.ToString(), !String.IsNullOrEmpty(salesTaxRegions) ? true : false);
            DelayBetweenRequests();
            hitStartTime = DateTime.UtcNow;
            var serviceTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.ServiceTypes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, serviceTypes, EZShredDataEnum.ServiceTypes.ToString(), !String.IsNullOrEmpty(serviceTypes) ? true : false);

            //---var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            EZShredDataModel data = new EZShredDataModel()
            {
                AccountObjectId = accountObjectId,
                MxtrAccountId = mxtrAccountId,
                //Building = null, //Use Below
                Building = DeserializeResultData(ResultSerlizer(buildingLists)) != null ? DeserializeResultData(ResultSerlizer(buildingLists)).tblBuilding : null,
                BuildingTypes = DeserializeResultData(ResultSerlizer(buildingTypes)) != null ? DeserializeResultData(ResultSerlizer(buildingTypes)).tblBuildingType : null,
                Routes = DeserializeResultData(ResultSerlizer(routes)) != null ? DeserializeResultData(ResultSerlizer(routes)).tblRoutes : null,
                Salesman = DeserializeResultData(ResultSerlizer(salesmen)) != null ? DeserializeResultData(ResultSerlizer(salesmen)).tblSalesman : null,
                SalesTaxRegions = DeserializeResultData(ResultSerlizer(salesTaxRegions)) != null ? DeserializeResultData(ResultSerlizer(salesTaxRegions)).tblSalesTaxRegion : null,
                ServiceTypes = DeserializeResultData(ResultSerlizer(serviceTypes)) != null ? DeserializeResultData(ResultSerlizer(serviceTypes)).tblServiceTypes : null,
            };
            return data;
        }

        private static EZShredDataModel GetEZShredServiceLists(string accountObjectId, string mxtrAccountId, string lastRun)
        {
            DateTime hitStartTime = DateTime.UtcNow;
            var serviceItems = _apiEZShredService.GetEZShredData(EZShredDataEnum.ServiceItems, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, serviceItems, EZShredDataEnum.ServiceItems.ToString(), !String.IsNullOrEmpty(serviceItems) ? true : false);

            //---var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            EZShredDataModel data = new EZShredDataModel()
            {
                AccountObjectId = accountObjectId,
                MxtrAccountId = mxtrAccountId,
                ServiceItems = DeserializeResultData(ResultSerlizer(serviceItems)) != null ? DeserializeResultData(ResultSerlizer(serviceItems)).tblServiceItems : null,
            };
            return data;
        }

        private static EZShredDataModel GetEZShredMiscLists(string accountObjectId, string mxtrAccountId, string lastRun)
        {
            DateTime hitStartTime = DateTime.UtcNow;
            var frequencyTypes = _apiEZShredService.GetEZShredData(EZShredDataEnum.FrequencyTypes, lastRun);
            AddUpdateEZMinerLogs(accountObjectId, hitStartTime, frequencyTypes, EZShredDataEnum.FrequencyTypes.ToString(), !String.IsNullOrEmpty(frequencyTypes) ? true : false);

            //---var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            EZShredDataModel data = new EZShredDataModel()
            {
                AccountObjectId = accountObjectId,
                MxtrAccountId = mxtrAccountId,
                Frequencys = DeserializeResultData(ResultSerlizer(frequencyTypes)) != null ? DeserializeResultData(ResultSerlizer(frequencyTypes)).Frequencies : null,
            };
            return data;
        }

        private static void AddUpdateEZMinerLogs(string accountObjectId, DateTime hitStartTime, string returnResponse, string apiname, bool returnResult)
        {
            EZShredMinerLogDataModel objEZShredMinerLogDataModel = new EZShredMinerLogDataModel
            {
                AccountObjectId = accountObjectId,
                APIDetails = new List<APIDetailDataModel>()
                        {
                            new APIDetailDataModel()
                            {
                                APIName=apiname,
                                HitTime=DateTime.UtcNow,
                                ReturnResponse=returnResponse,
                                ResponseTime=(DateTime.UtcNow-hitStartTime).TotalSeconds,
                                ReturnResult=returnResult,
                            }
                        },
            };
            _dbEZShredMinerLogService.UpdateEZMinerlog(objEZShredMinerLogDataModel);
        }
        private static void DelayBetweenRequests()
        {
            System.Threading.Thread.Sleep(500);
        }
        public static string ResultSerlizer(string ResultData)
        {
            if (!string.IsNullOrEmpty(ResultData))
            {
                RootObject objRoot = new RootObject();
                objRoot = JsonConvert.DeserializeObject<RootObject>(ResultData);
                return objRoot.Result;
            }
            else
                return null;
        }
        public static Result DeserializeResultData(string ResultData)
        {
            if (!string.IsNullOrEmpty(ResultData))
            {
                Result objRoot = new Result();
                objRoot = JsonConvert.DeserializeObject<Result>(ResultData);
                return objRoot;
            }
            else
                return null;
        }
        static void AppStart()
        {
            CreateKernel();
        }

        protected static IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();

            Assembly.Load("mxtrAutomation.Common");
            Assembly.Load("mxtrAutomation.Data");
            Assembly.Load("mxtrAutomation.Corporate.Data");
            Assembly.Load("mxtrAutomation.Api");

            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());

            NinjectServiceLocator locator = new NinjectServiceLocator(kernel);
            ServiceLocator.SetLocatorProvider(() => locator);

            return kernel;
        }

        protected static void PrepareServices()
        {
            _dbAccountService = ServiceLocator.Current.GetInstance<IAccountService>();
            _apiEZShredService = ServiceLocator.Current.GetInstance<IEZShredService>();
            _dbCRMEZShredService = ServiceLocator.Current.GetInstance<ICRMEZShredService>();
            _dbErrorLogService = ServiceLocator.Current.GetInstance<IErrorLogService>();
            _dbEZShredLeadMappingService = ServiceLocator.Current.GetInstance<IEZShredLeadMappingService>();
            _dbMinerRunService = ServiceLocator.Current.GetInstance<IMinerRunService>();
            _dbEZShredMinerLogService = ServiceLocator.Current.GetInstance<IEZShredMinerLogService>();
            _dbEZShredCustomerService = ServiceLocator.Current.GetInstance<ICRMEZShredCustomerService>();
            _dbEZShredBuildingService = ServiceLocator.Current.GetInstance<ICRMEZShredBuildingService>();
        }

        protected static CreateNotificationReturn UpdateMinerRunAuditTrail(mxtrAccount account, bool isFirstTimeRun, MinerRunMinerDetails minerRunMinerDetails)
        {
            if (isFirstTimeRun)
            {
                List<MinerRunMinerDetails> listDetails = new List<MinerRunMinerDetails>();
                listDetails.Add(minerRunMinerDetails);

                MinerRunAuditTrailDataModel minerRunAuditTrailDataModel = new MinerRunAuditTrailDataModel
                {
                    AccountObjectID = account.ObjectID,
                    MxtrAccountID = account.MxtrAccountID,
                    MinerRunDetails = listDetails
                };
                return _dbMinerRunService.CreateMinerRunAuditTrail(minerRunAuditTrailDataModel);
            }
            else
            {
                return _dbMinerRunService.UpdateMinerRunDetails(account.ObjectID, minerRunMinerDetails);
            }
        }
    }
}
