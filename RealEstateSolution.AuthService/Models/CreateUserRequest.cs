using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RealEstateSolution.AuthService.Models
{
    /// <summary>
    /// 创建用户请求
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度在6-100个字符之间")]
        public string Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Phone(ErrorMessage = "手机号码格式不正确")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [StringLength(50, ErrorMessage = "真实姓名长度不能超过50个字符")]
        public string RealName { get; set; }

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<string> RoleIds { get; set; } = new List<string>();

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
} 