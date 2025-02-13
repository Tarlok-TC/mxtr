using mxtrAutomation.Common.Utils;
using Ninject.Modules;

namespace mxtrAutomation.Common.Ioc
{
    public class CommonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IServiceLocator>().ToMethod(x => ServiceLocator.Current);
            Bind<IConfigManager>().To<DefaultConfigManager>().InSingletonScope();
            Bind<ICipherUtility>().To<CipherUtility>().InSingletonScope();
            Bind<IFileUtils>().To<FileUtils>().InSingletonScope();
            Bind<ICsvContext>().To<AdTrakCsvContext>();
        }
    }
}
