using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = false)]
    public class AdminAccountUserManagementWebQuery : WebQueryBase
    {
        public static readonly string Route = "/admin/account-user-management";

        public AdminAccountUserManagementWebQuery()
        {
            _accountObjectID = Parameters.Add<string>("accountobjectid", true);
            _isDelete = Parameters.Add<bool>("isDelete", false);
            _isAjax = Parameters.Add<bool>("isAjax", false);
        }

        public string AccountObjectID
        {
            get { return _accountObjectID.Value; }
            set { _accountObjectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectID;

        public bool IsDelete
        {
            get { return _isDelete.Value; }
            set { _isDelete.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isDelete;

        public bool IsAjax
        {
            get { return _isAjax.Value; }
            set { _isAjax.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isAjax;
    }
}
