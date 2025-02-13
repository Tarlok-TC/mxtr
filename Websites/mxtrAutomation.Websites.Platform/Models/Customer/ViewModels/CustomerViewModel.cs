using mxtrAutomation.Websites.Platform.Models.Customer.ViewData;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mxtrAutomation.Websites.Platform.Enums;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.Models.Customer.ViewModels
{
    public class CustomerViewModel : MainLayoutViewModelBase
    {
        public bool Success { get; set; }
        public string UpdateDataUrl { get; set; }
        public CustomerActionKind CustomerActionKind { get; set; }
        public CustomerViewData CustomerViewData { get; set; }
        public List<EZShredAccountDataModel> EZShredAccountDataModel { get; set; }
    }
}