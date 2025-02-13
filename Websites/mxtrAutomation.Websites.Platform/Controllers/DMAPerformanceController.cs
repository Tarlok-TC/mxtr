using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.DMA.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class DMAPerformanceController : MainLayoutControllerBase
    {
        private readonly IDMAPerformanceViewModelAdapter _viewModelAdapter;

        public DMAPerformanceController(IDMAPerformanceViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(DMAPerformanceWebQuery query)
        {
            // Get data...

            // Adapt data...
            DMAPerformanceViewModel model =
                _viewModelAdapter.BuildDMAPerformanceViewModel();

            // Handle...
            return View(ViewKind.DMAPerformance, model, query);
        }
    }
}
