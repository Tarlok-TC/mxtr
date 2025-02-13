using System;
using System.Collections.Generic;
using mxtrAutomation.Web.Common.UI;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.Models.Shared.ViewModels
{
    public class SiteTopNavViewModel : MainLayoutViewModelBase
    {
        public string Fullname { get; set; }
        public string LogoutUrl { get; set; }
        public bool HasManageAccountUserPermission { get; set; }
        public string AccountAdminUrl { get; set; }
        public bool HasAdminRole { get; set; }
        public string AccountType { get; set; }
    }
}
