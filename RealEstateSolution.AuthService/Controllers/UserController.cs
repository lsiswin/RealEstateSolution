using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.AuthService.Models;
using RealEstateSolution.AuthService.Services;
using RealEstateSolution.Common.Utils;
using System.Threading.Tasks;

namespace RealEstateSolution.AuthService.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        [HttpGet("user/info")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserInfo()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<UserDto>
                {
                    Code = 401,
                    Message = "未授权访问"
                });
            }

            var result = await _userService.GetUserInfoAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<PagedList<UserDto>>>> GetUsers(
            [FromQuery] string username = null, 
            [FromQuery] string realName = null, 
            [FromQuery] int? roleId = null, 
            [FromQuery] int pageNum = 1, 
            [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetUsersAsync(username, realName, roleId, pageNum, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(string id)
        {
            var result = await _userService.GetUserAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        [HttpPost("users")]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        [HttpPut("users/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            return Ok(result);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }
    }
} 