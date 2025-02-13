using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Activity.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class ActivityController : MainLayoutControllerBase
    {
        private readonly IActivityViewModelAdapter _viewModelAdapter;

        public ActivityController(IActivityViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(ActivityWebQuery query)
        {
            // Get data...

            // Adapt data...
            ActivityViewModel model =
                _viewModelAdapter.BuildActivityViewModel();

            // Handle...
            return View(ViewKind.Activity, model, query);
        }
    }
}
