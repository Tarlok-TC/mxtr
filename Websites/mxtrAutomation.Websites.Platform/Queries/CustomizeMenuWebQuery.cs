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
    public class CustomizeMenuWebQuery : WebQueryBase 
    {
        public static readonly string Route = "/customize-menu";
    }

    public class CustomizeMenuSubmitWebQuery : WebQueryBase
    {
        public static readonly string Route = "/customizemenu-submit";
        public CustomizeMenuSubmitWebQuery()
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

}