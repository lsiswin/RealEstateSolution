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
    [StringLength(20)]
    public string ContractNumber { get; set; } = string.Empty;

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
    /// 主要客户ID（甲方）
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// 主要客户信息（甲方）
    /// </summary>
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    /// <summary>
    /// 次要客户ID（乙方，可选）
    /// </summary>
    public int? ClientId1 { get; set; }

    /// <summary>
    /// 次要客户信息（乙方，可选）
    /// </summary>
    [ForeignKey("ClientId1")]
    public virtual Client? Client1 { get; set; }

    /// <summary>
    /// 合同金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 合同开始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 合同结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// 合同条款
    /// </summary>
    [StringLength(2000)]
    public string Terms { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 签署时间
    /// </summary>
    public DateTime? SignTime { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// 取消时间
    /// </summary>
    public DateTime? CancelTime { get; set; }

    // 为了兼容性，添加属性映射
    /// <summary>
    /// 甲方客户ID（映射到ClientId）
    /// </summary>
    [NotMapped]
    public int PartyAId 
    { 
        get => ClientId; 
        set => ClientId = value; 
    }

    /// <summary>
    /// 乙方客户ID（映射到ClientId1）
    /// </summary>
    [NotMapped]
    public int PartyBId 
    { 
        get => ClientId1 ?? 0; 
        set => ClientId1 = value; 
    }

    /// <summary>
    /// 甲方客户信息（映射到Client）
    /// </summary>
    [NotMapped]
    public virtual Client? PartyA 
    { 
        get => Client; 
        set => Client = value; 
    }

    /// <summary>
    /// 乙方客户信息（映射到Client1）
    /// </summary>
    [NotMapped]
    public virtual Client? PartyB 
    { 
        get => Client1; 
        set => Client1 = value; 
    }

    /// <summary>
    /// 合同标题（映射到Terms的前100个字符）
    /// </summary>
    [NotMapped]
    public string Title 
    { 
        get => Terms.Length > 100 ? Terms.Substring(0, 100) : Terms; 
        set => Terms = value; 
    }

    /// <summary>
    /// 签署日期（映射到SignTime）
    /// </summary>
    [NotMapped]
    public DateTime? SignDate 
    { 
        get => SignTime; 
        set => SignTime = value; 
    }

    /// <summary>
    /// 生效日期（映射到StartDate）
    /// </summary>
    [NotMapped]
    public DateTime? EffectiveDate 
    { 
        get => StartDate; 
        set => StartDate = value ?? DateTime.Now; 
    }

    /// <summary>
    /// 到期日期（映射到EndDate）
    /// </summary>
    [NotMapped]
    public DateTime? ExpiryDate 
    { 
        get => EndDate; 
        set => EndDate = value; 
    }

    /// <summary>
    /// 合同内容（映射到Terms）
    /// </summary>
    [NotMapped]
    public string Content 
    { 
        get => Terms; 
        set => Terms = value; 
    }

    /// <summary>
    /// 备注（映射到Remark）
    /// </summary>
    [NotMapped]
    public string? Notes 
    { 
        get => Remark; 
        set => Remark = value; 
    }

    /// <summary>
    /// 创建时间（映射到CreateTime）
    /// </summary>
    [NotMapped]
    public DateTime CreatedAt 
    { 
        get => CreateTime; 
        set => CreateTime = value; 
    }

    /// <summary>
    /// 更新时间（映射到UpdateTime）
    /// </summary>
    [NotMapped]
    public DateTime UpdatedAt 
    { 
        get => UpdateTime; 
        set => UpdateTime = value; 
    }

    /// <summary>
    /// 创建人ID（暂时不存储在数据库中）
    /// </summary>
    [NotMapped]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 是否删除（暂时不存储在数据库中）
    /// </summary>
    [NotMapped]
    public bool IsDeleted { get; set; }
} 