using System;
using System.Collections.Generic;

namespace RealEstateSolution.AuthService.Models
{
    /// <summary>
    /// 角色数据传输对象
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
} 