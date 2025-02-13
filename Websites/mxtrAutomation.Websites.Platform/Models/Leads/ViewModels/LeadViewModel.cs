using System;
using System.Linq;
using System.Collections.Generic;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using mxtrAutomation.Websites.Platform.Models.Leads.ViewData;
using mxtrAutomation.Common.Dto;

namespace mxtrAutomation.Websites.Platform.Models.Leads.ViewModels
{
    public class LeadViewModel : MainLayoutViewModelBase
    {
        public LeadViewData Lead { get; set; }
    }
}
