using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.AuthService.Models.IdentityModels;

/// <summary>
/// 权限
/// </summary>
public class Permission
{
    /// <summary>
    /// 权限ID
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 权限编码
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    /// <summary>
    /// 权限描述
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// 权限类型
    /// </summary>
    public PermissionType Type { get; set; }

    /// <summary>
    /// 父级权限ID
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

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
/// 权限类型
/// </summary>
public enum PermissionType
{
    /// <summary>
    /// 菜单
    /// </summary>
    Menu = 1,
    
    /// <summary>
    /// 按钮
    /// </summary>
    Button = 2,
    
    /// <summary>
    /// 接口
    /// </summary>
    Api = 3
} 