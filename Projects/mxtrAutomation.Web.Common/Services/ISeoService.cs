namespace mxtrAutomation.Web.Common.Services
{
    public interface ISeoService
    {
        string BuildSiteMapXml();
    }

    public interface ISeoServiceInternal : ISeoService
    {
    }
}
