using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Leads.ViewData
{
    public class EventViewData
    {
        public long EventID { get; set; }
        public long LeadID { get; set; }
        public string EventDescription { get; set; }
        public string WhatID { get; set; }
        public string EventType { get; set; }
        public List<EventEventData> EventData { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public bool IsCopied { get; set; }
        public bool CopiedToParent { get; set; }
        public string LeadAccountName { get; set; }
    }

    public class EventEventData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}