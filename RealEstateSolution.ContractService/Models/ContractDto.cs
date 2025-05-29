using RealEstateSolution.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.ContractService.Models;

/// <summary>
/// 合同DTO - 统一用于增删改查操作
/// </summary>
public class ContractDto
{
    /// <summary>
    /// 合同ID（创建时忽略）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 合同编号（创建时自动生成）
    /// </summary>
    public string ContractNumber { get; set; } = string.Empty;

    /// <summary>
    /// 合同标题
    /// </summary>
    [Required(ErrorMessage = "合同标题不能为空")]
    [StringLength(200, ErrorMessage = "合同标题长度不能超过200个字符")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 合同类型
    /// </summary>
    [Required(ErrorMessage = "合同类型不能为空")]
    public ContractType Type { get; set; }

    /// <summary>
    /// 合同状态（创建时默认为草稿）
    /// </summary>
    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    /// <summary>
    /// 房源ID
    /// </summary>
    [Required(ErrorMessage = "房源ID不能为空")]
    public int PropertyId { get; set; }

    /// <summary>
    /// 房源标题（只读）
    /// </summary>
    public string? PropertyTitle { get; set; }

    /// <summary>
    /// 房源地址（只读）
    /// </summary>
    public string? PropertyAddress { get; set; }

    /// <summary>
    /// 甲方客户ID
    /// </summary>
    [Required(ErrorMessage = "甲方客户ID不能为空")]
    public int PartyAId { get; set; }

    /// <summary>
    /// 甲方客户姓名（只读）
    /// </summary>
    public string? PartyAName { get; set; }

    /// <summary>
    /// 甲方客户电话（只读）
    /// </summary>
    public string? PartyAPhone { get; set; }

    /// <summary>
    /// 乙方客户ID
    /// </summary>
    [Required(ErrorMessage = "乙方客户ID不能为空")]
    public int PartyBId { get; set; }

    /// <summary>
    /// 乙方客户姓名（只读）
    /// </summary>
    public string? PartyBName { get; set; }

    /// <summary>
    /// 乙方客户电话（只读）
    /// </summary>
    public string? PartyBPhone { get; set; }

    /// <summary>
    /// 合同金额
    /// </summary>
    [Required(ErrorMessage = "合同金额不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "合同金额必须大于0")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 签署日期
    /// </summary>
    public DateTime? SignDate { get; set; }

    /// <summary>
    /// 生效日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 到期日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    [StringLength(100, ErrorMessage = "支付方式长度不能超过100个字符")]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 合同条款
    /// </summary>
    public string? Terms { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(1000, ErrorMessage = "备注长度不能超过1000个字符")]
    public string? Notes { get; set; }

    /// <summary>
    /// 合同内容（Word文档内容）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 创建时间（只读）
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间（只读）
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 合同统计DTO
/// </summary>
public class ContractStatsDto
{
    public int TotalContracts { get; set; }
    public int DraftContracts { get; set; }
    public int PendingContracts { get; set; }
    public int SignedContracts { get; set; }
    public int CompletedContracts { get; set; }
    public int CancelledContracts { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal MonthlyAmount { get; set; }
} 