using mxtrAutomation.Data;
using System;
using System.Collections.Generic;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class ShawListBasedLeadAnalytics : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        /// <summary>
        /// SharpSpringListID relates to account table SharpSpringShawFunnelListID
        /// </summary>
        public long SharpSpringListID { get; set; } 
        public string Name { get; set; }
        public string MemberCount { get; set; }
        public int? MemberCountForToday { get; set; }
        public int RemovedCount { get; set; }
        public string Description { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public DateTime CreatedOnMXTR { get; set; }
    }
}
