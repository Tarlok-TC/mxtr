using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Common.Dto
{
    public class LineChartDataModel
    {
        public List<string> labels { get; set; }
        public List<ChartDatasetDataModel> datasets { get; set; }
    }
}
