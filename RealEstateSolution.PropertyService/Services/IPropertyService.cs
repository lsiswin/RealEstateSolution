using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Services;

/// <summary>
/// 房源服务接口
/// </summary>
public interface IPropertyService
{
    /// <summary>
    /// 登记新房源
    /// </summary>
    Task<ApiResponse<Property>> RegisterPropertyAsync(Property property, int userId);

    /// <summary>
    /// 修改房源信息
    /// </summary>
    Task<ApiResponse<Property>> UpdatePropertyAsync(int id, Property property, int userId);

    /// <summary>
    /// 变更房源状态
    /// </summary>
    Task<ApiResponse<Property>> ChangePropertyStatusAsync(int id, PropertyStatus status, int userId);

    /// <summary>
    /// 获取房源详情
    /// </summary>
    Task<ApiResponse<Property>> GetPropertyByIdAsync(int id, int userId);

    /// <summary>
    /// 查询房源列表
    /// </summary>
    Task<ApiResponse<PagedList<Property>>> QueryPropertiesAsync(
        int userId,
        bool isAgent,
        PropertyType? type = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        decimal? minArea = null,
        decimal? maxArea = null,
        PropertyStatus? status = null,
        string? keyword = null,
        int pageIndex = 1,
        int pageSize = 10);

    /// <summary>
    /// 删除房源
    /// </summary>
    Task<ApiResponse> DeletePropertyAsync(int id, int userId);
} 