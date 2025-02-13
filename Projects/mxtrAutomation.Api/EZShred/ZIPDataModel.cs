using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.EZShred
{
    public class ZIPDataModel
    {
        public string Request { get; set; }
        public string status { get; set; }
        public List<AvailableDates> NextDatesByZip { get; set; }
    }
    public class AvailableDates
    {
        public string Date { get; set; }
        public string Zip { get; set; }
        public string Count { get; set; }

    }
}
