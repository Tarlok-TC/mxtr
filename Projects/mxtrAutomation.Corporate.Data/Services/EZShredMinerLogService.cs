using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class EZShredMinerLogService : MongoRepository<EZShredMinerLog>, IEZShredMinerLogServiceInternal
    {
        public CreateNotificationReturn CreateEZMinerlog(EZShredMinerLogDataModel logData)
        {
            EZShredMinerLog entry = new EZShredMinerLog()
            {
                AccountObjectId = logData.AccountObjectId,
                LocationName = logData.LocationName,
                Port = logData.Port,
                CreatedOn = DateTime.UtcNow,
            };

            using (MongoRepository<EZShredMinerLog> repo = new MongoRepository<EZShredMinerLog>())
            {
                try
                {
                    repo.Add(entry);
                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateEZMinerlog(EZShredMinerLogDataModel logData)
        {
            using (MongoRepository<EZShredMinerLog> repo = new MongoRepository<EZShredMinerLog>())
            {
                try
                {
                    var data = repo.FirstOrDefault(x => x.AccountObjectId == logData.AccountObjectId);
                    if (data != null)
                    {
                        if (data.APIDetails == null)
                        {
                            data.APIDetails = new List<APIDetail>();
                        }
                        data.APIDetails.Add(logData.APIDetails.Select(x => new APIDetail()
                        {
                            APIName = x.APIName,
                            HitTime = x.HitTime,
                            ReturnResponse = x.ReturnResponse,
                            ResponseTime = x.ResponseTime,
                            ReturnResult = x.ReturnResult,
                        }).ToList()[0]);

                        repo.Update(data);

                        return new CreateNotificationReturn { Success = true, ObjectID = data.Id };
                    }
                    else
                    {
                        EZShredMinerLog entry = new EZShredMinerLog()
                        {
                            AccountObjectId = logData.AccountObjectId,
                            LocationName = logData.LocationName,
                            Port = logData.Port,
                            CreatedOn = DateTime.UtcNow,
                        };
                        repo.Add(entry);

                        return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                    }
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public bool IsEZMinerRecordExist(string accountObjectId)
        {
            using (MongoRepository<EZShredMinerLog> repo = new MongoRepository<EZShredMinerLog>())
            {
                try
                {
                    var data = repo.FirstOrDefault(x => x.AccountObjectId == accountObjectId);
                    if (data == null)
                    {
                        return false;
                    }
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
