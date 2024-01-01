namespace RedisCache.DistributedCache
{
    public interface ICacheService
    {
        Task<T?> GetItemAsync<T>(string key);
        Task RemoveItemAsync(string key);
        Task<bool> SetItemAsync<T>(string key, T value, DateTimeOffset expirationTime);
    }
}
