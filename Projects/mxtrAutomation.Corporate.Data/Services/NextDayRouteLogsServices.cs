using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;
using System.Linq;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class NextDayRouteLogsServices : MongoRepository<NextDayRouteLogs>, INextDayRouteLogsServicesInternal
    {
        public NextDayRouteLogsDataModel AddNextDayRouteLogs(NextDayRouteLogsDataModel NextDayRouteLogsDataModel)
        {
            using (MongoRepository<NextDayRouteLogs> repo = new MongoRepository<NextDayRouteLogs>())
            {
                NextDayRouteLogs entity = Getdata(NextDayRouteLogsDataModel);
                repo.Add(entity);
                return AdaptData(entity);
            }
        }
        public NextDayRouteLogsDataModel CheckNextDayRouteTicketByAccountID(string AccountObjectId)
        {
            using (MongoRepository<NextDayRouteLogs> repo = new MongoRepository<NextDayRouteLogs>())
            {
                NextDayRouteLogs data = repo.FirstOrDefault(n => n.AccountObjectID == AccountObjectId);
                return AdaptData(data);
            }
        }
        private NextDayRouteLogs Getdata(NextDayRouteLogsDataModel data)
        {
            return new NextDayRouteLogs()
            {
                Id = data.Id,
                AccountObjectID = data.AccountObjectID,
                MxtrAccountID = data.MxtrAccountID,
                UserID = data.UserID,
                MxtrUserID = data.MxtrUserID,
                LocationName = data.LocationName,
                NextRunDate = data.NextRunDate,
                APIResponse = data.APIResponse,
                TicketGenerateFrom = data.TicketGenerateFrom,
                TicketGenerateRequestDate = DateTime.Now.ToString()
            };
        }
        private NextDayRouteLogsDataModel AdaptData(NextDayRouteLogs data)
        {
            if (data == null)
            {
                return new NextDayRouteLogsDataModel();
            }
            return new NextDayRouteLogsDataModel()
            {
                Id = data.Id,
                AccountObjectID = data.AccountObjectID,
                MxtrAccountID = data.MxtrAccountID,
                UserID = data.UserID,
                MxtrUserID = data.MxtrUserID,
                LocationName = data.LocationName,
                NextRunDate = data.NextRunDate,
                APIResponse = data.APIResponse,
                TicketGenerateFrom = data.TicketGenerateFrom
            };
        }
    }
}
