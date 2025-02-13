using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringEmailJobDataModel
    {
        public long EmailJobID { get; set; }
        public bool IsList { get; set; }
        public bool IsActive { get; set; }
        public long RecipientID { get; set; }
        public int SendCount { get; set; }
        public DateTime CreateTimestamp { get; set; }
    }
}
