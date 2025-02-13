using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class CRMLeadEventDataModel
    {
        public long EventID { get; set; }
        public long LeadID { get; set; }
        public string EventName { get; set; }
        public string WhatID { get; set; }
        public string WhatType { get; set; }
        public List<CRMLeadEventEventData> EventData { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public string LeadAccountName { get; set; }
        public bool CopiedToParent { get; set; }
    }

    public class CRMLeadEventEventData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
