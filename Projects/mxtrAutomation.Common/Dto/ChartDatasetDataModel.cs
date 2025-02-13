using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Common.Dto
{
    public class ChartDatasetDataModel
    {
        public string label { get; set; }
        public List<int> data { get; set; }
        public string yAxisID { get; set; }
        public string borderColor { get; set; }
        public string backgroundColor { get; set; }
    }
}
