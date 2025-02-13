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
    public class AddEditMenuWebQuery : WebQueryBase
    {
        public static readonly string Route = "/add-edit-menu";

        public AddEditMenuWebQuery()
        {
            _manageMenuData = Parameters.Add<string>("managemenudata", true);
        }

        public string ManageMenuData
        {
            get { return _manageMenuData.Value; }
            set { _manageMenuData.Value = value; }
        }
        private readonly UrlParameterBase<string> _manageMenuData;
    }
}