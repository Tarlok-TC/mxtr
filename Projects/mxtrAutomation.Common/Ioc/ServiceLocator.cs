using System;
using System.Collections.Generic;

namespace mxtrAutomation.Common.Ioc
{
    public interface IServiceLocator
    {
        T GetInstance<T>();
        T GetInstance<T>(string name);
        T TryGetInstance<T>();
        T TryGetInstance<T>(string name);        
        object GetInstance(Type type);
        IEnumerable<T> GetAllInstances<T>();
    }

    public abstract class ServiceLocator : IServiceLocator
    {
        public static IServiceLocator Current { get; protected set; }

        public abstract object GetInstance(Type type);
        public abstract T GetInstance<T>(string name);
        public abstract T TryGetInstance<T>(string name);
        public abstract IEnumerable<T> GetAllInstances<T>();

        public virtual T GetInstance<T>()
        {
            return GetInstance<T>(null);
        }

        public virtual T TryGetInstance<T>()
        {
            return TryGetInstance<T>(null);
        }

        public static void SetLocatorProvider(Func<IServiceLocator> newProvider)
        {
            Current = newProvider();
        }
    }
}
