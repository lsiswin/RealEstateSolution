using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.AuthService.Models
{
    /// <summary>
    /// 分配权限请求
    /// </summary>
    public class AssignPermissionsRequest
    {
        /// <summary>
        /// 权限ID列表
        /// </summary>
        [Required(ErrorMessage = "权限ID列表不能为空")]
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
} 