using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.Models;

/// <summary>
/// 匹配状态
/// </summary>
public enum MatchingStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已接受
    /// </summary>
    Accepted = 2,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// 已过期
    /// </summary>
    Expired = 4
}

/// <summary>
/// 匹配类型
/// </summary>
public enum MatchingType
{
    /// <summary>
    /// 自动匹配
    /// </summary>
    Auto = 1,

    /// <summary>
    /// 手动匹配
    /// </summary>
    Manual = 2
}

/// <summary>
/// 匹配模型
/// </summary>
public class Matching
{
    /// <summary>
    /// 匹配ID
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 匹配类型
    /// </summary>
    public MatchingType Type { get; set; }

    /// <summary>
    /// 匹配状态
    /// </summary>
    public MatchingStatus Status { get; set; }

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
    /// 匹配分数
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// 匹配原因
    /// </summary>
    [MaxLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpireTime { get; set; }
} 