using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.AuthService.Models
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "当前密码不能为空")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "新密码不能为空")]
        [MinLength(6, ErrorMessage = "密码长度不能小于6个字符")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
