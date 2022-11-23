using CurrencyExchanger.Models.APIModels;
using CurrencyExchanger.Models.Constants;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchanger.Services
{
    public static class CachingServics
    {
        /// <summary>
        /// Get cached object if exist
        /// </summary>
        /// <typeparam name="T"> object type</typeparam>
        /// <param name="cache">the cache that contains the object </param>
        /// <param name="key"> object key in the cache</param>
        /// <returns> true and the object if object found and false and default object value otherwise,
        /// </returns>
        public static (bool exists, T? value) GetCachedObject<T>(this IMemoryCache cache, object key)
        {
            try
            {
                var result = cache.TryGetValue(key, out T? cachedObject);
                return (result, cachedObject);
            }
            catch (Exception e)
            {
                Log.Error("Something went wrong while retrieve cached object");
                Log.Error(e.ToString());
                return (false, default(T));
            }

        }

        /// <summary>
        /// store an object in the cache
        /// </summary>
        /// <param name="cache">the cache that will contains the object</param>
        /// <param name="key">object key in the cache</param>
        /// <param name="objectToCache">the object which will be cached</param>
        /// <param name="priority">the priority of the cached object</param>
        /// <param name="inactiveDestroy">How long the object will be stored in the cache if there are no requests sent to retrieve that object</param>
        /// <param name="activeDestroy">How long the object will be stored in the cache even when there are requests sent to retrieve that object</param>
        /// <returns>true if the object stored successfully false otherwise</returns>
        public static bool AddCachedObject(this IMemoryCache cache, object key, object objectToCache, TimeSpan inactiveDestroy, TimeSpan activeDestroy, CacheItemPriority? priority = null)
        {
            try
            {
                if (objectToCache == null) return false;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetPriority(priority ?? CacheItemPriority.Normal)
                        .SetSlidingExpiration(inactiveDestroy)
                        .SetAbsoluteExpiration(activeDestroy);

                cache.Set(key, objectToCache, cacheEntryOptions);
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Something went wrong while retrieve cached object");
                Log.Error(e.ToString());
                return false;
            }

        }
    }
}
