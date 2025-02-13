using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Shared.ViewData
{
    public class WorkspaceFilterViewData
    {
        public string SelectedClass { get; set; }
        public string SelectedChildClass { get; set; }
        public string AccountObjectID { get; set; }
        public string ParentAccountObjectID { get; set; }
        public string ParentAccountName { get; set; }
        public string AccountName { get; set; }
        public string ChildrenClass { get; set; }
        public List<WorkspaceFilterViewData> ChildAccounts { get; set; }
        public string AccountType { get; set; }
    }
}