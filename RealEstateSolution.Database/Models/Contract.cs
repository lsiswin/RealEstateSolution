using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateSolution.Database.Models;

/// <summary>
/// 合同状态枚举
/// </summary>
public enum ContractStatus
{
    Draft = 0,      // 草稿
    Pending = 1,    // 待签署
    Signed = 2,     // 已签署
    Completed = 3,  // 已完成
    Cancelled = 4   // 已取消
}

/// <summary>
/// 合同类型枚举
/// </summary>
public enum ContractType
{
    Sale = 0,       // 买卖合同
    Rent = 1,       // 租赁合同
    Commission = 2  // 委托合同
}

/// <summary>
/// 合同实体
/// </summary>
[Table("Contracts")]
public class Contract
{
    /// <summary>
    /// 合同ID
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 合同编号
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ContractNumber { get; set; } = string.Empty;

    /// <summary>
    /// 合同标题
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 合同类型
    /// </summary>
    public ContractType Type { get; set; }

    /// <summary>
    /// 合同状态
    /// </summary>
    public ContractStatus Status { get; set; }

    /// <summary>
    /// 房源ID
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// 房源信息
    /// </summary>
    [ForeignKey("PropertyId")]
    public virtual Property? Property { get; set; }

    /// <summary>
    /// 甲方客户ID
    /// </summary>
    public int PartyAId { get; set; }

    /// <summary>
    /// 甲方客户信息
    /// </summary>
    [ForeignKey("PartyAId")]
    public virtual Client? PartyA { get; set; }

    /// <summary>
    /// 乙方客户ID
    /// </summary>
    public int PartyBId { get; set; }

    /// <summary>
    /// 乙方客户信息
    /// </summary>
    [ForeignKey("PartyBId")]
    public virtual Client? PartyB { get; set; }

    /// <summary>
    /// 合同金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 签署日期
    /// </summary>
    public DateTime? SignDate { get; set; }

    /// <summary>
    /// 生效日期
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// 到期日期
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// 合同内容（Word文档内容）
    /// </summary>
    [Column(TypeName = "ntext")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    [Required]
    [StringLength(450)]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; }
} 