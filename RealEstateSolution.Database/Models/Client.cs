using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.Database.Models;

/// <summary>
/// 客户类型
/// </summary>
public enum ClientType
{
    /// <summary>
    /// 买家
    /// </summary>
    Buyer = 1,

    /// <summary>
    /// 卖家
    /// </summary>
    Seller = 2,

    /// <summary>
    /// 租客
    /// </summary>
    Tenant = 3,

    /// <summary>
    /// 房东
    /// </summary>
    Landlord = 4
}

/// <summary>
/// 客户来源
/// </summary>
public enum ClientSource
{
    /// <summary>
    /// 网站
    /// </summary>
    Website = 1,

    /// <summary>
    /// 电话
    /// </summary>
    Phone = 2,

    /// <summary>
    /// 推荐
    /// </summary>
    Referral = 3,

    /// <summary>
    /// 其他
    /// </summary>
    Other = 4
}

/// <summary>
/// 客户状态
/// </summary>
public enum ClientStatus
{
    /// <summary>
    /// 潜在
    /// </summary>
    Potential = 1,

    /// <summary>
    /// 活跃
    /// </summary>
    Active = 2,

    /// <summary>
    /// 成交
    /// </summary>
    Closed = 3,

    /// <summary>
    /// 无效
    /// </summary>
    Invalid = 4
}

/// <summary>
/// 客户
/// </summary>
public class Client
{
    public ClientStatus Status;

    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(100)]
    public string Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [MaxLength(200)]
    public string Address { get; set; }

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
    /// 客户需求
    /// </summary>
    public virtual ClientRequirements Requirements { get; set; }

    /// <summary>
    /// 匹配记录
    /// </summary>
    public virtual ICollection<Matching> Matchings { get; set; }
}

/// <summary>
/// 客户需求
/// </summary>
public class ClientRequirements
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
    /// 最低价格
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// 最高价格
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 最小面积
    /// </summary>
    public decimal? MinArea { get; set; }

    /// <summary>
    /// 最大面积
    /// </summary>
    public decimal? MaxArea { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    [MaxLength(200)]
    public string Location { get; set; }

    /// <summary>
    /// 房源类型
    /// </summary>
    public PropertyType? PropertyType { get; set; }

    /// <summary>
    /// 其他要求
    /// </summary>
    [MaxLength(500)]
    public string OtherRequirements { get; set; }

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
}