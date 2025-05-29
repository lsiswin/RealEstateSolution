using Microsoft.AspNetCore.Http;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Dtos;

namespace RealEstateSolution.PropertyService.Services;

/// <summary>
/// 房源服务接口
/// 提供房源管理相关的业务逻辑操作，包括房源的增删改查、图片管理和统计功能
/// </summary>
public interface IPropertyService
{
    /// <summary>
    /// 获取房源列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页的房源列表</returns>
    Task<ApiResponse<PagedList<PropertyDto>>> GetPropertiesAsync(PropertyQueryDto query);

    /// <summary>
    /// 根据ID获取房源详细信息
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <returns>房源详细信息</returns>
    Task<ApiResponse<PropertyDto>> GetPropertyByIdAsync(int id);

    /// <summary>
    /// 创建新房源
    /// </summary>
    /// <param name="propertyDto">房源信息DTO</param>
    /// <param name="userId">创建人ID</param>
    /// <returns>创建成功的房源信息</returns>
    Task<ApiResponse<PropertyDto>> CreatePropertyAsync(PropertyDto propertyDto, string userId);

    /// <summary>
    /// 更新房源信息
    /// </summary>
    /// <param name="id">要更新的房源ID</param>
    /// <param name="propertyDto">新的房源信息DTO</param>
    /// <param name="userId">更新人ID</param>
    /// <returns>更新后的房源信息</returns>
    Task<ApiResponse<PropertyDto>> UpdatePropertyAsync(int id, PropertyDto propertyDto, string userId);

    /// <summary>
    /// 删除房源
    /// </summary>
    /// <param name="id">要删除的房源ID</param>
    /// <param name="userId">删除人ID</param>
    /// <returns>删除操作结果</returns>
    Task<ApiResponse> DeletePropertyAsync(int id, string userId);

    /// <summary>
    /// 更新房源状态
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <param name="statusDto">状态更新DTO</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>更新后的房源信息</returns>
    Task<ApiResponse<PropertyDto>> UpdatePropertyStatusAsync(int id, PropertyStatusUpdateDto statusDto, string userId);

    /// <summary>
    /// 获取房源统计数据
    /// </summary>
    /// <returns>房源相关的统计信息</returns>
    Task<ApiResponse<PropertyStatsDto>> GetPropertyStatsAsync();

    /// <summary>
    /// 上传房源图片
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <param name="file">图片文件</param>
    /// <param name="userId">上传人ID</param>
    /// <returns>上传成功的图片信息</returns>
    Task<ApiResponse<PropertyImageDto>> UploadPropertyImageAsync(int propertyId, IFormFile file, string userId);

    /// <summary>
    /// 删除房源图片
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <param name="imageId">图片ID</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>删除操作结果</returns>
    Task<ApiResponse> DeletePropertyImageAsync(int propertyId, int imageId, string userId);

    /// <summary>
    /// 获取房源图片列表
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <returns>房源图片列表</returns>
    Task<ApiResponse<List<PropertyImageDto>>> GetPropertyImagesAsync(int propertyId);
} 