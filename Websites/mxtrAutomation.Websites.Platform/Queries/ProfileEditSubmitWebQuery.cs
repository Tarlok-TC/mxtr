using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class ProfileEditSubmitWebQuery : WebQueryBase
    {
        public static readonly string Route = "/edit-profile-submit";

        public ProfileEditSubmitWebQuery()
        {
            _password = Parameters.Add<string>("password", true);
            _newPassword = Parameters.Add<string>("newpassword", true);
        }      

        public string Password
        {
            get { return _password.Value; }
            set { _password.Value = value; }
        }
        private readonly UrlParameterBase<string> _password;

        public string NewPassword
        {
            get { return _newPassword.Value; }
            set { _newPassword.Value = value; }
        }
        private readonly UrlParameterBase<string> _newPassword;

    }
}