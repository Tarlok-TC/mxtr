using mxtrAutomation.Websites.Platform.Models.Contacts.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IContactsViewModelAdapter
    {
        ContactsViewModel BuildContactsViewModel();
    }

    public class ContactsViewModelAdapter : IContactsViewModelAdapter
    {
        public ContactsViewModel BuildContactsViewModel()
        {
            ContactsViewModel model = new ContactsViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(ContactsViewModel model)
        {
            model.PageTitle = "Contacts";
        }
    }
}
