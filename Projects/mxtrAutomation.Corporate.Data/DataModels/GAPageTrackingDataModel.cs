using System;
using System.Collections.Generic;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class GAPageTrackingDataModel : CRMBaseDataModel
    {
        public string pageTitle { get; set; }
        public string pagePath { get; set; }
        public DateTime date { get; set; }
        public int pageviews { get; set; }
        public List<GAEventTrackingDataModel> Events { get; set; }

    }

    public class GAEventTrackingDataModel
    {
        public string eventLabel { get; set; }
        public string eventCategory { get; set; }
        public string eventAction { get; set; }
        public string pagePath { get; set; }
        public int totalEvents { get; set; }
    }
}
