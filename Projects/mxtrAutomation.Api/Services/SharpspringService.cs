using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Api.Sharpspring;

namespace mxtrAutomation.Api.Services
{
    public interface ISharpspringService
    {
        void SetConnectionTokens(string secretKey, string accountID);

        List<SharpspringLeadDataModel> GetAllLeads();
        List<SharpspringLeadDataModel> GetNewLeadsForDateRange(DateTime startDate, DateTime endDate);
        List<SharpspringLeadDataModel> GetUpdatedLeadsForDateRange(DateTime startDate, DateTime endDate);
        List<SharpspringLeadDataModel> GetLeadsForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind);
        string CreateLead(List<SharpspringLeadDataModel> lstLeadData);
        SharpspringLeadDataModel GetLead(long leadId);
        SharpspringLeadDataModel GetLead(string email);
        SharpspringLeadDataModel GetLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadId, string BuildingSet);
        string CreateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet);
        string UpdateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet);
        string UpdateLead(List<SharpspringLeadDataModel> lstLeadData);
        string DeleteLead(List<long> leadIds);
        string UpdateLeadARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadID, string arCustomerCode);

        List<SharpspringEventDataModel> GetEventsByDate(DateTime startDate);

        List<SharpspringEmailJobDataModel> GetAllEmailJobs();
        List<SharpspringEmailDataModel> GetAllEmails();
        List<SharpspringEmailEventDataModel> GetEmailEvents(long emailJobID, string eventType);

        List<SharpspringDealStageDataModel> GetAllDealStages();
        List<SharpspringDealStageDataModel> GetNewDealStagesForDateRange(DateTime startDate, DateTime endDate);
        List<SharpspringDealStageDataModel> GetUpdatedDealStagesForDateRange(DateTime startDate, DateTime endDate);

        List<SharpspringCampaignDataModel> GetAllCampaigns();
        List<SharpspringCampaignDataModel> GetNewCampaignsForDateRange(DateTime startDate, DateTime endDate);
        List<SharpspringCampaignDataModel> GetUpdatedCampaignsForDateRange(DateTime startDate, DateTime endDate);

        List<SharpspringOpportunityDataModel> GetAllOpportunities();
        List<SharpspringOpportunityDataModel> GetOpportunitiesWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long OpportunityID);
        List<SharpspringOpportunityDataModel> GetNewOpportunitiesForDateRange(DateTime startDate, DateTime endDate);
        List<SharpspringOpportunityDataModel> GetUpdatedOpportunitiesForDateRange(DateTime startDate, DateTime endDate);
        string CreateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData);
        string CreateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel);
        List<SharpspringFieldLabelDataModel> GetCustomFields();
        List<SharpspringUserProfileDataModel> GetUserProfiles();
        string CreateOpportunityLeads(long opportunityID, long leadID);
        string UpdateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData);
        string UpdateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel);
        List<SharpspringOpportunityDataModel> GetOpportunitiesById(long opportunityId);
        long GetDealStageIdByOpportunityId(long opportunityId);
        string UpdateOpportunitiesARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long opportunityId, string arCustomerCode);
        SharpspringGetActiveListDataModel GetActiveLists(string id);
        string SubscribeToLeadUpdates(string subscribedUrl);
    }

    public class SharpspringService : ISharpspringService
    {
        private readonly ISharpspringApi _sharpspringApi;

        public SharpspringService(ISharpspringApi argSharpspringApi)
        {
            _sharpspringApi = argSharpspringApi;
        }

        public void SetConnectionTokens(string secretKey, string accountID)
        {
            _sharpspringApi.SecretKey = secretKey;
            _sharpspringApi.AccountID = accountID;
        }

        #region Leads

        public List<SharpspringLeadDataModel> GetAllLeads()
        {
            return _sharpspringApi.GetAllLeads();
        }

        public List<SharpspringLeadDataModel> GetNewLeadsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetLeadsForDateRange(startDate, endDate, ActionKind.create);
        }

        public List<SharpspringLeadDataModel> GetUpdatedLeadsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetLeadsForDateRange(startDate, endDate, ActionKind.update);
        }

        public List<SharpspringLeadDataModel> GetLeadsForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind)
        {
            return _sharpspringApi.GetLeadsForDateRange(startDate, endDate, actionKind);
        }

        public string CreateLead(List<SharpspringLeadDataModel> lstLeadData)
        {
            return _sharpspringApi.CreateLead(lstLeadData);
        }
        public string CreateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet)
        {
            return _sharpspringApi.CreateLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, objSharpSpringDataModel, BuildingSet);
        }
        public string UpdateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet)
        {
            return _sharpspringApi.UpdateLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, objSharpSpringDataModel, BuildingSet);
        }
        public string UpdateLeadARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadID, string arCustomerCode)
        {
            return _sharpspringApi.UpdateLeadARCustomerCode(ObjSharpspringCustomFieldsDataModel, leadID, arCustomerCode);
        }
        public SharpspringLeadDataModel GetLead(long leadId)
        {
            return _sharpspringApi.GetLead(leadId);
        }
        public SharpspringLeadDataModel GetLead(string email)
        {
            return _sharpspringApi.GetLead(email);
        }
        public SharpspringLeadDataModel GetLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadId, string BuildingSet)
        {
            return _sharpspringApi.GetLeadWithCustomFields(ObjSharpspringCustomFieldsDataModel, leadId, BuildingSet);
        }
        public string UpdateLead(List<SharpspringLeadDataModel> lstLeadData)
        {
            return _sharpspringApi.UpdateLead(lstLeadData);
        }

        public string DeleteLead(List<long> leadIds)
        {
            return _sharpspringApi.DeleteLead(leadIds);
        }
        #endregion


        #region Events

        public List<SharpspringEventDataModel> GetEventsByDate(DateTime startDate)
        {
            return _sharpspringApi.GetEventsByDate(startDate);
        }

        #endregion


        #region Emails

        public List<SharpspringEmailDataModel> GetAllEmails()
        {
            return _sharpspringApi.GetAllEmails();
        }

        public List<SharpspringEmailJobDataModel> GetAllEmailJobs()
        {
            return _sharpspringApi.GetAllEmailJobs();
        }

        public List<SharpspringEmailEventDataModel> GetEmailEvents(long emailJobID, string eventType)
        {
            return _sharpspringApi.GetEmailEvents(emailJobID, eventType);

        }
        #endregion

        #region Deal Stages

        public List<SharpspringDealStageDataModel> GetAllDealStages()
        {
            return _sharpspringApi.GetAllDealStages();
        }

        public List<SharpspringDealStageDataModel> GetNewDealStagesForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetDealStagesForDateRange(startDate, endDate, ActionKind.create);
        }

        public List<SharpspringDealStageDataModel> GetUpdatedDealStagesForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetDealStagesForDateRange(startDate, endDate, ActionKind.update);
        }

        #endregion


        #region Campaigns

        public List<SharpspringCampaignDataModel> GetAllCampaigns()
        {
            return _sharpspringApi.GetAllCampaigns();
        }

        public List<SharpspringCampaignDataModel> GetNewCampaignsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetCampaignsForDateRange(startDate, endDate, ActionKind.create);
        }

        public List<SharpspringCampaignDataModel> GetUpdatedCampaignsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetCampaignsForDateRange(startDate, endDate, ActionKind.update);
        }

        #endregion


        #region Opportunities

        public List<SharpspringOpportunityDataModel> GetAllOpportunities()
        {
            return _sharpspringApi.GetAllOpportunities();
        }
        public List<SharpspringOpportunityDataModel> GetOpportunitiesWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long OpportunityID)
        {
            return _sharpspringApi.GetOpportunitiesWithCustomFields(ObjSharpspringCustomFieldsDataModel, OpportunityID);
        }
        public List<SharpspringOpportunityDataModel> GetNewOpportunitiesForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetOpportunitiesForDateRange(startDate, endDate, ActionKind.create);
        }

        public List<SharpspringOpportunityDataModel> GetUpdatedOpportunitiesForDateRange(DateTime startDate, DateTime endDate)
        {
            return _sharpspringApi.GetOpportunitiesForDateRange(startDate, endDate, ActionKind.update);
        }

        public string CreateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData)
        {
            return _sharpspringApi.CreateOpportunities(opportunitiesData);
        }
        public string CreateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel)
        {
            return _sharpspringApi.CreateOpportunitiesWithCustomFields(opportunitiesData, ObjSharpspringCustomFieldsDataModel);
        }
        public string CreateOpportunityLeads(long opportunityID, long leadID)
        {
            return _sharpspringApi.CreateOpportunityLeads(opportunityID, leadID);
        }

        public string UpdateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData)
        {
            return _sharpspringApi.UpdateOpportunities(opportunitiesData);
        }
        public string UpdateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel)
        {
            return _sharpspringApi.UpdateOpportunitiesWithCustomFields(opportunitiesData, ObjSharpspringCustomFieldsDataModel);
        }
        public List<SharpspringOpportunityDataModel> GetOpportunitiesById(long opportunityId)
        {
            return _sharpspringApi.GetOpportunitiesById(opportunityId);
        }
        public long GetDealStageIdByOpportunityId(long opportunityId)
        {
            return _sharpspringApi.GetDealStageIdByOpportunityId(opportunityId);
        }
        public string UpdateOpportunitiesARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long opportunityId, string arCustomerCode)
        {
            return _sharpspringApi.UpdateOpportunitiesARCustomerCode(ObjSharpspringCustomFieldsDataModel, opportunityId, arCustomerCode);
        }


        #endregion

        #region GetCustomFields
        public List<SharpspringFieldLabelDataModel> GetCustomFields()
        {
            return _sharpspringApi.GetCustomFields();
        }
        #endregion

        #region GetUserProfiles
        public List<SharpspringUserProfileDataModel> GetUserProfiles()
        {
            return _sharpspringApi.GetUserProfiles();
        }
        #endregion

        #region GetActiveLists
        public SharpspringGetActiveListDataModel GetActiveLists(string id)
        {
            return _sharpspringApi.GetActiveLists(id);
        }
        #endregion

        #region SubscribeToLeadUpdates
        public string SubscribeToLeadUpdates(string subscribedUrl)
        {
            return _sharpspringApi.SubscribeToLeadUpdates(subscribedUrl);
        }
        #endregion
    }
}
