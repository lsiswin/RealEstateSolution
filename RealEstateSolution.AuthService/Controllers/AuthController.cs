using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.AuthService.Models;
using RealEstateSolution.AuthService.Services;
using System.Threading.Tasks;

namespace RealEstateSolution.AuthService.Controllers
{
    /// <summary>
    /// 认证控制器
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model">登录请求模型</param>
        /// <returns>认证响应</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest model)
        {
            var result = await _authService.LoginAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="model">注册请求模型</param>
        /// <returns>认证响应</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest model)
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// 刷新访问令牌
        /// </summary>
        /// <param name="model">刷新令牌请求模型</param>
        /// <returns>认证响应</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            var result = await _authService.RefreshTokenAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="model">登出请求模型</param>
        /// <returns>认证响应</returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<AuthResponse>> Logout([FromBody] LogoutRequest model)
        {
            var result = await _authService.LogoutAsync(model);
            return Ok(result);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model">修改密码请求模型</param>
        /// <returns>认证响应</returns>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<AuthResponse>> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "未授权访问"
                });
            }

            var result = await _authService.ChangePasswordAsync(userId, model.CurrentPassword,model.NewPassword);
            return Ok(result);
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <param name="model">更新资料请求模型</param>
        /// <returns>认证响应</returns>
        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<AuthResponse>> UpdateProfile([FromBody] UpdateProfileRequest model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "未授权访问"
                });
            }

            var result = await _authService.UpdateProfileAsync(userId,model.UserName,model.Email);
            return Ok(result);
        }
    }
}