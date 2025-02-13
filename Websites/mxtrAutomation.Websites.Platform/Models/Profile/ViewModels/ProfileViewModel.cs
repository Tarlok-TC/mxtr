using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Models.Profile.ViewModels
{
    public class ProfileViewModel : MainLayoutViewModelBase
    {
        public string ObjectID { get; set; }
        public string MxtrUserID { get; set; }

        public string MxtrAccountID { get; set; }
        public string AccountObjectID { get; set; }

        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public List<EZShredAccountDataModel> EZShredAccountMappings { get; set; }
        public string Role { get; set; }
        public List<string> Permissions { get; set; }

        public string CreateDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public int FailedLoginAttempts { get; set; }
        public string LastLogin { get; set; }
        public string SharpspringUserName { get; set; }
        public string SharpspringPassword { get; set; }
    }
}