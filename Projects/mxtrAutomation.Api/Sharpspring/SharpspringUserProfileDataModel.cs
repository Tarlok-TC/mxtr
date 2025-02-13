using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringUserProfileDataModel
    {
        public string OwnerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string isActive { get; set; }
        public string isReseller { get; set; }
        public string UserTimezone { get; set; }
        public string Phone { get; set; }
    }

}
