using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;

namespace mxtrAutomation.Websites.Platform.Models.Sharpspring.ViewModels
{
    public class SharpspringViewModel : MainLayoutViewModelBase
    {
        public string UpdateDataUrl { get; set; }
        public bool Success { get; set; }
        public string SharpspringUserName { get; set; }
        public string SharpspringPassword { get; set; }
    }
}