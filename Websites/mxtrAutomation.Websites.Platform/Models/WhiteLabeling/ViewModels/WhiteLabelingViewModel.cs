using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;

namespace mxtrAutomation.Websites.Platform.Models.WhiteLabeling.ViewModels
{
    public class WhiteLabelingViewModel : MainLayoutViewModelBase
    {
        public string DomainName { get; set; }
        public string ApplicationLogoURL { get; set; }
        public string BrandingLogoURL { get; set; }
        public string FavIconURL { get; set; }
    }
}