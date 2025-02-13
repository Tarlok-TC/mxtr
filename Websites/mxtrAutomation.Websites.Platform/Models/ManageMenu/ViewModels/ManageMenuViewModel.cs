using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.ManageMenu.ViewModels
{
    public class ManageMenuViewModel : MainLayoutViewModelBase
    {
        public List<ManageMenuDataModel> Menus { get; set; }
        public List<OrganizationAccount> OrganizationAccounts { get; set; }
        public bool IsDefaultMenu { get; set; }
    }
}