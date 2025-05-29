using RealEstateSolution.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.PropertyService.Dtos;

/// <summary>
/// 房源DTO - 统一用于增删改查操作
/// </summary>
public class PropertyDto
{
    /// <summary>
    /// 房产ID（创建时忽略）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 房产类型
    /// </summary>
    [Required(ErrorMessage = "房产类型不能为空")]
    public PropertyType Type { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "房源标题不能为空")]
    [StringLength(100, ErrorMessage = "房源标题长度不能超过100个字符")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(1000, ErrorMessage = "房源描述长度不能超过1000个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 价格
    /// </summary>
    [Required(ErrorMessage = "价格不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "价格必须大于0")]
    public decimal Price { get; set; }

    /// <summary>
    /// 面积
    /// </summary>
    [Required(ErrorMessage = "面积不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "面积必须大于0")]
    public decimal Area { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [Required(ErrorMessage = "地址不能为空")]
    [StringLength(200, ErrorMessage = "地址长度不能超过200个字符")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    [Required(ErrorMessage = "城市不能为空")]
    [StringLength(50, ErrorMessage = "城市名称长度不能超过50个字符")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    [Required(ErrorMessage = "区县不能为空")]
    [StringLength(50, ErrorMessage = "区县名称长度不能超过50个字符")]
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// 卧室数
    /// </summary>
    [Range(0, 20, ErrorMessage = "卧室数必须在0-20之间")]
    public int? Bedrooms { get; set; }

    /// <summary>
    /// 卫生间数
    /// </summary>
    [Range(0, 10, ErrorMessage = "卫生间数必须在0-10之间")]
    public int? Bathrooms { get; set; }

    /// <summary>
    /// 楼层
    /// </summary>
    [Range(1, 200, ErrorMessage = "楼层必须在1-200之间")]
    public int? Floor { get; set; }

    /// <summary>
    /// 总楼层
    /// </summary>
    [Range(1, 200, ErrorMessage = "总楼层必须在1-200之间")]
    public int? TotalFloors { get; set; }

    /// <summary>
    /// 建造年份
    /// </summary>
    [Range(1900, 2100, ErrorMessage = "建造年份必须在1900-2100之间")]
    public int? YearBuilt { get; set; }

    /// <summary>
    /// 朝向
    /// </summary>
    [StringLength(20, ErrorMessage = "朝向长度不能超过20个字符")]
    public string? Orientation { get; set; }

    /// <summary>
    /// 装修类型
    /// </summary>
    [StringLength(50, ErrorMessage = "装修类型长度不能超过50个字符")]
    public string? Decoration { get; set; }

    /// <summary>
    /// 设施
    /// </summary>
    public List<string>? Facilities { get; set; }

    /// <summary>
    /// 价格显示格式（只读）
    /// </summary>
    public string PriceDisplay { get; set; } = string.Empty;

    /// <summary>
    /// 房产状态
    /// </summary>
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;

    /// <summary>
    /// 创建时间（只读）
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间（只读）
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 所有者ID
    /// </summary>
    [Required(ErrorMessage = "所有者ID不能为空")]
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// 代理ID
    /// </summary>
    public string? AgentId { get; set; }

    /// <summary>
    /// 房源图片信息（只读）
    /// </summary>
    public List<PropertyImageDto>? Images { get; set; }
}

/// <summary>
/// 房源查询DTO
/// </summary>
public class PropertyQueryDto
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
    /// 房产类型
    /// </summary>
    public PropertyType? Type { get; set; }

    /// <summary>
    /// 房产状态
    /// </summary>
    public PropertyStatus? Status { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 区县
    /// </summary>
    public string? District { get; set; }

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
    /// 卧室数
    /// </summary>
    public int? Bedrooms { get; set; }

    /// <summary>
    /// 卫生间数
    /// </summary>
    public int? Bathrooms { get; set; }

    /// <summary>
    /// 关键词搜索（标题、地址、描述）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 所有者ID
    /// </summary>
    public string? OwnerId { get; set; }

    /// <summary>
    /// 代理ID
    /// </summary>
    public string? AgentId { get; set; }
}

/// <summary>
/// 房源统计DTO
/// </summary>
public class PropertyStatsDto
{
    /// <summary>
    /// 总房源数
    /// </summary>
    public int TotalProperties { get; set; }

    /// <summary>
    /// 可用房源数
    /// </summary>
    public int AvailableProperties { get; set; }

    /// <summary>
    /// 已售房源数
    /// </summary>
    public int SoldProperties { get; set; }

    /// <summary>
    /// 已租房源数
    /// </summary>
    public int RentedProperties { get; set; }

    /// <summary>
    /// 下架房源数
    /// </summary>
    public int OfflineProperties { get; set; }

    /// <summary>
    /// 出售房源数
    /// </summary>
    public int ForSaleProperties { get; set; }

    /// <summary>
    /// 出租房源数
    /// </summary>
    public int ForRentProperties { get; set; }

    /// <summary>
    /// 最近30天新增房源数
    /// </summary>
    public int NewPropertiesLast30Days { get; set; }

    /// <summary>
    /// 最近30天成交房源数
    /// </summary>
    public int SoldPropertiesLast30Days { get; set; }

    /// <summary>
    /// 平均价格
    /// </summary>
    public decimal AveragePrice { get; set; }

    /// <summary>
    /// 平均面积
    /// </summary>
    public decimal AverageArea { get; set; }
} 