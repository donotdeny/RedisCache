using StackExchange.Redis;
using System.Text.Json;

namespace TestRedisCache
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheService;
        public CacheService(IDatabase database)
        {
            _cacheService = database;
        }
        public async Task<T?> GetItemAsync<T>(string key)
        {
            var value = await _cacheService.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public async Task RemoveItemAsync(string key)
        {
            var item = await _cacheService.KeyExistsAsync(key);
            if (item)
            {
                _cacheService.KeyDelete(key);
            }
        }

        public async Task<bool> SetItemAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiration = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSuccess = await _cacheService.StringSetAsync(key, JsonSerializer.Serialize(value), expiration);
            return isSuccess;
        }
    }
}
