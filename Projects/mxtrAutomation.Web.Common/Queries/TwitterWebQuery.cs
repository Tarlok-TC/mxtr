namespace mxtrAutomation.Web.Common.Queries
{
    public class TwitterWebQuery : TwitterWebQueryBase
    {
        public static readonly string Route = "/";
    }

    public abstract class TwitterWebQueryBase : WebQueryBase
    {
        public override string BasePath { get { return "http://twitter.com"; } }

        protected TwitterWebQueryBase()
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
