using RealEstateSolution.AuthService.Models;
using RealEstateSolution.Common.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateSolution.AuthService.Services
{
    /// <summary>
    /// 角色服务接口
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 获取所有角色
        /// </summary>
        Task<ApiResponse<List<RoleDto>>> GetRolesAsync();

        /// <summary>
        /// 获取单个角色
        /// </summary>
        Task<ApiResponse<RoleDto>> GetRoleAsync(string id);

        /// <summary>
        /// 创建角色
        /// </summary>
        Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request);

        /// <summary>
        /// 更新角色
        /// </summary>
        Task<ApiResponse<RoleDto>> UpdateRoleAsync(string id, UpdateRoleRequest request);

        /// <summary>
        /// 删除角色
        /// </summary>
        Task<ApiResponse> DeleteRoleAsync(string id);

        /// <summary>
        /// 获取所有权限
        /// </summary>
        Task<ApiResponse<List<PermissionDto>>> GetPermissionsAsync();

        /// <summary>
        /// 获取角色的权限
        /// </summary>
        Task<ApiResponse<List<PermissionDto>>> GetRolePermissionsAsync(string roleId);

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        Task<ApiResponse> AssignRolePermissionsAsync(string roleId, List<int> permissionIds);
    }
} 