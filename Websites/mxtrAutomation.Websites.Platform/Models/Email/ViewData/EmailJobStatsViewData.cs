using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Email.ViewData
{
    public class EmailJobStatsViewData
    {
        public string AccountObjectID { get; set; }
        public long EmailID { get; set; }
        public string DataDate { get; set; }
        public int Sends { get; set; }
        public int Opens { get; set; }
        public int Clicks { get; set; }
        public double OpenRate { get; set; }
        public double ClickRate { get; set; }
    }
}