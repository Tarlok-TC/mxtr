using System;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Web.Common.Attributes;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class AdminAddAccountUserSubmitWebQuery : WebQueryBase<AccountUserViewData>
    {
        public static readonly string Route = "/admin/add-account-user-submit";

        public AdminAddAccountUserSubmitWebQuery()
        {
            _accountObjectID = Parameters.Add<string>("accountobjectid", true);
            _mxtrAccountID = Parameters.Add<string>("mxtraccountid", true);
            _userAccountData = Parameters.Add<string>("useraccountdata", true);
        }

        public string AccountObjectID
        {
            get { return _accountObjectID.Value; }
            set { _accountObjectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectID;

        public string MxtrAccountID
        {
            get { return _mxtrAccountID.Value; }
            set { _mxtrAccountID.Value = value; }
        }
        private readonly UrlParameterBase<string> _mxtrAccountID;

        public string UserAccountData
        {
            get { return _userAccountData.Value; }
            set { _userAccountData.Value = value; }
        }
        private readonly UrlParameterBase<string> _userAccountData;
    }
}