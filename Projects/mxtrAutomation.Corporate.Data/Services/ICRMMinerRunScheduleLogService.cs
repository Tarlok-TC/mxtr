using mxtrAutomation.Corporate.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface ICRMMinerRunScheduleLogService
    {
        bool AddMinerRunLog(MinerRunScheduleLogDataModel logData);
    }
    public interface ICRMMinerRunScheduleLogServiceInternal : ICRMMinerRunScheduleLogService
    {
    }
}
