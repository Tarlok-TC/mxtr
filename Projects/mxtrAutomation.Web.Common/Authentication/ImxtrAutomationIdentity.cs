using System;
using System.Security.Principal;

namespace mxtrAutomation.Web.Common.Authentication
{
    public interface ImxtrAutomationIdentity : IIdentity
    {
        string MxtrUserObjectID { get; set; }
        string MxtrAccountObjectID { get; set; }
        string UserName { get; set; }
        string FullName { get; set; }
        string SelectedWorkspaceIDs { get; set; }
        string Role { get; set; }
        string SharpspringUserName { get; set; }
        string SharpspringPassword { get; set; }
    }
}
