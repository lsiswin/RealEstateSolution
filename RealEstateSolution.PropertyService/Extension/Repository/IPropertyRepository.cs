using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Extension.Repository;

/// <summary>
/// 房源仓储接口
/// </summary>
public interface IPropertyRepository : IGenericRepository<Property>
{
    /// <summary>
    /// 搜索房源
    /// </summary>
    Task<IEnumerable<Property>> SearchPropertiesAsync(
        string? keyword,
        PropertyType? type,
        PropertyStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        string? location);

    /// <summary>
    /// 获取房主的所有房源
    /// </summary>
    Task<IEnumerable<Property>> GetOwnerPropertiesAsync(int ownerId);

    /// <summary>
    /// 更新房源状态
    /// </summary>
    Task UpdateStatusAsync(int id, PropertyStatus status);

    /// <summary>
    /// 添加房源图片
    /// </summary>
    Task AddPropertyImageAsync(int propertyId, string imageUrl);

    /// <summary>
    /// 删除房源图片
    /// </summary>
    Task DeletePropertyImageAsync(int propertyId, string imageUrl);
}