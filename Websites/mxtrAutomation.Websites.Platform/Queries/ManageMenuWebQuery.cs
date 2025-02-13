using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Websites.Platform.Models.ManageMenu.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class ManageMenuWebQuery : WebQueryBase
    {
        public static readonly string Route = "/manage-menu";
        public ManageMenuWebQuery()
        {
            _isAjax = Parameters.Add<bool>("isajax", false);
        }
        public bool IsAjax
        {
            get { return _isAjax.Value; }
            set { _isAjax.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isAjax;
    }

    public class ManageMenuWebQuerySubmitWebQuery : WebQueryBase
    {
        public static readonly string Route = "/managemenu-submit";
        public ManageMenuWebQuerySubmitWebQuery()
        {
            _manageMenuData = Parameters.Add<string>("managemenudata", false);
        }

        public string ManageMenuData
        {
            get { return _manageMenuData.Value; }
            set { _manageMenuData.Value = value; }
        }
        private readonly UrlParameterBase<string> _manageMenuData;
    }

    public class DeleteMenuWebQuery : WebQueryBase
    {
        public static readonly string Route = "/delete-menu";
        public DeleteMenuWebQuery()
        {
            _menuId = Parameters.Add<string>("menuid", false);
            _subMenuId = Parameters.Add<string>("submenuId", false);
        }
        public string MenuId
        {
            get { return _menuId.Value; }
            set { _menuId.Value = value; }
        }
        public String SubMenuId
        {
            get { return _subMenuId.Value; }
            set { _subMenuId.Value = value; }
        }

        private readonly UrlParameterBase<string> _menuId;
        private readonly UrlParameterBase<string> _subMenuId;
    }

}