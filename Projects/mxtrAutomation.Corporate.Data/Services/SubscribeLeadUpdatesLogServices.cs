using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class SubscribeLeadUpdatesLogServices : MongoRepository<SubscribeLeadUpdatesLogs>, ISubscribeLeadUpdatesLogServicesInternal
    {
        public CreateNotificationReturn AddSubscribeLeadUpdatesLog(SubscribeLeadUpdatesLogsDataModel SLULogsDataModel)
        {
            try
            {
                SubscribeLeadUpdatesLogs entity = new SubscribeLeadUpdatesLogs
                {
                    AccountObjectID = SLULogsDataModel.AccountObjectID,
                    LocationName = SLULogsDataModel.LocationName,
                    Action = SLULogsDataModel.Action,
                    Status = SLULogsDataModel.Status,
                    RequestFrom = SLULogsDataModel.RequestFrom,
                    SSPostRequestString = SLULogsDataModel.SSPostRequestString,
                    LogCreateDate = DateTime.Now.ToString()

                };
                using (MongoRepository<SubscribeLeadUpdatesLogs> repo = new MongoRepository<SubscribeLeadUpdatesLogs>())
                {
                    repo.Add(entity);
                    return new CreateNotificationReturn { Success = true, ObjectID = entity.Id };
                }
            }
            catch (Exception ex)
            {

                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }
        }
    }
}
