using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public interface ISharpspringApi
    {
        string SecretKey { get; set; }
        string AccountID { get; set; }

        #region Leads

        List<SharpspringLeadDataModel> GetAllLeads();
        List<SharpspringLeadDataModel> GetLeadsForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind);
        string CreateLead(List<SharpspringLeadDataModel> lstLeadData);
        string CreateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet);
        string UpdateLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, SharpspringLeadDataModel objSharpSpringDataModel, string BuildingSet);
        SharpspringLeadDataModel GetLead(long leadId);
        SharpspringLeadDataModel GetLead(string email);
        SharpspringLeadDataModel GetLeadWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadId, string BuildingSet);
        string UpdateLead(List<SharpspringLeadDataModel> lstLeadData);
        string DeleteLead(List<long> leadIds);
        string UpdateLeadARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long leadID, string arCustomerCode);
        #endregion

        #region Events

        List<SharpspringEventDataModel> GetEventsByDate(DateTime startDate);

        #endregion

        #region Emails
        List<SharpspringEmailDataModel> GetAllEmails();
        List<SharpspringEmailJobDataModel> GetAllEmailJobs();

        List<SharpspringEmailEventDataModel> GetEmailEvents(long emailJobID, string eventType);

        #endregion

        #region Deal Stages

        List<SharpspringDealStageDataModel> GetAllDealStages();
        List<SharpspringDealStageDataModel> GetDealStagesForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind);

        #endregion

        #region Campaigns

        List<SharpspringCampaignDataModel> GetAllCampaigns();
        List<SharpspringCampaignDataModel> GetCampaignsForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind);

        #endregion

        #region Opportunities

        List<SharpspringOpportunityDataModel> GetAllOpportunities();
        List<SharpspringOpportunityDataModel> GetOpportunitiesWithCustomFields(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long OpportunityID);
        List<SharpspringOpportunityDataModel> GetOpportunitiesForDateRange(DateTime startDate, DateTime endDate, ActionKind actionKind);
        string CreateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData);
        string CreateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel);
        string CreateOpportunityLeads(long opportunityID, long leadID);
        string UpdateOpportunities(List<SharpspringOpportunityDataModel> opportunitiesData);
        string UpdateOpportunitiesWithCustomFields(List<SharpspringOpportunityDataModel> opportunitiesData, List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel);
        List<SharpspringOpportunityDataModel> GetOpportunitiesById(long opportunityId);
        long GetDealStageIdByOpportunityId(long opportunityId);
        string UpdateOpportunitiesARCustomerCode(List<SharpspringCustomFieldsDataModel> ObjSharpspringCustomFieldsDataModel, long opportunityId, string arCustomerCode);

        #endregion

        #region GetCustomFields
        List<SharpspringFieldLabelDataModel> GetCustomFields();
        #endregion

        #region GetUserProfiles
        List<SharpspringUserProfileDataModel> GetUserProfiles();
        #endregion

        #region GetActiveLists
        SharpspringGetActiveListDataModel GetActiveLists(string id);
        #endregion

        #region SubscribeToLeadUpdates
        string SubscribeToLeadUpdates(string subscribedUrl);
        #endregion
    }
}
