using System;
using System.Collections.Generic;
using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Websites.Platform.Helpers;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class LeadController : MainLayoutControllerBase
    {
        private readonly ILeadViewModelAdapter _viewModelAdapter;
        private readonly ICRMLeadService _crmLeadService;
        private readonly ICRMCampaignService _crmCampaignService;
        private readonly IAccountService _accountService;
        private readonly ISharpspringService _apiSharpspringService;

        public LeadController(ILeadViewModelAdapter viewModelAdapter, ICRMLeadService crmLeadService, ICRMCampaignService crmCampaignService, IAccountService accountService, ISharpspringService apiSharpspringService)
        {
            _viewModelAdapter = viewModelAdapter;
            _crmLeadService = crmLeadService;
            _crmCampaignService = crmCampaignService;
            _accountService = accountService;
            _apiSharpspringService = apiSharpspringService;
        }

        public ActionResult ViewPage(LeadWebQuery query)
        {
            IEnumerable<mxtrAccount> client = _accountService.GetFlattenedChildAccounts_Client(User.MxtrAccountObjectID);

            mxtrAccount parentMxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            // Get data...
            CRMLeadDataModel lead = _crmLeadService.GetLeadsByObjectID(query.ObjectID);

            List<CRMLeadEventDataModel> leadEvents = GetCopyLeadsEvents(query.ObjectID, lead.ClonedLeadID);

            Dictionary<string, Tuple<string, bool, long>> dicClonedAccount = _crmLeadService.GetAccountByCloneLeadID(lead.LeadID);

            mxtrAccount account = _accountService.GetAccountByMxtrAccountId(lead.MxtrAccountID);

            mxtrAccount parentAccount = new mxtrAccount();
            if (!String.IsNullOrEmpty(parentMxtrAccount.ParentAccountObjectID))
            {
                parentAccount = _accountService.GetAccountByAccountObjectID(parentMxtrAccount.ParentAccountObjectID);

                parentAccount = GetParentAccountOfLead(parentAccount);

                // Check if parent is super admin
                if (String.IsNullOrEmpty(parentAccount.ParentAccountObjectID))
                {
                    parentAccount = new mxtrAccount();
                }
            }

            // Adapt data...
            LeadViewModel model = _viewModelAdapter.BuildLeadViewModel(lead, account, client, parentMxtrAccount, leadEvents, parentAccount, dicClonedAccount);

            // Handle...
            return View(ViewKind.Lead, model, query);
        }

        private mxtrAccount GetParentAccountOfLead(mxtrAccount parentAccount)
        {
            if (parentAccount.AccountType == AccountKind.Group.ToString())
            {
                parentAccount = _accountService.GetAccountByAccountObjectID(parentAccount.ParentAccountObjectID);
                //call the function recursively until we get parent
               return GetParentAccountOfLead(parentAccount);
            }

            return parentAccount;
        }

        public ActionResult CopyLead(LeadCopyWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            List<SharpspringLeadDataModel> lstLeadData = new List<SharpspringLeadDataModel>();
            try
            {
                CRMLeadDataModel lead = _crmLeadService.GetLeadsByObjectID(query.LeadObjectId);
                mxtrAccount account = _accountService.GetAccountByAccountObjectID(query.ClientObjectId);
                if (lead != null && account != null)
                {
                    SharpspringLeadDataModel leadData = new SharpspringLeadDataModel();
                    Helper.Map(lead, leadData);
                    lstLeadData.Add(leadData);
                    _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);
                    string leadResult = _apiSharpspringService.CreateLead(lstLeadData);

                    SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(leadResult);
                    if (ssResponse.result.creates != null && ssResponse.result.creates.Count > 0 && ssResponse.result.creates[0].success)
                    {
                        lead.ClonedLeadID = lead.LeadID;
                        lead.LeadID = ssResponse.result.creates[0].id;
                        lead.OriginalLeadID = query.LeadObjectId;
                        lead.AccountObjectID = account.ObjectID;
                        lead.MxtrAccountID = account.MxtrAccountID;
                        lead.CopiedToParent = query.CopiedToParent;
                        DateTime tempDate = DateTime.Now;
                        lead.CreateDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);
                        lead.LastUpdatedDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0, DateTimeKind.Utc);
                        lead.Events = new List<CRMLeadEventDataModel>();
                        notification = _crmLeadService.CreateCRMLead(lead);
                        return Json(new { Success = notification.Success, Message = result, Key = account.ObjectID, Item1 = account.AccountName, Item3 = lead.LeadID });
                    }
                    else
                    {
                        result = ssResponse.error[0].message;
                    }
                }
                else
                {
                    result = "Some error in account";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Json(new { Success = notification.Success, Message = result });
        }

        public ActionResult DeleteLead(LeadDeleteWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            string result = String.Empty;
            string objectId = string.Empty;
            try
            {
                if (query.LeadId == 0)
                {
                    // Handle deletion of current lead
                    CRMLeadDataModel lead = _crmLeadService.GetLeadsByObjectID(query.ClientObjectId);
                    objectId = query.ClientObjectId;
                    query.ClientObjectId = lead.AccountObjectID;
                    query.LeadId = lead.LeadID;
                }

                mxtrAccount account = _accountService.GetAccountByAccountObjectID(query.ClientObjectId);
                _apiSharpspringService.SetConnectionTokens(account.SharpspringSecretKey, account.SharpspringAccountID);

                //Check lead exist
                SharpspringLeadDataModel objLeadData = _apiSharpspringService.GetLead(query.LeadId);
                if (objLeadData != null) // User exist in SS
                {
                    List<long> leadIds = new List<long>();
                    leadIds.Add(query.LeadId);
                    string leadResult = _apiSharpspringService.DeleteLead(leadIds);
                    SharpspringResponse ssResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<SharpspringResponse>(leadResult);
                    if (ssResponse.result.deletes != null && ssResponse.result.deletes.Count > 0 && ssResponse.result.deletes[0].success)
                    {
                        if (!string.IsNullOrEmpty(objectId))
                        {
                            // delete lead from database
                            notification = _crmLeadService.DeleteLead(objectId);
                        }
                        else
                        {
                            // Make IsActive false in database
                            notification = _crmLeadService.DeleteLead(query.LeadId);
                        }
                    }
                    else
                    {
                        result = ssResponse.error[0].message;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        // delete lead from database
                        notification = _crmLeadService.DeleteLead(objectId);
                    }
                    else
                    {
                        // Make IsActive false in database
                        notification = _crmLeadService.DeleteLead(query.LeadId);
                    }
                }

            }
            catch (Exception ex)
            {
                result = "Some error in account " + ex.Message;
            }


            return Json(new { Success = notification.Success, Message = result });
        }

        private List<CRMLeadEventDataModel> GetCopyLeadsEvents(string objectId, long clonedLeadID)
        {
            //check if it has been cloned 
            List<CRMLeadEventDataModel> leadEvents = new List<CRMLeadEventDataModel>();
            //Get all lead having ClonedLeadID and OriginalLeadID for the current lead

            //get events of cloned lead
            List<string> lstLeadHavingCloneId = _crmLeadService.GetLeadsIdByClonedObjectId(objectId);
            if (clonedLeadID > 0)
            {
                lstLeadHavingCloneId.AddRange(_crmLeadService.GetLeadsIdByCloneLeadID(clonedLeadID));
            }
            if (lstLeadHavingCloneId != null && lstLeadHavingCloneId.Count > 0)
            {
                IEnumerable<CRMLeadDataModel> leads = _crmLeadService.GetLeadEvents(lstLeadHavingCloneId);

                foreach (var lead in leads)
                {
                    foreach (var leadEvent in lead.Events)
                    {
                        if (leadEvents.IndexOf(leadEvent) < 0)
                        {
                            leadEvents.Add(leadEvent);
                        }
                    }
                }
            }

            return leadEvents;
        }
    }
}
