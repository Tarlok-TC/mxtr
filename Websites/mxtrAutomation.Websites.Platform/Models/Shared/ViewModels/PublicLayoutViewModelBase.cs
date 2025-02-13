using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.Models.Shared.ViewModels
{
    public abstract class PublicLayoutViewModelBase : ViewModelBase
    {
        public string PageTitle { get; set; }
        public string BodyClass { get; set; }
        public virtual string BrandingLogoURL { get; set; }
    }
}