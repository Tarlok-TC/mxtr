using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Contact.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class ContactController : MainLayoutControllerBase
    {
        private readonly IContactViewModelAdapter _viewModelAdapter;

        public ContactController(IContactViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(ContactWebQuery query)
        {
            // Get data...

            // Adapt data...
            ContactViewModel model =
                _viewModelAdapter.BuildContactViewModel();

            // Handle...
            return View(ViewKind.Contact, model, query);
        }
    }
}
