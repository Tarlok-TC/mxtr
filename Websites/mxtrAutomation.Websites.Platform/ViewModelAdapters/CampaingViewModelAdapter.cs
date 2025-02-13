using mxtrAutomation.Websites.Platform.Models.Campaing.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ICampaingViewModelAdapter
    {
        CampaingViewModel BuildCampaingViewModel();
    }

    public class CampaingViewModelAdapter : ICampaingViewModelAdapter
    {
        public CampaingViewModel BuildCampaingViewModel()
        {
            CampaingViewModel model = new CampaingViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(CampaingViewModel model)
        {
            model.PageTitle = "Campaing";
        }
    }
}
