using mxtrAutomation.Data;
using System;
using System.Collections.Generic;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class EZShredMinerLog : Entity
    {
        public string AccountObjectId { get; set; }
        public string LocationName { get; set; }
        public string Port { get; set; }
        public List<APIDetail> APIDetails { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class APIDetail
    {
        public string APIName { get; set; }
        public DateTime HitTime { get; set; }
        public string ReturnResponse { get; set; }
        public bool ReturnResult { get; set; }
        public double ResponseTime { get; set; }

    }
}
