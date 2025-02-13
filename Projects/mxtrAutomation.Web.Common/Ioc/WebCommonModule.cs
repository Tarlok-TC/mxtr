using mxtrAutomation.Web.Common.UI;
using Ninject.Modules;

namespace mxtrAutomation.Web.Common.Ioc
{
    public class WebCommonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IClientResourceBundleManager>().To<ClientResourceBundleManager>().InSingletonScope();
        }
    }
}
