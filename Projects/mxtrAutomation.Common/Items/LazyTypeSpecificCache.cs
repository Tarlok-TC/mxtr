using System;
using System.Collections.Generic;

namespace mxtrAutomation.Common.Items
{
    public class LazyTypeSpecificCache<TClass, TKey, TValue>
        where TClass : class
    {
        private readonly IDictionary<TKey, TValue> _valueDictionary = new Dictionary<TKey, TValue>();
        private readonly object _valueDictionaryLock = new object();

        private readonly Func<TClass, TKey> _getKey;
        private readonly Func<TClass, TValue> _getValue;

        public LazyTypeSpecificCache(Func<TClass, TKey> getKey, Func<TClass, TValue> getValue)
        {
            _getKey = getKey;
            _getValue = getValue;
        }

        public TValue GetValue(TClass obj)
        {
            TKey key = _getKey(obj);

            if (!_valueDictionary.ContainsKey(key))
            {
                lock (_valueDictionaryLock)
                {
                    if (!_valueDictionary.ContainsKey(key))
                    {
                        _valueDictionary[key] = _getValue(obj);
                    }
                }
            }

            return _valueDictionary[key];
        }
    }
}
