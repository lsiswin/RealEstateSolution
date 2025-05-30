using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateSolution.Database.Models;

/// <summary>
/// 合同模板实体
/// </summary>
[Table("ContractTemplates")]
public class ContractTemplate
{
    /// <summary>
    /// 模板ID
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 模板名称
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 模板描述
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 模板类型
    /// </summary>
    public ContractType Type { get; set; }

    /// <summary>
    /// 模板内容
    /// </summary>
    [Required]
    [Column(TypeName = "ntext")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
} 