using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class WhiteLabelingWebQuery : WebQueryBase
    {
        public static readonly string Route = "/white-labeling";
        private readonly UrlParameterBase<string> _applicationLogoURL;
        private readonly UrlParameterBase<string> _brandingLogoURL;
        public WhiteLabelingWebQuery()
        {
            _applicationLogoURL = Parameters.Add<string>("applicationLogoURL", true);
            _brandingLogoURL = Parameters.Add<string>("brandingLogoURL", true);
        }
        public string ApplicationLogoURL
        {
            get { return _applicationLogoURL.Value; }
            set { _applicationLogoURL.Value = value; }
        }
        public string BrandingLogoURL
        {
            get { return _brandingLogoURL.Value; }
            set { _brandingLogoURL.Value = value; }
        }
    }

    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class WhiteLabelingManageDomainWebQuery : WebQueryBase
    {
        public static readonly string Route = "/whitelabeling-managedomain";
        public WhiteLabelingManageDomainWebQuery()
        {
            _domainName = Parameters.Add<string>("domainName", true);
        }
        public string DomainName
        {
            get { return _domainName.Value; }
            set { _domainName.Value = value; }
        }
        private readonly UrlParameterBase<string> _domainName;
    }

    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class WhiteLabelingManageHomePageWebQuery : WebQueryBase
    {
        public static readonly string Route = "/whitelabeling-homepage";
        public WhiteLabelingManageHomePageWebQuery()
        {
            _homePageUrl = Parameters.Add<string>("homePageUrl", true);
        }
        public string HomePageUrl
        {
            get { return _homePageUrl.Value; }
            set { _homePageUrl.Value = value; }
        }
        private readonly UrlParameterBase<string> _homePageUrl;
    }
}