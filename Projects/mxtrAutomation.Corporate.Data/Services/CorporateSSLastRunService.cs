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
using mxtrAutomation.Corporate.Data.Enums;
using System.Text.RegularExpressions;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class CorporateSSLastRunService : MongoRepository<CorporateSSLastRun>, ICorporateSSLastRunServiceInternal
    {
        public DateTime GetCorporateSSLastRun(string accountObjectId)
        {
            using (MongoRepository<CorporateSSLastRun> repo = new MongoRepository<CorporateSSLastRun>())
            {
                var data = repo.Where(w => w.AccountObjectID.ToLower() == accountObjectId).OrderByDescending(x => x.LastRun).ToList();
                if (data != null && data.Count() > 0)
                {
                    return data.FirstOrDefault().LastRun;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        public void AddCorporateSSLastRunDate(string accountObjectId, DateTime lastRun)
        {
            using (MongoRepository<CorporateSSLastRun> repo = new MongoRepository<CorporateSSLastRun>())
            {
                CorporateSSLastRun entity = new CorporateSSLastRun
                {
                    AccountObjectID = accountObjectId,
                    LastRun = lastRun,
                    CreateDate = DateTime.UtcNow,
                };
                repo.Add(entity);
            }
        }
    }
}
