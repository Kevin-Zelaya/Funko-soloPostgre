public interface ICache
{
    Task ClearAsync();
    Task<T?> GetTAsync<T>(string key);
    Task RemoveAsync(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
}