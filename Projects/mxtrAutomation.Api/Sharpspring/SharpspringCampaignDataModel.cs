using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringCampaignDataModel
    {
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
