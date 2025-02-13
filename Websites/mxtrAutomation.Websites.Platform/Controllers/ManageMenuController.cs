using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.ManageMenu.ViewModels;
using Newtonsoft.Json;
using mxtrAutomation.Websites.Platform.Helpers;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    //[Authorize(Roles = "admin")]
    public class ManageMenuController : MainLayoutControllerBase
    {
        private readonly IManageMenuViewModelAdapter _viewModelAdapter;
        private readonly IManageMenuService _manageMenuService;
        private readonly IAccountService _accountService;

        public ManageMenuController(IManageMenuViewModelAdapter viewModelAdapter, IManageMenuService manageMenuService, IAccountService accountService)
        {
            _viewModelAdapter = viewModelAdapter;
            _manageMenuService = manageMenuService;
            _accountService = accountService;
        }

        public ActionResult ViewPage(ManageMenuWebQuery query)
        {
            mxtrAccount parentMxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            if (parentMxtrAccount.AccountType.ToLower() == "reseller" && User.Role.ToLower() == "admin") //Temporary check and need to remove once role based authorization will implement
            {
                IEnumerable<mxtrAccount> organizationAccounts = _accountService.GetAllAccountsWithOrganization();
                // Adapt data...
                ManageMenuViewModel model = _viewModelAdapter.BuildManageMenuModel(_manageMenuService, organizationAccounts);
                if (query.IsAjax)
                {
                    return Json(new { Success = true, MenuMasterData = model.Menus, OrganizationAccounts = model.OrganizationAccounts });
                }

                return View(ViewKind.ManageMenu, model, query);
            }
            else
            {
                return Redirect(new IndexWebQuery());
            }
        }

        public ActionResult AddEditMenuData(AddEditMenuWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            try
            {
                ManageMenuDataModel menuData = JsonConvert.DeserializeObject<ManageMenuDataModel>(query.ManageMenuData);
                menuData.LastModifiedBy = User.MxtrUserObjectID;
                menuData.Status = true;
                if (menuData.SubMenu != null && menuData.SubMenu.Count > 0)
                {
                    menuData.SubMenu.ForEach(x => x.Status = true);
                }
                notification = _manageMenuService.AddUpdateMenuMaster(menuData);
                return Json(new { Success = notification.Success, Message = "" });
            }
            catch (System.Exception ex)
            {
                return Json(new { Success = notification.Success, Message = "" });
            }
        }

        public ActionResult DeleteMenuData(DeleteMenuWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            try
            {
                notification = _manageMenuService.DeleteMenuMasterData(query.MenuId, query.SubMenuId);
                return Json(new { Success = notification.Success, Message = "" });
            }
            catch (System.Exception ex)
            {
                return Json(new { Success = notification.Success, Message = "" });
            }
        }
    }
}
