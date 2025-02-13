using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ISubscribeLeadUpdatesLogServices
    {
        CreateNotificationReturn AddSubscribeLeadUpdatesLog(SubscribeLeadUpdatesLogsDataModel SLULogs);
    }
    public interface ISubscribeLeadUpdatesLogServicesInternal : ISubscribeLeadUpdatesLogServices
    {
    }
}
