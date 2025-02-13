using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Email.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class CreativeController : MainLayoutControllerBase
    {
        private readonly ICreativeViewModelAdapter _viewModelAdapter;

        public CreativeController(ICreativeViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(CreativeWebQuery query)
        {
            // Get data...

            // Adapt data...
            CreativeViewModel model =
                _viewModelAdapter.BuildCreativeViewModel();

            // Handle...
            return View(ViewKind.Creative, model, query);
        }
    }
}
