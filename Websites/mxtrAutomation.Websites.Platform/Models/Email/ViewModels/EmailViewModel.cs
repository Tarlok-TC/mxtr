using System;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Email.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;

namespace mxtrAutomation.Websites.Platform.Models.Email.ViewModels
{
    public class EmailViewModel : MainLayoutViewModelBase
    {       
        public List<EmailActivityViewData> EmailActivityViewData { get; set; }
        public EmailChartViewData EmailChartViewData { get; set; }
        public string UpdateDataUrl { get; set; }
        public bool Success { get; set; }
    }   
}
