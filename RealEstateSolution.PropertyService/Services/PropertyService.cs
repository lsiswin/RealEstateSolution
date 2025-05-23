using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Models;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Data;
using RealEstateSolution.PropertyService.Dtos;

namespace RealEstateSolution.PropertyService.Services;

/// <summary>
/// 房源服务实现类
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork<PropertyDbContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public PropertyService(
        IUnitOfWork<PropertyDbContext> unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 创建房源
    /// </summary>
    /// <param name="propertyDto">房源信息</param>
    /// <returns>创建后的房源信息</returns>
    public async Task<PropertyDto> CreatePropertyAsync(PropertyDto propertyDto)
    {
        var property = _mapper.Map<Property>(propertyDto);
        property.CreateTime = DateTime.Now;
        property.UpdateTime = DateTime.Now;

        // 添加房源图片
        if (propertyDto.ImageUrls != null && propertyDto.ImageUrls.Any())
        {
            property.Images = propertyDto.ImageUrls.Select(url => new PropertyImage
            {
                ImageUrl = url,
                CreateTime = DateTime.Now
            }).ToList();
        }

        await _unitOfWork.Repository<Property>().AddAsync(property);
        await _unitOfWork.SaveChangesAsync();

        // 清除相关缓存
        _cacheService.Remove($"property_{property.Id}");
        _cacheService.Remove($"owner_properties_{property.OwnerId}");

        return _mapper.Map<PropertyDto>(property);
    }

    /// <summary>
    /// 更新房源信息
    /// </summary>
    /// <param name="propertyDto">房源信息</param>
    public async Task UpdatePropertyAsync(PropertyDto propertyDto)
    {
        var property = await _unitOfWork.Repository<Property>()
            .Query()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == propertyDto.Id);

        if (property == null)
        {
            throw new Exception("房源不存在");
        }

        _mapper.Map(propertyDto, property);
        property.UpdateTime = DateTime.Now;

        // 更新房源图片
        if (propertyDto.ImageUrls != null)
        {
            // 删除旧图片
            if (property.Images != null)
            {
                _unitOfWork.Repository<PropertyImage>().DeleteRange(property.Images);
            }

            // 添加新图片
            property.Images = propertyDto.ImageUrls.Select(url => new PropertyImage
            {
                ImageUrl = url,
                CreateTime = DateTime.Now
            }).ToList();
        }

        await _unitOfWork.SaveChangesAsync();

        // 清除相关缓存
        _cacheService.Remove($"property_{property.Id}");
        _cacheService.Remove($"owner_properties_{property.OwnerId}");
    }

    /// <summary>
    /// 获取房源详情
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <returns>房源信息</returns>
    public async Task<PropertyDto> GetPropertyByIdAsync(int id)
    {
        return await _cacheService.GetOrAdd($"property_{id}", async () =>
        {
            var property = await _unitOfWork.Repository<Property>()
                .Query()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            return property == null ? null : _mapper.Map<PropertyDto>(property);
        }, TimeSpan.FromMinutes(30));
    }

    /// <summary>
    /// 搜索房源
    /// </summary>
    /// <param name="keyword">关键词</param>
    /// <param name="type">房源类型</param>
    /// <param name="status">房源状态</param>
    /// <param name="minPrice">最低价格</param>
    /// <param name="maxPrice">最高价格</param>
    /// <param name="location">位置</param>
    /// <returns>房源列表</returns>
    public async Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(
        string? keyword,
        PropertyType? type,
        PropertyStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        string? location)
    {
        var query = _unitOfWork.Repository<Property>()
            .Query();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.Title.Contains(keyword) || p.Description.Contains(keyword));
        }

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(p => p.Address.Contains(location));
        }

        var properties = await query
            .Include(p => p.Images)
            .ToListAsync();
        return _mapper.Map<IEnumerable<PropertyDto>>(properties);
    }

    /// <summary>
    /// 删除房源
    /// </summary>
    /// <param name="id">房源ID</param>
    public async Task DeletePropertyAsync(int id)
    {
        var property = await _unitOfWork.Repository<Property>()
            .Query()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (property == null)
        {
            throw new Exception("房源不存在");
        }

        // 删除房源图片
        if (property.Images != null)
        {
            _unitOfWork.Repository<PropertyImage>().DeleteRange(property.Images);
        }

        _unitOfWork.Repository<Property>().Delete(property);
        await _unitOfWork.SaveChangesAsync();

        // 清除相关缓存
        _cacheService.Remove($"property_{id}");
        _cacheService.Remove($"owner_properties_{property.OwnerId}");
    }

    /// <summary>
    /// 更新房源状态
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <param name="status">新状态</param>
    public async Task UpdatePropertyStatusAsync(int id, PropertyStatus status)
    {
        var property = await _unitOfWork.Repository<Property>().GetByIdAsync(id);
        if (property == null)
        {
            throw new Exception("房源不存在");
        }

        property.Status = status;
        property.UpdateTime = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();

        // 清除相关缓存
        _cacheService.Remove($"property_{id}");
        _cacheService.Remove($"owner_properties_{property.OwnerId}");
    }

    /// <summary>
    /// 获取房主的所有房源
    /// </summary>
    /// <param name="ownerId">房主ID</param>
    /// <returns>房源列表</returns>
    public async Task<IEnumerable<PropertyDto>> GetOwnerPropertiesAsync(int ownerId)
    {
        return await _cacheService.GetOrAdd($"owner_properties_{ownerId}", async () =>
        {
            var properties = await _unitOfWork.Repository<Property>()
                .Query()
                .Include(p => p.Images)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }, TimeSpan.FromMinutes(30));
    }
}