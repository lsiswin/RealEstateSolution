using RealEstateSolution.AuthService.Models;

namespace RealEstateSolution.AuthService.Services
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="request">刷新令牌请求</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="request">登出请求</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> LogoutAsync(LogoutRequest request);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="currentPassword">当前密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="email">邮箱</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> UpdateProfileAsync(string userId, string userName, string email);
    }
}
