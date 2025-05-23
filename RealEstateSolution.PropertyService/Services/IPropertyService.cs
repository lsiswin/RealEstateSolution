using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Dtos;

namespace RealEstateSolution.PropertyService.Services;

/// <summary>
/// 房源服务接口
/// </summary>
public interface IPropertyService
{
    /// <summary>
    /// 创建房源
    /// </summary>
    Task<PropertyDto> CreatePropertyAsync(PropertyDto propertyDto);

    /// <summary>
    /// 更新房源
    /// </summary>
    Task UpdatePropertyAsync(PropertyDto propertyDto);

    /// <summary>
    /// 获取房源详情
    /// </summary>
    Task<PropertyDto?> GetPropertyByIdAsync(int id);

    /// <summary>
    /// 搜索房源
    /// </summary>
    Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(
        string? keyword,
        PropertyType? type,
        PropertyStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        string? location);

    /// <summary>
    /// 删除房源
    /// </summary>
    Task DeletePropertyAsync(int id);

    /// <summary>
    /// 更新房源状态
    /// </summary>
    Task UpdatePropertyStatusAsync(int id, PropertyStatus status);

    /// <summary>
    /// 获取房主的所有房源
    /// </summary>
    Task<IEnumerable<PropertyDto>> GetOwnerPropertiesAsync(int ownerId);

} 