using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.ProshredHome.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using System.Globalization;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class ProshredHomeController : MainLayoutControllerBase
    {
        private readonly IProshredHomeViewModelAdapter _viewModelAdapter;
        private readonly IUserService _userService;
        public ProshredHomeController(IProshredHomeViewModelAdapter viewModelAdapter, IUserService userService)
        {
            _viewModelAdapter = viewModelAdapter;
            _userService = userService;
        }
        public ActionResult ViewPage(ProshredHomeWebQuery query)
        {
            mxtrUser user = _userService.GetUserByUserObjectID(User.MxtrUserObjectID);

            ProshredHomeViewModel model =
                _viewModelAdapter.BuildProshredHomeViewModel();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            model.FullName = textInfo.ToTitleCase(user.FullName);
            return View(ViewKind.ProshredHome, model, query);
        }
    }
}