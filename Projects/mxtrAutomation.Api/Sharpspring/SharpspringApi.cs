using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using RestSharp;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringApi : ISharpspringApi
    {
        private readonly string _baseUrl;
        private string _secretKey;
        private string _accountID;

        public SharpspringApi(string argBaseUrl)
        {
            _baseUrl = argBaseUrl;
        }

        public string SecretKey
        {
            get { return _secretKey; }
            set { _secretKey = value; }
        }

        public string AccountID
        {
            get { return _accountID; }
            set { _accountID = value; }
        }


        #region Leads

        public List<SharpspringLeadDataModel> GetAllLeads()
        {
            var allLeads = new List<SharpspringLeadDataModel>();
            int returnedDataCount = 500;
            int skip = 0;
            while (returnedDataCount == 500)
            {
                JObject leadDataJson = GetSkippedDataFromSharpspring(SharpspringMethodKinds.getLeads.ToString(), skip);
                var leads = ConvertToSharpspringLeadDataModel(leadDataJson);
                allLeads.AddRange(leads);
                returnedDataCount = ConvertToSharpspringLeadDataModel(leadDataJson).Count;
                skip += 500;
            }

            return allLeads;
        }

        public List<SharpspringLeadDataModel> GetLeadsForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind)
        {
            JObject leadDataJson = GetByDateRangeDataFromSharpspring(SharpspringMethodKinds.getLeadsDateRange.ToString(), startDate.ToString("yyyy-MM-dd HH':'mm':'ss"), endDate.ToString("yyyy-MM-dd HH':'mm':'ss"), actionKind);

            return ConvertToSharpspringLeadDataModel(leadDataJson);
        }

        public string CreateLead(List<SharpspringLeadDataModel> lstLeadData)
        {
            string url = BuildUrl();
            var json = CreateUpdateLeadJson(SharpspringMethodKinds.createLeads.ToString(), lstLeadData);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }
        public string CreateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet)
        {
            string url = BuildUrl();
            var json = CreateUpdateLeadWithCustomFieldsJson(SharpspringMethodKinds.createLeads.ToString(), ObjSharpspringCustomFieldsDataModel, objSharpSpringDataModel, BuildingSet);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }
        public string UpdateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet)
        {
            string url = BuildUrl();
            var json = CreateUpdateLeadWithCustomFieldsJson(SharpspringMethodKinds.updateLeads.ToString(), ObjSharpspringCustomFieldsDataModel, objSharpSpringDataModel, BuildingSet);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }

        public string UpdateLeadARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadID, string arCustomerCode)
        {
            string url = BuildUrl();
            var json = UpdateLeadARCustomerCodeJson(SharpspringMethodKinds.updateLeads.ToString(), ObjSharpspringCustomFieldsDataModel, leadID, arCustomerCode);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }

        public SharpspringLeadDataModel GetLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadId, string BuildingSet)
        {
            JObject leadDataJson = new JObject();
            string url = BuildUrl();
            var json = GetLeadJson(SharpspringMethodKinds.getLead.ToString(), leadId);
            string responseText = ExecutePostRestCall(url, json);

            if (!string.IsNullOrEmpty(responseText))
                leadDataJson = JObject.Parse(responseText);
            var leads = ConvertToCustomFieldsSharpspringLeadDataModel(ObjSharpspringCustomFieldsDataModel, leadDataJson, BuildingSet);
            if (leads.Count > 0)
                return leads[0];
            return null;
        }
        public SharpspringLeadDataModel GetLead(long leadId)
        {
            JObject leadDataJson = new JObject();
            string url = BuildUrl();
            var json = GetLeadJson(SharpspringMethodKinds.getLead.ToString(), leadId);
            string responseText = ExecutePostRestCall(url, json);

            if (!string.IsNullOrEmpty(responseText))
                leadDataJson = JObject.Parse(responseText);
            var leads = ConvertToSharpspringLeadDataModel(leadDataJson);
            if (leads.Count > 0)
                return leads[0];
            return null;
        }
        public SharpspringLeadDataModel GetLead(string email)
        {
            JObject leadDataJson = new JObject();
            string url = BuildUrl();
            var json = GetLeadJson(SharpspringMethodKinds.getLeads.ToString(), email);
            string responseText = ExecutePostRestCall(url, json);

            if (!string.IsNullOrEmpty(responseText))
                leadDataJson = JObject.Parse(responseText);
            var leads = ConvertToSharpspringLeadDataModel(leadDataJson);
            if (leads.Count > 0)
                return leads[0];
            return null;
        }

        public string UpdateLead(List<SharpspringLeadDataModel> lstLeadData)
        {
            string url = BuildUrl();
            var json = CreateUpdateLeadJson(SharpspringMethodKinds.updateLeads.ToString(), lstLeadData);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }

        public string DeleteLead(List<long> leadIds)
        {
            string url = BuildUrl();
            var json = DeleteLeadsJson(SharpspringMethodKinds.deleteLeads.ToString(), leadIds);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }

        #endregion

        #region Emails

        public List<SharpspringEmailDataModel> GetAllEmails()
        {
            JObject emailsJson = GetAllDataFromSharpspring(SharpspringMethodKinds.getEmailListing.ToString());

            return ConvertToSharpspringEmailDataModel(emailsJson);
        }

        public List<SharpspringEmailJobDataModel> GetAllEmailJobs()
        {
            var allEmailJobs = new List<SharpspringEmailJobDataModel>();
            int returnedDataCount = 500;
            int skip = 0;
            //while (returnedDataCount == 500)
            while (skip < 5000)
            {
                JObject jobDataJson = GetSkippedDataFromSharpspring(SharpspringMethodKinds.getEmailJobs.ToString(), skip);
                var jobs = ConvertToSharpspringEmailJobDataModel(jobDataJson);
                allEmailJobs.AddRange(jobs);
                returnedDataCount = jobs.Count;
                skip += 500;
            }

            return allEmailJobs;
        }

        public List<SharpspringEmailEventDataModel> GetEmailEvents(long emailJobID, string eventType)
        {
            JObject emailEventsJson = GetByEmailJobIDJson(SharpspringMethodKinds.getEmailEvents.ToString(), emailJobID, eventType);
            return ConvertToSharpspringEmailEventDataModel(emailEventsJson);
        }

        #endregion

        #region Deal Stages

        public List<SharpspringDealStageDataModel> GetAllDealStages()
        {
            JObject dealStagesJson = GetAllDataFromSharpspring(SharpspringMethodKinds.getDealStages.ToString());

            return ConvertToSharpspringDealStageDataModel(dealStagesJson);
        }

        public List<SharpspringDealStageDataModel> GetDealStagesForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind)
        {
            JObject dealStagesJson = GetByDateRangeDataFromSharpspring(SharpspringMethodKinds.getDealStagesDateRange.ToString(), startDate.ToString("yyyy-MM-dd HH':'mm':'ss"), endDate.ToString("yyyy-MM-dd HH':'mm':'ss"), actionKind);

            return ConvertToSharpspringDealStageDataModel(dealStagesJson);
        }


        #endregion


        #region Events

        public List<SharpspringEventDataModel> GetEventsByDate(DateTime startDate)
        {
            JObject filter = new JObject(
                new JProperty("createTimestamp", startDate)
                );

            JObject eventDataJson = GetFilteredDataFromSharpspring(SharpspringMethodKinds.getEvents.ToString(), filter);

            if (!eventDataJson.HasValues)
                return new List<SharpspringEventDataModel>();

            var events =
                from x in eventDataJson["result"]["event"].Children()
                select new SharpspringEventDataModel()
                {
                    EventID = x.Value<long?>("id") ?? 0,
                    LeadID = x.Value<long?>("leadID") ?? 0,
                    EventName = (string)x["eventName"],
                    WhatID = (string)x["whatID"],
                    WhatType = (string)x["whatType"],
                    EventData = ParseEventData((JToken)x["eventData"]),
                    CreateTimestamp = (DateTime)x["createTimestamp"]
                };

            return events.ToList();
        }

        #endregion


        #region Campaigns

        public List<SharpspringCampaignDataModel> GetAllCampaigns()
        {
            JObject campaignsJson = GetAllDataFromSharpspring(SharpspringMethodKinds.getCampaigns.ToString());

            return ConvertToSharpspringCampaignDataModel(campaignsJson);
        }

        public List<SharpspringCampaignDataModel> GetCampaignsForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind)
        {
            JObject campaignsJson = GetByDateRangeDataFromSharpspring(SharpspringMethodKinds.getCampaignsDateRange.ToString(), startDate.ToString("yyyy-MM-dd HH':'mm':'ss"), endDate.ToString("yyyy-MM-dd HH':'mm':'ss"), actionKind);

            return ConvertToSharpspringCampaignDataModel(campaignsJson);
        }


        #endregion


        #region Opportunities

        public List<SharpspringOpportunityDataModel> GetAllOpportunities()
        {
            JObject opportunitiesJson = GetAllDataFromSharpspring(SharpspringMethodKinds.getOpportunities.ToString());

            return ConvertToSharpspringOpportunityDataModel(opportunitiesJson);
        }
        public List<SharpspringOpportunityDataModel> GetOpportunitiesWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long opportunityId)
        {
            JObject filter = new JObject(
                new JProperty("id", opportunityId)
                );
            JObject opportunitiesJson = GetFilteredDataFromSharpspring(SharpspringMethodKinds.getOpportunities.ToString(), filter);
            return ConvertToSharpspringOpportunityDataModelWithCustomFields(ObjSharpspringCustomFieldsDataModel, opportunitiesJson);
        }
        public List<SharpspringOpportunityDataModel> GetOpportunitiesForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind)
        {
            JObject opportunitiesJson = GetByDateRangeDataFromSharpspring(SharpspringMethodKinds.getOpportunitiesDateRange.ToString(), startDate.ToString("yyyy-MM-dd HH':'mm':'ss"), endDate.ToString("yyyy-MM-dd HH':'mm':'ss"), actionKind);

            return ConvertToSharpspringOpportunityDataModel(opportunitiesJson);
        }

        public string CreateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData)
        {
            string url = BuildUrl();
            var json = CreateOpportunitiesJson(SharpspringMethodKinds.createOpportunities.ToString(), opportunitiesData);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }
        public string CreateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel)
        {
            string url = BuildUrl();
            var json = CreateOpportunitiesWithCustomFields(SharpspringMethodKinds.createOpportunities.ToString(), opportunitiesData, ObjSharpspringCustomFieldsDataModel);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }
        public string CreateOpportunityLeads(long opportunityID, long leadID)
        {
            string url = BuildUrl();
            var json = CreateOpportunityLeadsJson(SharpspringMethodKinds.createOpportunityLeads.ToString(), opportunityID, leadID);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }

        public string UpdateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData)
        {
            string url = BuildUrl();
            var json = UpdateOpportunitiesJson(SharpspringMethodKinds.updateOpportunities.ToString(), opportunitiesData);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }
        public string UpdateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel)
        {
            string url = BuildUrl();
            var json = UpdateOpportunitiesWithCustomFieldsJson(SharpspringMethodKinds.updateOpportunities.ToString(), opportunitiesData, ObjSharpspringCustomFieldsDataModel);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }
        public string UpdateOpportunitiesARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long opportunityId, string arCustomerCode)
        {
            string url = BuildUrl();
            var json = UpdateOpportunitiesARCustomerCodeJson(SharpspringMethodKinds.updateOpportunities.ToString(), ObjSharpspringCustomFieldsDataModel, opportunityId, arCustomerCode);
            string responseText = ExecutePostRestCall(url, json);
            return responseText;
        }
        public List<SharpspringOpportunityDataModel> GetOpportunitiesById(long opportunityId)
        {
            JObject filter = new JObject(
                new JProperty("id", opportunityId)
                );

            JObject opportunitiesJson = GetFilteredDataFromSharpspring(SharpspringMethodKinds.getOpportunities.ToString(), filter);
            return ConvertToSharpspringOpportunityDataModel(opportunitiesJson);
        }

        public long GetDealStageIdByOpportunityId(long opportunityId)
        {
            JObject filter = new JObject(
                new JProperty("id", opportunityId)
                );

            JObject opportunitiesJson = GetFilteredDataFromSharpspring(SharpspringMethodKinds.getOpportunities.ToString(), filter);
            return ConvertSSToDealStageId(opportunitiesJson);
        }

        #endregion

        #region GetCustomFields
        public List<SharpspringFieldLabelDataModel> GetCustomFields()
        {
            JObject customFieldsJson = GetAllDataFromSharpspring(SharpspringMethodKinds.getFields.ToString());
            return ConvertToSharpspringFieldLabelDataModel(customFieldsJson);
        }
        #endregion

        #region GetUserProfiles
        public List<SharpspringUserProfileDataModel> GetUserProfiles()
        {
            JObject Json = GetAllDataFromSharpspring(SharpspringMethodKinds.getUserProfiles.ToString());
            return ConvertToSharpspringUserProfiles(Json);
        }
        #endregion

        #region GetActiveLists
        public SharpspringGetActiveListDataModel GetActiveLists(string id)
        {
            string url = BuildUrl();
            JObject filter = new JObject(
               new JProperty("id", id)
               );
            JObject json = GetByFilterJson(SharpspringMethodKinds.getActiveLists.ToString(), filter);
            string responseText = ExecutePostRestCall(url, json);
            if (string.IsNullOrEmpty(responseText))
                return new SharpspringGetActiveListDataModel();

            JObject o = new JObject();
            o = JObject.Parse(responseText);
            var data =
                    from x in o["result"]["activeList"].Children()
                    select new SharpspringGetActiveListDataModel()
                    {
                        Id = (string)x["id"],
                        Name = (string)x["name"],
                        MemberCount = (string)x["memberCount"],
                        RemovedCount = (string)x["removedCount"],
                        CreateTimestamp = (string)x["createTimestamp"],
                        Description = (string)x["description"]
                    };
            return data.FirstOrDefault();
        }
        #endregion

        #region Api Call Helper Methods

        private JObject GetAllDataFromSharpspring(string methodName)
        {
            string url = BuildUrl();
            JObject o = new JObject();

            JObject json = GetAllJson(methodName);
            string responseText = ExecutePostRestCall(url, json);

            if (string.IsNullOrEmpty(responseText))
                return o;

            o = JObject.Parse(responseText);
            return o;
        }

        private JObject GetSkippedDataFromSharpspring(string methodName, int skip)
        {
            string url = BuildUrl();
            JObject o = new JObject();

            JObject json = GetBySkippedJson(methodName, skip);
            string responseText = ExecutePostRestCall(url, json);

            if (string.IsNullOrEmpty(responseText))
                return o;

            o = JObject.Parse(responseText);
            return o;
        }

        private JObject GetByDateRangeDataFromSharpspring(string methodName, string startDate, string endDate, ActionKind actionKind)
        {
            string url = BuildUrl();
            JObject o = new JObject();

            JObject json = GetByDateRangeJson(methodName, startDate, endDate, actionKind);
            string responseText = ExecutePostRestCall(url, json);

            if (string.IsNullOrEmpty(responseText))
                return o;

            o = JObject.Parse(responseText);
            return o;
        }

        private JObject GetFilteredDataFromSharpspring(string methodName, JObject filter)
        {
            string url = BuildUrl();
            JObject o = new JObject();

            JObject json = GetByFilterJson(methodName, filter);
            string responseText = ExecutePostRestCall(url, json);

            if (string.IsNullOrEmpty(responseText))
                return o;

            o = JObject.Parse(responseText);
            return o;
        }

        private JObject GetByEmailJobIDJson(string methodName, long emailJobID, string eventType)
        {
            string url = BuildUrl();
            JObject o = new JObject();

            JObject json = GetFilteredByEmailJobIDJson(methodName, emailJobID, eventType);
            string responseText = ExecutePostRestCall(url, json);

            if (string.IsNullOrEmpty(responseText) || responseText == "[]")
                return o;

            o = JObject.Parse(responseText);
            return o;
        }

        private JObject GetAllJson(string methodName)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);

            JObject o =
                new JObject(
                    new JProperty("method", methodName),
                    new JProperty("params",
                        new JObject(
                            new JProperty("where", "")
                                    )
                                  ),
                    new JProperty("id", id)
                            );

            return o;
        }

        private JObject GetByFilterJson(string methodName, JObject filter)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);

            JObject o =
                new JObject(
                    new JProperty("method", methodName),
                    new JProperty("params",
                        new JObject(
                            new JProperty("where", filter)
                                    )
                                  ),
                    new JProperty("id", id)
                            );

            return o;
        }

        private JObject GetByDateRangeJson(string methodName, string startDate, string endDate, ActionKind actionKind)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);

            JObject o =
                new JObject(
                    new JProperty("method", methodName),
                    new JProperty("params",
                        new JObject(
                            new JProperty("startDate", startDate),
                            new JProperty("endDate", endDate),
                            new JProperty("timestamp", actionKind.ToString())
                                    )
                                  ),
                    new JProperty("id", id)
                            );

            return o;
        }

        private JObject GetBySkippedJson(string methodName, int skip)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);

            JObject o =
                new JObject(
                    new JProperty("method", methodName),
                    new JProperty("params",
                        new JObject(
                             new JProperty("where", ""),
                             new JProperty("offset", skip)
                                    )
                                  ),
                    new JProperty("id", id)
                            );

            return o;
        }

        private JObject GetFilteredByEmailJobIDJson(string methodName, long emailJobID, string eventType)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);

            JObject o =
                new JObject(
                    new JProperty("method", methodName),
                    new JProperty("params",
                        new JObject(
                             new JProperty("emailJobID", emailJobID),
                             new JProperty("eventType", eventType)
                                    )
                                  ),
                    new JProperty("id", id)
                            );

            return o;
        }

        private JObject CreateUpdateLeadJson(string methodName, List<SharpspringLeadDataModel> lstLeadData)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            foreach (var item in lstLeadData)
            {
                jLeadArray.Add(new JObject(
                      new JProperty("id", item.LeadID),
                      new JProperty("accountID", item.AccountID),
                      new JProperty("ownerID", item.OwnerID),
                      new JProperty("campaignID", item.CampaignID),
                      new JProperty("leadStatus", item.LeadStatus),
                      new JProperty("leadScore", item.LeadScore),
                      new JProperty("active", item.IsActive == true ? 1 : 0),
                      new JProperty("firstName", item.FirstName),
                      new JProperty("lastName", item.LastName),
                      new JProperty("emailAddress", item.EmailAddress),
                      new JProperty("companyName", item.CompanyName),
                      new JProperty("title", item.Title),
                      new JProperty("street", item.Street),
                      new JProperty("city", item.City),
                      new JProperty("country", item.Country),
                      new JProperty("state", item.State),
                      new JProperty("zipcode", item.ZipCode),
                      new JProperty("website", item.Website),
                      new JProperty("phoneNumber", item.PhoneNumber),
                      new JProperty("mobilePhoneNumber", item.MobilePhoneNumber),
                      new JProperty("faxNumber", item.FaxNumber),
                      new JProperty("description", item.Description),
                      new JProperty("industry", item.Industry),
                      new JProperty("isUnsubscribed", item.IsUnsubscribed == true ? 1 : 0)
                    ));
            }

            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private JObject CreateUpdateLeadWithCustomFieldsJson(string methodName, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            JObject ObjCustomerData = new JObject(
             new JProperty("id", objSharpSpringDataModel.LeadID),
             new JProperty("firstName", objSharpSpringDataModel.FirstName),
             new JProperty("lastName", objSharpSpringDataModel.LastName),
             new JProperty("companyName", objSharpSpringDataModel.CompanyName),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingCompanyName", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.CompanyName),
             new JProperty("emailAddress", objSharpSpringDataModel.EmailAddress),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ARCustomerCode", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.ARCustomerCode),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "NumberOfEmployees", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.NumberOfEmployees),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Attention", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.Attention),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ReferralSourceID", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.ReferralSourceID),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Notes", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.Notes),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "EmailInvoice", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.EmailInvoice),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingZip", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.BillingZip),
             new JProperty("phoneNumber", objSharpSpringDataModel.PhoneNumber),
             new JProperty("faxNumber", objSharpSpringDataModel.FaxNumber),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingStreet", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.BillingStreet),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingCity", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.BillingCity),
             new JProperty("country", objSharpSpringDataModel.Country),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingState", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.BillingState),
             new JProperty("mobilePhoneNumber", objSharpSpringDataModel.MobilePhoneNumber),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "PipelineStatus", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.PipelineStatus),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Suite", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.Suite),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "EmailCOD", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.EmailCOD),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "CustomerTypeID", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.CustomerTypeID),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "InvoiceTypeID", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.InvoiceTypeID),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "TermID", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.TermID),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Datasource", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.Datasource),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Email2", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.Email2),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Email3", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.Email3),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingContact", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.BillingContact),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingContactExtension", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.BillingContactExtension),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingContactPhone", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.BillingContactPhone),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "TaxExempt", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.TaxExempt),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "NumberOfBoxes", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.NumberOfBoxes),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "CompanyCountryCode", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.CompanyCountryCode),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ServicesProfessionalType", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.ServicesProfessionalType),
              new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "TravelTourismType", SharpspringCustomFieldKind.Lead.ToString(), ""), objSharpSpringDataModel.TravelTourismType)
           );

            ObjCustomerData.Merge(BuildingSetCustomFieldsJson(ObjSharpspringCustomFieldsDataModel, objSharpSpringDataModel, SharpspringCustomFieldKind.Lead.ToString(), BuildingSet));
            jLeadArray.Add(ObjCustomerData);
            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private JObject BuildingSetCustomFieldsJson(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string Type, string BuildingSet)
        {
            JObject ObjBuilidingData = new JObject(
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingCompanyName", Type, BuildingSet), objSharpSpringDataModel.BuildingCompanyName),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactFirstName1", Type, BuildingSet), objSharpSpringDataModel.BuildingContactFirstName1),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactLastName1", Type, BuildingSet), objSharpSpringDataModel.BuildingContactLastName1),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactPhone1", Type, BuildingSet), objSharpSpringDataModel.BuildingContactPhone1),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactFirstName2", Type, BuildingSet), objSharpSpringDataModel.BuildingContactFirstName2),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactLastName2", Type, BuildingSet), objSharpSpringDataModel.BuildingContactLastName2),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactPhone2", Type, BuildingSet), objSharpSpringDataModel.BuildingContactPhone2),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingTypeID", Type, BuildingSet), objSharpSpringDataModel.BuildingTypeID),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "SalesmanID", Type, BuildingSet), objSharpSpringDataModel.SalesmanID),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Directions", Type, BuildingSet), objSharpSpringDataModel.Directions),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "RoutineInstructions", Type, BuildingSet), objSharpSpringDataModel.RoutineInstructions),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingSuite", Type, BuildingSet), objSharpSpringDataModel.BuildingSuite),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingStreet", Type, BuildingSet), objSharpSpringDataModel.BuildingStreet),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingCity", Type, BuildingSet), objSharpSpringDataModel.BuildingCity),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingState", Type, BuildingSet), objSharpSpringDataModel.BuildingState),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingZip", Type, BuildingSet), objSharpSpringDataModel.BuildingZip),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BillingCountryCode", Type, BuildingSet), objSharpSpringDataModel.BillingCountryCode),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ScheduleFrequency", Type, BuildingSet), objSharpSpringDataModel.ScheduleFrequency),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ServiceTypeID", Type, BuildingSet), objSharpSpringDataModel.ServiceTypeID),
                       new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "TaxExempt", Type, BuildingSet), objSharpSpringDataModel.TaxExempt)
                    );
            return ObjBuilidingData;
        }
        private JObject UpdateLeadARCustomerCodeJson(string methodName, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadID, string arCustomerCode)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            jLeadArray.Add(new JObject(
             new JProperty("id", leadID),
             new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ARCustomerCode", SharpspringCustomFieldKind.Lead.ToString(), ""), arCustomerCode)
           ));

            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        //private string GetSSSystemNameByEZshredFieldName(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, string EZShredFieldsName, string Type, string Set)
        //{
        //    //var response = ObjSharpspringCustomFieldsDataModel.Where(e => e.EZShredFieldName == EZShredFieldsName && e.Type.ToLower() == Type.ToLower() && e.Set == Set).Select(a => a.SSSystemName).FirstOrDefault();
        //    var data = ObjSharpspringCustomFieldsDataModel.Where(e => e.EZShredFieldName == EZShredFieldsName && (e.Type != null && e.Type.ToLower() == Type.ToLower()) && e.Set == Set);
        //    if (data != null && data.Count() > 0)
        //    {
        //        var response = data.Select(a => a.SSSystemName).FirstOrDefault();
        //        if (response != null)
        //            return response.ToString();
        //        else
        //            return "customFields__" + Guid.NewGuid();
        //    }
        //    else
        //    {
        //        return "customFields__" + Guid.NewGuid();
        //    }

        //}
        private string GetSSSystemNameByEZshredFieldName(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, string EZShredFieldsName, string Type, string Set)
        {
            var response = ObjSharpspringCustomFieldsDataModel.Where(e => e.EZShredFieldName == EZShredFieldsName && e.Type.ToLower() == Type.ToLower() && e.Set == Set).Select(a => a.SSSystemName).FirstOrDefault();
            if (response != null)
                return response.ToString();
            else
                return "customFields__" + Guid.NewGuid();
        }
        private string GetSSSystemNameByEZShredFieldsName(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, string EZShredFieldsName, string Type, string Set)
        {
            return ObjSharpspringCustomFieldsDataModel.Where(e => e.EZShredFieldName == EZShredFieldsName && e.Type == Type && e.Set == Set).Select(s => s.SSSystemName).FirstOrDefault() ?? string.Empty;
        }
        private JObject GetLeadJson(string methodName, long leadId)
        {
            JArray jLeadArray = new JArray();

            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("id", leadId)
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private JObject GetLeadJson(string methodName, string email)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);

            JObject filter = new JObject(new JProperty("emailAddress", email));

            JObject o =
                new JObject(
                    new JProperty("method", methodName),
                    new JProperty("params",
                        new JObject(
                            new JProperty("where", filter)
                                    )
                                  ),
                    new JProperty("id", id)
                            );
            return o;
        }
        private JObject DeleteLeadsJson(string methodName, List<long> leadIds)
        {
            JArray jLeadArray = new JArray();
            foreach (var item in leadIds)
            {
                jLeadArray.Add(new JObject(new JProperty("id", item)));
            }

            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects", jLeadArray
                                           )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private JObject CreateOpportunitiesJson(string methodName, List<SharpspringOpportunityDataModel> opportunitiesData)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            foreach (var item in opportunitiesData)
            {
                jLeadArray.Add(new JObject(
                      new JProperty("ownerID", item.OwnerID),
                      new JProperty("dealStageID", item.DealStageID),
                      new JProperty("opportunityName", item.OpportunityName),
                      new JProperty("isActive", item.IsActive)
                    ));
            }

            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private JObject CreateOpportunitiesWithCustomFields(string methodName, List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            foreach (var item in opportunitiesData)
            {
                jLeadArray.Add(new JObject(
                      new JProperty("ownerID", item.OwnerID),
                      new JProperty("dealStageID", item.DealStageID),
                      new JProperty("opportunityName", item.OpportunityName),
                      new JProperty("isActive", item.IsActive),
                      new JProperty("amount", item.Amount),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "AdditionalTips", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.AdditionalTips),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "JobQuantitySize", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.JobQuantitySize),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedDateOfService", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedDateOfService),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "CertificateOfInsurance", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.CertificateOfInsurance),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HoursOfBusiness", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HoursOfBusiness),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Stairs", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Stairs),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedConsoleDeliveryDate", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedConsoleDeliveryDate),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedStartDate", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedStartDate),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECUnits", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECUnits),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECPriceUnit", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECPriceUnit),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECAdditionalPrice", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECAdditionalPrice),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonUnits_64),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonPriceUnit_64),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonAdditionalPrice_64),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonUnits_96),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonPriceUnit_96),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonAdditionalPrice_96),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "NumberOfTips", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.NumberOfTips),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "PriceQuotedForFirstTip", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.PriceQuotedForFirstTip),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BankerBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BankerBoxes),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "FileBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.FileBoxes),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Bags", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Bags),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Cabinets", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Cabinets),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Skids", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Skids),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HardDrivers", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HardDrivers),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Media", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Media),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Other", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Other),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "SellerComments", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.SellerComments),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HardDrive_Media_Other_Comment", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HardDrive_Media_Other_Comment),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactFirstName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactFirstName),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactLastName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactLastName),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactPhone", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactPhone),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingName),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingStreet", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingStreet),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingCity", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingCity),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingState", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingState),
                      new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ZipCode", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ZipCode)
                    ));
            }

            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }

        private JObject CreateOpportunityLeadsJson(string methodName, long opportunityID, long leadID)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();

            jLeadArray.Add(new JObject(
                  new JProperty("opportunityID", opportunityID),
                  new JProperty("leadID", leadID)
                ));


            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }

        private JObject UpdateOpportunitiesJson(string methodName, List<SharpspringOpportunityDataModel> opportunitiesData)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            foreach (var item in opportunitiesData)
            {
                jLeadArray.Add(new JObject(
                      new JProperty("id", item.OpportunityID),
                      new JProperty("dealStageID", item.DealStageID),
                      new JProperty("opportunityName", item.OpportunityName)
                    ));
            }

            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private JObject UpdateOpportunitiesWithCustomFieldsJson(string methodName, List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            foreach (var item in opportunitiesData)
            {
                if (item.IsClosed)
                {
                    jLeadArray.Add(new JObject(
                          new JProperty("id", item.OpportunityID),
                          new JProperty("dealStageID", item.DealStageID),
                          new JProperty("opportunityName", item.OpportunityName),
                          new JProperty("amount", item.Amount),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "AdditionalTips", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.AdditionalTips),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "JobQuantitySize", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.JobQuantitySize),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedDateOfService", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedDateOfService),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "CertificateOfInsurance", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.CertificateOfInsurance),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HoursOfBusiness", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HoursOfBusiness),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Stairs", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Stairs),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedConsoleDeliveryDate", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedConsoleDeliveryDate),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedStartDate", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedStartDate),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECUnits", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECUnits),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECPriceUnit", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECPriceUnit),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECAdditionalPrice", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECAdditionalPrice),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonUnits_64),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonPriceUnit_64),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonAdditionalPrice_64),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonUnits_96),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonPriceUnit_96),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonAdditionalPrice_96),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "NumberOfTips", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.NumberOfTips),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "PriceQuotedForFirstTip", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.PriceQuotedForFirstTip),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BankerBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BankerBoxes),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "FileBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.FileBoxes),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Bags", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Bags),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Cabinets", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Cabinets),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Skids", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Skids),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HardDrivers", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HardDrivers),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Media", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Media),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Other", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Other),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "SellerComments", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.SellerComments),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HardDrive_Media_Other_Comment", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HardDrive_Media_Other_Comment),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactFirstName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactFirstName),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactLastName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactLastName),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactPhone", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactPhone),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingName),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingStreet", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingStreet),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingCity", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingCity),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingState", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingState),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ZipCode", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ZipCode),
                          new JProperty("isClosed", item.IsClosed)
                        ));
                }
                else
                {
                    jLeadArray.Add(new JObject(
                          new JProperty("id", item.OpportunityID),
                          new JProperty("dealStageID", item.DealStageID),
                          new JProperty("opportunityName", item.OpportunityName),
                          new JProperty("amount", item.Amount),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "AdditionalTips", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.AdditionalTips),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "JobQuantitySize", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.JobQuantitySize),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedDateOfService", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedDateOfService),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "CertificateOfInsurance", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.CertificateOfInsurance),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HoursOfBusiness", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HoursOfBusiness),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Stairs", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Stairs),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedConsoleDeliveryDate", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedConsoleDeliveryDate),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ProposedStartDate", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ProposedStartDate),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECUnits", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECUnits),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECPriceUnit", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECPriceUnit),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ECAdditionalPrice", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ECAdditionalPrice),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonUnits_64),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonPriceUnit_64),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_64", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonAdditionalPrice_64),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonUnits_96),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonPriceUnit_96),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_96", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.GallonAdditionalPrice_96),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "NumberOfTips", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.NumberOfTips),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "PriceQuotedForFirstTip", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.PriceQuotedForFirstTip),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BankerBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BankerBoxes),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "FileBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.FileBoxes),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Bags", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Bags),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Cabinets", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Cabinets),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Skids", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Skids),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HardDrivers", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HardDrivers),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Media", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Media),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "Other", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.Other),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "SellerComments", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.SellerComments),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "HardDrive_Media_Other_Comment", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.HardDrive_Media_Other_Comment),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactFirstName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactFirstName),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactLastName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactLastName),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingContactPhone", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingContactPhone),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingName", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingName),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingStreet", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingStreet),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingCity", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingCity),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "BuildingState", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.BuildingState),
                          new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "ZipCode", SharpspringCustomFieldKind.Opportunity.ToString(), ""), item.ZipCode)
                        ));
                }
            }

            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private JObject UpdateOpportunitiesARCustomerCodeJson(string methodName, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long opportunityID, string arCustomerCode)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            JArray jLeadArray = new JArray();
            jLeadArray.Add(new JObject(
                  new JProperty("id", opportunityID),
                  new JProperty(GetSSSystemNameByEZshredFieldName(ObjSharpspringCustomFieldsDataModel, "OpportunityARCustomerCode", SharpspringCustomFieldKind.Opportunity.ToString(), ""), arCustomerCode)
                ));


            JObject o = new JObject(
                                    new JProperty("method", methodName),
                                    new JProperty("params",
                                    new JObject(
                                        new JProperty("objects",
                                                jLeadArray
                                            )
                                           )
                                    ),
                                     new JProperty("id", id)
                                );

            return o;
        }
        private string BuildUrl()
        {
            return string.Format("{0}?accountID={1}&secretKey={2}", _baseUrl, _accountID, _secretKey);
        }

        private string ExecutePostRestCall(string url, JObject json)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content;
            else
            {
                //LOG NOT OK ERROR
                return string.Empty;
            }
        }

        #endregion


        #region Data Model Converters and Parsers

        private List<SharpspringEventEventData> ParseEventData(JToken eventData)
        {
            List<SharpspringEventEventData> events = new List<SharpspringEventEventData>();
            JObject results = new JObject();

            if (eventData.Type == JTokenType.String)
            {
                if (string.IsNullOrEmpty(eventData.ToString()))
                    return events;

                results = JObject.Parse((string)eventData);
            }
            else if (eventData is JObject)
            {
                results = (JObject)eventData;
            }

            foreach (var item in results)
            {
                events.Add(new SharpspringEventEventData
                {
                    Name = item.Key,
                    Value = item.Value.ToString()
                });
            }

            return events;
        }
        private List<SharpspringFieldLabelDataModel> ConvertToSharpspringFieldLabelDataModel(JObject fieldLabelJson)
        {
            if (!fieldLabelJson.HasValues)
                return new List<SharpspringFieldLabelDataModel>();
            try
            {
                var fields =
                    from x in fieldLabelJson["result"]["field"].Children()
                    select new SharpspringFieldLabelDataModel()
                    {
                        SSSystemName = (string)x["systemName"],
                        Label = (string)x["label"],
                        IsCustom = (string)x["isCustom"],
                        Type = (string)x["relationship"]
                    };

                return fields.Where(x => x.IsCustom == "1").ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringFieldLabelDataModel>();
            }
        }
        private List<SharpspringUserProfileDataModel> ConvertToSharpspringUserProfiles(JObject fieldLabelJson)
        {
            if (!fieldLabelJson.HasValues)
                return new List<SharpspringUserProfileDataModel>();
            try
            {
                var fields =
                    from x in fieldLabelJson["result"]["userProfile"].Children()
                    select new SharpspringUserProfileDataModel()
                    {
                        OwnerID = (string)x["id"],
                        FirstName = (string)x["firstName"],
                        LastName = (string)x["lastName"],
                        EmailAddress = (string)x["emailAddress"],
                        DisplayName = (string)x["displayName"],
                        isActive = (string)x["isActive"],
                        isReseller = (string)x["isReseller"],
                        UserTimezone = (string)x["userTimezone"],
                        Phone = (string)x["phone"]
                    };

                return fields.Where(x => x.isActive == "1").ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringUserProfileDataModel>();
            }
        }
        private List<SharpspringDealStageDataModel> ConvertToSharpspringDealStageDataModel(JObject dealStagesJson)
        {
            if (!dealStagesJson.HasValues)
                return new List<SharpspringDealStageDataModel>();
            try
            {
                var dealStages =
                    from x in dealStagesJson["result"]["dealStage"].Children()
                    select new SharpspringDealStageDataModel()
                    {
                        DealStageID = (long)x["id"],
                        DealStageName = (string)x["dealStageName"],
                        Description = (string)x["description"],
                        DefaultProbability = x.Value<double?>("defaultProbability") ?? 0,
                        Weight = x.Value<int?>("weight") ?? 0,
                        IsEditable = ((string)x["isEditable"] == "1") ? true : false,
                    };

                return dealStages.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringDealStageDataModel>();
            }
        }

        private List<SharpspringEmailDataModel> ConvertToSharpspringEmailDataModel(JObject emailsJson)
        {
            if (!emailsJson.HasValues)
                return new List<SharpspringEmailDataModel>();

            try
            {
                var emails =
                    from x in emailsJson["result"]["getAllemailListings"].Children()
                    select new SharpspringEmailDataModel()
                    {
                        EmailID = (long)x["id"],
                        Subject = (string)x["subject"],
                        Title = (string)x["title"],
                        CreateTimestamp = (DateTime)x["createTimestamp"],
                        Thumbnail = (string)x["thumbnail"],
                    };

                return emails.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringEmailDataModel>();
            }
        }

        private List<SharpspringEmailJobDataModel> ConvertToSharpspringEmailJobDataModel(JObject emailsJson)
        {
            if (!emailsJson.HasValues)
                return new List<SharpspringEmailJobDataModel>();

            try
            {
                var emails =
                    from x in emailsJson["result"]["getAllgetEmailJobss"].Children()
                    select new SharpspringEmailJobDataModel()
                    {
                        EmailJobID = (long)x["id"],
                        IsList = (string)x["isList"] == "1",
                        IsActive = (string)x["isActive"] == "1",
                        RecipientID = (long)x["recipientID"],
                        SendCount = (int)x["sendCount"],
                        CreateTimestamp = (DateTime)x["createTimestamp"],
                    };

                return emails.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringEmailJobDataModel>();
            }
        }

        private List<SharpspringEmailEventDataModel> ConvertToSharpspringEmailEventDataModel(JObject emailsJson)
        {
            if (!emailsJson.HasValues)
                return new List<SharpspringEmailEventDataModel>();

            try
            {
                var emails =
                    from x in emailsJson["result"]["lead"].Children()
                    select new SharpspringEmailEventDataModel()
                    {
                        EmailJobID = (long)x["emailJobID"],
                        LeadID = (long)x["leadID"],
                        EmailID = (long)x["emailID"],
                        CreateDate = (DateTime)x["createDate"],
                        EmailAddress = (string)x["emailAddress"],
                    };

                return emails.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringEmailEventDataModel>();
            }
        }

        private List<SharpspringLeadDataModel> ConvertToSharpspringLeadDataModel(JObject leadDataJson)
        {
            if (!leadDataJson.HasValues)
                return new List<SharpspringLeadDataModel>();

            try
            {
                var leads =
                    from x in leadDataJson["result"]["lead"].Children()
                    select new SharpspringLeadDataModel()
                    {
                        LeadID = (long)x["id"],
                        AccountID = x.Value<long?>("accountID") ?? 0,
                        OwnerID = x.Value<long?>("ownerID") ?? 0,
                        CampaignID = x.Value<long?>("campaignID") ?? 0,
                        LeadStatus = (string)x["leadStatus"],
                        LeadScore = x.Value<int?>("leadScore") ?? 0,
                        LeadScoreWeighted= x.Value<int?>("leadScoreWeighted") ?? 0,
                        IsActive = ((string)x["active"] == "1") ? true : false,
                        FirstName = (string)x["firstName"],
                        LastName = (string)x["lastName"],
                        EmailAddress = (string)x["emailAddress"],
                        CompanyName = (string)x["companyName"],
                        Title = (string)x["title"],
                        Street = (string)x["street"],
                        City = (string)x["city"],
                        Country = (string)x["country"],
                        State = (string)x["state"],
                        ZipCode = (string)x["zipcode"],
                        Website = (string)x["website"],
                        PhoneNumber = (string)x["phoneNumber"],
                        MobilePhoneNumber = (string)x["mobilePhoneNumber"],
                        FaxNumber = (string)x["faxNumber"],
                        Description = (string)x["description"],
                        Industry = (string)x["industry"],
                        IsUnsubscribed = ((string)x["isUnsubscribed"] == "1") ? true : false,
                    };

                return leads.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringLeadDataModel>();
            }
        }
        private List<SharpspringLeadDataModel> ConvertToCustomFieldsSharpspringLeadDataModel(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, JObject leadDataJson, string BuildingSet)
        {
            if (!leadDataJson.HasValues)
                return new List<SharpspringLeadDataModel>();

            try
            {
                var leads =
                    from x in leadDataJson["result"]["lead"].Children()
                    select new SharpspringLeadDataModel()
                    {
                        LeadID = (long)x["id"],
                        AccountID = x.Value<long?>("accountID") ?? 0,
                        OwnerID = x.Value<long?>("ownerID") ?? 0,
                        CampaignID = x.Value<long?>("campaignID") ?? 0,
                        FirstName = (string)x["firstName"],
                        LastName = (string)x["lastName"],
                        CompanyName = (string)x["companyName"],
                        BillingCompanyName = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingCompanyName", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        EmailAddress = (string)x["emailAddress"],
                        ARCustomerCode = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ARCustomerCode", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        NumberOfEmployees = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "NumberOfEmployees", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        Attention = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Attention", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        ReferralSourceID = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ReferralSourceID", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        Notes = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Notes", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        EmailInvoice = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "EmailInvoice", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        BillingZip = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingZip", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        PhoneNumber = (string)x["phoneNumber"],
                        FaxNumber = (string)x["faxNumber"],
                        BillingStreet = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingStreet", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        BillingCity = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingCity", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        Country = (string)x["country"],
                        BillingState = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingState", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        MobilePhoneNumber = (string)x["mobilePhoneNumber"],
                        PipelineStatus = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "PipelineStatus", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        Suite = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Suite", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        EmailCOD = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "EmailCOD", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        CustomerTypeID = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "CustomerTypeID", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        InvoiceTypeID = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "InvoiceTypeID", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        TermID = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "TermID", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        Datasource = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Datasource", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        Email2 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Email2", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        Email3 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Email3", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        BillingContact = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingContact", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        BillingContactExtension = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingContactExtension", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        BillingContactPhone = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingContactPhone", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        NumberOfBoxes = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "NumberOfBoxes", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        CompanyCountryCode = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "CompanyCountryCode", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        ServicesProfessionalType = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ServicesProfessionalType", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        TravelTourismType = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "TravelTourismType", SharpspringCustomFieldKind.Lead.ToString(), "")],
                        BuildingCompanyName = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingCompanyName", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingStreet = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingStreet", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingCity = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingCity", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingState = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingState", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingZip = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingZip", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        SalesmanID = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "SalesmanID", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        Directions = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Directions", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        RoutineInstructions = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "RoutineInstructions", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        ScheduleFrequency = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ScheduleFrequency", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        ServiceTypeID = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ServiceTypeID", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingTypeID = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingTypeID", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingSuite = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingSuite", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingContactFirstName1 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingContactFirstName1", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingContactLastName1 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingContactLastName1", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingContactFirstName2 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingContactFirstName2", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingContactLastName2 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingContactLastName2", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingContactPhone1 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingContactPhone1", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BuildingContactPhone2 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BuildingContactPhone2", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        BillingCountryCode = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BillingCountryCode", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)],
                        TaxExempt = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "TaxExempt", SharpspringCustomFieldKind.Lead.ToString(), BuildingSet)]
                    };
                return leads.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringLeadDataModel>();
            }
        }
        private DateTime CastStringToDateTime(string strAvailableDate)
        {
            return string.IsNullOrEmpty(strAvailableDate) ? System.DateTime.MinValue : Convert.ToDateTime(strAvailableDate);
        }

        private List<SharpspringCampaignDataModel> ConvertToSharpspringCampaignDataModel(JObject campaignsJson)
        {
            if (!campaignsJson.HasValues)
                return new List<SharpspringCampaignDataModel>();

            try
            {
                var campaigns =
                    from x in campaignsJson["result"]["campaign"].Children()
                    select new SharpspringCampaignDataModel()
                    {
                        CampaignID = x.Value<long?>("id") ?? 0,
                        CampaignName = (string)x["campaignName"],
                        CampaignType = (string)x["campaignType"],
                        CampaignAlias = (string)x["campaignAlias"],
                        CampaignOrigin = (string)x["campaignOrigin"],
                        Quantity = x.Value<int?>("qty") ?? 0,
                        Price = x.Value<double?>("price") ?? 0,
                        Goal = x.Value<double?>("goal") ?? 0,
                        OtherCosts = (double)x["otherCosts"],
                        StartDate = string.IsNullOrEmpty((string)x["startDate"]) ? System.DateTime.Today : Convert.ToDateTime(x["startDate"]),
                        EndDate = string.IsNullOrEmpty((string)x["endDate"]) ? System.DateTime.Today.AddDays(1) : Convert.ToDateTime(x["endDate"]),
                        IsActive = ((string)x["isActive"] == "1") ? true : false,
                    };

                return campaigns.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringCampaignDataModel>();
            }
        }

        private List<SharpspringOpportunityDataModel> ConvertToSharpspringOpportunityDataModel(JObject opportunitiesJson)
        {
            if (!opportunitiesJson.HasValues)
                return new List<SharpspringOpportunityDataModel>();

            try
            {
                var opportunities =
                    from x in opportunitiesJson["result"]["opportunity"].Children()
                    select new SharpspringOpportunityDataModel()
                    {
                        OpportunityID = x.Value<long?>("id") ?? 0,
                        OwnerID = x.Value<long?>("ownerID") ?? 0,
                        PrimaryLeadID = x.Value<long?>("primaryLeadID") ?? 0,
                        DealStageID = x.Value<long?>("dealStageID") ?? 0,
                        AccountID = x.Value<long?>("accountID") ?? 0,
                        CampaignID = x.Value<long?>("campaignID") ?? 0,
                        OpportunityName = (string)x["opportunityName"],
                        Probability = x.Value<double?>("probability") ?? 0,
                        Amount = x.Value<double?>("amount") ?? 0,
                        IsClosed = ((string)x["isClosed"] == "1") ? true : false,
                        IsWon = ((string)x["isWon"] == "1") ? true : false,
                        IsActive = ((string)x["isActive"] == "1") ? true : false,
                        CloseDate = ((string)x["closeDate"] == null) ? (DateTime?)null : Convert.ToDateTime(x["closeDate"]),
                    };

                return opportunities.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringOpportunityDataModel>();
            }
        }

        private long ConvertSSToDealStageId(JObject opportunitiesJson)
        {
            if (!opportunitiesJson.HasValues)
                return 0;

            try
            {
                var opportunities =
                    from x in opportunitiesJson["result"]["opportunity"].Children()
                    select new SharpspringOpportunityDataModel()
                    {
                        DealStageID = x.Value<long?>("dealStageID") ?? 0,
                    };
                if (opportunities != null && opportunities.Count() > 0)
                {
                    return opportunities.FirstOrDefault() != null ? opportunities.FirstOrDefault().DealStageID : 0;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return 0;
        }

        private List<SharpspringOpportunityDataModel> ConvertToSharpspringOpportunityDataModelWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, JObject opportunitiesJson)
        {
            if (!opportunitiesJson.HasValues)
                return new List<SharpspringOpportunityDataModel>();

            try
            {
                var opportunities =
                    from x in opportunitiesJson["result"]["opportunity"].Children()
                    select new SharpspringOpportunityDataModel()
                    {
                        OpportunityID = x.Value<long?>("id") ?? 0,
                        OwnerID = x.Value<long?>("ownerID") ?? 0,
                        PrimaryLeadID = x.Value<long?>("primaryLeadID") ?? 0,
                        DealStageID = x.Value<long?>("dealStageID") ?? 0,
                        AccountID = x.Value<long?>("accountID") ?? 0,
                        CampaignID = x.Value<long?>("campaignID") ?? 0,
                        OpportunityName = (string)x["opportunityName"],
                        Probability = x.Value<double?>("probability") ?? 0,
                        Amount = x.Value<double?>("amount") ?? 0,
                        IsClosed = ((string)x["isClosed"] == "1") ? true : false,
                        IsWon = ((string)x["isWon"] == "1") ? true : false,
                        IsActive = ((string)x["isActive"] == "1") ? true : false,
                        CloseDate = ((string)x["closeDate"] == null) ? (DateTime?)null : Convert.ToDateTime(x["closeDate"]),
                        AdditionalTips = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "AdditionalTips", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        JobQuantitySize = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "JobQuantitySize", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        ProposedDateOfService = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ProposedDateOfService", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        CertificateOfInsurance = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "CertificateOfInsurance", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        HoursOfBusiness = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "HoursOfBusiness", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        Stairs = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Stairs", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        ProposedConsoleDeliveryDate = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ProposedConsoleDeliveryDate", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        ProposedStartDate = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ProposedStartDate", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        ECUnits = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ECUnits", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        ECPriceUnit = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ECPriceUnit", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        ECAdditionalPrice = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "ECAdditionalPrice", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        GallonUnits_64 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_64", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        GallonPriceUnit_64 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_64", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        GallonAdditionalPrice_64 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_64", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        GallonUnits_96 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "GallonUnits_96", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        GallonPriceUnit_96 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "GallonPriceUnit_96", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        GallonAdditionalPrice_96 = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "GallonAdditionalPrice_96", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        NumberOfTips = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "NumberOfTips", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        PriceQuotedForFirstTip = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "PriceQuotedForFirstTip", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        BankerBoxes = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "BankerBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        FileBoxes = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "FileBoxes", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        Bags = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Bags", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        Cabinets = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Cabinets", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        Skids = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Skids", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        HardDrivers = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "HardDrivers", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        Media = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Media", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        Other = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "Other", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        SellerComments = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "SellerComments", SharpspringCustomFieldKind.Opportunity.ToString(), "")],
                        HardDrive_Media_Other_Comment = (string)x[GetSSSystemNameByEZShredFieldsName(ObjSharpspringCustomFieldsDataModel, "HardDrive_Media_Other_Comment", SharpspringCustomFieldKind.Opportunity.ToString(), "")]
                    };

                return opportunities.ToList();
            }
            catch (Exception e)
            {
                return new List<SharpspringOpportunityDataModel>();
            }
        }
        #endregion

        #region SubscribeToLeadUpdates
        public string SubscribeToLeadUpdates(string subscribedUrl)
        {
            JObject SubscribeToLeadUpdatesResponse = SubscribeToLeadUpdates(SharpspringMethodKinds.subscribeToLeadUpdates.ToString(), subscribedUrl);
            return SubscribeToLeadUpdatesResponse.ToString();
        }
        private JObject SubscribeToLeadUpdates(string methodName, string subscribedUrl)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);
            string url = BuildUrl();
            JObject o = new JObject();

            JObject json = SubscribeToLeadUpdatesJSON(methodName, subscribedUrl);
            string responseText = ExecutePostRestCall(url, json);

            if (string.IsNullOrEmpty(responseText))
                return o;

            o = JObject.Parse(responseText);
            return o;
        }
        private JObject SubscribeToLeadUpdatesJSON(string methodName, string subscribedUrl)
        {
            string id = string.Format("{0}", System.DateTime.Now.Ticks);

            JObject o =
                new JObject(
                    new JProperty("method", methodName),
                    new JProperty("params",
                        new JObject(
                            new JProperty("url", subscribedUrl)
                                    )
                                  ),
                    new JProperty("id", id)
                            );

            return o;
        }
        #endregion
    }
}