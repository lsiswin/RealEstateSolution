using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.AuthService.Models;
using RealEstateSolution.AuthService.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstateSolution.Common.Utils;

namespace RealEstateSolution.AuthService.Controllers
{
    /// <summary>
    /// 角色管理控制器
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [Authorize(Roles = "admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetRoles()
        {
            var result = await _roleService.GetRolesAsync();
            return Ok(result);
        }

        /// <summary>
        /// 获取单个角色
        /// </summary>
        [HttpGet("roles/{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRole(string id)
        {
            var result = await _roleService.GetRoleAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        [HttpPost("roles")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleRequest request)
        {
            var result = await _roleService.CreateRoleAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        [HttpPut("roles/{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(string id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _roleService.UpdateRoleAsync(id, request);
            return Ok(result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpDelete("roles/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteRole(string id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 获取所有权限
        /// </summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetPermissions()
        {
            var result = await _roleService.GetPermissionsAsync();
            return Ok(result);
        }

        /// <summary>
        /// 获取角色的权限
        /// </summary>
        [HttpGet("roles/{roleId}/permissions")]
        public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetRolePermissions(string roleId)
        {
            var result = await _roleService.GetRolePermissionsAsync(roleId);
            return Ok(result);
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        [HttpPost("roles/{roleId}/permissions")]
        public async Task<ActionResult<ApiResponse>> AssignRolePermissions(string roleId, [FromBody] AssignPermissionsRequest request)
        {
            var result = await _roleService.AssignRolePermissionsAsync(roleId, request.PermissionIds);
            return Ok(result);
        }
    }
} 