using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.AuthService.Models.IdentityModels;

/// <summary>
/// 角色权限关联
/// </summary>
public class RolePermission
{
    /// <summary>
    /// 关联ID
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [Required]
    public string RoleId { get; set; } = null!;

    /// <summary>
    /// 权限ID
    /// </summary>
    [Required]
    public int PermissionId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
} 