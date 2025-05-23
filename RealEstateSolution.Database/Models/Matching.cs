using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.Database.Models;

/// <summary>
/// 匹配
/// </summary>
public class Matching
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    [Required]
    public int ClientId { get; set; }

    /// <summary>
    /// 房源ID
    /// </summary>
    [Required]
    public int PropertyId { get; set; }

    /// <summary>
    /// 匹配类型
    /// </summary>
    [Required]
    public MatchingType Type { get; set; }

    /// <summary>
    /// 匹配状态
    /// </summary>
    [Required]
    public MatchingStatus Status { get; set; }

    /// <summary>
    /// 匹配分数
    /// </summary>
    [Required]
    public double Score { get; set; }

    /// <summary>
    /// 匹配原因
    /// </summary>
    [MaxLength(500)]
    public string Reason { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Required]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 关联的客户
    /// </summary>
    public virtual Client Client { get; set; }

    /// <summary>
    /// 关联的房源
    /// </summary>
    public virtual Property Property { get; set; }
    
    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpireTime { get; set; }

}

/// <summary>
/// 匹配类型
/// </summary>
public enum MatchingType
{
    /// <summary>
    /// 自动匹配
    /// </summary>
    Auto,

    /// <summary>
    /// 手动匹配
    /// </summary>
    Manual
}

/// <summary>
/// 匹配状态
/// </summary>
public enum MatchingStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending,

    /// <summary>
    /// 已接受
    /// </summary>
    Accepted,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
} 