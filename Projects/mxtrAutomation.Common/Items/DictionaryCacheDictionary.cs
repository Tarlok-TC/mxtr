using System.Collections.Generic;

namespace mxtrAutomation.Common.Items
{
    public class DictionaryCacheDictionary<TKey, TValue> : ICacheDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public TValue Get(TKey key)
        {
            return _dictionary[key];
        }

        public void Remove(TKey key)
        {
            _dictionary.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}
