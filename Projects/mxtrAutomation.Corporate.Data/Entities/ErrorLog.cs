using mxtrAutomation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class ErrorLog : Entity
    {
        public DateTime LogTime { get; set; }
        public string Description { get; set; } // Any description like page name, miner name etc.
        public string LogType { get; set; } // Miner, Web etc.
        public string ErrorMessage { get; set; }
    }
}
