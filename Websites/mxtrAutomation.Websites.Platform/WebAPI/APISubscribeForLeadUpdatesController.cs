using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Common.Enums;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Websites.Platform.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace mxtrAutomation.Websites.Platform.WebAPI
{
    [RoutePrefix("api/v1")]
    public class APISubscribeForLeadUpdatesController : ApiController
    {
        private readonly ICRMLeadService _crmLeadService = ServiceLocator.Current.GetInstance<ICRMLeadServiceInternal>();
        private readonly IAccountService _accountService = ServiceLocator.Current.GetInstance<IAccountServiceInternal>();
        private readonly ISharpspringService _apiSharpspringService = ServiceLocator.Current.GetInstance<ISharpspringService>();
        private readonly ICorporateSSLastRunService _corporateSSLastRunService = ServiceLocator.Current.GetInstance<ICorporateSSLastRunServiceInternal>();
        private readonly ISubscribeLeadUpdatesLogServices _subscribeLeadUpdatesLogServices = ServiceLocator.Current.GetInstance<ISubscribeLeadUpdatesLogServicesInternal>();
        private readonly IUserService _userService = ServiceLocator.Current.GetInstance<IUserService>();
        private readonly IEZShredLeadMappingService _dbEZShredLeadMappingService = ServiceLocator.Current.GetInstance<IEZShredLeadMappingService>();
        [HttpPost]
        [Route("subscribeToLeadUpdates")]
        public HttpResponseMessage SubscribeToLeadUpdates(string AccountObjectID, string RequestFrom)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string MxtrUserObjectID = "59fc1014ed1f281e8c68cd08";//Default User//PRODUCTION/Stage--59fc1014ed1f281e8c68cd08
            mxtrUser mxtrUser = _userService.GetUserByUserObjectID(MxtrUserObjectID);
            mxtrAccount account = _accountService.GetAccountByAccountObjectID(AccountObjectID);
            if (mxtrUser != null)
            {
                string IsLocationAssigned = mxtrUser.EZShredAccountMappings.Where(e => e.AccountObjectId == AccountObjectID).Select(e => e.EZShredId).FirstOrDefault();
                if (string.IsNullOrEmpty(IsLocationAssigned)) return GetRequestResponse("Location not assigned", HttpStatusCode.OK, DateTime.Now.ToString());

                //HttpContext.Current.Request.InputStream.Position = 0;
                //Get the Sharpspring POST Request Data
                var SSPostRequestString = new System.IO.StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                try
                {
                    SubscribeLeadUpdatesLogsDataModel slulDataModel = new SubscribeLeadUpdatesLogsDataModel()
                    {
                        AccountObjectID = AccountObjectID,
                        LocationName = account.AccountName,
                        Action = EZShredActionTypeKind.Create.ToString(),
                        Status = EZShredStatusKind.Complete.ToString(),
                        RequestFrom = RequestFrom,
                        SSPostRequestString = SSPostRequestString
                    };

                    List<SharpspringPostRequestString> ObjSharpspringPostRequestString = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SharpspringPostRequestString>>(SSPostRequestString);
                    //Leadmapping/Opportunity creation
                    Boolean IsdataWriteToEZShredLeadMapping = AdptStringRequestString(mxtrUser, account, ObjSharpspringPostRequestString, LeadBuildingSet.Building1.ToString());
                    //Suscribe data log mention
                    if (IsdataWriteToEZShredLeadMapping) _subscribeLeadUpdatesLogServices.AddSubscribeLeadUpdatesLog(slulDataModel);
                    return GetRequestResponse("success", HttpStatusCode.OK, DateTime.Now.ToString());
                }
                catch (Exception ex)
                {
                    response = GetRequestResponse(ex.Message, HttpStatusCode.ExpectationFailed, string.Empty);
                }
            }
            else
            {
                return GetRequestResponse("User not found", HttpStatusCode.OK, DateTime.Now.ToString());
            }
            return response;
        }
        private Boolean AdptStringRequestString(mxtrUser mxtrUser, mxtrAccount mxtrAccount, List<SharpspringPostRequestString> SSPostRequestString, string BuildingSet)
        {
            long OpportunityID = 0;
            Boolean IsdataWriteToEZShredLeadMapping = false;
            List<EZShredLeadMappingDataModel> lstEZShredLeadMapping = _dbEZShredLeadMappingService.GetAllEZShredLeadDataByAccountId(mxtrAccount.ObjectID);
            if (lstEZShredLeadMapping != null)
            {
                foreach (SharpspringPostRequestString item in SSPostRequestString)
                {
                    if (lstEZShredLeadMapping.Where(e => e.LeadID == Convert.ToInt64(item.Id)).Count() == 0)
                    {
                        if (!string.IsNullOrEmpty(item.CompanyName))
                        {
                            //Add item to EZshredLeadMapping
                            EZShredLeadMappingDataModel objEZShredLeadMappingDataModel = _dbEZShredLeadMappingService.AddEZShredLeadMappingData(
                                new EZShredLeadMappingDataModel
                                {
                                    AccountObjectID = mxtrAccount.ObjectID,
                                    MxtrAccountID = mxtrAccount.MxtrAccountID,
                                    UserID = mxtrUser.ObjectID,
                                    MxtrUserID = mxtrUser.MxtrUserID,
                                    LeadID = Convert.ToInt64(item.Id),
                                    CustomerID = null,
                                    OpportunityID = OpportunityID,
                                    Street = null,
                                    ZIP = null,
                                    Company = item.CompanyName,
                                    EZShredActionType = EZShredActionTypeKind.NoAction.ToString(),
                                    EZShredStatus = EZShredStatusKind.Complete.ToString(),
                                    BuildingStage = BuildingSet
                                });

                            //Create Opportunity of Existing Lead
                            _apiSharpspringService.SetConnectionTokens(mxtrAccount.SharpspringSecretKey, mxtrAccount.SharpspringAccountID);
                            OpportunityID = CreateOpportunityForLead(objEZShredLeadMappingDataModel);

                            objEZShredLeadMappingDataModel.OpportunityID = OpportunityID;
                            //Add building to Leadmapping
                            AddLeadBuilding(objEZShredLeadMappingDataModel, BuildingSet);
                            //update with opportunityID
                            UpdateEZshredLeadMappingData(objEZShredLeadMappingDataModel);
                            IsdataWriteToEZShredLeadMapping = true;
                        }
                    }
                }
            }
            return IsdataWriteToEZShredLeadMapping;
        }
        private EZShredLeadMappingDataModel AddDataToEZShredLeadMapping(EZShredLeadMappingDataModel objEZShredLeadMappingDataModel)
        {
            return _dbEZShredLeadMappingService.AddEZShredLeadMappingData(objEZShredLeadMappingDataModel);
        }
        private void UpdateEZshredLeadMappingData(EZShredLeadMappingDataModel objEZShredLeadMappingDataModel)
        {
            _dbEZShredLeadMappingService.UpdateEZShredLeadMappingData(objEZShredLeadMappingDataModel);
        }
        private void AddLeadBuilding(EZShredLeadMappingDataModel objEZShredLeadMappingDataModel, string BuildingSet)
        {
            LeadBuildingDataModel objLeadBuilding = new LeadBuildingDataModel
            {
                BuildingID = 0,
                BuildingCompanyName = null,
                OpportunityID = objEZShredLeadMappingDataModel.OpportunityID,
                EZShredActionType = EZShredActionTypeKind.NoAction.ToString(),
                EZShredStatus = EZShredStatusKind.Complete.ToString(),
                CreateDate = DateTime.Now
            };
            _dbEZShredLeadMappingService.AddLeadBuilding(objEZShredLeadMappingDataModel, objLeadBuilding, BuildingSet);
        }
        private long CreateOpportunityForLead(EZShredLeadMappingDataModel objEZShredLeadMappingDataModel)
        {
            long OpportunityID = 0;
            List<SharpspringDealStageDataModel> lstDealStages = _apiSharpspringService.GetAllDealStages();
            List<SharpspringOpportunityDataModel> opportunitiesData = new List<SharpspringOpportunityDataModel>()
            {
                    new SharpspringOpportunityDataModel()
                    {
                        OpportunityName =objEZShredLeadMappingDataModel.Company+" Opportunity",
                        OwnerID =313387434,
                        DealStageID =Convert.ToInt64(lstDealStages .Where(d=>d.DealStageName== PipelineKind.Lead.ToString()).Select(d=>d.DealStageID).FirstOrDefault()),
                        IsActive=true
                    }
             };

            string opportunitiesResult = _apiSharpspringService.CreateOpportunities(opportunitiesData);
            SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(opportunitiesResult);
            if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
            {
                OpportunityID = ssResponse.result.creates[0].id;
                _apiSharpspringService.CreateOpportunityLeads(ssResponse.result.creates[0].id, Convert.ToInt64(objEZShredLeadMappingDataModel.LeadID));
            }
            return OpportunityID;
        }

        private HttpResponseMessage GetRequestResponse(string message, HttpStatusCode status, string lastRun)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                ReasonPhrase = "",
                StatusCode = status,
                Content = new ObjectContent<SSResponse>(SetRequestResponse(message, status, lastRun),
                new System.Net.Http.Formatting.JsonMediaTypeFormatter(), "application/json")
            };
            return response;
        }

        private SSResponse SetRequestResponse(string message, HttpStatusCode status, string lastRun)
        {
            return new SSResponse()
            {
                Message = message,
                Status = status,
                LastRun = lastRun,
            };
        }
    }

    public class SharpspringPostRequestString
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
    }

    public class SSResponse
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public string LastRun { get; set; }
    }
}
