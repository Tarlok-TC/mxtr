using System;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewData
{
    public class AccountUserPermissionsViewData
    {
        public string PermissionName { get; set; }
        public PermissionKind PermissionKind { get; set; }
        public bool Checked { get; set; }
    }
}