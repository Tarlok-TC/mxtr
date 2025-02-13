using System.Collections.Generic;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class mxtrUser
    {
        public string ObjectID { get; set; }
        public string MxtrUserID { get; set; }

        public string MxtrAccountID { get; set; }
        public string AccountObjectID { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public List<EZShredAccountDataModel> EZShredAccountMappings { get; set; }
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

    public class EZShredAccountDataModel
    {
        public string EZShredId { get; set; }
        public string AccountName { get; set; }
        public string AccountObjectId { get; set; }
        public string EZShredIP { get; set; }
        public string EZShredPort { get; set; }
    }
}
