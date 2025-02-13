using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ICRMOpportunityService
    {
        CreateNotificationReturn CreateCRMOpportunity(CRMOpportunityDataModel crmOpportunityDataModel);
        CreateNotificationReturn CreateBatchCRMOpportunities(List<CRMOpportunityDataModel> crmOpportunityDataModels);

        CreateNotificationReturn UpdateOpportunities(List<CRMOpportunityDataModel> opportunitiesDatas, string accountObjectID, string crmKind);
    }

    public interface ICRMOpportunityServiceInternal : ICRMOpportunityService
    {
    }
}
