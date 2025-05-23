using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.Models;

/// <summary>
/// 合同类型
/// </summary>
public enum ContractType
{
    /// <summary>
    /// 买卖合同
    /// </summary>
    Sale = 1,

    /// <summary>
    /// 租赁合同
    /// </summary>
    Rent = 2
}

/// <summary>
/// 合同状态
/// </summary>
public enum ContractStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 1,

    /// <summary>
    /// 待签署
    /// </summary>
    Pending = 2,

    /// <summary>
    /// 已签署
    /// </summary>
    Signed = 3,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 4,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 5
}

/// <summary>
/// 合同模型
/// </summary>
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
    [MaxLength(50)]
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
    /// 房产ID
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// 房产信息
    /// </summary>
    public Property? Property { get; set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// 客户信息
    /// </summary>
    public Client? Client { get; set; }

    /// <summary>
    /// 合同金额
    /// </summary>
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
    [MaxLength(200)]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 合同条款
    /// </summary>
    [MaxLength(2000)]
    public string? Terms { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
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
} 