using mxtrAutomation.Data;
using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;


namespace mxtrAutomation.Corporate.Data.Entities
{ 
    public class GAPageTracking : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public string PageTitle { get; set; }
        public string PagePath { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateDate { get; set; }
        public int PageViews { get; set; }
        public List<GAEventTracking> Events { get; set; }
    }

    public class GAEventTracking
    {
        public string EventLabel { get; set; }
        public string EventCategory { get; set; }
        public string EventAction { get; set; }
        public string PagePath { get; set; }
        public int Total { get; set; }
    }
}
