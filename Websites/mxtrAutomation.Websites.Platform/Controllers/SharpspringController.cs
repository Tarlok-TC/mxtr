using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Sharpspring.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class SharpspringController : MainLayoutControllerBase
    {
        private readonly ISharpspringViewModelAdapter _viewModelAdapter;

        public SharpspringController(ISharpspringViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }
        public SharpspringController()
        {
        }
        public ActionResult ViewPage(SharpspringWebQuery query)
        {
            // Get data...

            // Adapt data...
            SharpspringViewModel model =
                _viewModelAdapter.BuildSharpspringViewModel();
            //set Sharpspring details
            model.SharpspringUserName = User.SharpspringUserName;
            model.SharpspringPassword = User.SharpspringPassword;
                        
            // Handle...
            return View(ViewKind.Sharpspring, model, query);
        }
    }
}