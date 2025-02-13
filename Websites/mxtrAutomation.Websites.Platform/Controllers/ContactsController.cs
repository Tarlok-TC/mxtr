using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Contacts.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class ContactsController : MainLayoutControllerBase
    {
        private readonly IContactsViewModelAdapter _viewModelAdapter;

        public ContactsController(IContactsViewModelAdapter viewModelAdapter)
        {
            _viewModelAdapter = viewModelAdapter;
        }

        public ActionResult ViewPage(ContactsWebQuery query)
        {
            // Get data...

            // Adapt data...
            ContactsViewModel model =
                _viewModelAdapter.BuildContactsViewModel();

            // Handle...
            return View(ViewKind.Contacts, model, query);
        }
    }
}
