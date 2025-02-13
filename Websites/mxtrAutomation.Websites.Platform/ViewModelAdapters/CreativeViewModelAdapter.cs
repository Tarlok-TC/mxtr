using mxtrAutomation.Websites.Platform.Models.Email.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ICreativeViewModelAdapter
    {
        CreativeViewModel BuildCreativeViewModel();
    }

    public class CreativeViewModelAdapter : ICreativeViewModelAdapter
    {
        public CreativeViewModel BuildCreativeViewModel()
        {
            CreativeViewModel model = new CreativeViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(CreativeViewModel model)
        {
            model.PageTitle = "Creative";
        }
    }
}
