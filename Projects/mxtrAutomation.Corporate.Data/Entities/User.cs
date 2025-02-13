using System.Collections.Generic;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class User : Entity
    {
        public string MxtrUserID { get; set; }

        public string MxtrAccountID { get; set; }
        public string AccountObjectID { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public string PasswordSalt { get; set; }
        public string Password { get; set; }
        
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public List<EZShredAccountMapping> EZShredAccountMappings { get; set; }
        public string Role { get; set; }
        public List<string> Permissions { get; set; }
        public string KlipfolioSSOToken { get; set; }

        public string CreateDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public int FailedLoginAttempts { get; set; }
        public string LastLogin { get; set; }
        public string SharpspringUserName { get; set; }
        public string SharpspringPassword { get; set; }
    }

    public class EZShredAccountMapping
    {
        public string EZShredId { get; set; }
        public string AccountObjectId { get; set; }
    }
}
