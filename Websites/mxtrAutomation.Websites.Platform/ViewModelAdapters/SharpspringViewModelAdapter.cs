using mxtrAutomation.Websites.Platform.Models.Sharpspring.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ISharpspringViewModelAdapter
    {
        SharpspringViewModel BuildSharpspringViewModel();
    }

    public class SharpspringViewModelAdapter : ISharpspringViewModelAdapter
    {
        public SharpspringViewModel BuildSharpspringViewModel()
        {
            SharpspringViewModel model = new SharpspringViewModel();        

            AddPageTitle(model);
            AddAttributes(model);
            return model;
        }

        public void AddPageTitle(SharpspringViewModel model)
        {
            model.PageTitle = "Sharpspring";
        }

        private void AddAttributes(SharpspringViewModel model)
        {
            model.CallbackFunction = "updatePageFromWorkspace";
            model.ShowWorkspaceFilter = false;
            model.UpdateDataUrl = new SharpspringWebQuery();
        }
    }
}
