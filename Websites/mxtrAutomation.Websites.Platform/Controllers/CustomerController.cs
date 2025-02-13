using mxtrAutomation.Websites.Platform.Models.Customer.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Websites.Platform.Helpers;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Common.Enums;
using mxtrAutomation.Api.EZShred;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class CustomerController : MainLayoutControllerBase
    {
        private readonly ICustomerViewModelAdapter _viewModelAdapter;
        private readonly ICRMEZShredService _crmEZShredService;
        private readonly IEZShredFieldLabelMappingService _fieldLabelMapping;
        private readonly IAccountService _accountService;
        private readonly ISharpspringService _apiSharpspringService;
        private readonly IEZShredLeadMappingService _dbEZShredLeadMappingService;
        private readonly IEZShredService _apiEZShredService;
        private readonly ICRMEZShredCustomerService _dbEZShredCustomerService;
        private readonly ICRMEZShredBuildingService _dbEZShredBuildingService;
        public CustomerController(ICustomerViewModelAdapter viewModelAdapter, ICRMEZShredService crmEZShredService, IEZShredFieldLabelMappingService fieldLabelMapping, IAccountService accountService, ISharpspringService apiSharpspringService, IEZShredLeadMappingService dbEZShredLeadMappingService, IEZShredService apiEZShredService, ICRMEZShredCustomerService dbEZShredCustomerService, ICRMEZShredBuildingService dbEZShredBuildingService)
        {
            _viewModelAdapter = viewModelAdapter;
            _crmEZShredService = crmEZShredService;
            _fieldLabelMapping = fieldLabelMapping;
            _accountService = accountService;
            _apiSharpspringService = apiSharpspringService;
            _dbEZShredLeadMappingService = dbEZShredLeadMappingService;
            _apiEZShredService = apiEZShredService;
            _dbEZShredCustomerService = dbEZShredCustomerService;
            _dbEZShredBuildingService = dbEZShredBuildingService;
        }
        public ActionResult ViewPage(CustomerWebQuery query)
        {
            CustomerViewModel model = _viewModelAdapter.BuildIndexViewModel();
            model.CustomerViewData = new Models.Customer.ViewData.CustomerViewData();

            mxtrUser mxtrUser = UserService.GetUserByUserObjectID(User.MxtrUserObjectID);
            mxtrAccount mxtrAccount = AccountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            List<EZShredAccountDataModel> EZShredMarket = new List<EZShredAccountDataModel>();
            model.CustomerActionKind = Enums.CustomerActionKind.Add;
            if (mxtrUser.EZShredAccountMappings != null)
            {
                EZShredMarket = AccountService.GetEzshredByAccountObjectIds(mxtrUser.EZShredAccountMappings.Select(a => a.AccountObjectId).ToList(), mxtrUser);
                model.EZShredAccountDataModel = EZShredMarket;
            }
            if (model.CustomerActionKind == Enums.CustomerActionKind.Edit)
            {
                model.Success = true;
                model.CustomerViewData = new Models.Customer.ViewData.CustomerViewData()
                {
                };
            }
            return View(ViewKind.Customer, model, query);
        }

        [HttpPost]
        [Route("GetCustomers")]
        public ActionResult GetCustomers(string accountObjectID)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            List<EZShredCustomerDataModelMini> lstCustomers = new List<EZShredCustomerDataModelMini>();
            List<string> lstAccountObjectID = new List<string>();
            lstAccountObjectID.Add(accountObjectID);
            var EZShredDataModel = _crmEZShredService.GetEZShredDataByAccountObjectIDs(lstAccountObjectID);
            if (EZShredDataModel.Count() > 0)
            {
                //--- customers = _crmEZShredService.GetAllCustomers(accountObjectID);
                // get customers from EZshred Customer table 
                lstCustomers = _dbEZShredCustomerService.GetAllCustomerMiniByAccountObjectId(accountObjectID).ToList();
                lstCustomers.ForEach(x => x.DataSource = "EZShred");
                // get customers from lead mapping table 
                List<EZShredLeadMappingDataModel> lstSSCustomer = _dbEZShredLeadMappingService.GetCustomerDataFromEZShredLeadMapping(accountObjectID);
                // merge Customer from EZShred Customer and lead Mapping
                MergeCustomers(lstCustomers, lstSSCustomer);
            }

            var customerData = new { Data = lstCustomers.Take(lstCustomers.Count()).Reverse() };
            var result = new ContentResult
            {
                Content = serializer.Serialize(customerData),
                ContentType = "application/json"
            };
            return result;
        }

        [Route("GetCustomerAndBuildingInformations")]
        public ActionResult GetCustomerAndBuildingInformations(string accountObjectID, string CustomerID, string leadId, string opportunityId, string buildingId)
        {
            int buildingCount = 0;
            if (!String.IsNullOrEmpty(opportunityId) && Convert.ToInt64(opportunityId) > 0)
            {
                buildingCount = _dbEZShredLeadMappingService.GetBuildingCountByOpportunity(accountObjectID, Convert.ToInt64(opportunityId));
            }
            else
            {
                buildingCount = _dbEZShredBuildingService.GetBuildingCountAgaistCustomerId(accountObjectID, Convert.ToInt32(CustomerID));
            }

            string BuildingSet = LeadBuildingSet.Building1.ToString();
            EZShredDataModel EZShredDataModel = new EZShredDataModel();
            SharpspringOpportunityDataModel SharpspringOpportunityDataModel = new SharpspringOpportunityDataModel();
            List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel = GetAllEZShredSSCustomFields(accountObjectID);
            //check if customer have only SS reference then data from Sharpspring
            //Priority Sharpspring If customer available in both place (SS & EZshred)
            //if (!string.IsNullOrEmpty(leadId) && string.IsNullOrEmpty(CustomerID))
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(accountObjectID);
            if (leadId != "0")
            {
                EZShredLeadMappingDataModel LeadData = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, Convert.ToInt64(leadId));
                BuildingSet = GetNextBuildingStage(_dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, Convert.ToInt64(leadId)));

                SharpspringLeadDataModel customerDataFromSS = _apiSharpspringService.GetLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, Convert.ToInt64(leadId), GetBuildingSetByOpportunityID(accountObjectID, Convert.ToInt64(leadId), Convert.ToInt64(opportunityId)));
                EZShredDataModel = AdaptSharpSpringData(customerDataFromSS, mxtrAccount.MxtrAccountID, accountObjectID);
                //get All Oppurtinty custom & Native Fields Data
                SharpspringOpportunityDataModel = _apiSharpspringService.GetOpportunitiesWithCustomFields(ObjSharpspringCustomFieldsDataModel, Convert.ToInt64(opportunityId)).FirstOrDefault();

                if (EZShredDataModel.Building != null)
                {
                    if (String.IsNullOrEmpty(EZShredDataModel.Building[0].BuildingID) && buildingId != "0")
                    {
                        EZShredDataModel.Building[0].BuildingID = buildingId;
                    }
                }
                if (!string.IsNullOrEmpty(CustomerID) && CustomerID != "0")
                {
                    EZShredDataModel EZShredData = _crmEZShredService.GetCustomerAndBuildingInformations(accountObjectID, CustomerID, buildingId);
                    EZShredDataModel.Customer[0].UserID = EZShredData.Customer[0].UserID;
                    if (EZShredData.Building != null && EZShredData.Building.Count() > 0)
                    {
                        EZShredDataModel.Building[0].LastServiceDate = EZShredData.Building[0].LastServiceDate;
                        //EZShredDataModel.Building[0].NextServiceDate =EZShredData.Building[0].NextServiceDate;
                        EZShredDataModel.Building[0].SalesTaxRegionID = EZShredData.Building[0].SalesTaxRegionID;
                    }

                }
            }
            else
            {
                EZShredDataModel = _crmEZShredService.GetCustomerAndBuildingInformations(accountObjectID, CustomerID, buildingId);
                SharpspringOpportunityDataModel = null;
            }

            if (!string.IsNullOrEmpty(buildingId) && buildingId != "0")
            {
                _apiEZShredService.SetConnectionTokens(mxtrAccount.EZShredIP, mxtrAccount.EZShredPort);
                EZShredDataModel.Building[0].NextServiceDate = GetNextServiceDate(EZShredDataModel.Customer[0].UserID, EZShredDataModel.Building[0].BuildingID);
            }

            //Highlight the pipeline button 
            string currentDealStage = GetDealStageNameByOpportunityId(Convert.ToInt64(opportunityId));

            if (SharpspringOpportunityDataModel != null && SharpspringOpportunityDataModel.IsClosed)
            {
                SharpspringOpportunityDataModel = null;
                currentDealStage = MapDealStageName(string.Empty, mxtrAccount);
                EZShredDataModel.Customer[0].PipelineStatus = currentDealStage;
            }
            else
                currentDealStage = MapDealStageName(currentDealStage, mxtrAccount);

            var result = new { Data = EZShredDataModel, DealStageName = currentDealStage, Opportunity = SharpspringOpportunityDataModel, BuildingSet = BuildingSet, BuildingCount = buildingCount };
            return Json(result);

        }

        [Route("GetAllTypes")]
        public ActionResult GetAllTypes(string accountObjectID)
        {
            if (!string.IsNullOrEmpty(accountObjectID))
            {
                EZShredDataModel EZShredDataModel = _crmEZShredService.GetAllTypes(accountObjectID);
                mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(accountObjectID);
                var status = CheckSSCustomFieldAvailability(accountObjectID, mxtrAccount);
                var result = new { Data = EZShredDataModel, SSCustomFieldAvailability = status };
                return Json(result);
            }
            else
                return Json(new EZShredDataModel());
        }

        [Route("UpdateCustomer")]
        public ActionResult UpdateCustomer(string accountObjectID, EZShredDataModel objEZShredDataModel, bool IsAddNewBuilding)
        {
            Boolean EzshredStatus = false;
            Boolean SharpSpringStatus = false;
            Boolean IsCustomerIDAssigned = false;
            Boolean WasAttemptedToWriteDataOnEZShred = false;
            string BuildingSet = string.Empty;
            try
            {
                //Get All Custom Fields of Lead & Oppourtunity
                List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel = GetAllEZShredSSCustomFields(accountObjectID);
                mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
                _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
                objEZShredDataModel.AccountObjectId = accountObjectID;
                objEZShredDataModel.MxtrAccountId = account.MxtrAccountID;
                SharpspringLeadDataModel objSharpSpringDataModel = EZShredSSFieldsMapping(objEZShredDataModel);
                EZShredLeadMappingDataModel objEZShredLeadMappingDataModel = new EZShredLeadMappingDataModel()
                {
                    LeadID = objEZShredDataModel.Customer[0].LeadID,
                    CustomerID = Convert.ToInt32(objEZShredDataModel.Customer[0].CustomerID),
                    OpportunityID = objEZShredDataModel.Customer[0].OpportunityID,
                    Company = objEZShredDataModel.Customer[0].Company,
                    Street = objEZShredDataModel.Customer[0].Street,
                    ZIP = objEZShredDataModel.Customer[0].Zip,
                    EZShredApiRequest = CustomerAPIRequest(objEZShredDataModel.Customer[0], UpdateCustomerAPIRequestString(objEZShredDataModel.Customer[0])),
                    EZShredBuildingApiRequest = BuildingAPIRequest(objEZShredDataModel.Building[0], UpdateBuildingAPIRequestString(objEZShredDataModel.Building[0], objEZShredDataModel.Customer[0].UserID), objEZShredDataModel.Customer[0].UserID, objEZShredDataModel.Customer[0].CustomerID),
                };
                if (IsAddNewBuilding || objEZShredDataModel.Building[0].OpportunityID == 0)
                {
                    if (objEZShredDataModel.Customer[0].LeadID <= 0)
                    {
                        // Create Lead for old data data and save
                        BuildingSet = LeadBuildingSet.Building1.ToString();
                        string leadResult = CreateLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, accountObjectID, objSharpSpringDataModel, account, BuildingSet);
                        SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(leadResult);
                        if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                        {
                            SharpSpringStatus = true;
                            objEZShredDataModel.Customer[0].LeadID = ssResponse.result.creates[0].id;
                            objEZShredLeadMappingDataModel.LeadID = ssResponse.result.creates[0].id;
                            //objEZShredDataModel.Customer[0].OpportunityID = OpportunityDealStagesOperations(accountObjectID, objEZShredDataModel.Customer[0].EmailAddress, ssResponse.result.creates[0].id, objEZShredDataModel, ObjSharpspringCustomFieldsDataModel, account);
                            EZShredLeadMapping(accountObjectID, objEZShredLeadMappingDataModel, account);
                        }
                    }

                    if (objEZShredDataModel.Customer[0].LeadID > 0)
                    {
                        BuildingSet = GetNextBuildingStage(_dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, Convert.ToInt64(objEZShredDataModel.Customer[0].LeadID)));
                        if (BuildingSet != LeadBuildingSet.Complete.ToString())
                        {
                            //Add New Building in EZShredShredLeadMapping Collection
                            EZShredLeadMappingDataModel LeadData = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, objEZShredDataModel.Customer[0].LeadID);
                            objEZShredLeadMappingDataModel.EZShredBuildingApiRequest = "";
                            objEZShredLeadMappingDataModel.Id = LeadData.Id;
                            objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.NoAction.ToString();
                            objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Complete.ToString();//Complete
                            objEZShredLeadMappingDataModel.OpportunityID = OpportunityDealStagesOperations(accountObjectID, objEZShredDataModel.Customer[0].EmailAddress, objEZShredDataModel.Customer[0].LeadID, objEZShredDataModel, ObjSharpspringCustomFieldsDataModel, account);//Create Opportunity
                            objEZShredDataModel.Customer[0].OpportunityID = objEZShredLeadMappingDataModel.OpportunityID;//Assign New OpportunityID to EZShredDataModel
                            objEZShredDataModel.Building[0].OpportunityID = objEZShredLeadMappingDataModel.OpportunityID;

                            // Add new building in EZShred
                            if (IsAddNewBuilding && objEZShredDataModel.Customer[0].PipelineStatus == PipelineKind.Scheduled.ToString())
                            {
                                if (!String.IsNullOrEmpty(objEZShredDataModel.Customer[0].CustomerID))
                                {
                                    EZShredLeadMappingDataModel leadData = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, objEZShredDataModel.Customer[0].LeadID);
                                    objEZShredDataModel = AddNewBuildingToEZShred(accountObjectID, objEZShredDataModel.Customer[0].CustomerID, leadData, objEZShredDataModel, account, BuildingSet);
                                }
                                else
                                {
                                    // Handled in below part of code
                                }
                            }
                            else
                            {
                                AddLeadBuilding(objEZShredLeadMappingDataModel, AdaptLeadBuilding(objEZShredDataModel, objEZShredLeadMappingDataModel), BuildingSet);
                            }
                            // Update OpportunityID
                            _dbEZShredBuildingService.UpdateOpportunityIDByBuildingId(accountObjectID, objEZShredDataModel.Building[0].BuildingID, objEZShredDataModel.Building[0].OpportunityID);
                        }
                    }
                }
                else
                {
                    //BuildingSet = LeadBuildingSet.Building1.ToString();
                    BuildingSet = GetBuildingSetByOpportunityID(accountObjectID, Convert.ToInt64(objEZShredDataModel.Customer[0].LeadID), Convert.ToInt64(objEZShredDataModel.Customer[0].OpportunityID));
                    objEZShredDataModel.Building[0].OpportunityID = objEZShredDataModel.Customer[0].OpportunityID;
                }

                // Assign buildings data
                SetBuildingData(objEZShredDataModel, BuildingSet, objEZShredLeadMappingDataModel);

                if (objEZShredDataModel != null && objEZShredDataModel.Customer[0] != null && objEZShredDataModel.Building[0] != null)
                {
                    EZShredLeadMappingStatus objEZShredLeadMappingStatus = new EZShredLeadMappingStatus();
                    var previousDealStageInSS = objEZShredDataModel.Customer[0].OpportunityID <= 0 ? String.Empty : GetDealStageNameByOpportunityId(objEZShredDataModel.Customer[0].OpportunityID);
                    previousDealStageInSS = MapDealStageName(previousDealStageInSS, account);
                    //If user select Pipeline Status Close and If previous status is other than close then it will update in SS and create entry in EZShredleadMapping.
                    if (objEZShredDataModel.Customer[0].PipelineStatus == PipelineKind.Scheduled.ToString() && previousDealStageInSS != PipelineKind.Scheduled.ToString())
                    {
                        objEZShredLeadMappingStatus = AddUpdateEZData(accountObjectID, objEZShredDataModel, objEZShredLeadMappingDataModel, account, objEZShredDataModel.Customer[0].PipelineStatus, BuildingSet);
                        EzshredStatus = objEZShredLeadMappingStatus.Status;
                        WasAttemptedToWriteDataOnEZShred = true;
                    }
                    //If current status is other than close and previous status is closed. 
                    else if (objEZShredDataModel.Customer[0].PipelineStatus != PipelineKind.Scheduled.ToString() && previousDealStageInSS == PipelineKind.Scheduled.ToString())
                    {
                        objEZShredLeadMappingStatus = AddUpdateEZData(accountObjectID, objEZShredDataModel, objEZShredLeadMappingDataModel, account, objEZShredDataModel.Customer[0].PipelineStatus, BuildingSet);
                        EzshredStatus = objEZShredLeadMappingStatus.Status;
                        WasAttemptedToWriteDataOnEZShred = true;
                    }
                    //If current status is Close and previous status is also Close then we have to update on both SS and EZShredleadmapping
                    else if (objEZShredDataModel.Customer[0].PipelineStatus == PipelineKind.Scheduled.ToString() && previousDealStageInSS == PipelineKind.Scheduled.ToString())
                    {
                        objEZShredLeadMappingStatus = AddUpdateEZData(accountObjectID, objEZShredDataModel, objEZShredLeadMappingDataModel, account, objEZShredDataModel.Customer[0].PipelineStatus, BuildingSet);
                        EzshredStatus = objEZShredLeadMappingStatus.Status;
                        WasAttemptedToWriteDataOnEZShred = true;
                    }
                    //If previous pipeline status is Close or whatever and now user has selected any status other than Close then we will update SS only
                    else if (objEZShredDataModel.Customer[0].PipelineStatus != PipelineKind.Scheduled.ToString())
                    {
                        if (!String.IsNullOrEmpty(objEZShredDataModel.Customer[0].CustomerID))
                        {
                            // Update EZ data
                            EzshredStatus = UpdateCustomerAndBuildingToEZShred(accountObjectID, objEZShredDataModel, account, objEZShredDataModel.Customer[0].PipelineStatus);
                            WasAttemptedToWriteDataOnEZShred = true;
                            if (EzshredStatus)
                            {
                                objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.NoAction.ToString();
                                objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Complete.ToString();
                            }
                            else
                            {
                                objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.Update.ToString();
                                objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Failed.ToString();
                            }
                            //update EZLeadMapping
                            _dbEZShredLeadMappingService.UpdateEZShredLeadMappingData(objEZShredLeadMappingDataModel);
                            objEZShredLeadMappingStatus = new EZShredLeadMappingStatus()
                            {
                                EZShredActionType = objEZShredLeadMappingDataModel.EZShredActionType,
                                EZShredStatus = objEZShredLeadMappingDataModel.EZShredStatus,
                            };
                        }
                    }
                    // If the current status is closed then update in EZ and SS
                    else if (objEZShredDataModel.Customer[0].PipelineStatus == PipelineKind.Scheduled.ToString())
                    {
                        objEZShredLeadMappingStatus = AddUpdateEZData(accountObjectID, objEZShredDataModel, objEZShredLeadMappingDataModel, account, objEZShredDataModel.Customer[0].PipelineStatus, BuildingSet);
                        EzshredStatus = objEZShredLeadMappingStatus.Status;
                        WasAttemptedToWriteDataOnEZShred = true;
                    }

                    // Update SS
                    Tuple<bool, long, long> data = CheckSSLeadOpporunity(accountObjectID, objEZShredDataModel, SharpSpringStatus, objSharpSpringDataModel, ObjSharpspringCustomFieldsDataModel, account, BuildingSet, IsAddNewBuilding);
                    SharpSpringStatus = data.Item1;
                    if (objEZShredDataModel.Customer[0].LeadID > 0)
                    {
                        EZShredLeadMappingDataModel LeadMappingData = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, data.Item2);
                        if (LeadMappingData.Id == null)
                        {
                            objEZShredLeadMappingDataModel.LeadID = data.Item2;
                            objEZShredLeadMappingDataModel.OpportunityID = data.Item3;
                            objEZShredLeadMappingDataModel.EZShredActionType = objEZShredLeadMappingStatus.EZShredActionType;
                            objEZShredLeadMappingDataModel.EZShredStatus = objEZShredLeadMappingStatus.EZShredStatus;
                            EZShredLeadMapping(accountObjectID, objEZShredLeadMappingDataModel, account);
                        }
                        else
                        {
                            objEZShredLeadMappingDataModel.OpportunityID = data.Item3;//After Lost New Oppourtunity information will update.
                            if (Convert.ToInt32(objEZShredDataModel.Customer[0].CustomerID) > 0)
                            {
                                objEZShredLeadMappingDataModel.EZShredActionType = objEZShredLeadMappingStatus.EZShredActionType;
                                objEZShredLeadMappingDataModel.EZShredStatus = objEZShredLeadMappingStatus.EZShredStatus;
                            }
                            else
                            {
                                objEZShredLeadMappingDataModel.EZShredStatus = LeadMappingData.EZShredStatus;
                                objEZShredLeadMappingDataModel.EZShredActionType = LeadMappingData.EZShredActionType;
                            }
                            SetBuildingData(objEZShredDataModel, BuildingSet, objEZShredLeadMappingDataModel);
                            bool Leadresponse = UpdateEZShredLeadMappingDataWithLeadID(objEZShredLeadMappingDataModel, BuildingSet);
                        }
                    }
                    //Get EZShred Customer info
                    if (!string.IsNullOrEmpty(objEZShredDataModel.Customer[0].CustomerID))
                    {
                        CustomerInfoResult objCustomerInfoResult = _apiEZShredService.GetEZShredCustomerInfo(objEZShredDataModel.Customer[0].UserID, objEZShredDataModel.Customer[0].CustomerID);
                        if (objCustomerInfoResult != null && objCustomerInfoResult.tblCustomers != null && objCustomerInfoResult.tblCustomers.Count > 0)
                        {
                            UpdateLeadARCustomerCode(ObjSharpspringCustomFieldsDataModel, accountObjectID, objEZShredLeadMappingDataModel.LeadID, objCustomerInfoResult.tblCustomers[0].ARCustomerCode, account);
                            UpdateOpportunityARCustomerCode(ObjSharpspringCustomFieldsDataModel, accountObjectID, objEZShredLeadMappingDataModel.OpportunityID, objCustomerInfoResult.tblCustomers[0].ARCustomerCode, account);
                        }
                        if (Convert.ToInt32(objEZShredDataModel.Customer[0].CustomerID) > 0)
                            IsCustomerIDAssigned = true;
                    }

                }
                return Json(new { EZSharedStatus = EzshredStatus, SharpSpringStatus = SharpSpringStatus, IsCustomerIDAssigned = IsCustomerIDAssigned, WasAttemptedToWriteDataOnEZShred= WasAttemptedToWriteDataOnEZShred, Message = String.Empty });
            }
            catch (Exception ex)
            {
                return Json(new { EZSharedStatus = EzshredStatus, SharpSpringStatus = SharpSpringStatus, IsCustomerIDAssigned = IsCustomerIDAssigned, WasAttemptedToWriteDataOnEZShred = WasAttemptedToWriteDataOnEZShred, Message = ex.Message });
            }
        }

        private static void SetBuildingData(EZShredDataModel objEZShredDataModel, string BuildingSet, EZShredLeadMappingDataModel objEZShredLeadMappingDataModel)
        {
            if (BuildingSet == LeadBuildingSet.Building1.ToString())
            {
                objEZShredLeadMappingDataModel.Building1 = AssignBuildingData(objEZShredDataModel);
            }
            else if (BuildingSet == LeadBuildingSet.Building2.ToString())
            {
                objEZShredLeadMappingDataModel.Building2 = AssignBuildingData(objEZShredDataModel);
            }
            else if (BuildingSet == LeadBuildingSet.Building3.ToString())
            {
                objEZShredLeadMappingDataModel.Building3 = AssignBuildingData(objEZShredDataModel);
            }
            else if (BuildingSet == LeadBuildingSet.Building4.ToString())
            {
                objEZShredLeadMappingDataModel.Building4 = AssignBuildingData(objEZShredDataModel);
            }
            else if (BuildingSet == LeadBuildingSet.Building5.ToString())
            {
                objEZShredLeadMappingDataModel.Building5 = AssignBuildingData(objEZShredDataModel);
            }
        }

        private static LeadBuildingDataModel AssignBuildingData(EZShredDataModel objEZShredDataModel)
        {
            return new LeadBuildingDataModel
            {
                BuildingCompanyName = objEZShredDataModel.Building[0] == null ? String.Empty : objEZShredDataModel.Building[0].CompanyName,
                BuildingID = objEZShredDataModel.Building[0] == null ? 0 : Convert.ToInt32(objEZShredDataModel.Building[0].BuildingID),
                OpportunityID = objEZShredDataModel.Building[0] == null ? 0 : objEZShredDataModel.Building[0].OpportunityID,
                City = objEZShredDataModel.Building[0] == null ? String.Empty : objEZShredDataModel.Building[0].City,
                Country = objEZShredDataModel.Building[0] == null ? String.Empty : objEZShredDataModel.Building[0].CompanyCountryCode,
                Street = objEZShredDataModel.Building[0] == null ? String.Empty : objEZShredDataModel.Building[0].Street,
                State = objEZShredDataModel.Building[0] == null ? String.Empty : objEZShredDataModel.Building[0].State,
                ZIP = objEZShredDataModel.Building[0] == null ? String.Empty : objEZShredDataModel.Building[0].Zip,
            };
        }

        [Route("AddCustomer")]
        public ActionResult AddCustomer(string accountObjectID, EZShredDataModel objEZShredDataModel)
        {
            bool EZShredAddResponse = false;
            bool SharpSpringResponse = false;
            long leadID = 0;
            long OpportunityId = 0;
            string BuildingSet = LeadBuildingSet.Building1.ToString();
            try
            {
                if (objEZShredDataModel.Customer[0].LeadID > 0)
                {
                    EZShredLeadMappingDataModel LeadData = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, Convert.ToInt64(objEZShredDataModel.Customer[0].LeadID));
                    if (LeadData != null)
                        BuildingSet = GetNextBuildingStage(LeadData);
                }
                dynamic status = null;
                mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
                List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel = GetAllEZShredSSCustomFields(accountObjectID);
                objEZShredDataModel.AccountObjectId = accountObjectID;
                objEZShredDataModel.MxtrAccountId = account.MxtrAccountID;
                string leadResult = string.Empty;
                SharpspringLeadDataModel objSharpSpringDataModel = EZShredSSFieldsMapping(objEZShredDataModel);

                //If Email Already Exist in Sharpspring then Lead will update
                SharpspringLeadDataModel objSharpspringLeadDataModel = _apiSharpspringService.GetLead(objEZShredDataModel.Customer[0].EmailAddress);
                if (objSharpspringLeadDataModel != null)
                {
                    if (!SearchLeadInEZShredLeadMapping(objSharpspringLeadDataModel.LeadID, accountObjectID))
                    {
                        leadResult = UpdateLeadWithCustomFields(GetAllEZShredSSCustomFields(accountObjectID), accountObjectID, objSharpSpringDataModel, account, BuildingSet);
                        leadID = objSharpspringLeadDataModel.LeadID;
                        OpportunityId = OpportunityDealStagesOperations(accountObjectID, objEZShredDataModel.Customer[0].EmailAddress, leadID, objEZShredDataModel, ObjSharpspringCustomFieldsDataModel, account);
                        SharpSpringResponse = true;
                    }
                }
                else
                {
                    leadResult = CreateLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, accountObjectID, objSharpSpringDataModel, account, BuildingSet);
                    SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(leadResult);
                    if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                    {
                        SharpSpringResponse = true;
                        leadID = ssResponse.result.creates[0].id;
                        OpportunityId = OpportunityDealStagesOperations(accountObjectID, objEZShredDataModel.Customer[0].EmailAddress, ssResponse.result.creates[0].id, objEZShredDataModel, ObjSharpspringCustomFieldsDataModel, account);
                    }
                }

                //EZShredLeadMapping
                EZShredLeadMappingDataModel objEZShredLeadMappingDataModel = new EZShredLeadMappingDataModel()
                {
                    LeadID = leadID,
                    CustomerID = null,
                    OpportunityID = OpportunityId,
                    Company = objEZShredDataModel.Customer[0].Company,
                    Street = objEZShredDataModel.Customer[0].Street,
                    ZIP = objEZShredDataModel.Customer[0].Zip,
                    EZShredApiRequest = "",
                    EZShredBuildingApiRequest = "",
                    EZShredActionType = EZShredActionTypeKind.NoAction.ToString(),// "NoAction";
                    EZShredStatus = EZShredStatusKind.Complete.ToString(), //"Complete";
                };

                objEZShredDataModel.Building[0].OpportunityID = OpportunityId;

                //Action based on Pipeline            
                if (objEZShredDataModel.Customer[0].PipelineStatus == PipelineKind.Scheduled.ToString())
                {
                    EZShredLeadMappingDataModel LeadMappingresult = EZShredLeadMapping(accountObjectID, objEZShredLeadMappingDataModel, account);
                    objEZShredLeadMappingDataModel.Id = LeadMappingresult.Id;

                    objEZShredDataModel = AppendSuiteWithStreetAddress(objEZShredDataModel);
                    Tuple<bool, string, string, string> result = AddCustomerAndBuildingToEZShred(accountObjectID, objEZShredDataModel, objEZShredLeadMappingDataModel, account, BuildingSet);
                    EZShredAddResponse = result.Item1;
                    if (EZShredAddResponse)
                    {
                        //Get EZShred Customer info
                        CustomerInfoResult objCustomerInfoResult = _apiEZShredService.GetEZShredCustomerInfo(objEZShredDataModel.Customer[0].UserID, result.Item2);
                        //Save ARCustomerCode to SharpSpring
                        if (objCustomerInfoResult != null && objCustomerInfoResult.tblCustomers != null && objCustomerInfoResult.tblCustomers.Count > 0)
                        {
                            UpdateLeadARCustomerCode(ObjSharpspringCustomFieldsDataModel, accountObjectID, objEZShredLeadMappingDataModel.LeadID, objCustomerInfoResult.tblCustomers[0].ARCustomerCode, account);
                            UpdateOpportunityARCustomerCode(ObjSharpspringCustomFieldsDataModel, accountObjectID, OpportunityId, objCustomerInfoResult.tblCustomers[0].ARCustomerCode, account);
                        }
                    }

                    status = new { EZSharedStatus = EZShredAddResponse, SharpSpringStatus = SharpSpringResponse, Message = String.Empty };
                }
                else
                {
                    EZShredLeadMapping(accountObjectID, objEZShredLeadMappingDataModel, account);
                    AddLeadBuilding(objEZShredLeadMappingDataModel, AdaptLeadBuilding(objEZShredDataModel, objEZShredLeadMappingDataModel), LeadBuildingSet.Building1.ToString());
                    status = new { EZSharedStatus = false, SharpSpringStatus = SharpSpringResponse, Message = String.Empty };
                }
                return Json(status);
            }
            catch (Exception ex)
            {
                return Json(new { EZSharedStatus = EZShredAddResponse, SharpSpringStatus = SharpSpringResponse, Message = ex.Message });
            }
        }

        [Route("CheckSSCustomFields")]
        public ActionResult CheckSSCustomFields(string accountObjectID)
        {
            if (!string.IsNullOrEmpty(accountObjectID))
            {
                mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
                _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
                List<SharpspringFieldLabelDataModel> customFiledsData = _apiSharpspringService.GetCustomFields();
                if (customFiledsData == null || customFiledsData.Count == 0)
                {
                    return Json(false);
                }
            }
            return Json(true);
        }

        [HttpPost]
        [Route("SearchCustomers")]
        public ActionResult SearchCustomers(string accountObjectID, string searchText)
        {
            try
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                List<EZShredCustomerDataModelMini> lstCustomers = new List<EZShredCustomerDataModelMini>();

                // get customers from EZshred Customer table 
                lstCustomers = _dbEZShredCustomerService.SearchCustomer(accountObjectID, searchText).ToList();
                lstCustomers.ForEach(x => x.DataSource = "EZShred");

                // get customers from lead mapping table 
                List<EZShredLeadMappingDataModel> lstSSCustomer = _dbEZShredLeadMappingService.GetCustomerDataFromEZShredLeadMapping(accountObjectID, searchText);

                // merge Customer from EZShred Customer and lead Mapping
                MergeCustomers(lstCustomers, lstSSCustomer);

                List<CustomerSearchResult> lstBuildingsForCustomer = new List<CustomerSearchResult>();
                foreach (var item in lstCustomers)
                {
                    var data = new List<CustomerSearchResult>();
                    if (item.DataSource == "EZShred")
                    {
                        data = _dbEZShredBuildingService.GetBuildingsByCustomerId(accountObjectID, item.CustomerID);
                    }

                    AddBuildingsData(lstBuildingsForCustomer, item, item.Building1);
                    AddBuildingsData(lstBuildingsForCustomer, item, item.Building2);
                    AddBuildingsData(lstBuildingsForCustomer, item, item.Building3);
                    AddBuildingsData(lstBuildingsForCustomer, item, item.Building4);
                    AddBuildingsData(lstBuildingsForCustomer, item, item.Building5);

                    foreach (var buildData in data)
                    {
                        var result = lstBuildingsForCustomer.Where(x => x.BuildingID == buildData.BuildingID);
                        if (result == null || result.Count() == 0)
                        {
                            buildData.Company = item.Company;
                            buildData.CustomerID = item.CustomerID;
                            buildData.LeadID = item.LeadID;
                            lstBuildingsForCustomer.Add(buildData);
                        }
                    }
                }

                // get buildings from EZshred Building table 
                List<EZShredBuildingDataModel> lstBuildings = _dbEZShredBuildingService.SearchBuilding(accountObjectID, searchText).ToList();
                lstBuildings.ForEach(x => x.DataSource = "EZShred");

                // get buildings from lead mapping table 
                List<EZShredLeadMappingDataModel> lstSSBuildings = _dbEZShredLeadMappingService.GetBuildingsDataFromEZShredLeadMapping(accountObjectID, searchText);

                //--------- Get Customer -------------------------                
                List<CustomerSearchResult> lstCustomerForBuilding = new List<CustomerSearchResult>();
                foreach (var item in lstBuildings)
                {
                    if (item.DataSource == "EZShred")
                    {
                        int customerId = _dbEZShredBuildingService.GetCustomerByBuildingId(accountObjectID, item.BuildingID);
                        var customers = _dbEZShredCustomerService.GetAllCustomerMiniByCustomerId(accountObjectID, Convert.ToString(customerId));
                        foreach (var customer in customers)
                        {
                            lstCustomerForBuilding.Add(new CustomerSearchResult()
                            {
                                Company = customer.Company,
                                BuildingName = item.CompanyName,
                                Zip = item.Zip,
                                Street = item.Street,
                                CustomerID = item.CustomerID,
                                BuildingID = item.BuildingID,
                            });
                        }
                    }
                }
                //-------------------------------

                List<CustomerSearchResult> lstMergedBuildings = MergeBuildings(lstBuildings, lstSSBuildings, searchText);

                List<CustomerSearchResult> lstMergedSearchResult = MergeBuildingsCustomers(lstCustomers, lstMergedBuildings);

                // check for old data of customer
                foreach (var item in lstBuildingsForCustomer)
                {
                    var result = new List<CustomerSearchResult>();
                    if (item.OpportunityID > 0)
                    {
                        result = lstMergedSearchResult.Where(x => x.OpportunityID == item.OpportunityID).ToList();
                    }
                    else
                    {
                        result = lstMergedSearchResult.Where(x => x.BuildingID == item.BuildingID).ToList();
                    }

                    if (result == null || result.Count == 0)
                    {
                        lstMergedSearchResult.Add(new CustomerSearchResult()
                        {
                            Company = item.Company,
                            BuildingName = item.BuildingName,
                            Street = item.Street,
                            Zip = item.Zip,
                            AccountObjectId = accountObjectID,
                            CustomerID = item.CustomerID,
                            BuildingID = item.BuildingID,
                            LeadID = item.LeadID,
                            OpportunityID = item.OpportunityID,
                        });
                    }
                }
                //check for old data of building
                foreach (var item in lstCustomerForBuilding)
                {
                    var result = lstMergedSearchResult.Where(x => x.CustomerID == item.CustomerID).ToList();
                    if (result == null || result.Count == 0)
                    {
                        lstMergedSearchResult.Add(new CustomerSearchResult()
                        {
                            Company = item.Company,
                            BuildingName = item.BuildingName,
                            Street = item.Street,
                            Zip = item.Zip,
                            AccountObjectId = accountObjectID,
                            CustomerID = item.CustomerID,
                            BuildingID = item.BuildingID,
                        });
                    }
                }

                //final data
                var customerData = new { customer = lstMergedSearchResult };
                customerData.customer.ForEach(x =>
                {
                    if (x.CustomerID == null)
                    {
                        x.CustomerID = "";
                    }
                    if (x.Street == null)
                    {
                        x.Street = "";
                    }
                    if (x.Zip == null)
                    {
                        x.Zip = "";
                    }
                });
                return Json(customerData);
            }
            catch (Exception ex)
            {
                var customerData = new { customer = ex.Message };
                return Json(customerData);
            }

        }

        private static void AddBuildingsData(List<CustomerSearchResult> lstBuildingsForCustomer, EZShredCustomerDataModelMini item, LeadBuildingDataModel building)
        {
            if (building != null && !String.IsNullOrEmpty(building.BuildingCompanyName))
            {
                lstBuildingsForCustomer.Add(new CustomerSearchResult
                {
                    Company = item.Company,
                    CustomerID = item.CustomerID,
                    LeadID = item.LeadID,
                    BuildingName = building.BuildingCompanyName,
                    Zip = building.ZIP,
                    Street = building.Street,
                    OpportunityID = building.OpportunityID,
                    BuildingID = Convert.ToString(building.BuildingID),
                });
            }
        }

        private List<CustomerSearchResult> MergeBuildingsCustomers(List<EZShredCustomerDataModelMini> lstCustomers, List<CustomerSearchResult> lstMergedBuildings)
        {
            if (lstCustomers.Count > 0)
            {
                foreach (var item in lstCustomers)
                {
                    if (item.DataSource != "EZShred")
                    {
                        MergeCustomerBuildingData(lstMergedBuildings, item.Building1, item.Company, item.AccountObjectId, item.CustomerID, item.LeadID);
                        MergeCustomerBuildingData(lstMergedBuildings, item.Building2, item.Company, item.AccountObjectId, item.CustomerID, item.LeadID);
                        MergeCustomerBuildingData(lstMergedBuildings, item.Building3, item.Company, item.AccountObjectId, item.CustomerID, item.LeadID);
                        MergeCustomerBuildingData(lstMergedBuildings, item.Building4, item.Company, item.AccountObjectId, item.CustomerID, item.LeadID);
                        MergeCustomerBuildingData(lstMergedBuildings, item.Building5, item.Company, item.AccountObjectId, item.CustomerID, item.LeadID);
                    }
                }
            }
            return lstMergedBuildings;
        }

        private static void MergeCustomerBuildingData(List<CustomerSearchResult> lstMergedBuildings, LeadBuildingDataModel Building, string company, string accountObjectId, string customerID, long leadId)
        {
            if (Building != null && Building.OpportunityID > 0)
            {
                var res = lstMergedBuildings.Where(x => x.OpportunityID == Building.OpportunityID).ToList();
                if (res == null || res.Count == 0)
                {
                    lstMergedBuildings.Add(new CustomerSearchResult()
                    {
                        Company = company,
                        BuildingName = Building.BuildingCompanyName,
                        Street = Building.Street,
                        Zip = Building.ZIP,
                        AccountObjectId = accountObjectId,
                        CustomerID = customerID,
                        OpportunityID = Building.OpportunityID,
                        LeadID = leadId,
                    });
                }
            }
        }

        private List<CustomerSearchResult> MergeBuildings(List<EZShredBuildingDataModel> lstBuildings, List<EZShredLeadMappingDataModel> lstSSBuildings, string searchText)
        {
            List<CustomerSearchResult> lstCustomerSearchResult = new List<CustomerSearchResult>();
            foreach (var ssBuilding in lstSSBuildings)
            {
                CustomerSearchResult objCustomerSearchResult = AddBuilding(lstBuildings, ssBuilding.Building1, ssBuilding.Company, ssBuilding.CustomerID, ssBuilding.LeadID, searchText);
                if (objCustomerSearchResult != null)
                {
                    lstCustomerSearchResult.Add(objCustomerSearchResult);
                }

                objCustomerSearchResult = AddBuilding(lstBuildings, ssBuilding.Building2, ssBuilding.Company, ssBuilding.CustomerID, ssBuilding.LeadID, searchText);
                if (objCustomerSearchResult != null)
                {
                    lstCustomerSearchResult.Add(objCustomerSearchResult);
                }

                objCustomerSearchResult = AddBuilding(lstBuildings, ssBuilding.Building3, ssBuilding.Company, ssBuilding.CustomerID, ssBuilding.LeadID, searchText);
                if (objCustomerSearchResult != null)
                {
                    lstCustomerSearchResult.Add(objCustomerSearchResult);
                }

                objCustomerSearchResult = AddBuilding(lstBuildings, ssBuilding.Building4, ssBuilding.Company, ssBuilding.CustomerID, ssBuilding.LeadID, searchText);
                if (objCustomerSearchResult != null)
                {
                    lstCustomerSearchResult.Add(objCustomerSearchResult);
                }

                objCustomerSearchResult = AddBuilding(lstBuildings, ssBuilding.Building5, ssBuilding.Company, ssBuilding.CustomerID, ssBuilding.LeadID, searchText);
                if (objCustomerSearchResult != null)
                {
                    lstCustomerSearchResult.Add(objCustomerSearchResult);
                }
            }
            return lstCustomerSearchResult;
        }

        private CustomerSearchResult AddBuilding(List<EZShredBuildingDataModel> lstBuildings, LeadBuildingDataModel Building, string company, int? customerID, long leadId, string searchText)
        {
            if (Building.BuildingCompanyName.ToLower().Contains(searchText.ToLower()))
            {
                var result = lstBuildings.Where(x => x.OpportunityID == Building.OpportunityID);
                //if ((result == null || result.Count() == 0) && Building.OpportunityID > 0)
                if (Building.OpportunityID > 0)
                {
                    return new CustomerSearchResult()
                    {
                        Company = company,
                        BuildingName = Building.BuildingCompanyName,
                        Street = Building.Street,
                        Zip = Building.ZIP,
                        BuildingID = Convert.ToString(Building.BuildingID),
                        DataSource = "SS",
                        CustomerID = Convert.ToString(customerID),
                        OpportunityID = Building.OpportunityID,
                        LeadID = leadId,
                    };
                }
            }
            return null;
        }

        private List<CustomerSearchResult> EmbedBuilding(string accountObjectID, List<EZShredCustomerDataModelMini> lstCustomers)
        {
            List<CustomerSearchResult> lstCustomerSearchResult = new List<CustomerSearchResult>();
            foreach (var customer in lstCustomers)
            {
                IEnumerable<EZShredBuildingDataModelMini> buildings = null;
                if (!string.IsNullOrEmpty(customer.CustomerID))
                {
                    buildings = _dbEZShredBuildingService.GetBuildingsAgainstCustomerId(accountObjectID, customer.CustomerID);
                }

                if (buildings != null && buildings.Count() > 0)
                {
                    foreach (var building in buildings)
                    {
                        lstCustomerSearchResult.Add(new CustomerSearchResult()
                        {
                            BuildingID = building.BuildingID,
                            BuildingName = building.CompanyName,
                            Company = customer.Company,
                            Street = customer.Street,
                            Zip = customer.Zip,
                            LeadID = customer.LeadID,
                            CustomerID = Convert.ToString(customer.CustomerID),
                            OpportunityID = customer.OpportunityID,
                            DataSource = customer.DataSource,
                        });
                    }
                }
                else
                {
                    lstCustomerSearchResult.Add(new CustomerSearchResult()
                    {
                        Company = customer.Company,
                        Street = customer.Street,
                        Zip = customer.Zip,
                        LeadID = customer.LeadID,
                        CustomerID = Convert.ToString(customer.CustomerID),
                        OpportunityID = customer.OpportunityID,
                        DataSource = customer.DataSource,
                    });
                }
            }

            return lstCustomerSearchResult;
        }

        private static void MergeCustomers(List<EZShredCustomerDataModelMini> lstCustomers, List<EZShredLeadMappingDataModel> lstSSCustomer)
        {
            foreach (var ssCustomer in lstSSCustomer)
            {
                if (ssCustomer.CustomerID == null || ssCustomer.CustomerID == 0)
                {
                    AddCustomer(lstCustomers, ssCustomer);
                }
                else if (lstCustomers.Where(x => x.CustomerID == Convert.ToString(ssCustomer.CustomerID)).Count() == 0)
                {
                    AddCustomer(lstCustomers, ssCustomer);
                }
                else
                {
                    lstCustomers.Where(x => x.CustomerID == Convert.ToString(ssCustomer.CustomerID)).ToList().ForEach(c =>
                    {
                        c.OpportunityID = ssCustomer.OpportunityID;
                        c.LeadID = ssCustomer.LeadID;
                        c.Building1 = ssCustomer.Building1;
                        c.Building2 = ssCustomer.Building2;
                        c.Building3 = ssCustomer.Building3;
                        c.Building4 = ssCustomer.Building4;
                        c.Building5 = ssCustomer.Building5;
                    });
                }
            }
        }

        private static void AddCustomer(List<EZShredCustomerDataModelMini> lstCustomers, EZShredLeadMappingDataModel ssCustomer)
        {
            lstCustomers.Add(new EZShredCustomerDataModelMini()
            {
                Company = ssCustomer.Company,
                Street = ssCustomer.Street,
                Zip = ssCustomer.ZIP,
                DataSource = "SS",
                LeadID = ssCustomer.LeadID,
                CustomerID = Convert.ToString(ssCustomer.CustomerID),
                OpportunityID = ssCustomer.OpportunityID,
                Building1 = new LeadBuildingDataModel()
                {
                    BuildingCompanyName = ssCustomer.Building1 == null ? "" : ssCustomer.Building1.BuildingCompanyName,
                    BuildingID = ssCustomer.Building1 == null ? 0 : ssCustomer.Building1.BuildingID,
                    OpportunityID = ssCustomer.Building1 == null ? 0 : ssCustomer.Building1.OpportunityID,
                    Street = ssCustomer.Building1 == null ? "" : ssCustomer.Building1.Street,
                    ZIP = ssCustomer.Building1 == null ? "" : ssCustomer.Building1.ZIP,
                    City = ssCustomer.Building1 == null ? "" : ssCustomer.Building1.City,
                },
                Building2 = new LeadBuildingDataModel()
                {
                    BuildingCompanyName = ssCustomer.Building2 == null ? "" : ssCustomer.Building2.BuildingCompanyName,
                    BuildingID = ssCustomer.Building2 == null ? 0 : ssCustomer.Building2.BuildingID,
                    OpportunityID = ssCustomer.Building2 == null ? 0 : ssCustomer.Building2.OpportunityID,
                    Street = ssCustomer.Building2 == null ? "" : ssCustomer.Building2.Street,
                    ZIP = ssCustomer.Building2 == null ? "" : ssCustomer.Building2.ZIP,
                    City = ssCustomer.Building2 == null ? "" : ssCustomer.Building2.City,
                },
                Building3 = new LeadBuildingDataModel()
                {
                    BuildingCompanyName = ssCustomer.Building3 == null ? "" : ssCustomer.Building3.BuildingCompanyName,
                    BuildingID = ssCustomer.Building3 == null ? 0 : ssCustomer.Building3.BuildingID,
                    OpportunityID = ssCustomer.Building3 == null ? 0 : ssCustomer.Building3.OpportunityID,
                    Street = ssCustomer.Building3 == null ? "" : ssCustomer.Building3.Street,
                    ZIP = ssCustomer.Building3 == null ? "" : ssCustomer.Building3.ZIP,
                    City = ssCustomer.Building3 == null ? "" : ssCustomer.Building3.City,
                },
                Building4 = new LeadBuildingDataModel()
                {
                    BuildingCompanyName = ssCustomer.Building4 == null ? "" : ssCustomer.Building4.BuildingCompanyName,
                    BuildingID = ssCustomer.Building4 == null ? 0 : ssCustomer.Building4.BuildingID,
                    OpportunityID = ssCustomer.Building4 == null ? 0 : ssCustomer.Building4.OpportunityID,
                    Street = ssCustomer.Building4 == null ? "" : ssCustomer.Building4.Street,
                    ZIP = ssCustomer.Building4 == null ? "" : ssCustomer.Building4.ZIP,
                    City = ssCustomer.Building4 == null ? "" : ssCustomer.Building4.City,
                },
                Building5 = new LeadBuildingDataModel()
                {
                    BuildingCompanyName = ssCustomer.Building5 == null ? "" : ssCustomer.Building5.BuildingCompanyName,
                    BuildingID = ssCustomer.Building5 == null ? 0 : ssCustomer.Building5.BuildingID,
                    OpportunityID = ssCustomer.Building5 == null ? 0 : ssCustomer.Building5.OpportunityID,
                    Street = ssCustomer.Building5 == null ? "" : ssCustomer.Building5.Street,
                    ZIP = ssCustomer.Building5 == null ? "" : ssCustomer.Building5.ZIP,
                    City = ssCustomer.Building5 == null ? "" : ssCustomer.Building5.City,
                }
            });
        }

        private List<CustomerSearchResult> MergeBuildings(List<EZShredCustomerDataModelMini> lstCustomers, List<EZShredBuildingDataModel> lstBuildings, List<EZShredLeadMappingDataModel> lstSSBuildings, string accountObjectID)
        {
            foreach (var ssBuilding in lstSSBuildings)
            {
                AddBuilding(lstBuildings, ssBuilding, ssBuilding.Building1);
                AddBuilding(lstBuildings, ssBuilding, ssBuilding.Building2);
                AddBuilding(lstBuildings, ssBuilding, ssBuilding.Building3);
                AddBuilding(lstBuildings, ssBuilding, ssBuilding.Building4);
                AddBuilding(lstBuildings, ssBuilding, ssBuilding.Building5);
            }


            foreach (var item in lstBuildings)
            {
                if (lstCustomers.Where(x => x.CustomerID == item.CustomerID).Count() == 0)
                {
                    var customer = _dbEZShredCustomerService.GetAllCustomerMiniByCustomerId(accountObjectID, item.CustomerID).FirstOrDefault();
                    if (customer != null)
                    {
                        customer.OpportunityID = item.OpportunityID;
                        lstCustomers.Add(customer);
                    }
                    else
                    {
                        var ssCustomer = _dbEZShredLeadMappingService.GetEZShredLeadDataByCustomerId(accountObjectID, Convert.ToInt32(item.CustomerID));
                        List<EZShredCustomerDataModelMini> lstCust = ssCustomer.Select(x => new EZShredCustomerDataModelMini
                        {
                            CustomerID = Convert.ToString(x.CustomerID),
                            LeadID = x.LeadID,
                            OpportunityID = item.OpportunityID,
                            Company = x.Company,
                            Street = x.Street,
                            Zip = x.ZIP,
                            Building1 = x.Building1,
                        }).ToList();
                        lstCustomers.Add(lstCust.FirstOrDefault());
                    }
                }
                else
                {
                    lstCustomers.Where(x => x.CustomerID == item.CustomerID).ToList().ForEach(c =>
                    {
                        c.OpportunityID = item.OpportunityID;
                    });
                }
            }

            List<CustomerSearchResult> lstSearchResult = new List<CustomerSearchResult>();
            foreach (var item in lstCustomers)
            {
                if (!String.IsNullOrEmpty(item.CustomerID) && Convert.ToInt64(item.CustomerID) > 0)
                {
                    List<EZShredBuildingDataModel> custmerBuildings = lstBuildings.Where(w => w.CustomerID == item.CustomerID).ToList();
                    if (custmerBuildings == null || custmerBuildings.Count == 0)
                    {
                        if (item.Building1 == null && item.Building2 == null && item.Building3 == null && item.Building4 == null && item.Building5 == null)
                        {
                            lstSearchResult.Add(new CustomerSearchResult()
                            {
                                AccountObjectId = item.AccountObjectId,
                                AllowZeroInvoices = item.AllowZeroInvoices,
                                ARCustomerCode = item.ARCustomerCode,
                                Company = item.Company,
                                CustomerID = item.CustomerID,
                                LeadID = item.LeadID,
                                Street = item.Street,
                                Zip = item.Zip,
                                OpportunityID = item.OpportunityID,
                                Building1 = item.Building1,
                                Building2 = item.Building2,
                                Building3 = item.Building3,
                                Building4 = item.Building4,
                                Building5 = item.Building5,
                            });
                        }
                        else
                        {
                            if (item.Building1 != null)
                            {
                                AddBuildingData(lstSearchResult, item, item.Building1);
                            }
                            if (item.Building2 != null)
                            {
                                AddBuildingData(lstSearchResult, item, item.Building2);
                            }
                            if (item.Building3 != null)
                            {
                                AddBuildingData(lstSearchResult, item, item.Building3);
                            }
                            if (item.Building4 != null)
                            {
                                AddBuildingData(lstSearchResult, item, item.Building4);
                            }
                            if (item.Building5 != null)
                            {
                                AddBuildingData(lstSearchResult, item, item.Building5);
                            }
                        }
                    }
                    else
                    {
                        // now add these building info to customer
                        // we need a new datamodel which is combination of customer and building
                        foreach (var building in custmerBuildings)
                        {
                            lstSearchResult.Add(new CustomerSearchResult()
                            {
                                AccountObjectId = item.AccountObjectId,
                                AllowZeroInvoices = item.AllowZeroInvoices,
                                ARCustomerCode = item.ARCustomerCode,
                                BuildingID = building.BuildingID,
                                BuildingName = building.CompanyName,
                                Company = item.Company,
                                CustomerID = item.CustomerID,
                                LeadID = item.LeadID,
                                OpportunityID = building.OpportunityID,
                                //OpportunityID = GetOpportunityId(whichBuilding, item),
                                Street = item.Street,
                                Zip = item.Zip,
                                Building1 = item.Building1,
                                Building2 = item.Building2,
                                Building3 = item.Building3,
                                Building4 = item.Building4,
                                Building5 = item.Building5,
                            });
                        }
                    }
                }
                else
                {
                    //test code
                    lstSearchResult.Add(new CustomerSearchResult()
                    {
                        AccountObjectId = item.AccountObjectId,
                        AllowZeroInvoices = item.AllowZeroInvoices,
                        ARCustomerCode = item.ARCustomerCode,
                        Company = item.Company,
                        CustomerID = item.CustomerID,
                        LeadID = item.LeadID,
                        Street = item.Street,
                        Zip = item.Zip,
                        OpportunityID = item.OpportunityID,
                        Building1 = item.Building1,
                        Building2 = item.Building2,
                        Building3 = item.Building3,
                        Building4 = item.Building4,
                        Building5 = item.Building5,

                    });
                }
            }
            return lstSearchResult;
        }

        private static void AddBuildingData(List<CustomerSearchResult> lstSearchResult, EZShredCustomerDataModelMini item, LeadBuildingDataModel building)
        {
            var custmerBuildings = lstSearchResult.Where(w => w.BuildingID == Convert.ToString(building.BuildingID)).ToList();
            if ((custmerBuildings == null || custmerBuildings.Count == 0) && building.BuildingID > 0)
            {
                lstSearchResult.Add(new CustomerSearchResult()
                {
                    AccountObjectId = item.AccountObjectId,
                    AllowZeroInvoices = item.AllowZeroInvoices,
                    ARCustomerCode = item.ARCustomerCode,
                    Company = item.Company,
                    CustomerID = item.CustomerID,
                    LeadID = item.LeadID,
                    Street = item.Street,
                    Zip = item.Zip,
                    BuildingID = Convert.ToString(building.BuildingID),
                    BuildingName = building.BuildingCompanyName,
                });
            }
        }

        private static void AddBuilding(List<EZShredBuildingDataModel> lstBuildings, EZShredLeadMappingDataModel ssCustomer, LeadBuildingDataModel Building)
        {
            if (Building.BuildingID > 0)
            {
                var result = lstBuildings.Where(x => x.BuildingID == Convert.ToString(Building.BuildingID) && x.CustomerID == Convert.ToString(ssCustomer.CustomerID));
                if (result == null || result.Count() == 0)
                {
                    lstBuildings.Add(new EZShredBuildingDataModel()
                    {
                        //CompanyName = ssCustomer.Company,
                        CompanyName = Building.BuildingCompanyName,
                        Street = ssCustomer.Street,
                        Zip = ssCustomer.ZIP,
                        BuildingID = Convert.ToString(Building.BuildingID),
                        //DataSource = "SS",
                        LeadID = ssCustomer.LeadID,
                        CustomerID = Convert.ToString(ssCustomer.CustomerID),
                        OpportunityID = Building.OpportunityID,
                    });
                }
                else
                {
                    lstBuildings.Where(x => x.BuildingID == Convert.ToString(Building.BuildingID) && x.CustomerID == Convert.ToString(ssCustomer.CustomerID)).ToList().ForEach(c =>
                    {
                        c.OpportunityID = Building.OpportunityID;
                    });
                }
            }
            else if (Building.OpportunityID > 0)
            {
                var result = lstBuildings.Where(x => x.OpportunityID == Building.OpportunityID && x.CustomerID == Convert.ToString(ssCustomer.CustomerID));
                if (result == null || result.Count() == 0)
                {
                    lstBuildings.Add(new EZShredBuildingDataModel()
                    {
                        //CompanyName = ssCustomer.Company,
                        CompanyName = Building.BuildingCompanyName,
                        Street = ssCustomer.Street,
                        Zip = ssCustomer.ZIP,
                        BuildingID = Convert.ToString(Building.BuildingID),
                        //DataSource = "SS",
                        LeadID = ssCustomer.LeadID,
                        CustomerID = Convert.ToString(ssCustomer.CustomerID),
                        OpportunityID = Building.OpportunityID,
                    });
                }
            }
        }

        private Tuple<bool, long, long> CheckSSLeadOpporunity(string accountObjectID, EZShredDataModel objEZShredDataModel, bool SharpSpringStatus, SharpspringLeadDataModel objSharpSpringDataModel, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, mxtrAccount account, string BuildingSet, bool IsAddNewBuilding)
        {
            bool PipelineStatusIsClosed = false;
            if (objEZShredDataModel.Customer[0].LeadID > 0)
            {
                //if (IsAddNewBuilding)
                //    BuildingSet = GetBuildingSetByOpportunityID(accountObjectID, Convert.ToInt64(objEZShredDataModel.Customer[0].LeadID), Convert.ToInt64(objEZShredDataModel.Customer[0].OpportunityID));
                string leadResult = UpdateLeadWithCustomFields(GetAllEZShredSSCustomFields(accountObjectID), accountObjectID, objSharpSpringDataModel, account, BuildingSet);
                if (objEZShredDataModel.Customer[0].OpportunityID > 0)
                {
                    List<SharpspringDealStageDataModel> lstDealStages = _apiSharpspringService.GetAllDealStages();
                    List<SharpspringOpportunityDataModel> lstOpportunities = _apiSharpspringService.GetOpportunitiesById(objEZShredDataModel.Customer[0].OpportunityID);
                    //Set Pipeline Status Lost
                    if (objEZShredDataModel.Customer[0].PipelineStatus == PipelineStatus.Lost.ToString())
                    {
                        PipelineStatusIsClosed = true;
                        var previousDealStageInSS = objEZShredDataModel.Customer[0].OpportunityID <= 0 ? String.Empty : GetDealStageNameByOpportunityId(objEZShredDataModel.Customer[0].OpportunityID);
                        objEZShredDataModel.Customer[0].PipelineStatus = MapDealStageName(previousDealStageInSS, account);
                    }
                    var dealStageData = lstDealStages.FirstOrDefault(x => x.DealStageName.ToLower() == GetDealStageName(objEZShredDataModel.Customer[0].PipelineStatus, account).ToLower());//Highlight the pipeline button

                    if (lstOpportunities != null && lstOpportunities.Count > 0)
                    {
                        if (lstOpportunities[0].IsClosed == true)
                        {
                            //Anand please check
                            //List<SharpspringOpportunityDataModel> lstOpportunityData = AdaptOpportunityData(objEZShredDataModel, dealStageData.DealStageID);

                            List<SharpspringOpportunityDataModel> lstOpportunityData = new List<SharpspringOpportunityDataModel>
                            {
                               new SharpspringOpportunityDataModel()
                               {
                                   DealStageID=dealStageData.DealStageID,
                                   OpportunityName=objEZShredDataModel.Customer[0].Company + " Opportunity",
                                   IsActive=true,
                                   Amount=objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Amount:0,
                                   AdditionalTips = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].AdditionalTips:null,
                                   JobQuantitySize = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].JobQuantitySize:"0",
                                   ProposedDateOfService = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedDateOfService:null,
                                   CertificateOfInsurance = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].CertificateOfInsurance:null,
                                   HoursOfBusiness = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HoursOfBusiness:null,
                                   Stairs = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Stairs:null,
                                   ProposedConsoleDeliveryDate = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedConsoleDeliveryDate:null,
                                   ProposedStartDate = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedStartDate:null,
                                   ECUnits = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECUnits:null,
                                   ECPriceUnit = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECPriceUnit:null,
                                   ECAdditionalPrice = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECAdditionalPrice:null,
                                   GallonUnits_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonUnits_64:null,
                                   GallonPriceUnit_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_64:null,
                                   GallonAdditionalPrice_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_64:null,
                                   GallonUnits_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonUnits_96:null,
                                   GallonPriceUnit_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_96:null,
                                   GallonAdditionalPrice_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_96:null,
                                   NumberOfTips = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].NumberOfTips:null,
                                   PriceQuotedForFirstTip = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].PriceQuotedForFirstTip:null,
                                   BankerBoxes = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BankerBoxes:null,
                                   FileBoxes = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].FileBoxes:null,
                                   Bags = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Bags:null,
                                   Cabinets = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Cabinets:null,
                                   Skids = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Skids:null,
                                   HardDrivers = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HardDrivers:null,
                                   Media = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Media:null,
                                   Other = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Other:null,
                                   SellerComments = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].SellerComments:null,
                                   HardDrive_Media_Other_Comment = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HardDrive_Media_Other_Comment:null,
                                   BuildingContactFirstName = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingContactFirstName : null,
                                   BuildingContactLastName = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingContactLastName : null,
                                   BuildingContactPhone = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingContactPhone : null,
                                   BuildingName = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingName : null,
                                   BuildingStreet = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingStreet : null,
                                   BuildingCity = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingCity : null,
                                   BuildingState = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingState : null,
                                   ZipCode = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ZipCode : null,
                               }
                            };
                            string opportunitiesResult = _apiSharpspringService.CreateOpportunitiesWithCustomFields(lstOpportunityData, ObjSharpspringCustomFieldsDataModel);
                            SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(opportunitiesResult);
                            if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                            {
                                objEZShredDataModel.Customer[0].OpportunityID = ssResponse.result.creates[0].id;
                                _apiSharpspringService.CreateOpportunityLeads(ssResponse.result.creates[0].id, objEZShredDataModel.Customer[0].LeadID);
                            }
                        }
                        else
                        {
                            lstOpportunities.Add(AddOpportunity(objEZShredDataModel, PipelineStatusIsClosed, dealStageData.DealStageID));
                            _apiSharpspringService.UpdateOpportunitiesWithCustomFields(lstOpportunities, ObjSharpspringCustomFieldsDataModel);
                        }
                    }
                    SharpSpringStatus = true;
                }
                else
                {
                    // Create New Opportunity
                    if (!IsAddNewBuilding)
                    {
                        objEZShredDataModel.Customer[0].OpportunityID = OpportunityDealStagesOperations(accountObjectID, objEZShredDataModel.Customer[0].EmailAddress, objEZShredDataModel.Customer[0].LeadID, objEZShredDataModel, ObjSharpspringCustomFieldsDataModel, account);
                    }
                    SharpSpringStatus = true;
                }
            }
            else
            {
                //create lead & Opportunity
                var leadResult = CreateLeadWithCustomFields(GetAllEZShredSSCustomFields(accountObjectID), accountObjectID, objSharpSpringDataModel, account, BuildingSet);
                SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(leadResult);
                if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                {
                    SharpSpringStatus = true;
                    objEZShredDataModel.Customer[0].LeadID = ssResponse.result.creates[0].id;
                    objEZShredDataModel.Customer[0].OpportunityID = OpportunityDealStagesOperations(accountObjectID, objEZShredDataModel.Customer[0].EmailAddress, ssResponse.result.creates[0].id, objEZShredDataModel, ObjSharpspringCustomFieldsDataModel, account);
                }
            }
            return new Tuple<bool, long, long>(SharpSpringStatus, objEZShredDataModel.Customer[0].LeadID, objEZShredDataModel.Customer[0].OpportunityID);

        }

        private SharpspringOpportunityDataModel AddOpportunity(EZShredDataModel objEZShredDataModel, bool PipelineStatusIsClosed, long dealStageId)
        {
            return new SharpspringOpportunityDataModel
            {
                DealStageID = dealStageId,
                OpportunityID = objEZShredDataModel.Customer[0].OpportunityID,
                IsClosed = PipelineStatusIsClosed,
                OpportunityName = objEZShredDataModel.Customer[0].Company + " Opportunity",
                Amount = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].Amount : 0,
                AdditionalTips = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].AdditionalTips : null,
                JobQuantitySize = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].JobQuantitySize : "0",
                ProposedDateOfService = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ProposedDateOfService : null,
                CertificateOfInsurance = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].CertificateOfInsurance : null,
                HoursOfBusiness = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].HoursOfBusiness : null,
                Stairs = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].Stairs : null,
                ProposedConsoleDeliveryDate = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ProposedConsoleDeliveryDate : null,
                ProposedStartDate = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ProposedStartDate : null,
                ECUnits = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ECUnits : null,
                ECPriceUnit = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ECPriceUnit : null,
                ECAdditionalPrice = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ECAdditionalPrice : null,
                GallonUnits_64 = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].GallonUnits_64 : null,
                GallonPriceUnit_64 = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_64 : null,
                GallonAdditionalPrice_64 = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_64 : null,
                GallonUnits_96 = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].GallonUnits_96 : null,
                GallonPriceUnit_96 = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_96 : null,
                GallonAdditionalPrice_96 = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_96 : null,
                NumberOfTips = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].NumberOfTips : null,
                PriceQuotedForFirstTip = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].PriceQuotedForFirstTip : null,
                BankerBoxes = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BankerBoxes : null,
                FileBoxes = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].FileBoxes : null,
                Bags = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].Bags : null,
                Cabinets = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].Cabinets : null,
                Skids = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].Skids : null,
                HardDrivers = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].HardDrivers : null,
                Media = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].Media : null,
                Other = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].Other : null,
                SellerComments = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].SellerComments : null,
                HardDrive_Media_Other_Comment = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].HardDrive_Media_Other_Comment : null,
                BuildingContactFirstName = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingContactFirstName : null,
                BuildingContactLastName = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingContactLastName : null,
                BuildingContactPhone = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingContactPhone : null,
                BuildingName = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingName : null,
                BuildingStreet = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingStreet : null,
                BuildingCity = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingCity : null,
                BuildingState = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].BuildingState : null,
                ZipCode = objEZShredDataModel.SSOpportunity[0] != null ? objEZShredDataModel.SSOpportunity[0].ZipCode : null,
            };
        }

        private static List<SharpspringOpportunityDataModel> AdaptOpportunityData(EZShredDataModel objEZShredDataModel, long dealStageId)
        {
            List<SharpspringOpportunityDataModel> lstOpportunityData = new List<SharpspringOpportunityDataModel>
            {
                new SharpspringOpportunityDataModel()
                {
                    OwnerID=313386735,// fixs for now
                    DealStageID=dealStageId,
                    OpportunityName=objEZShredDataModel.Customer[0].Company + " Opportunity",
                    IsActive=true,
                    Amount=objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Amount:0,
                    AdditionalTips = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].AdditionalTips:null,
                    JobQuantitySize = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].JobQuantitySize:"0",
                    ProposedDateOfService = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedDateOfService:null,
                    CertificateOfInsurance = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].CertificateOfInsurance:null,
                    HoursOfBusiness = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HoursOfBusiness:null,
                    Stairs = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Stairs:null,
                    ProposedConsoleDeliveryDate = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedConsoleDeliveryDate:null,
                    ProposedStartDate = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedStartDate:null,
                    ECUnits = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECUnits:null,
                    ECPriceUnit = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECPriceUnit:null,
                    ECAdditionalPrice = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECAdditionalPrice:null,
                    GallonUnits_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonUnits_64:null,
                    GallonPriceUnit_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_64:null,
                    GallonAdditionalPrice_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_64:null,
                    GallonUnits_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonUnits_96:null,
                    GallonPriceUnit_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_96:null,
                    GallonAdditionalPrice_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_96:null,
                    NumberOfTips = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].NumberOfTips:null,
                    PriceQuotedForFirstTip = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].PriceQuotedForFirstTip:null,
                    BankerBoxes = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BankerBoxes:null,
                    FileBoxes = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].FileBoxes:null,
                    Bags = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Bags:null,
                    Cabinets = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Cabinets:null,
                    Skids = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Skids:null,
                    HardDrivers = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HardDrivers:null,
                    Media = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Media:null,
                    Other = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Other:null,
                    SellerComments = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].SellerComments:null,
                    HardDrive_Media_Other_Comment = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HardDrive_Media_Other_Comment:null,
                    BuildingContactFirstName = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingContactFirstName:null,
                    BuildingContactLastName = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingContactLastName:null,
                    BuildingContactPhone = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingContactPhone:null,
                    BuildingName = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingName:null,
                    BuildingStreet = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingStreet:null,
                    BuildingCity = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingCity:null,
                    BuildingState = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingState:null,
                    ZipCode = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ZipCode:null,
                }
            };
            return lstOpportunityData;
        }
        private List<SharpspringCustomFieldsDataModel> GetAllSharpspringCustomFields(string accountObjectID)
        {
            List<EZShredFieldLabelMappingDataModel> _eZShredFieldLabelMappingDataModel = _fieldLabelMapping.GetAllFieldLabels();//All Field Label from EZShredFieldLabelMapping Collection
            mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            var customFields = GetCustomFields(accountObjectID, account);//All Custom fields from Sharpspring

            List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel = new List<SharpspringCustomFieldsDataModel>();
            foreach (var item in _eZShredFieldLabelMappingDataModel)
            {
                ObjSharpspringCustomFieldsDataModel.Add(
                    new SharpspringCustomFieldsDataModel
                    {
                        SSSystemName = customFields.Where(e => e.Label == item.Label && e.Type.ToLower() == item.Type.ToLower()).Select(e => e.SSSystemName).FirstOrDefault(),
                        EZShredFieldName = item.EZShredFieldName,
                        Type = item.Type,
                        Label = item.Label,
                        Set = _eZShredFieldLabelMappingDataModel.Where(e => e.Label == item.Label && e.Type == item.Type).Select(e => e.Set).FirstOrDefault()
                    });
            }
            return ObjSharpspringCustomFieldsDataModel;
        }
        private List<SharpspringCustomFieldsDataModel> GetAllEZShredSSCustomFields(string accountObjectID)
        {
            List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel = new List<SharpspringCustomFieldsDataModel>();
            List<SSField> SSFields = _crmEZShredService.GetAllSSFields(accountObjectID);//All Field Label from EZShredDATA Collection
            foreach (var item in SSFields)
            {
                ObjSharpspringCustomFieldsDataModel.Add(
                    new SharpspringCustomFieldsDataModel
                    {
                        SSSystemName = item.SSSystemName,
                        EZShredFieldName = item.EZShredFieldName,
                        Type = item.Type,
                        Label = item.Label,
                        Set = item.Set
                    });
            }
            return ObjSharpspringCustomFieldsDataModel;
        }
        private List<SharpspringFieldLabelDataModel> GetCustomFields(string accountObjectID, mxtrAccount account)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
            return _apiSharpspringService.GetCustomFields();
        }
        private string CreateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, string accountObjectID, SharpspringLeadDataModel objSharpSpringDataModel, mxtrAccount account, string BuildingSet)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
            return _apiSharpspringService.CreateLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, objSharpSpringDataModel, BuildingSet);
        }
        private string UpdateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, string accountObjectID, SharpspringLeadDataModel objSharpSpringDataModel, mxtrAccount account, string BuildingSet)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
            return _apiSharpspringService.UpdateLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, objSharpSpringDataModel, BuildingSet);
        }
        private string UpdateLeadARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, string accountObjectID, long leadID, string arCustomerCode, mxtrAccount account)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
            return _apiSharpspringService.UpdateLeadARCustomerCode(ObjSharpspringCustomFieldsDataModel, leadID, arCustomerCode);
        }
        private string UpdateOpportunityARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, string accountObjectID, long opportunityId, string arCustomerCode, mxtrAccount account)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
            return _apiSharpspringService.UpdateOpportunitiesARCustomerCode(ObjSharpspringCustomFieldsDataModel, opportunityId, arCustomerCode);
        }
        private long OpportunityDealStagesOperations(string accountObjectID, string leadName, long leadId, EZShredDataModel objEZShredDataModel, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, mxtrAccount account)
        {
            long OpportunityId = 0;
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
            List<SharpspringDealStageDataModel> lstDealStages = _apiSharpspringService.GetAllDealStages();
            var dealStageData = lstDealStages.FirstOrDefault(x => x.DealStageName == GetDealStageName(objEZShredDataModel.Customer[0].PipelineStatus, account));
            if (dealStageData != null)
            {
                List<SharpspringOpportunityDataModel> lstOpportunityData = new List<SharpspringOpportunityDataModel>
                   {
                       new SharpspringOpportunityDataModel()
                       {
                           OwnerID=Convert.ToInt64(GetOwnerID(UserService.GetUserByUserObjectID(User.MxtrUserObjectID).Email)),// fixs for now
                           DealStageID=dealStageData.DealStageID,
                           OpportunityName=objEZShredDataModel.Customer[0].Company + " Opportunity",
                           IsActive=true,
                           Amount=objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Amount:0,
                           AdditionalTips = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].AdditionalTips:null,
                           JobQuantitySize = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].JobQuantitySize:"0",
                           ProposedDateOfService= objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedDateOfService:null,
                                   CertificateOfInsurance = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].CertificateOfInsurance:null,
                                   HoursOfBusiness = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HoursOfBusiness:null,
                                   Stairs = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Stairs:null,
                                   ProposedConsoleDeliveryDate = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedConsoleDeliveryDate:null,
                                   ProposedStartDate = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ProposedStartDate:null,
                                   ECUnits = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECUnits:null,
                                   ECPriceUnit = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECPriceUnit:null,
                                   ECAdditionalPrice = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ECAdditionalPrice:null,
                                   GallonUnits_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonUnits_64:null,
                                   GallonPriceUnit_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_64:null,
                                   GallonAdditionalPrice_64 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_64:null,
                                   GallonUnits_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonUnits_96:null,
                                   GallonPriceUnit_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonPriceUnit_96:null,
                                   GallonAdditionalPrice_96 = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].GallonAdditionalPrice_96:null,
                                   NumberOfTips = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].NumberOfTips:null,
                                   PriceQuotedForFirstTip = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].PriceQuotedForFirstTip:null,
                                   BankerBoxes = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BankerBoxes:null,
                                   FileBoxes = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].FileBoxes:null,
                                   Bags = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Bags:null,
                                   Cabinets = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Cabinets:null,
                                   Skids = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Skids:null,
                                   HardDrivers = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HardDrivers:null,
                                   Media = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Media:null,
                                   Other = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].Other:null,
                                   SellerComments = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].SellerComments:null,
                                   HardDrive_Media_Other_Comment = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].HardDrive_Media_Other_Comment:null,
                                   BuildingContactFirstName = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingContactFirstName:null,
                                   BuildingContactLastName = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingContactLastName:null,
                                   BuildingContactPhone = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingContactPhone:null,
                                   BuildingName = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingName:null,
                                   BuildingStreet = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingStreet:null,
                                   BuildingCity = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingCity:null,
                                   BuildingState = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].BuildingState:null,
                                   ZipCode = objEZShredDataModel.SSOpportunity[0]!=null?objEZShredDataModel.SSOpportunity[0].ZipCode:null,
                       }
                    };

                string opportunitiesResult = _apiSharpspringService.CreateOpportunitiesWithCustomFields(lstOpportunityData, ObjSharpspringCustomFieldsDataModel);
                SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(opportunitiesResult);
                if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                {
                    OpportunityId = ssResponse.result.creates[0].id;
                    _apiSharpspringService.CreateOpportunityLeads(ssResponse.result.creates[0].id, leadId);
                }
            }
            return OpportunityId;
        }
        private EZShredLeadMappingDataModel EZShredLeadMapping(string accountObjectID, EZShredLeadMappingDataModel objEZShredLeadMappingDataModel, mxtrAccount mxtrAccount)
        {
            //mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(accountObjectID);
            mxtrUser mxtrUser = UserService.GetUserByUserObjectID(User.MxtrUserObjectID);
            objEZShredLeadMappingDataModel.AccountObjectID = mxtrAccount.ObjectID;
            objEZShredLeadMappingDataModel.MxtrAccountID = mxtrAccount.MxtrAccountID;
            objEZShredLeadMappingDataModel.UserID = mxtrUser.ObjectID;
            objEZShredLeadMappingDataModel.MxtrUserID = mxtrUser.MxtrUserID;
            return _dbEZShredLeadMappingService.AddEZShredLeadMappingData(objEZShredLeadMappingDataModel);
        }
        private void UpdateEZshredLeadMappingData(EZShredLeadMappingDataModel objEZShredLeadMappingDataModel)
        {
            _dbEZShredLeadMappingService.UpdateEZShredLeadMappingData(objEZShredLeadMappingDataModel);
        }
        private void AddLeadBuilding(EZShredLeadMappingDataModel objEZShredLeadMappingDataModel, LeadBuildingDataModel objLeadBuilding, string BuildingSet)
        {
            _dbEZShredLeadMappingService.AddLeadBuilding(objEZShredLeadMappingDataModel, objLeadBuilding, BuildingSet);
        }
        private bool UpdateEZShredLeadMappingDataWithLeadID(EZShredLeadMappingDataModel objEZShredLeadMappingDataModel, string buildingSet)
        {
            return _dbEZShredLeadMappingService.UpdateEZShredLeadMappingDataWithLeadID(objEZShredLeadMappingDataModel, buildingSet);
        }
        private bool CheckSSCustomFieldAvailability(string accountObjectID, mxtrAccount mxtrAccount)
        {
            return GetCustomFields(accountObjectID, mxtrAccount).Count() == 0 ? false : true;
        }
        private SharpspringLeadDataModel EZShredSSFieldsMapping(EZShredDataModel objEZShredDataModel)
        {
            string Name = objEZShredDataModel.Customer[0].Contact.Trim();
            string BulidingName1 = null;
            if (!string.IsNullOrEmpty(objEZShredDataModel.Building[0].SiteContact1))
                BulidingName1 = objEZShredDataModel.Building[0].SiteContact1.Trim();
            string BulidingName2 = null;
            if (!string.IsNullOrEmpty(objEZShredDataModel.Building[0].SiteContact2))
                BulidingName2 = objEZShredDataModel.Building[0].SiteContact2;

            string[] EmailAddress = objEZShredDataModel.Customer[0].EmailAddress.Split(';');
            SharpspringLeadDataModel objSharpspringLeadDataModel = new SharpspringLeadDataModel()
            {
                LeadID = objEZShredDataModel.Customer[0].LeadID,
                FirstName = Name.Substring(0, Name.LastIndexOf(" ")),
                LastName = Name.Substring(Name.LastIndexOf(" ") + 1, Name.Length - Name.LastIndexOf(" ") - 1),
                CompanyName = objEZShredDataModel.Customer[0].Company,//For Native
                BillingCompanyName = objEZShredDataModel.Customer[0].Company,
                EmailAddress = EmailAddress[0],
                ARCustomerCode = objEZShredDataModel.Customer[0].ARCustomerCode,
                NumberOfEmployees = objEZShredDataModel.Customer[0].NumberOfEmployees,
                Attention = objEZShredDataModel.Customer[0].Attention,
                ReferralSourceID = objEZShredDataModel.Customer[0].ReferralSourceID,
                Notes = objEZShredDataModel.Customer[0].Notes,
                EmailInvoice = objEZShredDataModel.Customer[0].EmailInvoice,
                BillingZip = objEZShredDataModel.Customer[0].Zip,
                PhoneNumber = objEZShredDataModel.Customer[0].Phone,
                FaxNumber = objEZShredDataModel.Customer[0].Fax,
                BillingStreet = objEZShredDataModel.Customer[0].Street,
                BillingCity = objEZShredDataModel.Customer[0].City,
                BillingState = objEZShredDataModel.Customer[0].State,
                MobilePhoneNumber = objEZShredDataModel.Customer[0].Mobile,
                PipelineStatus = objEZShredDataModel.Customer[0].PipelineStatus,
                Suite = objEZShredDataModel.Customer[0].Suite,
                EmailCOD = objEZShredDataModel.Customer[0].EmailCOD,
                CustomerTypeID = objEZShredDataModel.Customer[0].CustomerTypeID,
                InvoiceTypeID = objEZShredDataModel.Customer[0].InvoiceTypeID,
                TermID = objEZShredDataModel.Customer[0].TermID,
                Datasource = objEZShredDataModel.Customer[0].DataSource,
                Email2 = EmailAddress.Count() == 2 || EmailAddress.Count() == 3 ? EmailAddress[1] : "",
                Email3 = EmailAddress.Count() == 3 ? EmailAddress[2] : "",
                BuildingCompanyName = objEZShredDataModel.Building[0].CompanyName,
                BuildingStreet = objEZShredDataModel.Building[0].Street,
                BuildingCity = objEZShredDataModel.Building[0].City,
                BuildingState = objEZShredDataModel.Building[0].State,
                BuildingZip = objEZShredDataModel.Building[0].Zip,
                SalesmanID = objEZShredDataModel.Building[0].SalesmanID,
                Directions = objEZShredDataModel.Building[0].Directions,
                RoutineInstructions = objEZShredDataModel.Building[0].RoutineInstructions,
                ScheduleFrequency = objEZShredDataModel.Building[0].ScheduleFrequency,
                ServiceTypeID = objEZShredDataModel.Building[0].ServiceTypeID,
                BuildingTypeID = objEZShredDataModel.Building[0].BuildingTypeID,
                BuildingSuite = objEZShredDataModel.Building[0].Suite,
                BuildingContactFirstName1 = BulidingName1 != null ? BulidingName1.Substring(0, BulidingName1.LastIndexOf(" ")) : string.Empty,
                BuildingContactLastName1 = BulidingName1 != null ? BulidingName1.Substring(BulidingName1.LastIndexOf(" ") + 1, BulidingName1.Length - BulidingName1.LastIndexOf(" ") - 1) : string.Empty,
                BuildingContactFirstName2 = BulidingName2 != null ? BulidingName2.Substring(0, BulidingName2.LastIndexOf(" ")) : string.Empty,
                BuildingContactLastName2 = BulidingName2 != null ? BulidingName2.Substring(BulidingName2.LastIndexOf(" ") + 1, BulidingName2.Length - BulidingName2.LastIndexOf(" ") - 1) : string.Empty,
                BuildingContactPhone1 = objEZShredDataModel.Building[0].Phone1,
                BuildingContactPhone2 = objEZShredDataModel.Building[0].Phone2,
                BillingContact = objEZShredDataModel.Customer[0].BillingContact,
                BillingContactExtension = objEZShredDataModel.Customer[0].BillingContactExtension,
                BillingContactPhone = objEZShredDataModel.Customer[0].BillingContactPhone,
                BillingCountryCode = objEZShredDataModel.Customer[0].BillingCountryCode == null ? objEZShredDataModel.Customer[0].BillingCountryCode : "US",
                TaxExempt = objEZShredDataModel.Building[0].TaxExempt,
                NumberOfBoxes = objEZShredDataModel.Customer[0].NumberOfBoxes,
                CompanyCountryCode = objEZShredDataModel.Building[0].CompanyCountryCode == null ? objEZShredDataModel.Building[0].CompanyCountryCode : "US",
                ServicesProfessionalType = objEZShredDataModel.Customer[0].ServicesProfessionalType,
                TravelTourismType = objEZShredDataModel.Customer[0].TravelTourismType,
            };
            return objSharpspringLeadDataModel;
        }
        private string CreateCustomerAPIRequestString(EZShredCustomerDataModel objEZShredCustomerDataModel)
        {
            return @"{""Request"":""{\""Request\"":\""AddCustomer\"",\""UserId\"": \""" + objEZShredCustomerDataModel.UserID + "\\\",\\\"tblCustomers\\\": [{\\\"Company\\\": \\\"" + objEZShredCustomerDataModel.Company + "\\\",\\\"Attention\\\": \\\"" + objEZShredCustomerDataModel.Attention + "\\\",\\\"Street\\\": \\\"" + objEZShredCustomerDataModel.Street + "\\\",\\\"City\\\": \\\"" + objEZShredCustomerDataModel.City + "\\\",\\\"State\\\": \\\"" + objEZShredCustomerDataModel.State + "\\\",\\\"Zip\\\": \\\"" + objEZShredCustomerDataModel.Zip + "\\\",\\\"Contact\\\": \\\"" + objEZShredCustomerDataModel.Contact + "\\\",\\\"Phone\\\": \\\"" + objEZShredCustomerDataModel.Phone + "\\\",\\\"Fax\\\": \\\"" + objEZShredCustomerDataModel.Fax + "\\\",\\\"CustomerTypeID\\\": \\\"" + objEZShredCustomerDataModel.CustomerTypeID + "\\\",\\\"InvoiceTypeID\\\": \\\"" + objEZShredCustomerDataModel.InvoiceTypeID + "\\\",\\\"EmailAddress\\\": \\\"" + objEZShredCustomerDataModel.EmailAddress + "\\\",\\\"EmailInvoice\\\": \\\"" + objEZShredCustomerDataModel.EmailInvoice + "\\\",\\\"EmailCOD\\\": \\\"" + objEZShredCustomerDataModel.EmailCOD + "\\\",\\\"ReferralsourceID\\\": \\\"" + objEZShredCustomerDataModel.ReferralSourceID + "\\\",\\\"TermID\\\": \\\"" + objEZShredCustomerDataModel.TermID + "\\\",\\\"Notes\\\": \\\"" + objEZShredCustomerDataModel.Notes + "\\\"}]\"}";
        }
        private string UpdateCustomerAPIRequestString(EZShredCustomerDataModel objEZShredCustomerDataModel)
        {
            return @"{""Request"":""{\""Request\"":\""UpdateCustomer\"",\""UserId\"": \""" + objEZShredCustomerDataModel.UserID + "\\\",\\\"CustomerID\\\":\\\"" + objEZShredCustomerDataModel.CustomerID + "\\\" ,\\\"tblCustomers\\\": [{\\\"Company\\\": \\\"" + objEZShredCustomerDataModel.Company + "\\\",\\\"Attention\\\": \\\"" + objEZShredCustomerDataModel.Attention + "\\\",\\\"Street\\\": \\\"" + objEZShredCustomerDataModel.Street + "\\\",\\\"City\\\": \\\"" + objEZShredCustomerDataModel.City + "\\\",\\\"State\\\": \\\"" + objEZShredCustomerDataModel.State + "\\\",\\\"Zip\\\": \\\"" + objEZShredCustomerDataModel.Zip + "\\\",\\\"Contact\\\": \\\"" + objEZShredCustomerDataModel.Contact + "\\\",\\\"Phone\\\": \\\"" + objEZShredCustomerDataModel.Phone + "\\\",\\\"Fax\\\": \\\"" + objEZShredCustomerDataModel.Fax + "\\\",\\\"CustomerTypeID\\\": \\\"" + objEZShredCustomerDataModel.CustomerTypeID + "\\\",\\\"InvoiceTypeID\\\": \\\"" + objEZShredCustomerDataModel.InvoiceTypeID + "\\\",\\\"EmailAddress\\\": \\\"" + objEZShredCustomerDataModel.EmailAddress + "\\\",\\\"EmailInvoice\\\": \\\"" + objEZShredCustomerDataModel.EmailInvoice + "\\\",\\\"EmailCOD\\\": \\\"" + objEZShredCustomerDataModel.EmailCOD + "\\\",\\\"ReferralsourceID\\\": \\\"" + objEZShredCustomerDataModel.ReferralSourceID + "\\\",\\\"TermID\\\": \\\"" + objEZShredCustomerDataModel.TermID + "\\\",\\\"Notes\\\": \\\"" + objEZShredCustomerDataModel.Notes + "\\\"}]\"}";
        }
        private string CreateBuildingAPIRequestString(EZShredBuildingDataModel objEZShredBuildingDataModel, string userId)
        {
            objEZShredBuildingDataModel.Stop = String.IsNullOrEmpty(objEZShredBuildingDataModel.Stop) ? "99" : objEZShredBuildingDataModel.Stop;
            objEZShredBuildingDataModel.RouteID = String.IsNullOrEmpty(objEZShredBuildingDataModel.RouteID) ? "0" : objEZShredBuildingDataModel.RouteID;
            objEZShredBuildingDataModel.SalesTaxRegionID = String.IsNullOrEmpty(objEZShredBuildingDataModel.SalesTaxRegionID) ? "1" : objEZShredBuildingDataModel.SalesTaxRegionID;
            return @"{""Request"":""{\""Request\"":\""AddBuilding\"",\""UserId\"": \""" + userId + "\\\",\\\"tblBuilding\\\": [{\\\"BuildingTypeID\\\": \\\"" + objEZShredBuildingDataModel.BuildingTypeID + "\\\",\\\"CustomerID\\\": \\\"" + objEZShredBuildingDataModel.CustomerID + "\\\",\\\"CompanyName\\\": \\\"" + objEZShredBuildingDataModel.CompanyName + "\\\",\\\"Street\\\": \\\"" + objEZShredBuildingDataModel.Street + "\\\",\\\"City\\\": \\\"" + objEZShredBuildingDataModel.City + "\\\",\\\"State\\\": \\\"" + objEZShredBuildingDataModel.State + "\\\",\\\"Zip\\\": \\\"" + objEZShredBuildingDataModel.Zip + "\\\",\\\"Phone1\\\": \\\"" + objEZShredBuildingDataModel.Phone1 + "\\\",\\\"Phone2\\\": \\\"" + objEZShredBuildingDataModel.Phone2 + "\\\",\\\"SalesmanID\\\": \\\"" + objEZShredBuildingDataModel.SalesmanID + "\\\",\\\"Directions\\\": \\\"" + objEZShredBuildingDataModel.Directions + "\\\",\\\"RoutineInstructions\\\": \\\"" + objEZShredBuildingDataModel.RoutineInstructions + "\\\",\\\"SiteContact1\\\": \\\"" + objEZShredBuildingDataModel.SiteContact1 + "\\\",\\\"SiteContact2\\\": \\\"" + objEZShredBuildingDataModel.SiteContact2 + "\\\",\\\"RouteID\\\": \\\"" + objEZShredBuildingDataModel.RouteID + "\\\",\\\"Stop\\\": \\\"" + objEZShredBuildingDataModel.Stop + "\\\",\\\"ScheduleFrequency\\\": \\\"" + objEZShredBuildingDataModel.ScheduleFrequency + "\\\",\\\"ServiceTypeID\\\": \\\"" + objEZShredBuildingDataModel.ServiceTypeID + "\\\",\\\"SalesTaxRegionID\\\": \\\"" + objEZShredBuildingDataModel.SalesTaxRegionID + "\\\",\\\"TimeTaken\\\": \\\"" + objEZShredBuildingDataModel.TimeTaken + "\\\"}]\"}";
        }
        private string UpdateBuildingAPIRequestString(EZShredBuildingDataModel objEZShredBuildingDataModel, string userId)
        {
            objEZShredBuildingDataModel.Stop = String.IsNullOrEmpty(objEZShredBuildingDataModel.Stop) ? "99" : objEZShredBuildingDataModel.Stop;
            objEZShredBuildingDataModel.RouteID = String.IsNullOrEmpty(objEZShredBuildingDataModel.RouteID) ? "0" : objEZShredBuildingDataModel.RouteID;
            objEZShredBuildingDataModel.SalesTaxRegionID = String.IsNullOrEmpty(objEZShredBuildingDataModel.SalesTaxRegionID) ? "1" : objEZShredBuildingDataModel.SalesTaxRegionID;
            return @"{""Request"":""{\""Request\"":\""UpdateBuilding\"",\""UserId\"": \""" + userId + "\\\",\\\"BuildingID\\\": \\\"" + objEZShredBuildingDataModel.BuildingID + "\\\",\\\"tblBuilding\\\": [{\\\"BuildingTypeID\\\": \\\"" + objEZShredBuildingDataModel.BuildingTypeID + "\\\",\\\"CustomerID\\\": \\\"" + objEZShredBuildingDataModel.CustomerID + "\\\",\\\"CompanyName\\\": \\\"" + objEZShredBuildingDataModel.CompanyName + "\\\",\\\"Street\\\": \\\"" + objEZShredBuildingDataModel.Street + "\\\",\\\"City\\\": \\\"" + objEZShredBuildingDataModel.City + "\\\",\\\"State\\\": \\\"" + objEZShredBuildingDataModel.State + "\\\",\\\"Zip\\\": \\\"" + objEZShredBuildingDataModel.Zip + "\\\",\\\"Phone1\\\": \\\"" + objEZShredBuildingDataModel.Phone1 + "\\\",\\\"Phone2\\\": \\\"" + objEZShredBuildingDataModel.Phone2 + "\\\",\\\"SalesmanID\\\": \\\"" + objEZShredBuildingDataModel.SalesmanID + "\\\",\\\"Directions\\\": \\\"" + objEZShredBuildingDataModel.Directions + "\\\",\\\"RoutineInstructions\\\": \\\"" + objEZShredBuildingDataModel.RoutineInstructions + "\\\",\\\"SiteContact1\\\": \\\"" + objEZShredBuildingDataModel.SiteContact1 + "\\\",\\\"SiteContact2\\\": \\\"" + objEZShredBuildingDataModel.SiteContact2 + "\\\",\\\"RouteID\\\": \\\"" + objEZShredBuildingDataModel.RouteID + "\\\",\\\"Stop\\\": \\\"" + objEZShredBuildingDataModel.Stop + "\\\",\\\"ScheduleFrequency\\\": \\\"" + objEZShredBuildingDataModel.ScheduleFrequency + "\\\",\\\"ServiceTypeID\\\": \\\"" + objEZShredBuildingDataModel.ServiceTypeID + "\\\",\\\"SalesTaxRegionID\\\": \\\"" + objEZShredBuildingDataModel.SalesTaxRegionID + "\\\",\\\"TimeTaken\\\": \\\"" + objEZShredBuildingDataModel.TimeTaken + "\\\"}]\"}";
        }
        private string CustomerAPIRequest(EZShredCustomerDataModel objEZShredCustomerDataModel, string APIRequestString)
        {
            return @"{'Request':'" + APIRequestString + "','UserId':'" + objEZShredCustomerDataModel.UserID + "','CustomerData': [{'Company':'" + objEZShredCustomerDataModel.Company + "','Attention':'" + objEZShredCustomerDataModel.Attention + "','Street':'" + objEZShredCustomerDataModel.Street + "','City':'" + objEZShredCustomerDataModel.City + "','State':'" + objEZShredCustomerDataModel.State + "','Zip':'" + objEZShredCustomerDataModel.Zip + "','Contact':'" + objEZShredCustomerDataModel.Contact + "','Phone':'" + objEZShredCustomerDataModel.Phone + "','Fax':'" + objEZShredCustomerDataModel.Fax + "','CustomerTypeID':'" + objEZShredCustomerDataModel.CustomerTypeID + "','InvoiceTypeID':'" + objEZShredCustomerDataModel.InvoiceTypeID + "','EmailAddress':'" + objEZShredCustomerDataModel.EmailAddress + "','EmailInvoice':'" + objEZShredCustomerDataModel.EmailInvoice + "','EmailCOD':'" + objEZShredCustomerDataModel.EmailCOD + "','ReferralsourceID':'" + objEZShredCustomerDataModel.ReferralSourceID + "','TermID':'" + objEZShredCustomerDataModel.TermID + "','Notes':'" + objEZShredCustomerDataModel.Notes + "','CustomerId':'" + objEZShredCustomerDataModel.CustomerID + "'}]}";
        }
        private string BuildingAPIRequest(EZShredBuildingDataModel objEZShredBuildingDataModel, string APIRequestString, string userId, string customerId)
        {
            objEZShredBuildingDataModel.Stop = String.IsNullOrEmpty(objEZShredBuildingDataModel.Stop) ? "99" : objEZShredBuildingDataModel.Stop;
            objEZShredBuildingDataModel.RouteID = String.IsNullOrEmpty(objEZShredBuildingDataModel.RouteID) ? "0" : objEZShredBuildingDataModel.RouteID;
            objEZShredBuildingDataModel.SalesTaxRegionID = String.IsNullOrEmpty(objEZShredBuildingDataModel.SalesTaxRegionID) ? "1" : objEZShredBuildingDataModel.SalesTaxRegionID;
            return @"{'Request':'" + APIRequestString + "','UserId':'" + userId + "','BuildingData': [{'BuildingTypeID':'" + objEZShredBuildingDataModel.BuildingTypeID + "','CustomerID':'" + customerId + "','CompanyName':'" + objEZShredBuildingDataModel.CompanyName + "','Street':'" + objEZShredBuildingDataModel.Street + "','City':'" + objEZShredBuildingDataModel.City + "','State':'" + objEZShredBuildingDataModel.State + "','Zip':'" + objEZShredBuildingDataModel.Zip + "','Phone1':'" + objEZShredBuildingDataModel.Phone1 + "','Phone2':'" + objEZShredBuildingDataModel.Phone2 + "','SalesmanID':'" + objEZShredBuildingDataModel.SalesmanID + "','Directions':'" + objEZShredBuildingDataModel.Directions + "','RoutineInstructions':'" + objEZShredBuildingDataModel.RoutineInstructions + "','SiteContact1':'" + objEZShredBuildingDataModel.SiteContact1 + "','SiteContact2':'" + objEZShredBuildingDataModel.SiteContact2 + "','RouteID':'" + objEZShredBuildingDataModel.RouteID + "','Stop':'" + objEZShredBuildingDataModel.Stop + "','ScheduleFrequency':'" + objEZShredBuildingDataModel.ScheduleFrequency + "','ServiceTypeID':'" + objEZShredBuildingDataModel.ServiceTypeID + "','SalesTaxRegionID':'" + objEZShredBuildingDataModel.SalesTaxRegionID + "','TimeTaken':'" + objEZShredBuildingDataModel.TimeTaken + "','BuildingID':'" + objEZShredBuildingDataModel.BuildingID + "'}]}";
        }
        private AddUpdateCustomerResult CreateCustomersToEZShred(string accountObjectID, EZShredDataModel objEZShredDataModel, mxtrAccount account)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiEZShredService.SetConnectionTokens(account.EZShredIP, account.EZShredPort);
            string jsonRequestString = CreateCustomerAPIRequestString(objEZShredDataModel.Customer[0]);
            return _apiEZShredService.CreateUpdateEZShredCustomerWithRequestString(jsonRequestString);
        }
        private AddUpdateCustomerResult UpdateCustomersToEZShred(string accountObjectID, EZShredDataModel objEZShredDataModel, mxtrAccount account)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiEZShredService.SetConnectionTokens(account.EZShredIP, account.EZShredPort);
            string jsonRequestString = UpdateCustomerAPIRequestString(objEZShredDataModel.Customer[0]);
            return _apiEZShredService.CreateUpdateEZShredCustomerWithRequestString(jsonRequestString);
        }
        private AddUpdateBuildingResult CreateBuildingToEZShred(string accountObjectID, EZShredDataModel objEZShredDataModel, mxtrAccount account)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiEZShredService.SetConnectionTokens(account.EZShredIP, account.EZShredPort);
            string jsonRequestString = CreateBuildingAPIRequestString(objEZShredDataModel.Building[0], objEZShredDataModel.Customer[0].UserID);
            return _apiEZShredService.CreateUpdateEZShredBuildingWithRequestString(jsonRequestString);
        }
        private AddUpdateBuildingResult UpdateBuildingToEZShred(string accountObjectID, EZShredDataModel objEZShredDataModel, mxtrAccount account)
        {
            //mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
            _apiEZShredService.SetConnectionTokens(account.EZShredIP, account.EZShredPort);
            string jsonRequestString = UpdateBuildingAPIRequestString(objEZShredDataModel.Building[0], objEZShredDataModel.Customer[0].UserID);
            return _apiEZShredService.CreateUpdateEZShredBuildingWithRequestString(jsonRequestString);
        }
        private bool UpdateCustomerAndBuildingToEZShred(string accountObjectID, EZShredDataModel objEZShredDataModel, mxtrAccount account, string PipelineStatus)
        {
            bool ResultStatus = false;
            //Update Customer In EZShred
            AddUpdateCustomerResult objAddUpdateCustomerResult = UpdateCustomersToEZShred(accountObjectID, objEZShredDataModel, account);
            if (objAddUpdateCustomerResult != null)
            {
                if (objAddUpdateCustomerResult.status == "OK")
                    ResultStatus = true;
            }
            else
                ResultStatus = false;

            //update Building In EZShred
            //if (PipelineStatus == PipelineKind.Scheduled.ToString())
            if (Convert.ToInt32(objEZShredDataModel.Building[0].BuildingID) > 0)
            {
                AddUpdateBuildingResult objAddUpdateBuildingResult = UpdateBuildingToEZShred(accountObjectID, objEZShredDataModel, account);
                if (objAddUpdateBuildingResult != null)
                {
                    if (objAddUpdateBuildingResult.status == "OK")
                    {
                        ResultStatus = true;
                    }
                    else
                    {
                        ResultStatus = false;
                    }
                }
                else
                    ResultStatus = false;
            }
            //Update Customer & Building In EZShred DB
            //_crmEZShredService.UpdateCustomerAndBuilding(objEZShredDataModel);
            objEZShredDataModel.Customer[0].AccountObjectId = objEZShredDataModel.AccountObjectId;
            objEZShredDataModel.Customer[0].MxtrAccountId = objEZShredDataModel.MxtrAccountId;
            _dbEZShredCustomerService.AddUpdateCustomerData(objEZShredDataModel.Customer[0]);
            //if (PipelineStatus == PipelineKind.Scheduled.ToString())
            if (Convert.ToInt32(objEZShredDataModel.Building[0].BuildingID) > 0)
            {
                objEZShredDataModel.Building[0].AccountObjectId = objEZShredDataModel.AccountObjectId;
                objEZShredDataModel.Building[0].MxtrAccountId = objEZShredDataModel.MxtrAccountId;
                _dbEZShredBuildingService.AddUpdateBuildingData(objEZShredDataModel.Building[0]);
            }
            return ResultStatus;
        }
        private Tuple<bool, string, string, string> AddCustomerAndBuildingToEZShred(string accountObjectID, EZShredDataModel objEZShredDataModel, EZShredLeadMappingDataModel objEZShredLeadMappingDataModel, mxtrAccount account, string buildingSet)
        {
            bool ResultStatus = false;
            objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.InProgress.ToString();//In progress
            //Add Customer In EZShred
            AddUpdateCustomerResult objAddUpdateCustomerResult = CreateCustomersToEZShred(accountObjectID, objEZShredDataModel, account);
            if (objAddUpdateCustomerResult != null && objAddUpdateCustomerResult.status == "OK")
            {
                objEZShredLeadMappingDataModel.CustomerID = Convert.ToInt32(objAddUpdateCustomerResult.CustomerID);
                objEZShredLeadMappingDataModel.EZShredApiRequest = CustomerAPIRequest(objEZShredDataModel.Customer[0], CreateCustomerAPIRequestString(objEZShredDataModel.Customer[0]));
                objEZShredDataModel.Customer[0].CustomerID = objAddUpdateCustomerResult.CustomerID;
                objEZShredDataModel.Building[0].CustomerID = objAddUpdateCustomerResult.CustomerID;
                objEZShredLeadMappingDataModel.EZShredBuildingApiRequest = BuildingAPIRequest(objEZShredDataModel.Building[0], CreateBuildingAPIRequestString(objEZShredDataModel.Building[0], objEZShredDataModel.Customer[0].UserID), objEZShredDataModel.Customer[0].UserID, objEZShredDataModel.Customer[0].CustomerID);
                //Add Building In EZShred
                AddUpdateBuildingResult objAddUpdateBuildingResult = CreateBuildingToEZShred(accountObjectID, objEZShredDataModel, account);
                if (objAddUpdateBuildingResult != null && objAddUpdateBuildingResult.status == "OK")
                {
                    objEZShredDataModel.Building[0].BuildingID = objAddUpdateBuildingResult.BuildingId;
                    objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.NoAction.ToString();
                    objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Complete.ToString();//Complete
                    UpdateEZshredLeadMappingData(objEZShredLeadMappingDataModel);
                    AddLeadBuilding(objEZShredLeadMappingDataModel, AdaptLeadBuilding(objEZShredDataModel, objEZShredLeadMappingDataModel), buildingSet);
                    //Add Customer & Building In EZShred DB
                    //_crmEZShredService.AddCustomerAndBuilding(objEZShredDataModel);
                    //Add Customer to corresponding tables in mxtr database
                    objEZShredDataModel.Customer[0].AccountObjectId = objEZShredDataModel.AccountObjectId;
                    objEZShredDataModel.Customer[0].MxtrAccountId = objEZShredDataModel.MxtrAccountId;
                    _dbEZShredCustomerService.AddUpdateCustomerData(objEZShredDataModel.Customer[0]);
                    //Add Building to tables in mxtr database
                    objEZShredDataModel.Building[0].AccountObjectId = objEZShredDataModel.AccountObjectId;
                    objEZShredDataModel.Building[0].MxtrAccountId = objEZShredDataModel.MxtrAccountId;
                    _dbEZShredBuildingService.AddUpdateBuildingData(objEZShredDataModel.Building[0]);
                    //AddCustomerToDatabase(objEZShredDataModel.Customer[0]); 
                    //AddBuildingToDatabase(objEZShredDataModel.Building[0]);
                    ResultStatus = true;
                }
                else
                {
                    objEZShredLeadMappingDataModel.CustomerID = Convert.ToInt32(objAddUpdateCustomerResult.CustomerID);
                    objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.Create.ToString();
                    objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Failed.ToString();//Failed
                    UpdateEZshredLeadMappingData(objEZShredLeadMappingDataModel);
                    AddLeadBuilding(objEZShredLeadMappingDataModel, AdaptLeadBuilding(objEZShredDataModel, objEZShredLeadMappingDataModel), buildingSet);
                    ResultStatus = false;
                }
            }
            else
            {
                objEZShredLeadMappingDataModel.CustomerID = null;
                objEZShredLeadMappingDataModel.EZShredApiRequest = CustomerAPIRequest(objEZShredDataModel.Customer[0], CreateCustomerAPIRequestString(objEZShredDataModel.Customer[0]));
                objEZShredLeadMappingDataModel.EZShredBuildingApiRequest = BuildingAPIRequest(objEZShredDataModel.Building[0], CreateBuildingAPIRequestString(objEZShredDataModel.Building[0], objEZShredDataModel.Customer[0].UserID), objEZShredDataModel.Customer[0].UserID, objEZShredDataModel.Customer[0].CustomerID);
                objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.Create.ToString();
                objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Failed.ToString();//Failed
                UpdateEZshredLeadMappingData(objEZShredLeadMappingDataModel);
                AddLeadBuilding(objEZShredLeadMappingDataModel, AdaptLeadBuilding(objEZShredDataModel, objEZShredLeadMappingDataModel), buildingSet);
                ResultStatus = false;
            }
            return new Tuple<bool, string, string, string>(
                ResultStatus,
                objAddUpdateCustomerResult == null ? string.Empty : objAddUpdateCustomerResult.CustomerID,
                objEZShredLeadMappingDataModel.EZShredActionType,
                objEZShredLeadMappingDataModel.EZShredStatus
                );
        }

        private string GetDealStageNameByOpportunityId(long opportunityId)
        {
            List<SharpspringDealStageDataModel> lstDealStages = _apiSharpspringService.GetAllDealStages();
            SharpspringDealStageDataModel dealStage = new SharpspringDealStageDataModel();
            //List<SharpspringOpportunityDataModel> lstOpportunities = _apiSharpspringService.GetOpportunitiesById(opportunityId);
            //if (lstOpportunities != null && lstOpportunities.Count > 0)
            //{
            //    dealStage = lstDealStages.FirstOrDefault(x => x.DealStageID == lstOpportunities[0].DealStageID);
            //}

            long dealStageId = _apiSharpspringService.GetDealStageIdByOpportunityId(opportunityId);
            if (dealStageId > 0 && lstDealStages != null && lstDealStages.Count > 0)
            {
                return lstDealStages.FirstOrDefault(x => x.DealStageID == dealStageId).DealStageName;
            }
            return dealStage.DealStageName;
        }

        private static EZShredDataModel AdaptSharpSpringData(SharpspringLeadDataModel customerDataFromSS, string mxtrAccountId, string accountObjectId)
        {
            EZShredDataModel EZShredDataModel = new EZShredDataModel()
            {
                Customer = new List<EZShredCustomerDataModel>()
                {
                   new EZShredCustomerDataModel()
                        {
                             MxtrAccountId = mxtrAccountId,
                             AccountObjectId = accountObjectId,
                             Contact=customerDataFromSS.FirstName+" "+customerDataFromSS.LastName,
                             //Company=customerDataFromSS.BillingCompanyName,
                             Company=customerDataFromSS.CompanyName,
                             EmailAddress= customerDataFromSS.EmailAddress+";"+customerDataFromSS.Email2+";"+customerDataFromSS.Email3,
                             ARCustomerCode= customerDataFromSS.ARCustomerCode,
                             NumberOfEmployees= customerDataFromSS.NumberOfEmployees,
                             Attention= customerDataFromSS.Attention,
                             ReferralSourceID= customerDataFromSS.ReferralSourceID,
                             Notes= customerDataFromSS.Notes,
                             EmailInvoice= customerDataFromSS.EmailInvoice,
                             Zip= customerDataFromSS.BillingZip,
                             Phone= customerDataFromSS.PhoneNumber,
                             Fax= customerDataFromSS.FaxNumber,
                             Street= customerDataFromSS.BillingStreet + ",\r\nSuite " + customerDataFromSS.Suite,//Append Street & Suite for Client Side retrive
                             City= customerDataFromSS.BillingCity,
                             State= customerDataFromSS.BillingState,
                             Mobile= customerDataFromSS.MobilePhoneNumber,
                             PipelineStatus= customerDataFromSS.PipelineStatus,
                             Suite= customerDataFromSS.Suite,
                             EmailCOD= customerDataFromSS.EmailCOD,
                             CustomerTypeID= customerDataFromSS.CustomerTypeID,
                             InvoiceTypeID= customerDataFromSS.InvoiceTypeID,
                             TermID= customerDataFromSS.TermID,
                             DataSource= customerDataFromSS.Datasource,
                             BillingContact=customerDataFromSS.BillingContact,
                             BillingContactPhone=customerDataFromSS.BillingContactPhone,
                             BillingContactExtension=customerDataFromSS.BillingContactExtension,
                             BillingCountryCode=customerDataFromSS.BillingCountryCode,
                             NumberOfBoxes=customerDataFromSS.NumberOfBoxes,
                             ServicesProfessionalType=customerDataFromSS.ServicesProfessionalType,
                             TravelTourismType=customerDataFromSS.TravelTourismType
                        },
                },
                Building = new List<EZShredBuildingDataModel>()
                {
                    new EZShredBuildingDataModel() {
                            MxtrAccountId = mxtrAccountId,
                            AccountObjectId = accountObjectId,
                            CompanyName = customerDataFromSS.BuildingCompanyName,
                            Street = customerDataFromSS.BuildingStreet + ",\r\nSuite "+ customerDataFromSS.BuildingSuite,//Append Street & Suite for Client Side
                            City = customerDataFromSS.BuildingCity,
                            State = customerDataFromSS.BuildingState,
                            Zip = customerDataFromSS.BuildingZip,
                            SalesmanID = customerDataFromSS.SalesmanID,
                            Directions = customerDataFromSS.Directions,
                            RoutineInstructions = customerDataFromSS.RoutineInstructions,
                            ScheduleFrequency = customerDataFromSS.ScheduleFrequency,
                            ServiceTypeID = customerDataFromSS.ServiceTypeID,
                            RouteID = customerDataFromSS.RouteID,
                            BuildingTypeID = customerDataFromSS.BuildingTypeID,
                            Suite=customerDataFromSS.BuildingSuite,
                            SiteContact1=customerDataFromSS.BuildingContactFirstName1+ " "+customerDataFromSS.BuildingContactLastName1,
                            SiteContact2=customerDataFromSS.BuildingContactFirstName2+ " "+customerDataFromSS.BuildingContactLastName2,
                            Phone1=customerDataFromSS.BuildingContactPhone1,
                            Phone2=customerDataFromSS.BuildingContactPhone2,
                            CompanyCountryCode=customerDataFromSS.CompanyCountryCode,
                            TaxExempt=customerDataFromSS.TaxExempt,
                        },
                },
            };
            return EZShredDataModel;
        }

        private EZShredLeadMappingStatus AddUpdateEZData(string accountObjectID, EZShredDataModel objEZShredDataModel, EZShredLeadMappingDataModel objEZShredLeadMappingDataModel, mxtrAccount account, string pipelineStatus, string buildingSet)
        {
            bool EzshredStatus;
            string eZShredActionType = string.Empty;
            string eZShredStatus = string.Empty;
            objEZShredDataModel = AppendSuiteWithStreetAddress(objEZShredDataModel);
            if (String.IsNullOrEmpty(objEZShredDataModel.Customer[0].CustomerID))
            {
                //Add customer in EZShred
                EZShredLeadMappingDataModel LeadData = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, objEZShredDataModel.Customer[0].LeadID);
                objEZShredLeadMappingDataModel.Id = LeadData.Id;
                Tuple<bool, string, string, string> result = AddCustomerAndBuildingToEZShred(accountObjectID, objEZShredDataModel, objEZShredLeadMappingDataModel, account, buildingSet);
                EzshredStatus = result.Item1;
                eZShredActionType = result.Item3;
                eZShredStatus = result.Item4;
            }
            else
            {
                //Update customer in EZSherd
                EzshredStatus = UpdateCustomerAndBuildingToEZShred(accountObjectID, objEZShredDataModel, account, pipelineStatus);
                if (EzshredStatus)
                {
                    objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.NoAction.ToString();
                    objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Complete.ToString();
                    eZShredActionType = EZShredActionTypeKind.NoAction.ToString();
                    eZShredStatus = EZShredStatusKind.Complete.ToString();
                }
                else
                {
                    objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.Update.ToString();
                    objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Failed.ToString();
                    eZShredActionType = EZShredActionTypeKind.Update.ToString();
                    eZShredStatus = EZShredStatusKind.Failed.ToString();
                }
                //update data in EZShred lead mapping table
                objEZShredLeadMappingDataModel.EZShredApiRequest = CustomerAPIRequest(objEZShredDataModel.Customer[0], CreateCustomerAPIRequestString(objEZShredDataModel.Customer[0]));
                objEZShredLeadMappingDataModel.EZShredBuildingApiRequest = BuildingAPIRequest(objEZShredDataModel.Building[0], CreateBuildingAPIRequestString(objEZShredDataModel.Building[0], objEZShredDataModel.Customer[0].UserID), objEZShredDataModel.Customer[0].UserID, objEZShredDataModel.Customer[0].CustomerID);
                _dbEZShredLeadMappingService.UpdateEZShredLeadMappingData(objEZShredLeadMappingDataModel);
            }
            return new EZShredLeadMappingStatus()
            {
                EZShredActionType = eZShredActionType,
                EZShredStatus = eZShredStatus,
                Status = EzshredStatus,
            };
        }
        private string GetDealStageName(string pipeLineStatus, mxtrAccount account)
        {
            string dealStage = string.Empty;
            switch ((PipelineKind)Enum.Parse(typeof(PipelineKind), pipeLineStatus))
            {
                case PipelineKind.Lead:
                    dealStage = account.Lead;
                    break;
                case PipelineKind.Contact:
                    dealStage = account.ContactMade;
                    break;
                case PipelineKind.QuoteSent:
                    dealStage = account.ProposalSent;
                    break;
                case PipelineKind.Scheduled:
                    dealStage = account.WonNotScheduled;
                    break;
                default:
                    dealStage = account.Lead;
                    break;
            }
            return dealStage;
        }
        private string MapDealStageName(string pipeLineStatus, mxtrAccount account)
        {
            string dealStage = string.Empty;
            if (pipeLineStatus == account.Lead)
            {
                dealStage = PipelineKind.Lead.ToString();
            }
            else if (pipeLineStatus == account.ContactMade)
            {
                dealStage = PipelineKind.Contact.ToString();
            }
            else if (pipeLineStatus == account.ProposalSent)
            {
                dealStage = PipelineKind.QuoteSent.ToString();
            }
            else if (pipeLineStatus == account.WonNotScheduled)
            {
                dealStage = PipelineKind.Scheduled.ToString();
            }
            else
            {
                dealStage = PipelineKind.Lead.ToString();
            }
            return dealStage;
        }
        [Route("ValidateEmaildAddressForLead")]
        public ActionResult ValidateEmaildAddressForLead(string EmailAddress, string accountObjectID)
        {
            if (!string.IsNullOrEmpty(EmailAddress))
            {
                SharpspringLeadDataModel objSharpspringLeadDataModel = _apiSharpspringService.GetLead(EmailAddress);
                if (objSharpspringLeadDataModel != null)
                {
                    if (SearchLeadInEZShredLeadMapping(objSharpspringLeadDataModel.LeadID, accountObjectID))
                    {
                        var response = new { Data = objSharpspringLeadDataModel, IsLeadInEZShredLeadMapping = true, Status = true };
                        return Json(response);
                    }
                    else
                    {
                        var response = new { Data = objSharpspringLeadDataModel, IsLeadInEZShredLeadMapping = false, Status = true };
                        return Json(response);
                    }
                }
            }
            return Json(new { Status = false });
        }
        [Route("GetAvailableDates")]
        public ActionResult GetAvailableDates(string userId, string zipCode, string accountObjectID)
        {
            ZIPDataModel response = new ZIPDataModel();
            if (!string.IsNullOrEmpty(zipCode))
            {
                mxtrAccount account = _accountService.GetAccountByAccountObjectID(accountObjectID);
                _apiEZShredService.SetConnectionTokens(account.EZShredIP, account.EZShredPort);
                response = _apiEZShredService.GetAvailableDates(userId, zipCode);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private bool SearchLeadInEZShredLeadMapping(long LeadID, string accountObjectID)
        {
            EZShredLeadMappingDataModel ObjEZShredLeadMappingDataModel = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, LeadID);
            if (ObjEZShredLeadMappingDataModel.Id != null)
                return true;
            else
                return false;
        }
        private string GetOwnerID(string EmailAddress)
        {
            string OwnerID = string.Empty;
            List<SharpspringUserProfileDataModel> SharpspringUserProfileDataModel = _apiSharpspringService.GetUserProfiles();
            if (SharpspringUserProfileDataModel.Count() > 0)
            {
                OwnerID = SharpspringUserProfileDataModel.Where(x => x.EmailAddress == EmailAddress).Select(x => x.OwnerID).FirstOrDefault();
                if (string.IsNullOrEmpty(OwnerID))
                    OwnerID = SharpspringUserProfileDataModel.Select(x => x.OwnerID).FirstOrDefault();
            }
            return OwnerID;
        }
        private LeadBuildingDataModel AdaptLeadBuilding(EZShredDataModel objEZShredDataModel, EZShredLeadMappingDataModel objEZShredLeadMappingDataModel)
        {
            LeadBuildingDataModel objLeadBuilding = new LeadBuildingDataModel()
            {
                BuildingID = objEZShredDataModel.Building[0].BuildingID != null ? Convert.ToInt32(objEZShredDataModel.Building[0].BuildingID) : 0,
                BuildingCompanyName = objEZShredDataModel.Building[0].CompanyName,
                OpportunityID = objEZShredLeadMappingDataModel.OpportunityID,
                EZShredBuildingApiRequest = objEZShredLeadMappingDataModel.EZShredBuildingApiRequest,
                EZShredActionType = objEZShredLeadMappingDataModel.EZShredActionType,
                EZShredStatus = objEZShredLeadMappingDataModel.EZShredStatus,
                Street = objEZShredDataModel.Building[0].Street,
                City = objEZShredDataModel.Building[0].City,
                State = objEZShredDataModel.Building[0].State,
                ZIP = objEZShredDataModel.Building[0].Zip,
                Country = objEZShredDataModel.Building[0].CompanyCountryCode,
            };
            return objLeadBuilding;
        }
        public string GetNextBuildingStage(EZShredLeadMappingDataModel ObjEZShredLeadMappingDataModel)
        {
            string BuildingStage = string.Empty;
            if (string.IsNullOrEmpty(ObjEZShredLeadMappingDataModel.BuildingStage))
                BuildingStage = LeadBuildingSet.Building1.ToString();
            else if (ObjEZShredLeadMappingDataModel.BuildingStage == LeadBuildingSet.Building1.ToString())
                BuildingStage = LeadBuildingSet.Building2.ToString();
            else if (ObjEZShredLeadMappingDataModel.BuildingStage == LeadBuildingSet.Building2.ToString())
                BuildingStage = LeadBuildingSet.Building3.ToString();
            else if (ObjEZShredLeadMappingDataModel.BuildingStage == LeadBuildingSet.Building3.ToString())
                BuildingStage = LeadBuildingSet.Building4.ToString();
            else if (ObjEZShredLeadMappingDataModel.BuildingStage == LeadBuildingSet.Building4.ToString())
                BuildingStage = LeadBuildingSet.Building5.ToString();
            else if (ObjEZShredLeadMappingDataModel.BuildingStage == LeadBuildingSet.Building5.ToString())
                BuildingStage = LeadBuildingSet.Complete.ToString();

            return BuildingStage;
        }
        private string GetBuildingSetByOpportunityID(string accountObjectID, long LeadID, long OpportunityID)
        {
            string BuildingSet = LeadBuildingSet.Building1.ToString();
            if (LeadID > 0 && OpportunityID > 0)
            {
                EZShredLeadMappingDataModel ObjEZShredLeadMappingDataModel = _dbEZShredLeadMappingService.GetEZShredLeadDataByLeadID(accountObjectID, LeadID);

                if (ObjEZShredLeadMappingDataModel.Building1.OpportunityID == OpportunityID)
                    BuildingSet = LeadBuildingSet.Building1.ToString();
                else if (ObjEZShredLeadMappingDataModel.Building2.OpportunityID == OpportunityID)
                    BuildingSet = LeadBuildingSet.Building2.ToString();
                else if (ObjEZShredLeadMappingDataModel.Building3.OpportunityID == OpportunityID)
                    BuildingSet = LeadBuildingSet.Building3.ToString();
                else if (ObjEZShredLeadMappingDataModel.Building4.OpportunityID == OpportunityID)
                    BuildingSet = LeadBuildingSet.Building4.ToString();
                else if (ObjEZShredLeadMappingDataModel.Building5.OpportunityID == OpportunityID)
                    BuildingSet = LeadBuildingSet.Building5.ToString();
            }
            return BuildingSet;
        }
        private EZShredDataModel AddNewBuildingToEZShred(string accountObjectID, string customerId, EZShredLeadMappingDataModel objEZShredLeadMappingDataModel, EZShredDataModel objEZShredDataModel, mxtrAccount account, string buildingSet)
        {
            objEZShredDataModel = AppendSuiteWithStreetAddress(objEZShredDataModel);
            objEZShredLeadMappingDataModel.EZShredBuildingApiRequest = BuildingAPIRequest(objEZShredDataModel.Building[0], CreateBuildingAPIRequestString(objEZShredDataModel.Building[0], objEZShredDataModel.Customer[0].UserID), objEZShredDataModel.Customer[0].UserID, objEZShredDataModel.Customer[0].CustomerID);
            //Add Building In EZShred
            AddUpdateBuildingResult objAddUpdateBuildingResult = CreateBuildingToEZShred(accountObjectID, objEZShredDataModel, account);
            if (objAddUpdateBuildingResult != null && objAddUpdateBuildingResult.status == "OK")
            {
                objEZShredDataModel.Building[0].BuildingID = objAddUpdateBuildingResult.BuildingId;
                objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.NoAction.ToString();
                objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Complete.ToString();//Complete
                UpdateEZshredLeadMappingData(objEZShredLeadMappingDataModel);
                AddLeadBuilding(objEZShredLeadMappingDataModel, AdaptLeadBuilding(objEZShredDataModel, objEZShredLeadMappingDataModel), buildingSet);
                //Add Customer to corresponding tables in mxtr database
                objEZShredDataModel.Customer[0].AccountObjectId = objEZShredDataModel.AccountObjectId;
                objEZShredDataModel.Customer[0].MxtrAccountId = objEZShredDataModel.MxtrAccountId;
                _dbEZShredCustomerService.AddUpdateCustomerData(objEZShredDataModel.Customer[0]);
                //Add Building to tables in mxtr database
                objEZShredDataModel.Building[0].AccountObjectId = objEZShredDataModel.AccountObjectId;
                objEZShredDataModel.Building[0].MxtrAccountId = objEZShredDataModel.MxtrAccountId;
                _dbEZShredBuildingService.AddUpdateBuildingData(objEZShredDataModel.Building[0]);
            }
            else
            {
                objEZShredLeadMappingDataModel.CustomerID = Convert.ToInt32(customerId);
                objEZShredLeadMappingDataModel.EZShredActionType = EZShredActionTypeKind.Create.ToString();
                objEZShredLeadMappingDataModel.EZShredStatus = EZShredStatusKind.Failed.ToString();//Failed
                UpdateEZshredLeadMappingData(objEZShredLeadMappingDataModel);
                AddLeadBuilding(objEZShredLeadMappingDataModel, AdaptLeadBuilding(objEZShredDataModel, objEZShredLeadMappingDataModel), GetBuildingSetByOpportunityID(accountObjectID, objEZShredLeadMappingDataModel.LeadID, objEZShredLeadMappingDataModel.OpportunityID));
            }
            return objEZShredDataModel;
        }

        private string GetNextServiceDate(string userId, string buildingId)
        {
            return _apiEZShredService.GetNextAvailableDate(String.IsNullOrEmpty(userId) ? 0 : Convert.ToInt32(userId), String.IsNullOrEmpty(buildingId) ? 0 : Convert.ToInt32(buildingId));
        }
        private EZShredDataModel AppendSuiteWithStreetAddress(EZShredDataModel objEZShredDataModel)
        {
            if (!string.IsNullOrEmpty(objEZShredDataModel.Customer[0].Suite))
                objEZShredDataModel.Customer[0].Street = objEZShredDataModel.Customer[0].Street + ",\r\nSuite " + objEZShredDataModel.Customer[0].Suite;

            if (!string.IsNullOrEmpty(objEZShredDataModel.Building[0].Suite))
                objEZShredDataModel.Building[0].Street = objEZShredDataModel.Building[0].Street + ",\r\nSuite " + objEZShredDataModel.Building[0].Suite;

            return objEZShredDataModel;
        }
    }
    public class EZShredLeadMappingStatus
    {
        public bool Status { get; set; }
        public string EZShredActionType { get; set; }
        public string EZShredStatus { get; set; }
    }
}
