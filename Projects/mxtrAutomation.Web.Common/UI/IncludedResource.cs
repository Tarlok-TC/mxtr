using System;
using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.UI
{
    public class IncludedResource<T> : IEquatable<IncludedResource<T>>
    {
        public T ResourceKind { get; private set; }
        public BundleKind BundleKind { get; private set; }

        public IncludedResource(T kind, BundleKind bundleKind)
        {
            ResourceKind = kind;
            BundleKind = bundleKind;
        }

        public bool Equals(IncludedResource<T> other)
        {
            return (EqualityComparer<T>.Default.Equals(ResourceKind, other.ResourceKind)) && (BundleKind == other.BundleKind);
        }
    }
}
