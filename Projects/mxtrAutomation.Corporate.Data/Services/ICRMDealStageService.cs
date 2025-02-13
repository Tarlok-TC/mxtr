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
    public interface ICRMDealStageService
    {
        CreateNotificationReturn CreateCRMDealStage(CRMDealStageDataModel crmDealStageDataModel);
        CreateNotificationReturn CreateBatchCRMDealStages(List<CRMDealStageDataModel> crmDealStageDataModels);

        CreateNotificationReturn UpdateDealStages(List<CRMDealStageDataModel> dealStageDatas, string accountObjectID, string crmKind);
    }

    public interface ICRMDealStageServiceInternal : ICRMDealStageService
    {
    }
}
