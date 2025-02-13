using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IEZShredMinerLogService
    {
        CreateNotificationReturn CreateEZMinerlog(EZShredMinerLogDataModel logData);
        CreateNotificationReturn UpdateEZMinerlog(EZShredMinerLogDataModel logData);
        bool IsEZMinerRecordExist(string accountObjectId);
    }
    public interface IEZShredMinerLogServiceInternal : IEZShredMinerLogService
    {
    }
}