namespace mxtrAutomation.Web.Common.Queries
{
    public class GooglePlusWebQuery : GooglePlusWebQueryBase
    {
        public static readonly string Route = "/";
    }

    public abstract class GooglePlusWebQueryBase : WebQueryBase
    {
        public override string BasePath { get { return "https://plus.google.com"; } }

        protected GooglePlusWebQueryBase()
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
