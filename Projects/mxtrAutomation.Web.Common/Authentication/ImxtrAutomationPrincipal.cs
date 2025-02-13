using System.Security.Principal;

namespace mxtrAutomation.Web.Common.Authentication
{
    public interface ImxtrAutomationPrincipal : IPrincipal
    {
        new ImxtrAutomationIdentity Identity { get; set; }
    }
}
