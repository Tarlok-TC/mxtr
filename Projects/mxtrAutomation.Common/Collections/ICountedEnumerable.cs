using System.Collections.Generic;

namespace mxtrAutomation.Common.Collections
{
    public interface ICountedEnumerable<T> : IEnumerable<T>
    {
        int Count { get; }
    }
}
