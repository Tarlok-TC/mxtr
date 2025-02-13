namespace mxtrAutomation.Web.Common.Queries
{
    public class YouTubeWebQuery : YouTubeUserWebQueryBase
    {
        public static readonly string Route = "/";
    }

    public abstract class YouTubeWebQueryBase : WebQueryBase
    {
        public override string BasePath { get { return "http://www.youtube.com"; } }
    }

    public abstract class YouTubeUserWebQueryBase : LinkedInWebQueryBase
    {
        public override string BasePath { get { return base.BasePath + "/user"; } }

        protected YouTubeUserWebQueryBase()
        {
            _user = Parameters.Add(new UrlPathParameter<string>("user", true));
        }

        public string User
        {
            get { return _user.Value; }
            set { _user.Value = value; }
        }
        private readonly UrlParameterBase<string> _user;
    }
}
