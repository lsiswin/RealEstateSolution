using Microsoft.AspNetCore.Identity;
using RealEstateSolution.AuthService.Models;
using AutoMapper;
using System.Security.Claims;
using RealEstateSolution.AuthService.Models.IdentityModels;
using RealEstateSolution.AuthService.Extension;
using RealEstateSolution.Common.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace RealEstateSolution.AuthService.Services
{
    /// <summary>
    /// 认证服务实现
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRedisService _redisService;
        private readonly JwtTokenHelper _jwtTokenHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRedisService redisService,
            JwtTokenHelper jwtTokenHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _redisService = redisService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "用户不存在" };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResponse { Success = false, Message = "密码错误" };
            }

            // 获取用户角色
            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _jwtTokenHelper.GenerateAccessToken(user, roles);
            var refreshToken = _jwtTokenHelper.GenerateRefreshToken();
            var accessTokenExpiration = _jwtTokenHelper.GetAccessTokenExpiration();
            var refreshTokenExpiration = _jwtTokenHelper.GetRefreshTokenExpiration();

            await _redisService.SaveRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiration - DateTime.Now);

            return new AuthResponse
            {
                Success = true,
                Message = "登录成功",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = accessTokenExpiration,
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                }
            };
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // 获取用户角色（新注册用户可能没有角色）
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenHelper.GenerateAccessToken(user, roles);
            var refreshToken = _jwtTokenHelper.GenerateRefreshToken();
            var accessTokenExpiration = _jwtTokenHelper.GetAccessTokenExpiration();
            var refreshTokenExpiration = _jwtTokenHelper.GetRefreshTokenExpiration();

            await _redisService.SaveRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiration - DateTime.Now);

            return new AuthResponse
            {
                Success = true,
                Message = "注册成功",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = accessTokenExpiration,
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                }
            };

        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = _jwtTokenHelper.ValidateToken(request.AccessToken);
            if (principal == null)
            {
                return new AuthResponse { Success = false, Message = "无效的访问令牌" };
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResponse { Success = false, Message = "无效的用户ID" };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "用户不存在" };
            }

            var storedRefreshToken = await _redisService.GetRefreshTokenAsync(userId);
            if (storedRefreshToken != request.RefreshToken)
            {
                return new AuthResponse { Success = false, Message = "无效的刷新令牌" };
            }

            // 获取用户角色
            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _jwtTokenHelper.GenerateAccessToken(user, roles);
            var refreshToken = _jwtTokenHelper.GenerateRefreshToken();
            var accessTokenExpiration = _jwtTokenHelper.GetAccessTokenExpiration();
            var refreshTokenExpiration = _jwtTokenHelper.GetRefreshTokenExpiration();

            await _redisService.SaveRefreshTokenAsync(userId, refreshToken, refreshTokenExpiration - DateTime.Now);

            return new AuthResponse
            {
                Success = true,
                Message = "令牌刷新成功",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = accessTokenExpiration,
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                }
            };
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        public async Task<AuthResponse> LogoutAsync(LogoutRequest request)
        {
            var principal = _jwtTokenHelper.ValidateToken(request.AccessToken);
            if (principal == null)
            {
                return new AuthResponse { Success = false, Message = "无效的访问令牌" };
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResponse { Success = false, Message = "无效的用户ID" };
            }
            var token = request.AccessToken;
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid token format"
                }; 
            }
            var jwt = handler.ReadJwtToken(token);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            // 计算剩余有效时间
            var exp = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp!));
            var remainingTime = expiryDate - DateTimeOffset.UtcNow;
            await _redisService.AddToBlacklistAsync(jti, TimeSpan.FromMinutes(30));
            await _redisService.RemoveRefreshTokenAsync(userId);

            return new AuthResponse
            {
                Success = true,
                Message = "登出成功"
            };
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public async Task<AuthResponse> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "用户不存在" };
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new AuthResponse
            {
                Success = true,
                Message = "密码修改成功"
            };
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        public async Task<AuthResponse> UpdateProfileAsync(string userId, string userName, string email)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "用户不存在" };
            }

            user.UserName = userName;
            user.Email = email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new AuthResponse
            {
                Success = true,
                Message = "资料更新成功",
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                }
            };
        }
    }
}
