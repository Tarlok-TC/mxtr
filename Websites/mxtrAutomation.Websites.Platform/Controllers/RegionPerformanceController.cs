using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.RegionPerformance.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class RegionPerformanceController : MainLayoutControllerBase
    {
        private readonly IRegionPerformanceViewModelAdapter _viewModelAdapter;

        public RegionPerformanceController(IRegionPerformanceViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(RegionPerformanceWebQuery query)
        {
            // Get data...

            // Adapt data...
            RegionPerformanceViewModel model =
                _viewModelAdapter.BuildRegionPerformanceViewModel();

            // Handle...
            return View(ViewKind.RegionPerformance, model, query);
        }
    }
}
