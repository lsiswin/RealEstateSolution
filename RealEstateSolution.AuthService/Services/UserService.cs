using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateSolution.AuthService.Data;
using RealEstateSolution.AuthService.Models;
using RealEstateSolution.AuthService.Models.IdentityModels;
using RealEstateSolution.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstateSolution.AuthService.Services
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        public async Task<ApiResponse<UserDto>> GetUserInfoAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Code = 404,
                    Message = "用户不存在"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await GetUserPermissionsAsync(user);

            return new ApiResponse<UserDto>
            {
                Code = 200,
                Message = "获取用户信息成功",
                Data = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    RealName = user.RealName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Permissions = permissions
                }
            };
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        public async Task<ApiResponse<PagedList<UserDto>>> GetUsersAsync(string username, string realName, int? roleId, int pageNum, int pageSize)
        {
            IQueryable<ApplicationUser> query = _userManager.Users;

            // 应用过滤条件
            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(u => u.UserName.Contains(username));
            }

            if (!string.IsNullOrEmpty(realName))
            {
                query = query.Where(u => u.RealName.Contains(realName));
            }

            // 获取总记录数
            var totalCount = await query.CountAsync();
            
            // 分页
            var users = await query
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 转换为DTO并获取角色
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                // 如果有角色过滤条件，则只返回有该角色的用户
                if (roleId.HasValue)
                {
                    var role = await _roleManager.FindByIdAsync(roleId.Value.ToString());
                    if (role != null && !roles.Contains(role.Name))
                    {
                        continue;
                    }
                }

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    RealName = user.RealName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    IsActive = user.IsEnabled
                });
            }

            return new ApiResponse<PagedList<UserDto>>
            {
                Code = 200,
                Message = "获取用户列表成功",
                Data = new PagedList<UserDto>
                   (
                       userDtos, // items: 用户DTO列表  
                       pageNum, // pageIndex: 当前页码  
                       pageSize, // pageSize: 每页大小  
                       totalCount // totalCount: 总记录数  
                   )
            };


            }

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        public async Task<ApiResponse<UserDto>> GetUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Code = 404,
                    Message = "用户不存在"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var rolesList = new List<RoleDto>();
            
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    rolesList.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name
                    });
                }
            }

            return new ApiResponse<UserDto>
            {
                Code = 200,
                Message = "获取用户信息成功",
                Data = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    RealName = user.RealName,
                    Email = user.Email,      
                }
            };
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request)
        {
            // 检查用户名是否已存在
            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                return new ApiResponse<UserDto>
                {
                    Code = 400,
                    Message = "用户名已存在"
                };
            }

            // 创建新用户
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                RealName = request.RealName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse<UserDto>
                {
                    Code = 400,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // 分配角色
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId.ToString());
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }
            }

            // 返回创建的用户信息
            return await GetUserAsync(user.Id);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public async Task<ApiResponse<UserDto>> UpdateUserAsync(string id, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ApiResponse<UserDto>
                {
                    Code = 404,
                    Message = "用户不存在"
                };
            }

            // 更新用户信息
            user.RealName = request.RealName;
            user.Email = request.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new ApiResponse<UserDto>
                {
                    Code = 400,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // 更新角色
            if (request.RoleIds != null)
            {
                // 获取当前角色
                var currentRoles = await _userManager.GetRolesAsync(user);
                
                // 移除所有当前角色
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // 添加新角色
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId.ToString());
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }
            }

            // 返回更新后的用户信息
            return await GetUserAsync(id);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public async Task<ApiResponse> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ApiResponse
                {
                    Code = 404,
                    Message = "用户不存在"
                };
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new ApiResponse
                {
                    Code = 400,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new ApiResponse
            {
                Code = 200,
                Message = "删除用户成功"
            };
        }

        /// <summary>
        /// 获取用户权限
        /// </summary>
        private async Task<List<string>> GetUserPermissionsAsync(ApplicationUser user)
        {
            var permissions = new List<string>();
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    // 获取角色对应的权限
                    var rolePermissions = await _dbContext.RolePermissions
                        .Where(rp => rp.RoleId == role.Id)
                        .Select(rp => rp.RoleId)
                        .ToListAsync();

                    permissions.AddRange(rolePermissions);
                }
            }

            // 去重
            return permissions.Distinct().ToList();
        }
    }
} 