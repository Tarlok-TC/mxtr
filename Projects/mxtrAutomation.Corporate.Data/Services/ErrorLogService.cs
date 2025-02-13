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
    public class ErrorLogService : MongoRepository<Account>, IErrorLogServiceInternal
    {
        public bool CreateErrorLog(ErrorLogModel errorData)
        {
            ErrorLog entry = new ErrorLog()
            {
                LogTime = errorData.LogTime,
                Description = errorData.Description,
                LogType = errorData.LogType,
                ErrorMessage = errorData.ErrorMessage,
            };

            using (MongoRepository<ErrorLog> repo = new MongoRepository<ErrorLog>())
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

    public interface IErrorLogServiceInternal : IErrorLogService
    {
    }
}
