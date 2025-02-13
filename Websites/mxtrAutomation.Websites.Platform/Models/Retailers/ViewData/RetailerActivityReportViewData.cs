using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Retailers.ViewData
{
    public class RetailerActivityReportViewData : RetailerActivityReportViewDataMini
    {
        public string AccountObjectID { get; set; }
        public string AccountName { get; set; }
        public int UrlClicks { get; set; }
        public int EmailClicks { get; set; }
        public int FormSubmissions { get; set; }
        public bool IsActive { get; set; }
    }
}