using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using Ninject.Modules;

namespace mxtrAutomation.Websites.Platform.Ioc
{
    public partial class ViewModelAdapterModule : NinjectModule
    {
        partial void LoadViews();

        public override void Load()
        {
            Bind<IMainLayoutViewModelAdapter>().To<MainLayoutViewModelAdapter>().InSingletonScope();
            Bind<IPublicLayoutViewModelAdapter>().To<PublicLayoutViewModelAdapter>().InSingletonScope();

            LoadViews();
        }
    }
}