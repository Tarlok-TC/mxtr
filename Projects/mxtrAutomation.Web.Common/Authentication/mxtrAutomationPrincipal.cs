using System.Security.Principal;

namespace mxtrAutomation.Web.Common.Authentication
{
    public class mxtrAutomationPrincipal : GenericPrincipal, ImxtrAutomationPrincipal
    {
        public new ImxtrAutomationIdentity Identity { get; set; }

        public mxtrAutomationPrincipal(ImxtrAutomationIdentity identity, string[] roles)
            : base(identity, roles)
        {
            Identity = identity;
        }
    }
}
