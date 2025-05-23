using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RealEstateSolution.Common.Redis
{
    /// <summary>
    /// Redis服务实现
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="redis">Redis连接</param>
        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        /// <summary>
        /// 将令牌添加到黑名单
        /// </summary>
        public async Task<bool> AddToBlacklistAsync(string token, TimeSpan expiration)
        {
            return await _db.StringSetAsync($"blacklist:{token}", "1", expiration);
        }

        /// <summary>
        /// 检查令牌是否在黑名单中
        /// </summary>
        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _db.KeyExistsAsync($"blacklist:{token}");
        }

        /// <summary>
        /// 从黑名单中移除令牌
        /// </summary>
        public async Task<bool> RemoveFromBlacklistAsync(string token)
        {
            return await _db.KeyDeleteAsync($"blacklist:{token}");
        }

        /// <summary>
        /// 保存刷新令牌
        /// </summary>
        public async Task<bool> SaveRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiration)
        {
            return await _db.StringSetAsync($"refresh_token:{userId}", refreshToken, expiration);
        }

        /// <summary>
        /// 获取刷新令牌
        /// </summary>
        public async Task<string?> GetRefreshTokenAsync(string userId)
        {
            var token = await _db.StringGetAsync($"refresh_token:{userId}");
            return token.ToString();
        }

        /// <summary>
        /// 移除刷新令牌
        /// </summary>
        public async Task<bool> RemoveRefreshTokenAsync(string userId)
        {
            return await _db.KeyDeleteAsync($"refresh_token:{userId}");
        }
    }
}