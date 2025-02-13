using System.Linq;
using mxtrAutomation.Common.Extensions;
using Ninject;
using Ninject.Modules;

namespace mxtrAutomation.Common.Ioc
{
    public abstract class ServiceModuleBase : NinjectModule
    {
        // ReSharper disable InconsistentNaming
        protected void SetUpBinding<TInterface, TInterfaceInternal, TImplementation>()
            where TImplementation : TInterfaceInternal
            where TInterfaceInternal : TInterface
        {
            Bind<TInterfaceInternal>().To<TImplementation>().InSingletonScope();
            Bind<TInterface>().ToMethod(c => c.Kernel.Get<TInterfaceInternal>());
        }
        // ReSharper restore InconsistentNaming

        protected void BindEnumNameMap<T>()
        {
            Bind<IEnumNameMap<T>>().ToMethod(x => CreateNameMap<T>()).InSingletonScope();            
        }

        protected IEnumNameMap<T> CreateNameMap<T>()
        {
            return new EnumNameMap<T>(EnumExtensions.ToList<T>().ToDictionary(y => y.ToString(), y => y));
        }
    }
}
