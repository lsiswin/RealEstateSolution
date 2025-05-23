using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RealEstateSolution.AuthService.Models.IdentityModels;

/// <summary>
/// 应用程序用户
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// 真实姓名
    /// </summary>
    [MaxLength(50)]
    public string? RealName { get; set; }

    /// <summary>
    /// 用户类型（管理员、经纪人、客户等）
    /// </summary>
    public UserType UserType { get; set; }

    /// <summary>
    /// 所属部门
    /// </summary>
    [MaxLength(50)]
    public string? Department { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [MaxLength(50)]
    public string? Position { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [MaxLength(200)]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}

/// <summary>
/// 用户类型
/// </summary>
public enum UserType
{
    /// <summary>
    /// 管理员
    /// </summary>
    Admin = 1,
    
    /// <summary>
    /// 经纪人
    /// </summary>
    Agent = 2,
    
    /// <summary>
    /// 客户
    /// </summary>
    Customer = 3
} 