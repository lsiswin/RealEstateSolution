using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Common.Redis;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Data;
using RealEstateSolution.PropertyService.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RealEstateSolution.Common.Extensions;
using System.Text.Json;

namespace RealEstateSolution.PropertyService.Services;

/// <summary>
/// 房源服务实现类
/// 实现房源管理相关的所有业务逻辑，包括房源信息的CRUD操作、图片管理和数据统计
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork<PropertyDbContext> _unitOfWork;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IGenericRepository<PropertyImage> _propertyImageRepository;
    private readonly IRedisService _redisService;
    private readonly IMapper _mapper;
    private const string PropertyCacheKeyPrefix = "property:";
    private const int CacheExpirationMinutes = 30;
    private readonly ILogger<PropertyService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元</param>
    /// <param name="redisService">Redis缓存服务</param>
    /// <param name="mapper">对象映射器，用于DTO和实体之间的转换</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="httpContextAccessor">HTTP上下文访问器</param>
    public PropertyService(
        IUnitOfWork<PropertyDbContext> unitOfWork,
        IRedisService redisService,
        IMapper mapper,
        ILogger<PropertyService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _propertyRepository = unitOfWork.Repository<Property>();
        _propertyImageRepository = unitOfWork.Repository<PropertyImage>();
        _redisService = redisService;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 获取房源列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页的房源列表</returns>
    public async Task<ApiResponse<PagedList<PropertyDto>>> GetPropertiesAsync(PropertyQueryDto query)
    {
        try
        {
            // 参数验证
            if (query.PageIndex < 1)
                return ApiResponse<PagedList<PropertyDto>>.Error("页码必须大于0");
            
            if (query.PageSize < 1 || query.PageSize > 100)
                return ApiResponse<PagedList<PropertyDto>>.Error("每页记录数必须在1-100之间");

            // 构建查询条件 - 使用IQueryable提升性能
            var queryable = _propertyRepository.Query();

            // 按关键词搜索
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim();
                queryable = queryable.Where(p => 
                    p.Title.Contains(keyword) || 
                    p.Address.Contains(keyword) || 
                    (p.Description != null && p.Description.Contains(keyword)));
            }

            // 按房产类型筛选
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(p => p.Type == query.Type.Value);
            }

            // 按状态筛选
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(p => p.Status == query.Status.Value);
            }

            // 按价格范围筛选
            if (query.MinPrice.HasValue)
            {
                queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);
            }
            if (query.MaxPrice.HasValue)
            {
                queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);
            }

            // 按面积范围筛选
            if (query.MinArea.HasValue)
            {
                queryable = queryable.Where(p => p.Area >= query.MinArea.Value);
            }
            if (query.MaxArea.HasValue)
            {
                queryable = queryable.Where(p => p.Area <= query.MaxArea.Value);
            }

            // 按卧室数筛选
            if (query.Bedrooms.HasValue)
            {
                queryable = queryable.Where(p => p.Rooms == query.Bedrooms.Value);
            }

            // 按卫生间数筛选
            if (query.Bathrooms.HasValue)
            {
                queryable = queryable.Where(p => p.Bathrooms == query.Bathrooms.Value);
            }

            // 按所有者ID筛选
            if (!string.IsNullOrWhiteSpace(query.OwnerId))
            {
                queryable = queryable.Where(p => p.OwnerId == query.OwnerId);
            }

            // 排序
            queryable = queryable.OrderByDescending(p => p.CreateTime);

            // 使用AutoMapper扩展方法进行分页查询和映射
            var result = await queryable.ToPagedListAsync<Property, PropertyDto>(_mapper, query.PageIndex, query.PageSize);

            _logger.LogInformation("查询房源列表完成，共找到 {TotalCount} 条记录", result.TotalCount);
            return ApiResponse<PagedList<PropertyDto>>.Ok(result, $"成功获取第{query.PageIndex}页房源列表，共{result.TotalCount}条记录");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取房源列表失败");
            return ApiResponse<PagedList<PropertyDto>>.Error($"获取房源列表失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 根据ID获取房源详细信息
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <returns>房源详细信息</returns>
    public async Task<ApiResponse<PropertyDto>> GetPropertyByIdAsync(int id)
    {
        try
        {
            // 参数验证
            if (id <= 0)
                return ApiResponse<PropertyDto>.Error("房源ID必须大于0");

            // 尝试从缓存获取
            var cacheKey = $"{PropertyCacheKeyPrefix}{id}";
            var cachedProperty = await _redisService.GetAsync(cacheKey);
            
            PropertyDto? propertyDto = null;
            
            if (!string.IsNullOrEmpty(cachedProperty))
            {
                try
                {
                    var property = JsonSerializer.Deserialize<Property>(cachedProperty);
                    if (property != null)
                    {
                        propertyDto = _mapper.Map<PropertyDto>(property);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "缓存数据反序列化失败，将从数据库重新获取，房源ID: {PropertyId}", id);
                }
            }

            if (propertyDto == null)
            {
                // 从数据库查询
                var queryable = _propertyRepository.Query().Where(p => p.Id == id);
                propertyDto = await queryable.ProjectToFirstOrDefaultAsync<Property, PropertyDto>(_mapper);
                
                if (propertyDto == null)
                {
                    return ApiResponse<PropertyDto>.Error($"未找到ID为{id}的房源");
                }

                // 设置缓存
                var property = await _propertyRepository.GetByIdAsync(id);
                if (property != null)
                {
                    await _redisService.SetAsync(
                        cacheKey,
                        JsonSerializer.Serialize(property),
                        TimeSpan.FromMinutes(CacheExpirationMinutes));
                }
            }

            // 获取房源图片信息
            var images = await _propertyImageRepository.FindAsync(img => img.PropertyId == id);
            if (images.Any())
            {
                propertyDto.Images = _mapper.MapList<PropertyImage, PropertyImageDto>(images);
            }

            return ApiResponse<PropertyDto>.Ok(propertyDto, "成功获取房源详细信息");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取房源详细信息失败，房源ID: {PropertyId}", id);
            return ApiResponse<PropertyDto>.Error($"获取房源详细信息失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 创建新房源
    /// </summary>
    /// <param name="propertyDto">房源信息DTO</param>
    /// <param name="userId">创建人ID</param>
    /// <returns>创建成功的房源信息</returns>
    public async Task<ApiResponse<PropertyDto>> CreatePropertyAsync(PropertyDto propertyDto, string userId)
    {
        try
        {
            // 参数验证
            if (propertyDto == null)
                return ApiResponse<PropertyDto>.Error("房源信息不能为空");

            await _unitOfWork.BeginTransactionAsync();

            // 转换为实体
            var property = _mapper.Map<Property>(propertyDto);
            
            // 设置系统字段
            property.OwnerId = userId;
            property.CreateTime = DateTime.Now;
            property.UpdateTime = DateTime.Now;
            property.Status = PropertyStatus.Available; // 默认状态
            
            await _propertyRepository.AddAsync(property);
            await _unitOfWork.CommitAsync();

            // 缓存新房源
            await _redisService.SetAsync(
                $"{PropertyCacheKeyPrefix}{property.Id}",
                System.Text.Json.JsonSerializer.Serialize(property),
                TimeSpan.FromMinutes(CacheExpirationMinutes));
            
            var resultDto = _mapper.Map<PropertyDto>(property);
            return ApiResponse<PropertyDto>.Ok(resultDto, $"房源{property.Title}创建成功");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<PropertyDto>.Error($"创建房源失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新房源信息
    /// </summary>
    /// <param name="id">要更新的房源ID</param>
    /// <param name="propertyDto">新的房源信息DTO</param>
    /// <param name="userId">更新人ID</param>
    /// <returns>更新后的房源信息</returns>
    public async Task<ApiResponse<PropertyDto>> UpdatePropertyAsync(int id, PropertyDto propertyDto, string userId)
    {
        try
        {
            // 参数验证
            if (id <= 0)
                return ApiResponse<PropertyDto>.Error("房源ID必须大于0");
            
            if (propertyDto == null)
                return ApiResponse<PropertyDto>.Error("房源信息不能为空");

            await _unitOfWork.BeginTransactionAsync();

            var existingProperty = await _propertyRepository.GetByIdAsync(id);
            if (existingProperty == null)
            {
                return ApiResponse<PropertyDto>.Error($"未找到ID为{id}的房源");
            }

            // 权限验证：房源所有者或管理员可以修改
            var httpContext = _httpContextAccessor.HttpContext;
            var isAdmin = httpContext?.User?.IsInRole("admin") ?? false;
            
            if (existingProperty.OwnerId != userId && !isAdmin)
            {
                return ApiResponse<PropertyDto>.Error("您没有权限修改此房源");
            }

            // 更新允许修改的字段 - 使用AutoMapper
            _mapper.Map(propertyDto, existingProperty);
            existingProperty.UpdateTime = DateTime.Now;

            _propertyRepository.Update(existingProperty);
            await _unitOfWork.CommitAsync();

            // 更新缓存
            await _redisService.SetAsync(
                $"{PropertyCacheKeyPrefix}{id}",
                System.Text.Json.JsonSerializer.Serialize(existingProperty),
                TimeSpan.FromMinutes(CacheExpirationMinutes));
            
            var resultDto = _mapper.Map<PropertyDto>(existingProperty);
            return ApiResponse<PropertyDto>.Ok(resultDto, $"房源{existingProperty.Title}信息更新成功");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<PropertyDto>.Error($"更新房源信息失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 删除房源
    /// </summary>
    /// <param name="id">要删除的房源ID</param>
    /// <param name="userId">删除人ID</param>
    /// <returns>删除操作结果</returns>
    public async Task<ApiResponse> DeletePropertyAsync(int id, string userId)
    {
        try
        {
            // 参数验证
            if (id <= 0)
                return ApiResponse.Error("房源ID必须大于0");

            await _unitOfWork.BeginTransactionAsync();

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                return ApiResponse.Error($"未找到ID为{id}的房源");
            }

            // 权限验证：房源所有者或管理员可以删除
            // 注意：这里需要从HttpContext获取用户角色信息
            var httpContext = _httpContextAccessor.HttpContext;
            var isAdmin = httpContext?.User?.IsInRole("admin") ?? false;
            
            if (property.OwnerId != userId && !isAdmin)
            {
                return ApiResponse.Error("您没有权限删除此房源");
            }

            // 先删除关联的图片
            var images = await _propertyImageRepository.FindAsync(img => img.PropertyId == id);
            if (images.Any())
            {
                _propertyImageRepository.DeleteRange(images);
            }
            
            // 删除房源
            _propertyRepository.Delete(property);
            await _unitOfWork.CommitAsync();

            // 清除缓存
            await _redisService.DeleteAsync($"{PropertyCacheKeyPrefix}{id}");
            
            return ApiResponse.Ok($"房源{property.Title}删除成功");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse.Error($"删除房源失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新房源状态
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <param name="statusDto">状态更新DTO</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>更新后的房源信息</returns>
    public async Task<ApiResponse<PropertyDto>> UpdatePropertyStatusAsync(int id, PropertyStatusUpdateDto statusDto, string userId)
    {
        try
        {
            // 参数验证
            if (id <= 0)
                return ApiResponse<PropertyDto>.Error("房源ID必须大于0");
            
            if (statusDto == null)
                return ApiResponse<PropertyDto>.Error("状态信息不能为空");

            await _unitOfWork.BeginTransactionAsync();

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                return ApiResponse<PropertyDto>.Error($"未找到ID为{id}的房源");
            }

            // 权限验证：只有房源所有者可以修改状态
            if (property.OwnerId != userId)
            {
                return ApiResponse<PropertyDto>.Error("您没有权限修改此房源状态");
            }

            property.Status = statusDto.Status;
            property.UpdateTime = DateTime.Now;

            _propertyRepository.Update(property);
            await _unitOfWork.CommitAsync();

            // 更新缓存
            await _redisService.SetAsync(
                $"{PropertyCacheKeyPrefix}{id}",
                System.Text.Json.JsonSerializer.Serialize(property),
                TimeSpan.FromMinutes(CacheExpirationMinutes));

            var resultDto = _mapper.Map<PropertyDto>(property);
            return ApiResponse<PropertyDto>.Ok(resultDto, "房源状态更新成功");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<PropertyDto>.Error($"更新房源状态失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取房源统计数据
    /// </summary>
    /// <returns>房源相关的统计信息</returns>
    public async Task<ApiResponse<PropertyStatsDto>> GetPropertyStatsAsync()
    {
        try
        {
            var allProperties = await _propertyRepository.GetAllAsync();
            var now = DateTime.Now;
            var thirtyDaysAgo = now.AddDays(-30);

            var stats = new PropertyStatsDto
            {
                TotalProperties = allProperties.Count(),
                AvailableProperties = allProperties.Count(p => p.Status == PropertyStatus.Available),
                SoldProperties = allProperties.Count(p => p.Status == PropertyStatus.Sold),
                RentedProperties = allProperties.Count(p => p.Status == PropertyStatus.Rented),
                OfflineProperties = allProperties.Count(p => p.Status == PropertyStatus.Offline),
                ForSaleProperties = allProperties.Count(p => p.Status == PropertyStatus.ForSale),
                ForRentProperties = allProperties.Count(p => p.Status == PropertyStatus.ForRent),
                NewPropertiesLast30Days = allProperties.Count(p => p.CreateTime >= thirtyDaysAgo),
                SoldPropertiesLast30Days = allProperties.Count(p => p.Status == PropertyStatus.Sold && p.UpdateTime >= thirtyDaysAgo),
                AveragePrice = allProperties.Any() ? allProperties.Average(p => p.Price) : 0,
                AverageArea = allProperties.Any() ? allProperties.Average(p => p.Area) : 0
            };

            return ApiResponse<PropertyStatsDto>.Ok(stats, "成功获取房源统计数据");
        }
        catch (Exception ex)
        {
            return ApiResponse<PropertyStatsDto>.Error($"获取房源统计数据失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 上传房源图片
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <param name="file">图片文件</param>
    /// <param name="userId">上传人ID</param>
    /// <returns>上传成功的图片信息</returns>
    public async Task<ApiResponse<PropertyImageDto>> UploadPropertyImageAsync(int propertyId, IFormFile file, string userId)
    {
        try
        {
            // 参数验证
            if (propertyId <= 0)
                return ApiResponse<PropertyImageDto>.Error("房源ID必须大于0");
            
            if (file == null || file.Length == 0)
                return ApiResponse<PropertyImageDto>.Error("请选择要上传的图片文件");

            // 验证房源是否存在
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
            {
                return ApiResponse<PropertyImageDto>.Error($"未找到ID为{propertyId}的房源");
            }

            // 权限验证：房源所有者或管理员可以上传图片
            var httpContext = _httpContextAccessor.HttpContext;
            var isAdmin = httpContext?.User?.IsInRole("admin") ?? false;
            
            if (property.OwnerId != userId && !isAdmin)
            {
                return ApiResponse<PropertyImageDto>.Error("您没有权限为此房源上传图片");
            }

            // 验证文件类型
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return ApiResponse<PropertyImageDto>.Error("只支持 JPG、PNG、GIF、BMP 格式的图片文件");
            }

            // 验证文件大小（限制为5MB）
            if (file.Length > 5 * 1024 * 1024)
            {
                return ApiResponse<PropertyImageDto>.Error("图片文件大小不能超过5MB");
            }

            await _unitOfWork.BeginTransactionAsync();

            // 生成文件名和路径
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine("uploads", "properties", propertyId.ToString());
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadPath);
            
            // 确保目录存在
            Directory.CreateDirectory(fullPath);
            
            var filePath = Path.Combine(fullPath, fileName);
            var relativePath = Path.Combine(uploadPath, fileName).Replace("\\", "/");

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 检查是否为第一张图片（设为主图）
            var existingImages = await _propertyImageRepository.FindAsync(img => img.PropertyId == propertyId);
            var isMain = !existingImages.Any();

            // 创建图片记录
            var propertyImage = new PropertyImage
            {
                PropertyId = propertyId,
                FileName = file.FileName,
                FilePath = relativePath,
                FileSize = file.Length,
                FileType = file.ContentType,
                IsMain = isMain,
                UploadedAt = DateTime.Now,
                UploadedBy = int.TryParse(userId, out var userIdInt) ? userIdInt : 0
            };

            await _propertyImageRepository.AddAsync(propertyImage);
            await _unitOfWork.CommitAsync();

            var imageDto = _mapper.Map<PropertyImageDto>(propertyImage);
            return ApiResponse<PropertyImageDto>.Ok(imageDto, "图片上传成功");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<PropertyImageDto>.Error($"上传图片失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 删除房源图片
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <param name="imageId">图片ID</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>删除操作结果</returns>
    public async Task<ApiResponse> DeletePropertyImageAsync(int propertyId, int imageId, string userId)
    {
        try
        {
            // 参数验证
            if (propertyId <= 0)
                return ApiResponse.Error("房源ID必须大于0");
            
            if (imageId <= 0)
                return ApiResponse.Error("图片ID必须大于0");

            // 验证房源是否存在
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
            {
                return ApiResponse.Error($"未找到ID为{propertyId}的房源");
            }

            // 权限验证：房源所有者或管理员可以删除图片
            var httpContext = _httpContextAccessor.HttpContext;
            var isAdmin = httpContext?.User?.IsInRole("admin") ?? false;
            
            if (property.OwnerId != userId && !isAdmin)
            {
                return ApiResponse.Error("您没有权限删除此房源的图片");
            }

            await _unitOfWork.BeginTransactionAsync();

            // 查找图片
            var image = await _propertyImageRepository.GetByIdAsync(imageId);
            if (image == null || image.PropertyId != propertyId)
            {
                return ApiResponse.Error("未找到指定的图片");
            }

            // 删除物理文件
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.FilePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 删除数据库记录
            _propertyImageRepository.Delete(image);
            await _unitOfWork.CommitAsync();

            return ApiResponse.Ok("图片删除成功");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse.Error($"删除图片失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取房源图片列表
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <returns>房源图片列表</returns>
    public async Task<ApiResponse<List<PropertyImageDto>>> GetPropertyImagesAsync(int propertyId)
    {
        try
        {
            // 参数验证
            if (propertyId <= 0)
                return ApiResponse<List<PropertyImageDto>>.Error("房源ID必须大于0");

            // 验证房源是否存在
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
            {
                return ApiResponse<List<PropertyImageDto>>.Error($"未找到ID为{propertyId}的房源");
            }

            // 获取图片列表
            var images = await _propertyImageRepository.FindAsync(img => img.PropertyId == propertyId);
            var imageDtos = _mapper.Map<List<PropertyImageDto>>(images.OrderBy(img => img.IsMain ? 0 : 1).ThenBy(img => img.UploadedAt));

            return ApiResponse<List<PropertyImageDto>>.Ok(imageDtos, $"成功获取房源图片列表，共{imageDtos.Count}张图片");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PropertyImageDto>>.Error($"获取房源图片列表失败：{ex.Message}");
        }
    }
}