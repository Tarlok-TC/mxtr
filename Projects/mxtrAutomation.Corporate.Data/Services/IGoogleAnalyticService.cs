using mxtrAutomation.Corporate.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IGoogleAnalyticService
    {
        IEnumerable<GASearchDataModel> GetGoogleAnalyticData(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
    }

    public interface IGoogleAnalyticServiceInternal : IGoogleAnalyticService
    {
    }
}
