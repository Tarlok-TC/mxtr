using System;
using System.Collections.Generic;
using Ninject;

namespace mxtrAutomation.Common.Ioc
{
    public class NinjectServiceLocator : ServiceLocator
    {
        private readonly IKernel _kernel;

        public NinjectServiceLocator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object GetInstance(Type type)
        {
            return _kernel.Get(type);
        }

        public override T GetInstance<T>(string name)
        {
            return _kernel.Get<T>(name);
        }

        public override T TryGetInstance<T>(string name)
        {
            return _kernel.TryGet<T>(name);
        }

        public override IEnumerable<T> GetAllInstances<T>()
        {
            return _kernel.GetAll<T>();
        }
    }
}
