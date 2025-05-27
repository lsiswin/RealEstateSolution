using System;

namespace RealEstateSolution.AuthService.Models
{
    /// <summary>
    /// 权限数据传输对象
    /// </summary>
    public class PermissionDto
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
} 