using System;

namespace mxtrAutomation.Common.Items
{
    public abstract class CacheBase<TKey, TValue> : ICache<TKey, TValue>
    {
        private static readonly LazyTypeSpecificCache<CacheBase<TKey, TValue>, Type, ICacheDictionary<TKey, TValue>> Cache =
            new LazyTypeSpecificCache<CacheBase<TKey, TValue>, Type, ICacheDictionary<TKey, TValue>>(x => x.GetType(), x => x.NewCacheDictionary());
        private readonly object _lock = new object();

        protected abstract TValue GetInternal(TKey key);
        protected abstract ICacheDictionary<TKey, TValue> NewCacheDictionary();
        
        public TValue Get(TKey key)
        {
            ICacheDictionary<TKey, TValue> cache = Cache.GetValue(this);

            if (!cache.ContainsKey(key))
            {
                lock (_lock)
                {
                    if (!cache.ContainsKey(key))
                        cache.Add(key, GetInternal(key));
                }
            }

            return cache.Get(key);
        }

        public void Clear(TKey key)
        {
            ICacheDictionary<TKey, TValue> cache = Cache.GetValue(this);

            cache.Remove(key);
        }

        public void Clear()
        {
            ICacheDictionary<TKey, TValue> cache = Cache.GetValue(this);

            cache.Clear();
        }
    }
}
