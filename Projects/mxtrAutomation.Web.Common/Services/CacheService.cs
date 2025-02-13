using System;
using System.Web;
using System.Web.Caching;
using System.Collections;
using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.Services
{
    public class CacheService : ICacheServiceInternal
    {
        public Cache Cache { get { return HttpRuntime.Cache; } }

        public List<string> GetAllCacheKeys()
        {
            if (Cache == null)
                return default(List<string>);

            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }
            return keys;
        }

        public T GetFromCache<T>(string key)
        {
            if (Cache == null)
                return default(T);

            return (T)Cache[key];
        }

        public bool RemoveFromCache(string key)
        {
            if (Cache == null)
                return false;

            if (Cache[key] != null)
            {
                Cache.Remove(key);
                return true;
            }

            return false;
        }

        public T SearchCache<T>(string searchTerm)
        {
            if (Cache == null)
                return default(T);

            T value = default(T);
            IDictionaryEnumerator enumerator = Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = (string)enumerator.Key;
                if (searchTerm.ToLower().IndexOf(key) != -1)
                    value = (T)enumerator.Value;

                break;
            }

            return (T)value;
        }

        public T AddToSlidingCache<T>(string key, T value, TimeSpan timeSpan)
        {
            if (Cache != null)
                Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, timeSpan);

            return value;
        }

        public T AddToAbsoluteCache<T>(string key, T value, DateTime expireDate)
        {
            if (Cache != null)
                Cache.Insert(key, value, null, expireDate, Cache.NoSlidingExpiration);

            return value;
        }
    }
}
