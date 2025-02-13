using mxtrAutomation.Websites.Platform.Models.RegionPerformance.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IRegionPerformanceViewModelAdapter
    {
        RegionPerformanceViewModel BuildRegionPerformanceViewModel();
    }

    public class RegionPerformanceViewModelAdapter : IRegionPerformanceViewModelAdapter
    {
        public RegionPerformanceViewModel BuildRegionPerformanceViewModel()
        {
            RegionPerformanceViewModel model = new RegionPerformanceViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(RegionPerformanceViewModel model)
        {
            model.PageTitle = "RegionPerformance";
        }
    }
}
