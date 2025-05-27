using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstateSolution.Common.Utils;

namespace RealEstateSolution.AuthService.Extension
{
    /// <summary>
    /// JWT令牌生成辅助类
    /// </summary>
    public class JwtTokenHelper
    {
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="jwtSettings">JWT配置</param>
        public JwtTokenHelper(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// 生成访问令牌
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>访问令牌</returns>
        public string GenerateAccessToken(IdentityUser user)
        {
            return GenerateAccessToken(user, new List<string>());
        }

        /// <summary>
        /// 生成访问令牌（包含角色信息）
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="roles">用户角色</param>
        /// <returns>访问令牌</returns>
        public string GenerateAccessToken(IdentityUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                
                // 同时保留传统Claims以保持兼容性
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // 生成唯一jti                
                new Claim("SecurityStamp", user.SecurityStamp ?? string.Empty)
            };

            // 添加角色信息
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // 传统角色声明
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        /// <returns>刷新令牌</returns>
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>令牌验证结果</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取令牌过期时间
        /// </summary>
        /// <returns>过期时间</returns>
        public DateTime GetAccessTokenExpiration()
        {
            return DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
        }

        /// <summary>
        /// 获取刷新令牌过期时间
        /// </summary>
        /// <returns>过期时间</returns>
        public DateTime GetRefreshTokenExpiration()
        {
            return DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        }
    }
}