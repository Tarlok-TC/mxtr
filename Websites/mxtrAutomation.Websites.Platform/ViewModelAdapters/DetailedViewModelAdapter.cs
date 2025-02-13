using mxtrAutomation.Websites.Platform.Models.DMA.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IDetailedViewModelAdapter
    {
        DetailedViewModel BuildDetailedViewModel();
    }

    public class DetailedViewModelAdapter : IDetailedViewModelAdapter
    {
        public DetailedViewModel BuildDetailedViewModel()
        {
            DetailedViewModel model = new DetailedViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(DetailedViewModel model)
        {
            model.PageTitle = "Detailed";
        }
    }
}
