using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class MinerRunMinerDetails
    {
        public string MinerName { get; set; }
        public bool IsFirstMinerRunComplete { get; set; }
        public DateTime LastMinerRunTime { get; set; }
        public DateTime LastStartDateForDataCollection { get; set; }
        public DateTime LastEndDateForDataCollection { get; set; }
        public List<MinerRunDataCollectionSummary> Summary { get; set; }

    }
}
