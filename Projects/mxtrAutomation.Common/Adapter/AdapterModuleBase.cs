using System;
using Ninject.Activation;
using Ninject.Modules;

namespace mxtrAutomation.Common.Adapter
{
    public abstract class AdapterModuleBase : NinjectModule
    {
        protected internal static bool ProjectionHasMatchingArguments(IRequest context)
        {
            var args = context.Service.GetGenericArguments();
            return args.Length == 2 && args[0] == args[1];
        }

        protected internal static object ConstructIdentityProjection(IContext context)
        {
            var targetType = context.Request.Service.GetGenericArguments()[0];
            var identityType =
                typeof(IdentityProjectionAdapter<>)
                    .MakeGenericType(targetType);
            return Activator.CreateInstance(identityType);
        }

        protected void BindProjection<IN, OUT, IMPL>() where IMPL : IProjection<IN, OUT>
        {
            Bind<IProjection<IN, OUT>>().To<IMPL>().InSingletonScope();
        }
    }
}
