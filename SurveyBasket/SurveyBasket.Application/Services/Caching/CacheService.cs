using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Services.Caching
{
    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var cachedValue = await _distributedCache.GetStringAsync(key);
            return cachedValue is null ? null : JsonSerializer.Deserialize<T>(cachedValue);
        }
        public async Task SetAsync<T>(string key, T value) where T : class
        {
            var json = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, json);
        }
        public async Task RemoveAsync(string key) 
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}
