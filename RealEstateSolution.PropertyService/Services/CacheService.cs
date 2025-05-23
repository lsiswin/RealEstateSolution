using Microsoft.Extensions.Caching.Memory;

namespace RealEstateSolution.PropertyService.Services;


/// <summary>
/// 缓存服务实现
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        try
        {
            return _cache.Get<T>(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取缓存失败，Key: {Key}", key);
            return default;
        }
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            _cache.Set(key, value, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置缓存失败，Key: {Key}", key);
        }
    }

    public void Remove(string key)
    {
        try
        {
            _cache.Remove(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除缓存失败，Key: {Key}", key);
        }
    }

    public T GetOrAdd<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        try
        {
            return _cache.GetOrCreate(key, entry =>
            {
                if (expiration.HasValue)
                {
                    entry.AbsoluteExpirationRelativeToNow = expiration;
                }
                return factory();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取或添加缓存失败，Key: {Key}", key);
            return factory();
        }
    }
} 