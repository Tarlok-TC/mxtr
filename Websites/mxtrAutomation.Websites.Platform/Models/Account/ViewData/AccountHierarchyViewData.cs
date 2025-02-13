using System;
using System.Collections.Generic;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewData
{
    public class AccountHierarchyViewData
    {
        public string AccountName { get; set; }
        public string AccountObjectID { get; set; }
        public string ParentAccountObjectID { get; set; }
        public string EditAccountUrl { get; set; }
        public string AddChildAccountUrl { get; set; }
        public List<AccountHierarchyViewData> Children { get; set; }
        public int ChildrenCount { get; set; }
    }
}