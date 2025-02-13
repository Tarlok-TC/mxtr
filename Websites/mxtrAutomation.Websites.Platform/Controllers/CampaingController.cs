using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Campaing.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class CampaingController : MainLayoutControllerBase
    {
        private readonly ICampaingViewModelAdapter _viewModelAdapter;

        public CampaingController(ICampaingViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(CampaingWebQuery query)
        {
            // Get data...

            // Adapt data...
            CampaingViewModel model =
                _viewModelAdapter.BuildCampaingViewModel();

            // Handle...
            return View(ViewKind.Campaing, model, query);
        }
    }
}
