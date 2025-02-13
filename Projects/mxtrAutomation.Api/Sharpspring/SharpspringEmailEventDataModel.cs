using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringEmailEventDataModel
    {
        public long LeadID { get; set; }
        public long EmailID { get; set; }
        public long EmailJobID { get; set; }
        public string EmailAddress { get; set; }
        public string EventType { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
