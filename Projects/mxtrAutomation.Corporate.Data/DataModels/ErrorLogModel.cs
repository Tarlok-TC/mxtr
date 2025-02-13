using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class ErrorLogModel
    {
        public string ErrorID { get; set; }
        public DateTime LogTime { get; set; }
        public string Description { get; set; } // Any description like page name, miner name etc.
        public string LogType { get; set; } // Miner, Web etc.
        public string ErrorMessage { get; set; }
    }

}
