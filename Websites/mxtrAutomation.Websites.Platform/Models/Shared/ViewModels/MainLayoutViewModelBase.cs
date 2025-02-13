using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Web.Common.UI;
using System.Collections.Generic;

namespace mxtrAutomation.Websites.Platform.Models.Shared.ViewModels
{
    public abstract class MainLayoutViewModelBase : ViewModelBase
    {
        public string PageTitle { get; set; }
        public string BodyClass { get; set; }
        public string MainPageHeader { get; set; }
        public string SubPageHeader { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MxtrUserID { get; set; }
        public string AccountName { get; set; }
        public string ApplicationLogoURL { get; set; }
        public string BrandingLogoURL { get; set; }
        public string FavIconURL { get; set; }
        public string HomePageUrl { get; set; }

        public SiteTopNavViewModel SiteTopNav { get; set; }
        public WorkspaceFilterViewModel WorkspaceFilter { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CurrentAccountIDs { get; set; }
        public bool ShowWorkspaceFilter { get; set; }
        public bool ShowDateFilter { get; set; }
        public string CallbackFunction { get; set; }
        //public string RefreshUrl { get; set; }
        public List<ManageMenuDataModel> SideMenus { get; set; }
    }
}