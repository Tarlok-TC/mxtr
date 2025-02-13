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
    public class CustomizeMenuController : MainLayoutControllerBase
    {
        private readonly IManageMenuViewModelAdapter _viewModelAdapter;
        private readonly IManageMenuService _manageMenuService;
        private readonly IAccountService _accountService;

        public CustomizeMenuController(IManageMenuViewModelAdapter viewModelAdapter, IManageMenuService manageMenuService, IAccountService accountService)
        {
            _viewModelAdapter = viewModelAdapter;
            _manageMenuService = manageMenuService;
            _accountService = accountService;
        }

        public ActionResult ViewPage(CustomizeMenuWebQuery query)
        {
            mxtrAccount parentMxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);

            if ((parentMxtrAccount.AccountType == AccountKind.Organization.ToString() || parentMxtrAccount.AccountType.ToLower() == "reseller") && User.Role.ToLower() == "admin") //Temporary check and need to remove once role based authorization will implement
            {
                // Adapt data...
                ManageMenuViewModel model = _viewModelAdapter.BuildCustomizeMenuModel(_manageMenuService, User.MxtrAccountObjectID, User.MxtrUserObjectID);

                return View(ViewKind.CustomizeMenu, model, query);
            }
            else
            {
                return Redirect(new IndexWebQuery());
            }
        }

        public ActionResult UpdateMenuData(CustomizeMenuSubmitWebQuery query)
        {
            ManageMenuViewModel model = new ManageMenuViewModel();
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            List<ManageMenuDataModel> menuData = JsonConvert.DeserializeObject<List<ManageMenuDataModel>>(query.ManageMenuData);
            notification = _manageMenuService.UpdateMenuData(menuData);
            if (notification.Success)
            {
                model.Menus = _manageMenuService.GetMenuData(User.MxtrAccountObjectID);
                model.IsDefaultMenu = false;
            }

            return Json(new { Success = notification.Success, Message = "", MenusData = model });
        }

        public ActionResult ResetMenuData(ResetMenuWebQuery query)
        {
            ManageMenuViewModel model = new ManageMenuViewModel();
            CreateNotificationReturn notification = new CreateNotificationReturn { Success = false };
            notification = _manageMenuService.ResetMenuData(User.MxtrAccountObjectID, User.MxtrUserObjectID);
            if (notification.Success)
            {
                List<ManageMenuDataModel> lstMenu = _manageMenuService.GetMenuMaster();
                lstMenu.ForEach(c => c.MenuID = null); //Set menu id as null
                lstMenu.ForEach(c => c.AccountObjectID = User.MxtrAccountObjectID);
                lstMenu.ForEach(c => c.LastModifiedBy = User.MxtrUserObjectID);
                model.Menus = lstMenu;
                model.IsDefaultMenu = true;
            }

            return Json(new { Success = notification.Success, Message = "", MenusData = model });
        }

    }
}
