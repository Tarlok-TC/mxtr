namespace mxtrAutomation.Web.Common.Queries
{
    public class FeedburnerWebQuery : FeedburnerWebQueryBase
    {
        public static readonly string Route = "/";
    }

    public abstract class FeedburnerWebQueryBase : WebQueryBase
    {
        public override string BasePath { get { return "http://feeds.feedburner.com"; } }

        protected FeedburnerWebQueryBase()
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
