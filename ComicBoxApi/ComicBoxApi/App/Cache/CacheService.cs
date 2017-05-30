using Microsoft.Extensions.Caching.Memory;
using System;

namespace ComicBoxApi.App.Cache
{
    public interface ICacheService
    {
        T LoadFromCache<T>(string key, Func<T> service);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T LoadFromCache<T>(string key, Func<T> service)
        {
            T result;
            if (!_memoryCache.TryGetValue(key, out result))
            {
                result = service();
                _memoryCache.Set(key, result);
            }

            return result;
        }
    }
}
