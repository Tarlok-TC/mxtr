using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringGetActiveListDataModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MemberCount { get; set; }
        public string RemovedCount { get; set; }
        public string CreateTimestamp { get; set; }
        public string Description { get; set; }
    }
}
