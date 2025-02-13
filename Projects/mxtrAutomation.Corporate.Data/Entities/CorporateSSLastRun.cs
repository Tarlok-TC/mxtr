using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CorporateSSLastRun : Entity
    {
        public string AccountObjectID { get; set; }
        public DateTime LastRun { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
