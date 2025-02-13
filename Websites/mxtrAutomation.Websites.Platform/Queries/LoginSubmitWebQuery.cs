using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    public class LoginSubmitWebQuery : WebQueryBase
    {
        public static readonly string Route = "/login-submit";

        public LoginSubmitWebQuery()
        {
            _username = Parameters.Add<string>("username", true);
            _password = Parameters.Add<string>("password", true);
        }

        public string Username
        {
            get { return _username.Value; }
            set { _username.Value = value; }
        }
        private readonly UrlParameterBase<string> _username;

        public string Password
        {
            get { return _password.Value; }
            set { _password.Value = value; }
        }
        private readonly UrlParameterBase<string> _password;
    }
}