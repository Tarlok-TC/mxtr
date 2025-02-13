using System;
using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.Services
{
    public interface ICacheService
    {
        T GetFromCache<T>(string key);
        bool RemoveFromCache(string key);
        T SearchCache<T>(string searchTerm);
        T AddToSlidingCache<T>(string key, T value, TimeSpan timeSpan);
        T AddToAbsoluteCache<T>(string key, T value, DateTime expireDate);
        List<string> GetAllCacheKeys();
    }

    public interface ICacheServiceInternal : ICacheService
    {
    }
}
