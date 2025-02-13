using System;

namespace mxtrAutomation.Common.Ioc
{
    public class LazyInjected<T> : Lazy<T>
    {
        public LazyInjected() : base(() => ServiceLocator.Current.TryGetInstance<T>()) { }
    }
}
