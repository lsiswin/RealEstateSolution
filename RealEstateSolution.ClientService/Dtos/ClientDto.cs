using RealEstateSolution.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.ClientService.Dtos;

/// <summary>
/// 客户DTO - 统一用于增删改查操作
/// </summary>
public class ClientDto
{
    /// <summary>
    /// 客户ID（创建时忽略）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 客户姓名
    /// </summary>
    [Required(ErrorMessage = "客户姓名不能为空")]
    [StringLength(50, ErrorMessage = "客户姓名长度不能超过50个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客户电话
    /// </summary>
    [Required(ErrorMessage = "客户电话不能为空")]
    [StringLength(20, ErrorMessage = "客户电话长度不能超过20个字符")]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "请输入有效的手机号码")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 客户邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "请输入有效的邮箱地址")]
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 客户类型
    /// </summary>
    [Required(ErrorMessage = "客户类型不能为空")]
    public ClientType Type { get; set; }

    /// <summary>
    /// 客户状态
    /// </summary>
    public ClientStatus Status { get; set; } = ClientStatus.Active;

    /// <summary>
    /// 客户地址
    /// </summary>
    [StringLength(200, ErrorMessage = "地址长度不能超过200个字符")]
    public string? Address { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    [StringLength(18, ErrorMessage = "身份证号长度不能超过18个字符")]
    public string? IdCard { get; set; }

    /// <summary>
    /// 职业
    /// </summary>
    [StringLength(50, ErrorMessage = "职业长度不能超过50个字符")]
    public string? Occupation { get; set; }

    /// <summary>
    /// 年收入
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "年收入不能为负数")]
    public decimal? AnnualIncome { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Notes { get; set; }

    /// <summary>
    /// 创建时间（只读）
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间（只读）
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 客户显示名称（只读）
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 状态显示文本（只读）
    /// </summary>
    public string StatusDisplay { get; set; } = string.Empty;

    /// <summary>
    /// 类型显示文本（只读）
    /// </summary>
    public string TypeDisplay { get; set; } = string.Empty;

    /// <summary>
    /// 客户需求信息（只读）
    /// </summary>
    public ClientRequirementDto? Requirements { get; set; }
}

/// <summary>
/// 客户查询DTO
/// </summary>
public class ClientQueryDto
{
    /// <summary>
    /// 页码，从1开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页记录数
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 客户姓名（模糊查询）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 客户电话（模糊查询）
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 客户邮箱（模糊查询）
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 客户类型
    /// </summary>
    public ClientType? Type { get; set; }

    /// <summary>
    /// 客户状态
    /// </summary>
    public ClientStatus? Status { get; set; }

    /// <summary>
    /// 关键词搜索（姓名、电话、邮箱）
    /// </summary>
    public string? Keyword { get; set; }
} 