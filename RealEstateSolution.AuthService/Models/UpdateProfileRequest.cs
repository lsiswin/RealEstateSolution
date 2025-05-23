using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.AuthService.Models
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "真实姓名不能为空")]
        [MaxLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
        public string UserName { get; set; } = null!;



        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string? Email { get; set; }
    }
}
