using System;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Web.Common.UI;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Websites.Platform.Models.Account.ViewData
{
    public class AccountUserViewData
    {
        public string ObjectID { get; set; }
        public Guid MxtrUserID { get; set; }

        public Guid MxtrAccountID { get; set; }
        public string AccountObjectID { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public List<EZShredAccountDataModel> EZShredAccountMappings { get; set; }
        public string Role { get; set; }

        //public List<AccountUserPermissionsViewData> Permissions { get; set; }
        public string Permissions { get; set; }
        
        public string CreateDate { get; set; }
        public bool IsActive { get; set; }
        public string SharpspringUserName { get; set; }
        public string SharpspringPassword { get; set; }
    }
}