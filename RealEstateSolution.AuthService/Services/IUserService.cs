using RealEstateSolution.AuthService.Models;
using RealEstateSolution.Common.Utils;
using System.Threading.Tasks;

namespace RealEstateSolution.AuthService.Services
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        Task<ApiResponse<UserDto>> GetUserInfoAsync(string userId);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        Task<ApiResponse<PagedList<UserDto>>> GetUsersAsync(string username, string realName, int? roleId, int pageNum, int pageSize);

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        Task<ApiResponse<UserDto>> GetUserAsync(string id);

        /// <summary>
        /// 创建用户
        /// </summary>
        Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request);

        /// <summary>
        /// 更新用户
        /// </summary>
        Task<ApiResponse<UserDto>> UpdateUserAsync(string id, UpdateUserRequest request);

        /// <summary>
        /// 删除用户
        /// </summary>
        Task<ApiResponse> DeleteUserAsync(string id);
    }
} 