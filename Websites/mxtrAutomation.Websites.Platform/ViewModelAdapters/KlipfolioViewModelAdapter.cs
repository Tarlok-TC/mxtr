using mxtrAutomation.Websites.Platform.Models.Klipfolio.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IKlipfolioViewModelAdapter
    {
        KlipfolioViewModel BuildKlipfolioAuthenticationViewModel();
    }

    public class KlipfolioViewModelAdapter : IKlipfolioViewModelAdapter
    {
        public KlipfolioViewModel BuildKlipfolioAuthenticationViewModel()
        {
            KlipfolioViewModel model = new KlipfolioViewModel();        

            AddPageTitle(model);
            AddAttributes(model);
            return model;
        }

        public void AddPageTitle(KlipfolioViewModel model)
        {
            model.PageTitle = "Klipfolio";
        }

        private void AddAttributes(KlipfolioViewModel model)
        {
            model.CallbackFunction = "updatePageFromWorkspace";
            model.ShowWorkspaceFilter = false;
            model.UpdateDataUrl = new KlipfolioWebQuery();
        }
    }
}
