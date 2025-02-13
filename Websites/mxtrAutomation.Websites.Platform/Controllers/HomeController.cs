using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Home.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class HomeController : MainLayoutControllerBase
    {
        private readonly IHomeViewModelAdapter _viewModelAdapter;

        public HomeController(IHomeViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(HomeWebQuery query)
        {
            // Get data...

            // Adapt data...
            HomeViewModel model =
                _viewModelAdapter.BuildHomeViewModel();

            // Handle...
            return View(ViewKind.Home, model, query);
        }
    }
}
