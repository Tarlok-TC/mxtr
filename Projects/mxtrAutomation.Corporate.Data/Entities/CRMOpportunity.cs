using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CRMOpportunity : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public long OpportunityID { get; set; }
        public long OwnerID { get; set; }
        public long PrimaryLeadID { get; set; }
        public long DealStageID { get; set; }
        public long AccountID { get; set; }
        public long CampaignID { get; set; }
        public string OpportunityName { get; set; }
        public double Probability { get; set; }
        public double Amount { get; set; }
        public bool IsClosed { get; set; }
        public bool IsWon { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CloseDate { get; set; }
    }
}
