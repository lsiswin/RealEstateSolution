using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Common.Redis;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Data;

namespace RealEstateSolution.PropertyService.Services;

/// <summary>
/// 房源服务实现类
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork<PropertyDbContext> _unitOfWork;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRedisService _redisService;
    private const string PropertyCacheKeyPrefix = "property:";
    private const int CacheExpirationMinutes = 30;

    public PropertyService(
        IUnitOfWork<PropertyDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _propertyRepository = unitOfWork.Repository<Property>();
        _httpContextAccessor = httpContextAccessor;
        _redisService = redisService;
    }

    /// <summary>
    /// 验证用户是否有权限操作房源
    /// </summary>
    private async Task<bool> ValidatePropertyAccess(int propertyId, int userId, bool isGet = false)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null)
        {
            throw new KeyNotFoundException($"未找到ID为{propertyId}的房源");
        }

        // 如果是获取操作，且用户是经纪人，则允许访问
        if (isGet && _httpContextAccessor.HttpContext?.User.IsInRole("Agent") == true)
        {
            return true;
        }

        // 验证房源是否属于当前用户
        if (property.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("您没有权限操作此房源");
        }

        return true;
    }

    /// <summary>
    /// 登记新房源
    /// </summary>
    public async Task<ApiResponse<Property>> RegisterPropertyAsync(Property property, int userId)
    {
        try
        {
            property.OwnerId = userId;
            property.CreateTime = DateTime.Now;
            property.UpdateTime = DateTime.Now;
            
            await _propertyRepository.AddAsync(property);
            await _unitOfWork.SaveChangesAsync();

            // 缓存新房源
            await _redisService.SetAsync(
                $"{PropertyCacheKeyPrefix}{property.Id}",
                System.Text.Json.JsonSerializer.Serialize(property),
                TimeSpan.FromMinutes(CacheExpirationMinutes));
            
            return ApiResponse<Property>.Ok(property, "房源登记成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<Property>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 修改房源信息
    /// </summary>
    public async Task<ApiResponse<Property>> UpdatePropertyAsync(int id, Property property, int userId)
    {
        try
        {
            await ValidatePropertyAccess(id, userId);

            var existingProperty = await _propertyRepository.GetByIdAsync(id);
            
            // 更新属性
            existingProperty.Title = property.Title;
            existingProperty.Description = property.Description;
            existingProperty.Price = property.Price;
            existingProperty.Area = property.Area;
            existingProperty.Address = property.Address;
            existingProperty.Type = property.Type;
            existingProperty.Decoration = property.Decoration;
            existingProperty.Orientation = property.Orientation;
            existingProperty.Floor = property.Floor;
            existingProperty.TotalFloors = property.TotalFloors;
            existingProperty.Rooms = property.Rooms;
            existingProperty.Bathrooms = property.Bathrooms;
            existingProperty.Images = property.Images;
            existingProperty.UpdateTime = DateTime.Now;

            _propertyRepository.Update(existingProperty);
            await _unitOfWork.SaveChangesAsync();

            // 更新缓存
            await _redisService.SetAsync(
                $"{PropertyCacheKeyPrefix}{id}",
                System.Text.Json.JsonSerializer.Serialize(existingProperty),
                TimeSpan.FromMinutes(CacheExpirationMinutes));

            return ApiResponse<Property>.Ok(existingProperty, "房源信息更新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<Property>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 变更房源状态
    /// </summary>
    public async Task<ApiResponse<Property>> ChangePropertyStatusAsync(int id, PropertyStatus status, int userId)
    {
        try
        {
            await ValidatePropertyAccess(id, userId);

            var property = await _propertyRepository.GetByIdAsync(id);
            property.Status = status;
            property.UpdateTime = DateTime.Now;

            _propertyRepository.Update(property);
            await _unitOfWork.SaveChangesAsync();

            // 更新缓存
            await _redisService.SetAsync(
                $"{PropertyCacheKeyPrefix}{id}",
                System.Text.Json.JsonSerializer.Serialize(property),
                TimeSpan.FromMinutes(CacheExpirationMinutes));

            return ApiResponse<Property>.Ok(property, "房源状态更新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<Property>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 获取房源详情
    /// </summary>
    public async Task<ApiResponse<Property>> GetPropertyByIdAsync(int id, int userId)
    {
        try
        {
            await ValidatePropertyAccess(id, userId, true);

            // 尝试从缓存获取
            var cacheKey = $"{PropertyCacheKeyPrefix}{id}";
            var cachedProperty = await _redisService.GetAsync(cacheKey);
            
            Property? property;
            if (!string.IsNullOrEmpty(cachedProperty))
            {
                property = System.Text.Json.JsonSerializer.Deserialize<Property>(cachedProperty);
            }
            else
            {
                property = await _propertyRepository.GetByIdAsync(id);
                if (property != null)
                {
                    // 设置缓存
                    await _redisService.SetAsync(
                        cacheKey,
                        System.Text.Json.JsonSerializer.Serialize(property),
                        TimeSpan.FromMinutes(CacheExpirationMinutes));
                }
            }

            return property == null 
                ? ApiResponse<Property>.Error($"未找到ID为{id}的房源") 
                : ApiResponse<Property>.Ok(property);
        }
        catch (Exception ex)
        {
            return ApiResponse<Property>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 查询房源列表
    /// </summary>
    public async Task<ApiResponse<PagedList<Property>>> QueryPropertiesAsync(
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
        int pageSize = 10)
    {
        try
        {
            var query = _propertyRepository.Query();

            // 如果不是经纪人，只能查看自己的房源
            if (!isAgent)
            {
                query = query.Where(p => p.OwnerId == userId);
            }

            // 应用筛选条件
            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);
                
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
                
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);
                
            if (minArea.HasValue)
                query = query.Where(p => p.Area >= minArea.Value);
                
            if (maxArea.HasValue)
                query = query.Where(p => p.Area <= maxArea.Value);
                
            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);
                
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Title.Contains(keyword) || p.Description!.Contains(keyword) || p.Address.Contains(keyword));

            // 获取总数
            var total = await query.CountAsync();

            // 分页
            var properties = await query
                .OrderByDescending(p => p.UpdateTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedList = new PagedList<Property>(properties, total, pageIndex, pageSize);
            return ApiResponse<PagedList<Property>>.Ok(pagedList);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedList<Property>>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除房源
    /// </summary>
    public async Task<ApiResponse> DeletePropertyAsync(int id, int userId)
    {
        try
        {
            await ValidatePropertyAccess(id, userId);

            var property = await _propertyRepository.GetByIdAsync(id);
            _propertyRepository.Delete(property);
            await _unitOfWork.SaveChangesAsync();

            // 删除缓存
            await _redisService.DeleteAsync($"{PropertyCacheKeyPrefix}{id}");

            return ApiResponse.Ok("房源删除成功");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error(ex.Message);
        }
    }
}