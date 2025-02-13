using mxtrAutomation.Corporate.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Services
{
    public interface IEZShredFieldLabelMappingService
    {
        CreateNotificationReturn AddFieldLabel(EZShredFieldLabelMappingDataModel fields);
        CreateNotificationReturn AddFieldLabel(List<EZShredFieldLabelMappingDataModel> fields);
        List<EZShredFieldLabelMappingDataModel> GetAllFieldLabels();
        int IsEZShredFieldLabelExist();
    }
    public interface IEZShredFieldLabelMappingServiceInternal : IEZShredFieldLabelMappingService
    {
    }
}
