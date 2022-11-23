using CurrencyExchanger.Models.Constants;
using CurrencyExchanger.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CurrencyExchanger.Test
{
    public class CachingServiceTest : IClassFixture<TestServiceProvider>
    {
        private readonly IMemoryCache _cache;

        public CachingServiceTest(TestServiceProvider serviceProvider)
        {
            _cache = serviceProvider.ServiceProvider.GetRequiredService<IMemoryCache>();
        }

        [Fact]
        public void CachingService_AddObjectToCache_ObjectAddedCorrectly()
        {
            bool exist = _cache.GetCachedObject<string>(CacheKeys.TestCacheKey).exists;
            _cache.AddCachedObject(CacheKeys.TestCacheKey, "Test Caching", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
            (bool afterAddExist, string? afterAddValue) = _cache.GetCachedObject<string>(CacheKeys.TestCacheKey);
            Assert.True(!exist && afterAddExist && afterAddValue == "Test Caching");
        }
        
        [Fact]
        public void CachingService_CachedObjectExist_MustNotBeRemovedBefore3Sec()
        {
            _cache.Remove(CacheKeys.TestCacheKey);
            _cache.AddCachedObject(CacheKeys.TestCacheKey, "Test Caching", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
            bool exist = _cache.GetCachedObject<string>(CacheKeys.TestCacheKey).exists;
            Thread.Sleep(1000);
            bool afterRemoveExist = _cache.GetCachedObject<string>(CacheKeys.TestCacheKey).exists;
            Assert.True(exist && afterRemoveExist);
        }

        [Fact]
        public void CachingService_CachedObjectRemoved_MustBeRemovedAfter3Sec()
        {
            _cache.Remove(CacheKeys.TestCacheKey);
            _cache.AddCachedObject(CacheKeys.TestCacheKey, "Test Caching", TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
            bool exist = _cache.GetCachedObject<string>(CacheKeys.TestCacheKey).exists;
            Thread.Sleep(3020);
            (bool afterRemoveExist, string? afterRemoveValue) = _cache.GetCachedObject<string>(CacheKeys.TestCacheKey);
            Assert.True(exist && !afterRemoveExist && afterRemoveValue == null);
        }

    }
}
