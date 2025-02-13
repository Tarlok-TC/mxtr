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
    public interface ICRMEmailService
    {
        CreateNotificationReturn CreateCRMEmail(CRMEmailDataModel crmEmailDataModel);
        CreateNotificationReturn CreateBatchCRMEmails(List<CRMEmailDataModel> crmEmailDataModels);
        List<CRMEmailDataModel> GetEmailsByAccountObjectIDs(List<string> accountObjectIDs);
        IEnumerable<CRMEmailDataModel> GetEmailsByAccountObjectIDsAsEnumerable(List<string> accountObjectIDs);
        CreateNotificationReturn UpdateEmail(CRMEmailDataModel emailData);
        CreateNotificationReturn UpdateEmails(List<CRMEmailDataModel> emailDatas, string accountObjectID, string crmKind);
        CreateNotificationReturn UpsertEmails(List<CRMEmailDataModel> emailDatas, string accountObjectID, string crmKind);
    }

    public interface ICRMEmailServiceInternal : ICRMEmailService
    {
    }
}
