using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class CreateNotificationReturn
    {
        public bool Success { get; set; }
        public string ObjectID { get; set; } //the mongo object id of the newly created object
    }
}
