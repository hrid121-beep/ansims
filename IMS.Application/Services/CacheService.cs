using IMS.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace IMS.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                cacheOptions.SetSlidingExpiration(expiration.Value);
            }
            else
            {
                cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Default 30 minutes
            }

            _cache.Set(key, value, cacheOptions);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public Task ClearAsync()
        {
            // Note: IMemoryCache doesn't have a clear method
            // You'd need to track keys separately if you need this functionality
            return Task.CompletedTask;
        }

        public async Task RemovePatternAsync(string pattern)
        {
            // Memory cache doesn't support pattern-based removal
            // This would need a more sophisticated implementation
            // For now, we'll need to track keys separately if pattern removal is needed

            // In a real implementation, you might want to maintain a separate
            // collection of cache keys to enable pattern-based removal
            await Task.CompletedTask;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue<T>(key, out var cachedValue))
            {
                return cachedValue;
            }

            var value = await getItem();

            if (value != null)
            {
                await SetAsync(key, value, expiration);
            }

            return value;
        }
    }
}