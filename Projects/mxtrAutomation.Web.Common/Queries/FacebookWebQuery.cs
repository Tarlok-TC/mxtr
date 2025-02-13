namespace mxtrAutomation.Web.Common.Queries
{
    public class FacebookWebQuery : FacebookWebQueryBase
    {
        public static readonly string Route = "/";
    }

    public abstract class FacebookWebQueryBase : WebQueryBase
    {
        public override string BasePath { get { return "http://www.facebook.com"; } }

        protected FacebookWebQueryBase()
        {
            _page = Parameters.Add(new UrlPathParameter<string>("page", false));
        }

        public string Page
        {
            get { return _page.Value; }
            set { _page.Value = value; }
        }
        private readonly UrlParameterBase<string> _page;
    }
}
