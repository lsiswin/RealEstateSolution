using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.Database.Models;

/// <summary>
/// 房产类型
/// </summary>
public enum PropertyType
{
    /// <summary>
    /// 住宅
    /// </summary>
    Residential = 1,

    /// <summary>
    /// 商业
    /// </summary>
    Commercial = 2,

    /// <summary>
    /// 办公
    /// </summary>
    Office = 3,

    /// <summary>
    /// 工业
    /// </summary>
    Industrial = 4,

    /// <summary>
    /// 土地
    /// </summary>
    Land = 5
}

/// <summary>
/// 装修类型
/// </summary>
public enum DecorationType
{
    /// <summary>
    /// 毛坯
    /// </summary>
    Rough = 1,

    /// <summary>
    /// 简装
    /// </summary>
    Simple = 2,

    /// <summary>
    /// 精装
    /// </summary>
    Fine = 3,

    /// <summary>
    /// 豪装
    /// </summary>
    Luxury = 4
}

/// <summary>
/// 朝向类型
/// </summary>
public enum OrientationType
{
    /// <summary>
    /// 东
    /// </summary>
    East = 1,

    /// <summary>
    /// 南
    /// </summary>
    South = 2,

    /// <summary>
    /// 西
    /// </summary>
    West = 3,

    /// <summary>
    /// 北
    /// </summary>
    North = 4,

    /// <summary>
    /// 东南
    /// </summary>
    Southeast = 5,

    /// <summary>
    /// 东北
    /// </summary>
    Northeast = 6,

    /// <summary>
    /// 西南
    /// </summary>
    Southwest = 7,

    /// <summary>
    /// 西北
    /// </summary>
    Northwest = 8
}

/// <summary>
/// 房产状态
/// </summary>
public enum PropertyStatus
{
    /// <summary>
    /// 待售
    /// </summary>
    ForSale = 1,

    /// <summary>
    /// 已售
    /// </summary>
    Sold = 2,

    /// <summary>
    /// 待租
    /// </summary>
    ForRent = 3,

    /// <summary>
    /// 已租
    /// </summary>
    Rented = 4,

    /// <summary>
    /// 下架
    /// </summary>
    Offline = 5,

    /// <summary>
    /// 可用
    /// </summary>
    Available = 6
}

/// <summary>
/// 房产模型
/// </summary>
public class Property
{
    /// <summary>
    /// 房产ID
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 房产类型
    /// </summary>
    public PropertyType Type { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 面积
    /// </summary>
    public decimal Area { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 装修类型
    /// </summary>
    public DecorationType Decoration { get; set; }

    /// <summary>
    /// 朝向
    /// </summary>
    public OrientationType Orientation { get; set; }

    /// <summary>
    /// 楼层
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 总楼层
    /// </summary>
    public int TotalFloors { get; set; }

    /// <summary>
    /// 房间数
    /// </summary>
    public int Rooms { get; set; }

    /// <summary>
    /// 卫生间数
    /// </summary>
    public int Bathrooms { get; set; }

    /// <summary>
    /// 房产状态
    /// </summary>
    public PropertyStatus Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 业主ID
    /// </summary>
    [Required]
    public string OwnerId { get; set; }

    /// <summary>
    /// 房源图片
    /// </summary>
    public virtual ICollection<PropertyImage> Images { get; set; }
} 