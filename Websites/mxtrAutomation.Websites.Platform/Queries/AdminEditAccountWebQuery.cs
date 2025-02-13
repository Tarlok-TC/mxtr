using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = false)]
    public class AdminEditAccountWebQuery : WebQueryBase
    {
        public static readonly string Route = "/admin/edit-account";

        public AdminEditAccountWebQuery()
        {
            _accountObjectID = Parameters.Add<string>("accountobjectid", true);
        }

        public string AccountObjectID
        {
            get { return _accountObjectID.Value; }
            set { _accountObjectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectID;    
    
    }
}
