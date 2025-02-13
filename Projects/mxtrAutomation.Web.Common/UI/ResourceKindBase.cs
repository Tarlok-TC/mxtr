using System;

namespace mxtrAutomation.Web.Common.UI
{
    public abstract class ResourceKindBase<T> : IEquatable<ResourceKindBase<T>>
        where T : IEquatable<T>
    {
        public T Value { get; private set; }

        protected ResourceKindBase(T value)
        {
            Value = value;
        }

        public bool Equals(ResourceKindBase<T> other)
        {
            return Value.Equals(other.Value);
        }
    }
}
