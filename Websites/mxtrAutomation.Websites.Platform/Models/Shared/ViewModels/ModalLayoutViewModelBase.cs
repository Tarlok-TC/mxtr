using System;
using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.Models.Shared.ViewModels
{
    public abstract class ModalLayoutViewModelBase : ViewModelBase
    {
        public string PageTitle { get; set; }
        public string BodyClass { get; set; }
    }
}
