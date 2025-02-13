using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Klipfolio.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class KlipfolioController : MainLayoutControllerBase
    {
        private readonly IKlipfolioViewModelAdapter _viewModelAdapter;

        public KlipfolioController(IKlipfolioViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }
        public KlipfolioController()
        {
        }
        public ActionResult ViewPage(KlipfolioWebQuery query)
        {
            // Get data...

            // Adapt data...
            KlipfolioViewModel model =
                _viewModelAdapter.BuildKlipfolioAuthenticationViewModel();

            // Handle...
            return View(ViewKind.Klipfolio, model, query);
        }
    }
}