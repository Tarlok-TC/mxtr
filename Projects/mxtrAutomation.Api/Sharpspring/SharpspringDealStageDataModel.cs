using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringDealStageDataModel
    {
        public long DealStageID { get; set; }
        public string DealStageName { get; set; }
        public string Description { get; set; }
        public double DefaultProbability { get; set; }
        public int Weight { get; set; }
        public bool IsEditable { get; set; }
    }
}
