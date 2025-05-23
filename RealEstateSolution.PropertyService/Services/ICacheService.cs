namespace RealEstateSolution.PropertyService.Services
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        T? Get<T>(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        void Set<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// 移除缓存
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        T GetOrAdd<T>(string key, Func<T> factory, TimeSpan? expiration = null);
    }

}