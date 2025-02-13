using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Web.Common.Services;

namespace mxtrAutomation.Web.Common.Ioc
{
    public class WebCommonServiceModule : ServiceModuleBase
    {
        public override void Load()
        {
            SetUpBinding<ISeoService, ISeoServiceInternal, SeoService>();
            SetUpBinding<ICacheService, ICacheServiceInternal, CacheService>();
        }
    }
}
