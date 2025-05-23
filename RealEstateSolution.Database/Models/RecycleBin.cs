using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.Database.Models;

/// <summary>
/// 回收站模型
/// </summary>
public class RecycleBin
{
    /// <summary>
    /// 回收站ID
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 实体类型
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string EntityType { get; set; } = null!;

    /// <summary>
    /// 实体ID
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// 实体数据（JSON格式）
    /// </summary>
    [Required]
    public string EntityData { get; set; } = null!;

    /// <summary>
    /// 删除原因
    /// </summary>
    [MaxLength(500)]
    public string? DeleteReason { get; set; }

    /// <summary>
    /// 删除人ID
    /// </summary>
    public int DeletedBy { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime DeleteTime { get; set; }

    /// <summary>
    /// 恢复时间
    /// </summary>
    public DateTime? RestoreTime { get; set; }

    /// <summary>
    /// 是否已恢复
    /// </summary>
    public bool IsRestored { get; set; }
} 