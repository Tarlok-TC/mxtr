using System;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Web.Common.UI;
using mxtrAutomation.Websites.Platform.Enums;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewModels
{
    public class AccountProfileViewModel : ViewModelBase
    {
        public string ObjectID { get; set; }
        public Guid MxtrAccountID { get; set; }
        public Guid ParentMxtrAccountID { get; set; }
        public string ParentAccountObjectID { get; set; }
        public string ParentAccountName { get; set; }
        public string AccountName { get; set; }
        public string StreetAddress { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }

        public List<SelectListItem> AccountTypeOptions { get; set; }
        public string AccountType { get; set; }

        public string AccountSubmitUrl { get; set; }
        public string AccountSubmitText { get; set; }

        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }

        public AccountActionKind AccountActionKind { get; set; }
        public List<SelectListItem> MoveToAccountOptions { get; set; }
        public string DomainName { get; set; }
        public string StoreId { get; set; }
        public string HomePageUrl { get; set; }

    }
}
