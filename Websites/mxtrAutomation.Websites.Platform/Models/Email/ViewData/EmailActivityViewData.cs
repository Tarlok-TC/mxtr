using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Email.ViewData
{
    public class EmailActivityViewData : EmailActivityViewDataMini
    {
        public string AccountObjectID { get; set; }
        public string AccountName { get; set; }
        public long EmailID { get; set; }
        public string EmailTitle { get; set; }
    }

}