using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateSolution.AuthService.Data;
using RealEstateSolution.AuthService.Models;
using RealEstateSolution.AuthService.Models.IdentityModels;
using RealEstateSolution.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstateSolution.AuthService.Services
{
    /// <summary>
    /// 角色服务实现
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RoleService(
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        public async Task<ApiResponse<List<RoleDto>>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = GetRoleDescription(r.Id)
            }).ToList();

            return new ApiResponse<List<RoleDto>>
            {
                Code = 200,
                Message = "获取角色列表成功",
                Data = roleDtos
            };
        }

        /// <summary>
        /// 获取单个角色
        /// </summary>
        public async Task<ApiResponse<RoleDto>> GetRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new ApiResponse<RoleDto>
                {
                    Code = 404,
                    Message = "角色不存在"
                };
            }

            return new ApiResponse<RoleDto>
            {
                Code = 200,
                Message = "获取角色成功",
                Data = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = GetRoleDescription(role.Id)
                }
            };
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        public async Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request)
        {
            // 检查角色名是否已存在
            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole != null)
            {
                return new ApiResponse<RoleDto>
                {
                    Code = 400,
                    Message = "角色名已存在"
                };
            }

            // 创建角色
            var role = new IdentityRole(request.Name);
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return new ApiResponse<RoleDto>
                {
                    Code = 400,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            //// 保存角色描述
            //await SaveRoleDescriptionAsync(role.Id, request.Description);

            return new ApiResponse<RoleDto>
            {
                Code = 200,
                Message = "创建角色成功",
                Data = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = request.Description
                }
            };
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        public async Task<ApiResponse<RoleDto>> UpdateRoleAsync(string id, UpdateRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new ApiResponse<RoleDto>
                {
                    Code = 404,
                    Message = "角色不存在"
                };
            }

            // 如果角色名变更了，需要检查新名称是否已存在
            if (role.Name != request.Name)
            {
                var existingRole = await _roleManager.FindByNameAsync(request.Name);
                if (existingRole != null && existingRole.Id != id)
                {
                    return new ApiResponse<RoleDto>
                    {
                        Code = 400,
                        Message = "角色名已存在"
                    };
                }

                role.Name = request.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    return new ApiResponse<RoleDto>
                    {
                        Code = 400,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }
            }

            //// 更新角色描述
            //await SaveRoleDescriptionAsync(id, request.Description);

            return new ApiResponse<RoleDto>
            {
                Code = 200,
                Message = "更新角色成功",
                Data = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = request.Description
                }
            };
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        public async Task<ApiResponse> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new ApiResponse
                {
                    Code = 404,
                    Message = "角色不存在"
                };
            }

            // 检查是否为内置角色
            if (role.Name == "admin")
            {
                return new ApiResponse
                {
                    Code = 400,
                    Message = "不能删除内置的管理员角色"
                };
            }

            // 删除角色
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return new ApiResponse
                {
                    Code = 400,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // 删除角色描述和权限
            await DeleteRoleDescriptionAsync(id);
            await DeleteRolePermissionsAsync(id);

            return new ApiResponse
            {
                Code = 200,
                Message = "删除角色成功"
            };
        }

        /// <summary>
        /// 获取所有权限
        /// </summary>
        public async Task<ApiResponse<List<PermissionDto>>> GetPermissionsAsync()
        {
            var permissions = await _dbContext.Permissions.ToListAsync();
            var permissionDtos = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code
            }).ToList();

            return new ApiResponse<List<PermissionDto>>
            {
                Code = 200,
                Message = "获取权限列表成功",
                Data = permissionDtos
            };
        }

        /// <summary>
        /// 获取角色的权限
        /// </summary>
        public async Task<ApiResponse<List<PermissionDto>>> GetRolePermissionsAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return new ApiResponse<List<PermissionDto>>
                {
                    Code = 404,
                    Message = "角色不存在"
                };
            }

            var permissionIds = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var permissions = await _dbContext.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync();

            var permissionDtos = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code
            }).ToList();

            return new ApiResponse<List<PermissionDto>>
            {
                Code = 200,
                Message = "获取角色权限成功",
                Data = permissionDtos
            };
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        public async Task<ApiResponse> AssignRolePermissionsAsync(string roleId, List<int> permissionIds)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return new ApiResponse
                {
                    Code = 404,
                    Message = "角色不存在"
                };
            }

            // 删除现有权限
            await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ExecuteDeleteAsync();

            // 添加新权限
            if (permissionIds != null && permissionIds.Any())
            {
                foreach (var permissionId in permissionIds)
                {
                    var permission = await _dbContext.Permissions.FindAsync(permissionId);
                    if (permission != null)
                    {
                        _dbContext.RolePermissions.Add(new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permissionId
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            return new ApiResponse
            {
                Code = 200,
                Message = "分配角色权限成功"
            };
        }

        /// <summary>
        /// 获取角色描述
        /// </summary>
        private string GetRoleDescription(string roleId)
        {
            return _dbContext.RolePermissions
                .FirstOrDefault(rd => rd.RoleId == roleId).ToString();
        }

        /// <summary>
        /// 保存角色描述
        /// </summary>
        //private async Task SaveRoleDescriptionAsync(string roleId, string description)
        //{
        //    var roleDesc = await _dbContext.RoleDescriptions
        //        .FirstOrDefaultAsync(rd => rd.RoleId == roleId);

        //    if (roleDesc == null)
        //    {
        //        _dbContext.RoleDescriptions.Add(new RoleDescription
        //        {
        //            RoleId = roleId,
        //            Description = description
        //        });
        //    }
        //    else
        //    {
        //        roleDesc.Description = description;
        //    }

        //    await _dbContext.SaveChangesAsync();
        //}

        /// <summary>
        /// 删除角色描述
        /// </summary>
        private async Task DeleteRoleDescriptionAsync(string roleId)
        {
            await _dbContext.RolePermissions
                .Where(rd => rd.RoleId == roleId)
                .ExecuteDeleteAsync();
        }

        /// <summary>
        /// 删除角色权限
        /// </summary>
        private async Task DeleteRolePermissionsAsync(string roleId)
        {
            await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ExecuteDeleteAsync();
        }
    }
} 