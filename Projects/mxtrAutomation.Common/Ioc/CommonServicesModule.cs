using mxtrAutomation.Common.Downloader;
using Ninject.Modules;

namespace mxtrAutomation.Common.Ioc
{
    public class CommonServicesModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDownloader>().To<HttpDownloader>().InSingletonScope();
        }
    }
}
