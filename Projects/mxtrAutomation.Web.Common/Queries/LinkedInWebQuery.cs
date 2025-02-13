namespace mxtrAutomation.Web.Common.Queries
{
    public class LinkedInWebQuery : LinkedInCompanyWebQueryBase
    {
        public static readonly string Route = "/";
    }

    public abstract class LinkedInWebQueryBase : WebQueryBase
    {
        public override string BasePath { get { return "http://www.linkedin.com"; } }
    }

    public abstract class LinkedInCompanyWebQueryBase : LinkedInWebQueryBase
    {
        public override string BasePath { get { return base.BasePath + "/company"; } }

        protected LinkedInCompanyWebQueryBase()
        {
            _company = Parameters.Add(new UrlPathParameter<string>("company", true));
        }

        public string Company
        {
            get { return _company.Value; }
            set { _company.Value = value; }
        }
        private readonly UrlParameterBase<string> _company;
    }
}