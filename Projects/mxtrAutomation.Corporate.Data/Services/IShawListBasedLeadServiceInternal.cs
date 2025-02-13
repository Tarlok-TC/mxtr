using mxtrAutomation.Corporate.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IShawListBasedLeadService
    {
        CreateNotificationReturn AddUpdateShawListBasedData(ShawListBasedDataModel data);
        ShawListBasedDataModel GetShawListBasedData(string accountObjectId, DateTime startDate, DateTime endDate);
    }
    public interface IShawListBasedLeadServiceInternal : IShawListBasedLeadService
    {
    }
}
