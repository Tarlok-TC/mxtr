namespace mxtrAutomation.Web.Common.Queries
{
    public class PinterestWebQuery : PinterestWebQueryBase
    {
        public static readonly string Route = "/";
    }

    public abstract class PinterestWebQueryBase : WebQueryBase
    {
        public override string BasePath { get { return "http://pinterest.com"; } }

        protected PinterestWebQueryBase()
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
