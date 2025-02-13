using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public enum SharpspringMethodKinds
    {
        getLead,
        getLeads,
        getLeadsDateRange,
        getDealStage,
        getDealStages,
        getDealStagesDateRange,
        getUserProfiles,
        getOpportunity,
        getOpportunities,
        getOpportunitiesDateRange,
        createOpportunities,
        createOpportunityLeads,
        updateOpportunities,
        getAccount,
        getAccounts,
        getAccountsDateRange,
        getEvents,
        getEmailJobs,
        getEmailEvents,
        getEmail,
        getEmailListing,
        getCampaign,
        getCampaigns,
        getCampaignsDateRange,
        createLeads,
        updateLeads,
        deleteLeads,
        getFields,
        getActiveLists,
        subscribeToLeadUpdates
    }
}
