using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
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
using System.Web.Http;

namespace mxtrAutomation.Websites.Platform.WebAPI
{
    [RoutePrefix("api/v1")]
    public class APILeadSharpspringController : ApiController
    {
        private readonly ICRMLeadService _crmLeadService = ServiceLocator.Current.GetInstance<ICRMLeadServiceInternal>();
        private readonly IAccountService _accountService = ServiceLocator.Current.GetInstance<IAccountServiceInternal>();
        private readonly ISharpspringService _apiSharpspringService = ServiceLocator.Current.GetInstance<ISharpspringService>();
        private readonly ICorporateSSLastRunService _corporateSSLastRunService = ServiceLocator.Current.GetInstance<ICorporateSSLastRunServiceInternal>();


        [HttpGet]
        [Route("")]
        public IEnumerable<string> Get()
        {
            //_apiSharpspringService.SetConnectionTokens("5A4781CD32F5D9ED9E508EDA64F1BEB6", "3_057241C0944EC4B6BB0E6A82771C2E1E");
            //SharpspringLeadDataModel lead = _apiSharpspringService.GetLead("sample23@gmail.com");
            return new string[] { "value1", "value2" };
        }


        [HttpGet]
        [Route("{id:int}")]
        public string Get(int id)
        {
            return "value";
        }


        [HttpPost]
        [Route("leadcopy")]
        public HttpResponseMessage Post([FromBody]CopyLeadData objLeadData)
        {
            bool IsleadEmailProvided = true;
            if (objLeadData == null)
            {
                return GetResponse("No data received", HttpStatusCode.NotFound);
            }
            if (string.IsNullOrEmpty(objLeadData.AccountObjectId))
            {
                return GetResponse("Invalid Account Id", HttpStatusCode.BadRequest);
            }
            else if (string.IsNullOrEmpty(objLeadData.DealerID))
            {
                return GetResponse("Invalid Dealer Id", HttpStatusCode.BadRequest);
            }
            else if (string.IsNullOrEmpty(objLeadData.LeadEmail))
            {
                IsleadEmailProvided = false;
                if (objLeadData.LeadObj != null)
                {
                    //validate data 
                    bool IsVallidData = true;
                    if (!IsVallidData)
                    {
                        return GetResponse("Invalid Lead Object data", HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return GetResponse("Invalid Lead Email", HttpStatusCode.BadRequest);
                }
            }

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                //get lead from account details           
                mxtrAccount accountParent = _accountService.GetAccountByAccountObjectID(objLeadData.AccountObjectId);
                SharpspringLeadDataModel leadFromSharpSpring = null;
                if (accountParent == null)
                {
                    return GetResponse("Parent account not found", HttpStatusCode.NotFound);
                }
                else
                {
                    //_apiSharpspringService.SetConnectionTokens("5A4781CD32F5D9ED9E508EDA64F1BEB6", "3_057241C0944EC4B6BB0E6A82771C2E1E");
                    //leadFromSharpSpring = _apiSharpspringService.GetLead("sample23@gmail.com");
                    if (string.IsNullOrEmpty(accountParent.SharpspringSecretKey) || string.IsNullOrEmpty(accountParent.SharpspringAccountID))
                    {
                        return GetResponse("Sharpspring details are not available for this account", HttpStatusCode.ExpectationFailed);
                    }
                    _apiSharpspringService.SetConnectionTokens(accountParent.SharpspringSecretKey, accountParent.SharpspringAccountID);
                    if (IsleadEmailProvided)
                    {
                        leadFromSharpSpring = _apiSharpspringService.GetLead(objLeadData.LeadEmail);
                    }
                    else
                    {
                        //assign the lead data that has been sent in POST
                        leadFromSharpSpring = AssignLeadData(objLeadData.LeadObj);
                    }
                }

                CreateNotificationReturn dbSaveLeadNotification = new CreateNotificationReturn();
                if (leadFromSharpSpring == null)
                {
                    return GetResponse("Some error in getting lead from Sharpspring", HttpStatusCode.NotFound);
                }
                else
                {
                    //save the lead (got from sharpspring) to mxtr database
                    dbSaveLeadNotification = SaveLeadToDatabase(accountParent, leadFromSharpSpring);
                    if (!dbSaveLeadNotification.Success)
                    {
                        response = GetResponse("Lead not saved in database", HttpStatusCode.ExpectationFailed);
                    }
                }

                //transfer lead to account details
                mxtrAccount accountDealer = _accountService.GetAccountByDealerId(objLeadData.DealerID, accountParent.ObjectID);
                if (accountDealer == null)
                {
                    response = GetResponse("Dealer account not found", HttpStatusCode.NotFound);
                }
                else
                {
                    if (string.IsNullOrEmpty(accountDealer.SharpspringSecretKey) || string.IsNullOrEmpty(accountDealer.SharpspringAccountID))
                    {
                        return GetResponse("Sharpspring details are not available for dealer account", HttpStatusCode.ExpectationFailed);
                    }
                }

                if (accountParent != null && leadFromSharpSpring != null && accountDealer != null && dbSaveLeadNotification.Success)
                {
                    List<SharpspringLeadDataModel> lstLeadData = new List<SharpspringLeadDataModel>();
                    SharpspringLeadDataModel leadData = new SharpspringLeadDataModel();
                    Helper.Map(leadFromSharpSpring, leadData);
                    lstLeadData.Add(leadData);

                    //copy using sparspring 
                    //_apiSharpspringService.SetConnectionTokens("7C841CEDCC457BBF952A8D6C94D11691", "3_0BB3C17E58219B9A17223CD33855D8FE");
                    _apiSharpspringService.SetConnectionTokens(accountDealer.SharpspringSecretKey, accountDealer.SharpspringAccountID);

                    //string leadResult = @"{'result':{'creates':[{'success':true,'error':null,'id':533521345539}]},'error':[],'id':'636385810846935972'}";
                    string leadResult = _apiSharpspringService.CreateLead(lstLeadData);

                    //Save newly created lead to mxtr Database                    
                    SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(leadResult);
                    if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                    {
                        //get newly created lead details
                        //-- SharpspringLeadDataModel copiedLeadFromSharpSpring = _apiSharpspringService.GetLead(ssResponse.result.creates[0].id);
                        long leadId = ssResponse.result.creates[0].id;

                        CRMLeadDataModel copiedLead = SaveCopiedLeadToDatabase(accountParent, leadFromSharpSpring, dbSaveLeadNotification, accountDealer, ssResponse);
                        CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
                        notification = _crmLeadService.CreateCRMLead(copiedLead);

                        // Create opportunities
                        List<SharpspringDealStageDataModel> lstDealStages = _apiSharpspringService.GetAllDealStages().OrderByDescending(x => x.DealStageID).ToList();
                        List<SharpspringOpportunityDataModel> opportunitiesData = new List<SharpspringOpportunityDataModel>()
                        {
                            new SharpspringOpportunityDataModel()
                            {
                                OpportunityName =(String.IsNullOrEmpty(leadFromSharpSpring.CompanyName)?leadFromSharpSpring.FirstName+" "+leadFromSharpSpring.LastName:leadFromSharpSpring.CompanyName)+" Shaw Opportunity",
                                OwnerID =GetOwnerID(_apiSharpspringService,leadFromSharpSpring.EmailAddress),
                                DealStageID =lstDealStages.FirstOrDefault().DealStageID, //Convert.ToInt64(lstDealStages.Where(d=>d.DealStageName== PipelineKind.Lead.ToString()).Select(d=>d.DealStageID).FirstOrDefault()),
                                IsActive=true
                            }
                        };

                        string opportunitiesResult = _apiSharpspringService.CreateOpportunities(opportunitiesData);
                        ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(opportunitiesResult);
                        if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                        {
                            _apiSharpspringService.CreateOpportunityLeads(ssResponse.result.creates[0].id, leadId);
                        }

                        if (!notification.Success)
                        {
                            response = GetResponse("Copied lead not saved in database", HttpStatusCode.ExpectationFailed, leadId);
                        }
                        else
                        {
                            response = GetResponse("success", HttpStatusCode.OK, leadId);
                        }
                    }
                    else
                    {
                        response = GetResponse(ssResponse.error[0].message, HttpStatusCode.ExpectationFailed);
                    }
                }
            }
            catch (Exception ex)
            {
                response = GetResponse(ex.Message, HttpStatusCode.ExpectationFailed);
            }

            return response;
        }

        private static SharpspringLeadDataModel AssignLeadData(LeadObject objLeadData)
        {
            return new SharpspringLeadDataModel()
            {
                AccountID = objLeadData.AccountID,
                LeadID = objLeadData.Id,
                OwnerID = objLeadData.OwnerID,
                CampaignID = objLeadData.CampaignID,
                LeadStatus = objLeadData.LeadStatus,
                LeadScore = objLeadData.LeadScore != 0 ? objLeadData.LeadScore : objLeadData.LeadScoreWeighted,
                LeadScoreWeighted = objLeadData.LeadScoreWeighted,
                IsActive = objLeadData.IsActive,
                FirstName = objLeadData.FirstName,
                LastName = objLeadData.LastName,
                EmailAddress = objLeadData.EmailAddress,
                CompanyName = objLeadData.CompanyName,
                Title = objLeadData.Title,
                Street = objLeadData.Street,
                City = objLeadData.City,
                Country = objLeadData.Country,
                State = objLeadData.State,
                ZipCode = objLeadData.Zipcode,
                Website = objLeadData.Website,
                PhoneNumber = objLeadData.PhoneNumber,
                MobilePhoneNumber = objLeadData.MobilePhoneNumber,
                FaxNumber = objLeadData.FaxNumber,
                Description = objLeadData.Description,
                Industry = objLeadData.Industry,
                IsUnsubscribed = objLeadData.IsUnsubscribed,
            };
        }

        private CopyLeadResponse SetResponse(string message, HttpStatusCode status, long leadId)
        {
            return new CopyLeadResponse()
            {
                Message = message,
                Status = status,
                CreatedLeadId = leadId,
            };
        }

        private HttpResponseMessage GetResponse(string message, HttpStatusCode status, long leadId = 0)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                ReasonPhrase = "",
                StatusCode = status,
                Content = new ObjectContent<CopyLeadResponse>(SetResponse(message, status, leadId),
                new System.Net.Http.Formatting.JsonMediaTypeFormatter(), "application/json")
            };
            return response;
        }

        private static CRMLeadDataModel SaveCopiedLeadToDatabase(mxtrAccount accountParent, SharpspringLeadDataModel leadFromSharpSpring, CreateNotificationReturn dbSaveLeadNotification, mxtrAccount accountDealer, SharpspringResponse ssResponse)
        {
            DateTime tempDate = DateTime.Now;
            CRMLeadDataModel lead = new CRMLeadDataModel()
            {
                ClonedLeadID = leadFromSharpSpring.LeadID,
                LeadID = ssResponse.result.creates[0].id,
                OriginalLeadID = dbSaveLeadNotification.ObjectID,
                AccountObjectID = accountDealer.ObjectID,
                MxtrAccountID = accountDealer.MxtrAccountID,
                CopiedToParent = false,
                CreateDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc),
                LastUpdatedDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc),
                CRMKind = CRMKind.Sharpspring.ToString(),
                AccountID = leadFromSharpSpring.AccountID,
                OwnerID = leadFromSharpSpring.OwnerID,
                CampaignID = leadFromSharpSpring.CampaignID,
                LeadStatus = leadFromSharpSpring.LeadStatus,
                LeadScore = leadFromSharpSpring.LeadScore,
                IsActive = leadFromSharpSpring.IsActive,
                FirstName = leadFromSharpSpring.FirstName,
                LastName = leadFromSharpSpring.LastName,
                EmailAddress = leadFromSharpSpring.EmailAddress,
                CompanyName = leadFromSharpSpring.CompanyName,
                Title = leadFromSharpSpring.Title,
                Street = leadFromSharpSpring.Street,
                City = leadFromSharpSpring.City,
                Country = leadFromSharpSpring.Country,
                State = leadFromSharpSpring.State,
                ZipCode = leadFromSharpSpring.ZipCode,
                Website = leadFromSharpSpring.Website,
                PhoneNumber = leadFromSharpSpring.PhoneNumber,
                MobilePhoneNumber = leadFromSharpSpring.MobilePhoneNumber,
                FaxNumber = leadFromSharpSpring.FaxNumber,
                Description = leadFromSharpSpring.Description,
                Industry = leadFromSharpSpring.Industry,
                IsUnsubscribed = leadFromSharpSpring.IsUnsubscribed,
                LeadParentAccount = accountParent.ObjectID,
                //Events = leadFromSharpSpring.Events
                Events = new List<CRMLeadEventDataModel>(),
            };
            return lead;
        }

        private CreateNotificationReturn SaveLeadToDatabase(mxtrAccount accountParent, SharpspringLeadDataModel leadFromSharpSpring)
        {
            CreateNotificationReturn dbNotificationleadFromSharpSpring = new CreateNotificationReturn { Success = false };
            DateTime tempDateLead = DateTime.Now;
            dbNotificationleadFromSharpSpring = _crmLeadService.CreateCRMLead(new CRMLeadDataModel()
            {                
                FirstName = leadFromSharpSpring.FirstName,
                LastName = leadFromSharpSpring.LastName,
                ClonedLeadID = 0,
                LeadID = leadFromSharpSpring.LeadID,
                OriginalLeadID = null,
                AccountObjectID = accountParent.ObjectID,
                MxtrAccountID = accountParent.MxtrAccountID,
                CopiedToParent = false,
                CreateDate = new DateTime(tempDateLead.Year, tempDateLead.Month, tempDateLead.Day, 0, 0, 0, DateTimeKind.Utc),
                LastUpdatedDate = new DateTime(tempDateLead.Year, tempDateLead.Month, tempDateLead.Day, 0, 0, 0, DateTimeKind.Utc),
                Events = new List<CRMLeadEventDataModel>(),
                CRMKind = CRMKind.Sharpspring.ToString(),
                AccountID = leadFromSharpSpring.AccountID,
                OwnerID = leadFromSharpSpring.OwnerID,
                CampaignID = leadFromSharpSpring.CampaignID,
                LeadStatus = leadFromSharpSpring.LeadStatus,
                LeadScore = leadFromSharpSpring.LeadScore,
                IsActive = leadFromSharpSpring.IsActive,
                EmailAddress = leadFromSharpSpring.EmailAddress,
                CompanyName = leadFromSharpSpring.CompanyName,
                Title = leadFromSharpSpring.Title,
                Street = leadFromSharpSpring.Street,
                City = leadFromSharpSpring.City,
                Country = leadFromSharpSpring.Country,
                State = leadFromSharpSpring.State,
                ZipCode = leadFromSharpSpring.ZipCode,
                Website = leadFromSharpSpring.Website,
                PhoneNumber = leadFromSharpSpring.PhoneNumber,
                MobilePhoneNumber = leadFromSharpSpring.MobilePhoneNumber,
                FaxNumber = leadFromSharpSpring.FaxNumber,
                Description = leadFromSharpSpring.Description,
                Industry = leadFromSharpSpring.Industry,
                IsUnsubscribed = leadFromSharpSpring.IsUnsubscribed,
                LeadParentAccount = accountParent.ObjectID,
            });
            return dbNotificationleadFromSharpSpring;
        }

        private long GetOwnerID(ISharpspringService apiSharpspringService, string EmailAddress)
        {
            string OwnerID = string.Empty;
            List<SharpspringUserProfileDataModel> SharpspringUserProfileDataModel = apiSharpspringService.GetUserProfiles();
            if (SharpspringUserProfileDataModel.Count() > 0)
            {
                OwnerID = SharpspringUserProfileDataModel.Where(x => x.EmailAddress == EmailAddress).Select(x => x.OwnerID).FirstOrDefault();
                if (string.IsNullOrEmpty(OwnerID))
                    OwnerID = SharpspringUserProfileDataModel.Select(x => x.OwnerID).FirstOrDefault();
            }
            return String.IsNullOrEmpty(OwnerID) ? 0 : Convert.ToInt64(OwnerID);
        }

        // PUT api/<controller>/5
        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("CorporateSSLastRun")]
        public HttpResponseMessage Post([FromBody]CorporateSSData data)
        {
            if (string.IsNullOrEmpty(data.AccountObjectId))
            {
                return GetCorporateSSResponse("Invalid Account ObjectId", HttpStatusCode.BadRequest, string.Empty);
            }

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                if (data.LastRunDate == null)
                {
                    DateTime lastRun = _corporateSSLastRunService.GetCorporateSSLastRun(data.AccountObjectId);
                    if (DateTime.MinValue == lastRun)
                    {
                        return GetCorporateSSResponse("success", HttpStatusCode.OK, String.Empty);
                    }
                    return GetCorporateSSResponse("success", HttpStatusCode.OK, lastRun.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    _corporateSSLastRunService.AddCorporateSSLastRunDate(data.AccountObjectId, Convert.ToDateTime(data.LastRunDate));
                    return GetCorporateSSResponse("success", HttpStatusCode.OK, data.LastRunDate);
                }
            }
            catch (Exception ex)
            {
                response = GetCorporateSSResponse(ex.Message, HttpStatusCode.ExpectationFailed, string.Empty);
            }

            return response;
        }

        private HttpResponseMessage GetCorporateSSResponse(string message, HttpStatusCode status, string lastRun)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                ReasonPhrase = "",
                StatusCode = status,
                Content = new ObjectContent<CorporateSSResponse>(SetCorporateSSResponse(message, status, lastRun),
                new System.Net.Http.Formatting.JsonMediaTypeFormatter(), "application/json")
            };
            return response;
        }

        private CorporateSSResponse SetCorporateSSResponse(string message, HttpStatusCode status, string lastRun)
        {
            return new CorporateSSResponse()
            {
                Message = message,
                Status = status,
                LastRun = lastRun,
            };
        }
    }
    public class CopyLeadData
    {
        public string AccountObjectId { get; set; }
        public string DealerID { get; set; }
        public string LeadEmail { get; set; }
        public LeadObject LeadObj { get; set; }
    }
    public class CopyLeadResponse
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public long CreatedLeadId { get; set; }
    }
    public class CorporateSSData
    {
        public string AccountObjectId { get; set; }
        public string LastRunDate { get; set; }
    }
    public class CorporateSSResponse
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public string LastRun { get; set; }
    }
    public class LeadObject
    {
        public long AccountID { get; set; }
        public long Id { get; set; }
        public long OwnerID { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public object OfficePhoneNumber { get; set; }
        public object PhoneNumberExtension { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Description { get; set; }
        public long CampaignID { get; set; }
        public string TrackingID { get; set; }
        public string Industry { get; set; }
        public string Active { get; set; }
        public bool IsUnsubscribed { get; set; }
        public string UpdateTimestamp { get; set; }
        public int LeadScoreWeighted { get; set; }
        public string LeadStatus { get; set; }
        public int LeadScore { get; set; }
        public string Persona { get; set; }
        public bool IsActive { get; set; }
        //public string coupon_number_58e28ff42ab98 { get; set; }
        //public string flooring_preference_58e3bc9ebf97f { get; set; }
        //public string coupon_status_58e4ec9817867 { get; set; }
        //public string coupon_origin_58e4ecea9a38d { get; set; }
        //public string coupon_total_sale_58e4ed1479272 { get; set; }
        //public string last_coupon_promotion_58e5024d883ff { get; set; }
        //public string origin_id_58e6a72e4a645 { get; set; }
        //public string coupon_issued_on_58e7d92c458fd { get; set; }
        //public string coupon_purchase_date_58e7d9597ef9a { get; set; }
        //public string concierge_chat_date_58e7d9caa951c { get; set; }
        //public string type_of_user_58edd5b96de99 { get; set; }
        //public string is_subscribed_58efc8650fa4a { get; set; }
        //public string sample_tracking_code_58fa5f6d9eeab { get; set; }
        //public string sample__1_productsku_5900b1c2d44e4 { get; set; }
        //public string sample__1_productname_5900b1e79e5c7 { get; set; }
        //public string sample__1_productname__1__5900b1fecab88 { get; set; }
        //public string sample__2_productname__1__5900b21af1a3c { get; set; }
        //public string sample__3_productname__1__5900b23a3c20f { get; set; }
        //public string sample__1_productsku__1__5900b25ec003a { get; set; }
        //public string sample__1_productsku__2__5900b267ebd39 { get; set; }
        //public string sample__1_productsku__3__5900b2716ae03 { get; set; }
        //public string sample__1_productname__1__5900b3d4e4934 { get; set; }
        //public string sample__1_product_type__1__5900cc990540e { get; set; }
        //public string sws___start_date_5910ba24759c8 { get; set; }
        //public string sws___status_5915de141b9cd { get; set; }
        //public string sws___memberships_5915dee847d9c { get; set; }
        //public string sws___site_status_5915df31a97b8 { get; set; }
        //public string sif_membership_5915df63157c7 { get; set; }
        //public string contact_us__subject_5919c3355eb67 { get; set; }
        //public string contact_us__comments_5919c35090351 { get; set; }
        //public string sws___region_5919c6bab00ff { get; set; }
        //public string would_you_spend_a_little_extra_on_your_flooring_if_593ffcb672930 { get; set; }
        //public string do_home_improvement_ideas_overwhelm_or_confuse_you_593ffd459949a { get; set; }
        //public string what_type_of_flooring_are_you_interested_in__59403feb9adc5 { get; set; }
        //public string what_s_the_approximate_square_footage_of_your_proj_5940406f1de73 { get; set; }
        //public string what_is_the_best_time_of_day_to_contact_you__59492dab98eb7 { get; set; }
        //public string when_do_you_need_your_new_flooring__59492dea8e540 { get; set; }
        //public string i_accept_shaw_s_terms___conditions_59492f972661c { get; set; }
        //public string i_would_like_to_receive_special_offers_from_shaw_59493028b7e94 { get; set; }
        //public string what_type_of_flooring_are_you_interested_in_carpet_594c15a3df97c { get; set; }
        //public string what_type_of_flooring_are_you_interested_in_hardwo_594c15ba38726 { get; set; }
        //public string what_type_of_flooring_are_you_interested_in_lamina_594c15f051bd5 { get; set; }
        //public string what_type_of_flooring_are_you_interested_in_tile_a_594c1652252e7 { get; set; }
        //public string what_type_of_flooring_are_you_interested_in_luxury_594c167d836ef { get; set; }
        //public string what_type_of_flooring_are_you_interested_in_rugs__594c16a64db16 { get; set; }
        //public string what_type_of_flooring_are_you_interested_in_resili_594c16d9cf774 { get; set; }
        //public string preferred_contact_method_594c16f5865e2 { get; set; }
        //public string your_blog_596fb436a6874 { get; set; }
        //public string time_zone_5994a0a0665ec { get; set; }
        //public string preferred_contact_method__1__5994a11881246 { get; set; }
        //public string how_did_you_find_us__5994a1bcc81b7 { get; set; }
        //public string referral_type_5994a1ebe1190 { get; set; }
        //public string dealer_name_59a5b09d40395 { get; set; }
        //public string dealer_address_59a5b0aa59049 { get; set; }
        //public string dealer_phone_59a5b0bd58d3d { get; set; }
        //public string dealer_website_59a5b0d3d36fd { get; set; }
        //public string dealer_email_59a7bf3140a5f { get; set; }
        //public string dealer_city_59a8047edebe0 { get; set; }
        //public string dealer_address_1_59a80493587d7 { get; set; }
        //public string dealer_address_2_59a8049e70d39 { get; set; }
        //public string dealer_postal_code_59a804bb0fada { get; set; }
        //public string dealer_state_code_59a804cb407f0 { get; set; }
        //public string dealer_state_name_59a804dd0f52a { get; set; }
        //public string have_you_ordered_samples__59a99ddf408e3 { get; set; }
        //public string full_name_59afffd7cef77 { get; set; }
        //public string subject_59b15a8f86567 { get; set; }
        //public string sent_group_59b2f76c34697 { get; set; }
        //public string suggested_retail_name_59b8361bc3c95 { get; set; }
        //public string coupon_issued_on___spring__1__59b836b34d0f0 { get; set; }
        //public string coupon_number___spring__1__59b836f6ae944 { get; set; }
        //public string coupon_origin___spring__1__59b83b6964db8 { get; set; }
        //public string coupon_purchase_date___spring__1__59b83b8a4ce8e { get; set; }
        //public string coupon_status__spring__1__59b83b93650c1 { get; set; }
        //public string coupon_total_sale___spring__1__59b83ba234be0 { get; set; }
        //public string last_coupon_promotion___spring__1__59b83bad8772e { get; set; }
        //public string suggested_retail_name___spring__1__59b83bb803d86 { get; set; }
        //public string suggested_retail_name___spring__1__59b84890ae0e1 { get; set; }
        //public string suggested_retail_name___fall__1__59b8490ac7f47 { get; set; }
        //public string transferred_to_concierge_59ba74574a4cc { get; set; }
        //public string suggested_retail_name___spring__1__59bbe3ec1bae1 { get; set; }
        //public string suggested_retail_address___fall__1__59bbe4194e657 { get; set; }
        //public string suggested_dealer_id___fall_59ddf2757b5c1 { get; set; }
        //public string consumer_connect_contact_59de0e48c3d61 { get; set; }
        //public string test_field_59e5a519d6a5b { get; set; }
    }
}