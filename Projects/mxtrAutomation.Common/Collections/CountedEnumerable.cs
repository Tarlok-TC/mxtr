using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Common.Collections
{
    public class CountedEnumerable<T> : ICountedEnumerable<T>
    {
        public int Count
        {
            get
            {
                if (!_count.HasValue)
                {
                    _count = Items.IsNullOrEmpty() ? 0 : Items.Count();
                }
                return _count.Value;
            }
            set
            {
                _count = value;
            }
        }
        private int? _count;

        public IEnumerable<T> Items { get; private set; }

        public CountedEnumerable(IEnumerable<T> items)
        {
            Items = items;
        }

        public CountedEnumerable(IEnumerable<T> items, int count)
        {
            Items = items;
            Count = count;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
