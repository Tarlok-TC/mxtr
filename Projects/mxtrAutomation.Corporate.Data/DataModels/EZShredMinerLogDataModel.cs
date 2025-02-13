using System;
using System.Collections.Generic;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class EZShredMinerLogDataModel
    {
        public string AccountObjectId { get; set; }
        public string LocationName { get; set; }
        public string Port { get; set; }
        public List<APIDetailDataModel> APIDetails { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class APIDetailDataModel
    {
        public string APIName { get; set; }
        public DateTime HitTime { get; set; }
        public string ReturnResponse { get; set; }
        public bool ReturnResult { get; set; }
        public double ResponseTime { get; set; }

    }
}
