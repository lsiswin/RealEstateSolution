using Microsoft.AspNetCore.Identity;

namespace RealEstateSolution.AuthService.Models.IdentityModels;

/// <summary>
/// 角色
/// </summary>
public class ApplicationRole : IdentityRole
{
    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
} 