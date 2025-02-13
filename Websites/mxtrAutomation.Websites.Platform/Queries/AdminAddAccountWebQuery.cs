using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = false)]
    public class AdminAddAccountWebQuery : WebQueryBase
    {
        public static readonly string Route = "/admin/add-account";

        public AdminAddAccountWebQuery()
        {
            _parentAccountObjectID = Parameters.Add<string>("parentaccountobjectid", true);
        }

        public string ParentAccountObjectID
        {
            get { return _parentAccountObjectID.Value; }
            set { _parentAccountObjectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _parentAccountObjectID; 
    }
}
