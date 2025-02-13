using mxtrAutomation.Websites.Platform.Models.Home.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IHomeViewModelAdapter
    {
        HomeViewModel BuildHomeViewModel();
    }

    public class HomeViewModelAdapter : IHomeViewModelAdapter
    {
        public HomeViewModel BuildHomeViewModel()
        {
            HomeViewModel model = new HomeViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(HomeViewModel model)
        {
            model.PageTitle = "Home";
        }
    }
}
