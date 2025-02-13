using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class CRMMinerRunScheduleLogService : MongoRepository<MinerRunScheduleLog>, ICRMMinerRunScheduleLogServiceInternal
    {
        public bool AddMinerRunLog(MinerRunScheduleLogDataModel logData)
        {
            MinerRunScheduleLog entry = new MinerRunScheduleLog()
            {
                EndTime = logData.EndTime,
                Error = logData.Error,
                IsRunSuccessfully = logData.IsRunSuccessfully,
                MinerType = logData.MinerType,
                StartTime = logData.StartTime,
                Desciption =logData.Desciption,          
            };

            using (MongoRepository<MinerRunScheduleLog> repo = new MongoRepository<MinerRunScheduleLog>())
            {
                try
                {
                    repo.Add(entry);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }
}
