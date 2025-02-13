using mxtrAutomation.Websites.Platform.Models.Account.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewData
{
    public class AccountProfileViewData
    {
        public string ObjectID { get; set; }
        public Guid MxtrAccountID { get; set; }
        public Guid ParentMxtrAccountID { get; set; }
        public string ParentAccountObjectID { get; set; }
        public string AccountName { get; set; }
        public string StreetAddress { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string AccountType { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        public string StoreId { get; set; }
    }
}