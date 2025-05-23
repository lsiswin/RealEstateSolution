namespace RealEstateSolution.Common.Models
{
    /// <summary>
    /// JWT配置设置
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 发行者
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// 访问令牌过期时间（分钟）
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; }

        /// <summary>
        /// 刷新令牌过期时间（天）
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; }
    }
}