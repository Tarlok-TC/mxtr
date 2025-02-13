using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System.Collections.Generic;

namespace mxtrAutomation.Websites.Platform.Models.NextDayRoute.ViewModels
{
    public class NextDayRouteViewModel : MainLayoutViewModelBase
    {
        public string UpdateDataUrl { get; set; }
        public bool Success { get; set; }
        public List<EZShredAccountDataModel> EZShredAccountDataModel { get; set; }
    }
}