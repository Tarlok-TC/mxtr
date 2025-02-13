using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface INextDayRouteLogsServices
    {
        NextDayRouteLogsDataModel AddNextDayRouteLogs(NextDayRouteLogsDataModel NextDayRouteLogsDataModel);
        NextDayRouteLogsDataModel CheckNextDayRouteTicketByAccountID(string AccountObjectId);
    }
    public interface INextDayRouteLogsServicesInternal : INextDayRouteLogsServices
    {
    }
}
