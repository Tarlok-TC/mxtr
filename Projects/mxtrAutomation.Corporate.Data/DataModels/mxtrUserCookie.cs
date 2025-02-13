using System;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class mxtrUserCookie
    {
        public string ObjectID { get; set; }
        public string MxtrUserID { get; set; }

        public string MxtrAccountID { get; set; }
        public string AccountObjectID { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string SharpspringUserName { get; set; }
        public string SharpspringPassword { get; set; }
    }
}
