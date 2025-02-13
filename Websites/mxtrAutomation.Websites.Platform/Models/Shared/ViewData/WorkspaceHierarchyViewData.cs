using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Shared.ViewData
{
    public class WorkspaceHierarchyViewData
    {
        public string AccountName { get; set; }
        public string AccountObjectID { get; set; }
        public string ParentAccountObjectID { get; set; }
        public string EditAccountUrl { get; set; }
        public string AddChildAccountUrl { get; set; }
        public List<WorkspaceHierarchyViewData> Children { get; set; }
        public int ChildrenCount { get; set; }
        public string AccountType { get; set; }
    }
}