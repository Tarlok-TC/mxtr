using mxtrAutomation.Websites.Platform.Models.DMA.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IDMAPerformanceViewModelAdapter
    {
        DMAPerformanceViewModel BuildDMAPerformanceViewModel();
    }

    public class DMAPerformanceViewModelAdapter : IDMAPerformanceViewModelAdapter
    {
        public DMAPerformanceViewModel BuildDMAPerformanceViewModel()
        {
            DMAPerformanceViewModel model = new DMAPerformanceViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(DMAPerformanceViewModel model)
        {
            model.PageTitle = "DMAPerformance";
        }
    }
}
