using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.WhiteLabeling.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IWhiteLabelingViewModelAdapter
    {
        WhiteLabelingViewModel BuildWhiteLabelingViewModel(mxtrAccount account);
    }
    public class WhiteLabelingViewModelAdapter : IWhiteLabelingViewModelAdapter
    {
        public WhiteLabelingViewModel BuildWhiteLabelingViewModel(mxtrAccount account)
        {
            WhiteLabelingViewModel model = new WhiteLabelingViewModel();
            AddData(model, account);
            model.PageTitle = "White Labeling";
            return model;
        }
        private void AddData(WhiteLabelingViewModel model, mxtrAccount account)
        {
            model.DomainName = account.DomainName;
            model.ApplicationLogoURL = account.ApplicationLogoURL;
            model.FavIconURL = account.FavIconURL;
            model.BrandingLogoURL = account.BrandingLogoURL;
            model.HomePageUrl = account.HomePageUrl;
        }
    }
}