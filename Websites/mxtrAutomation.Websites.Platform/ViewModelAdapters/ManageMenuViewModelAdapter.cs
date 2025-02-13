using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewData;
using mxtrAutomation.Websites.Platform.Models.Retailers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Websites.Platform.Queries;
using System.Web;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;
using mxtrAutomation.Websites.Platform.Models.ManageMenu.ViewModels;
using mxtrAutomation.Websites.Platform.Helpers;
using mxtrAutomation.Corporate.Data.Services;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IManageMenuViewModelAdapter
    {
        ManageMenuViewModel BuildManageMenuModel(IManageMenuService manageMenuService, IEnumerable<mxtrAccount> organizationAccounts);

        ManageMenuViewModel BuildCustomizeMenuModel(IManageMenuService manageMenuService, string accountObjectID, string userObjectID);
    }
    public class ManageMenuViewModelAdapter : IManageMenuViewModelAdapter
    {
        public ManageMenuViewModel BuildManageMenuModel(IManageMenuService manageMenuService, IEnumerable<mxtrAccount> organizationAccounts)
        {
            ManageMenuViewModel model = new ManageMenuViewModel();
            List<ManageMenuDataModel> lstMenuData = manageMenuService.GetAllMenuMaster();
            if (lstMenuData != null && lstMenuData.Count > 0)
            {
                model = new ManageMenuViewModel
                {
                    Menus = lstMenuData,
                    OrganizationAccounts = (from o in organizationAccounts
                                            select new OrganizationAccount
                                            {
                                                AccountName = o.AccountName,
                                                AccountObjectID = o.ObjectID,
                                                IsSelected = false,
                                            }).ToList(),
                };
            }

            AddPageTitle(model, "Manage Menu", "Manage Menu");
            return model;
        }

        public ManageMenuViewModel BuildCustomizeMenuModel(IManageMenuService manageMenuService, string accountObjectID, string userObjectID)
        {
            ManageMenuViewModel model = new ManageMenuViewModel();
            List<ManageMenuDataModel> lstMenuData = manageMenuService.GetMenuData(accountObjectID);
            if (lstMenuData != null && lstMenuData.Count > 0)
            {
                model = new ManageMenuViewModel
                {
                    Menus = lstMenuData,
                    IsDefaultMenu = false,
                };
            }
            else
            {
                // Default menus
                List<ManageMenuDataModel> lstMenu = manageMenuService.GetMenuMaster();
                lstMenu.ForEach(c => c.MenuID = null); //Set menu id as null
                lstMenu.ForEach(c => c.AccountObjectID = accountObjectID);
                lstMenu.ForEach(c => c.LastModifiedBy = userObjectID);

                model = new ManageMenuViewModel
                {
                    Menus = lstMenu,
                    IsDefaultMenu = true,
                };
            }

            AddPageTitle(model, "Customize Menu", "Customize Menu");
            return model;
        }

        public void AddPageTitle(ManageMenuViewModel model, string PageTitle, string MainPageHeader)
        {
            model.PageTitle = PageTitle;
            model.MainPageHeader = MainPageHeader;
            model.ShowWorkspaceFilter = false;
            model.ShowDateFilter = false;
        }
    }
}