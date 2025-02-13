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
    public interface ICRMCampaignService
    {
        CreateNotificationReturn CreateCRMCampaign(CRMCampaignDataModel crmCampaignDataModel);
        CreateNotificationReturn CreateBatchCRMCampaigns(List<CRMCampaignDataModel> crmCampaignDataModels);

        CreateNotificationReturn UpdateCampaigns(List<CRMCampaignDataModel> campaignDatas, string accountObjectID, string crmKind);

        List<CRMCampaignDataModel> GetCampaignsByAccountObjectIDs(List<string> accountObjectIDs);
        IEnumerable<CRMCampaignDataModel> GetCampaignsByAccountObjectIDs(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
    }

    public interface ICRMCampaignServiceInternal : ICRMCampaignService
    {
    }
}
