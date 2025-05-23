using System.Threading.Tasks;

namespace RealEstateSolution.Common.Redis
{
    /// <summary>
    /// Redis服务接口
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// 将令牌添加到黑名单
        /// </summary>
        /// <param name="token">要加入黑名单的令牌</param>
        /// <param name="expiration">过期时间</param>
        /// <returns>是否添加成功</returns>
        Task<bool> AddToBlacklistAsync(string token, TimeSpan expiration);

        /// <summary>
        /// 检查令牌是否在黑名单中
        /// </summary>
        /// <param name="token">要检查的令牌</param>
        /// <returns>是否在黑名单中</returns>
        Task<bool> IsTokenBlacklistedAsync(string token);

        /// <summary>
        /// 从黑名单中移除令牌
        /// </summary>
        /// <param name="token">要移除的令牌</param>
        /// <returns>是否移除成功</returns>
        Task<bool> RemoveFromBlacklistAsync(string token);

        /// <summary>
        /// 保存刷新令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="refreshToken">刷新令牌</param>
        /// <param name="expiration">过期时间</param>
        /// <returns>是否保存成功</returns>
        Task<bool> SaveRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiration);

        /// <summary>
        /// 获取刷新令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>刷新令牌</returns>
        Task<string?> GetRefreshTokenAsync(string userId);

        /// <summary>
        /// 移除刷新令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否移除成功</returns>
        Task<bool> RemoveRefreshTokenAsync(string userId);
    }
}