using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.DMA.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class DetailedController : MainLayoutControllerBase
    {
        private readonly IDetailedViewModelAdapter _viewModelAdapter;

        public DetailedController(IDetailedViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(DetailedWebQuery query)
        {
            // Get data...

            // Adapt data...
            DetailedViewModel model =
                _viewModelAdapter.BuildDetailedViewModel();

            // Handle...
            return View(ViewKind.Detailed, model, query);
        }
    }
}
