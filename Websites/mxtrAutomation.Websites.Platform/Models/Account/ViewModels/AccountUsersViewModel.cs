using System;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Web.Common.UI;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Websites.Platform.Enums;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewModels
{
    public class AccountUsersViewModel : ViewModelBase
    {
        public Guid MxtrAccountID { get; set; }
        public string AccountObjectID { get; set; }
        public string UserSubmitUrl { get; set; }
        public string UserFinishedUrl { get; set; }
        public string EZShredIP { get; set; }
        public string EZShredPort { get; set; }
        public string DealerId { get; set; }
        public string SSLead { get; set; }
        public string SSContact { get; set; }
        public string SSQuoteSent { get; set; }
        public string SSWonNotScheduled { get; set; }
        public string SSClosed { get; set; }

        public List<AccountUserViewData> Users { get; set; }

        public List<EZShredAccountDataModel> MarketPlaces { get; set; }

        public List<AccountUserPermissionsViewData> PermissionKinds { get; set; }

        public AccountActionKind AccountActionKind { get; set; }
    }
}
