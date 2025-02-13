using mxtrAutomation.Websites.Platform.Models.Contact.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IContactViewModelAdapter
    {
        ContactViewModel BuildContactViewModel();
    }

    public class ContactViewModelAdapter : IContactViewModelAdapter
    {
        public ContactViewModel BuildContactViewModel()
        {
            ContactViewModel model = new ContactViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(ContactViewModel model)
        {
            model.PageTitle = "Contact";
        }
    }
}
