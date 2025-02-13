using mxtrAutomation.Corporate.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class MinerRunScheduleLogDataModel
    {
        public string MinerType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsRunSuccessfully { get; set; }
        public string Desciption { get; set; }
        public string Error { get; set; }
    }
}
