using System;

namespace RealEstateSolution.AuthService.Models
{
    /// <summary>
    /// 认证响应模型
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// 用户信息模型
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }
    }

    /// <summary>
    /// 登录请求模型
    /// </summary>
    public class LoginRequest
    {

        /// <summary>
        /// 邮箱
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// 记住密码
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }

    /// <summary>
    /// 注册请求模型
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 刷新令牌请求模型
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// 登出请求模型
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
    }
} 