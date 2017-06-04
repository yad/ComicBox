using Microsoft.Extensions.Caching.Memory;
using System;

namespace ComicBoxApi.App.Cache
{
    public interface ICacheService
    {
        T TryLoadFromCache<T>(string key, Func<T> service, bool cache);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T TryLoadFromCache<T>(string key, Func<T> service, bool cache)
        {
            T result;
            if (!_memoryCache.TryGetValue(key, out result))
            {
                result = service();
                if (cache)
                {
                    _memoryCache.Set(key, result);
                }
            }

            return result;
        }
    }
}
