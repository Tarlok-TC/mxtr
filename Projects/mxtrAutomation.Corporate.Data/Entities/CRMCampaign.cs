using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CRMCampaign : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public long CampaignID { get; set; }
        public string CampaignName { get; set; }
        public string CampaignType { get; set; }
        public string CampaignAlias { get; set; }
        public string CampaignOrigin { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Goal { get; set; }
        public double OtherCosts { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
