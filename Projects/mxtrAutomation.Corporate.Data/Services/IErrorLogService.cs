using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IErrorLogService
    {
        bool CreateErrorLog(ErrorLogModel accountData);
    }
}
