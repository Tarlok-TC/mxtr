using mxtrAutomation.Websites.Platform.Models.ProshredHome.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IProshredHomeViewModelAdapter
    {
        ProshredHomeViewModel BuildProshredHomeViewModel();
    }

    public class ProshredHomeViewModelAdapter : IProshredHomeViewModelAdapter
    {
        public ProshredHomeViewModel BuildProshredHomeViewModel()
        {
            ProshredHomeViewModel model = new ProshredHomeViewModel();        

            AddPageTitle(model);
            AddAttributes(model);
            return model;
        }

        public void AddPageTitle(ProshredHomeViewModel model)
        {
            model.PageTitle = "Proshred Home";
        }

        private void AddAttributes(ProshredHomeViewModel model)
        {
            model.CallbackFunction = "updatePageFromWorkspace";
            model.ShowWorkspaceFilter = false;
            model.UpdateDataUrl = new ProshredHomeWebQuery();
        }
    }
}
