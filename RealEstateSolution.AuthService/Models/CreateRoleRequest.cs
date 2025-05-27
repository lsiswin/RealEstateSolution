using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.AuthService.Models
{
    /// <summary>
    /// 创建角色请求
    /// </summary>
    public class CreateRoleRequest
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "角色名称不能为空")]
        [StringLength(50, ErrorMessage = "角色名称长度不能超过50个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(200, ErrorMessage = "角色描述长度不能超过200个字符")]
        public string Description { get; set; }
    }
} 