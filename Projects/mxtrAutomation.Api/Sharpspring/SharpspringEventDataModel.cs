using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringEventDataModel
    {
        public long EventID { get; set; }
        public long LeadID { get; set; }
        public string EventName { get; set; }
        public string WhatID { get; set; }
        public string WhatType { get; set; }
        public List<SharpspringEventEventData> EventData { get; set; }
        public DateTime CreateTimestamp { get; set; }

    }

    public class SharpspringEventEventData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
