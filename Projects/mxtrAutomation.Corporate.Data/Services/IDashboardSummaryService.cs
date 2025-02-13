using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IDashboardSummaryService
    {
        DashboardSummaryDataModel GetDashboardSummaryByAccountObjectID(string accountObjectID, int year, int month);
        List<DashboardSummaryDataModel> GetDashboardSummaries(List<string> accountObjectIDs, DateTime startDate, DateTime endDate);
    }

    public interface IDashboardSummaryServiceInternal : IDashboardSummaryService
    {
    }
}
