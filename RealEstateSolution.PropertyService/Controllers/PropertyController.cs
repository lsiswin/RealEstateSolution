using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.Common.Models;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Services;
using RealEstateSolution.PropertyService.Dtos;

namespace RealEstateSolution.PropertyService.Controllers;

/// <summary>
/// 房源服务控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>
    /// 创建房源
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<PropertyDto>> CreateProperty([FromBody] PropertyDto propertyDto)
    {
        var result = await _propertyService.CreatePropertyAsync(propertyDto);
        return new ApiResponse<PropertyDto> { Data = result };
    }

    /// <summary>
    /// 更新房源
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateProperty(int id, [FromBody] PropertyDto propertyDto)
    {
        if (id != propertyDto.Id)
        {
            return new ApiResponse { Message = "ID不匹配" };
        }
        await _propertyService.UpdatePropertyAsync(propertyDto);
        return new ApiResponse { Message = "房源已更新" };
    }

    /// <summary>
    /// 获取房源详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<PropertyDto>> GetProperty(int id)
    {
        var property = await _propertyService.GetPropertyByIdAsync(id);
        if (property == null)
        {
            return new ApiResponse<PropertyDto> { Message = "未找到房源" };
        }
        return new ApiResponse<PropertyDto> { Data = property };
    }

    /// <summary>
    /// 搜索房源
    /// </summary>
    [HttpGet("search")]
    public async Task<ApiResponse<IEnumerable<PropertyDto>>> SearchProperties(
        [FromQuery] string? keyword,
        [FromQuery] PropertyType? type,
        [FromQuery] PropertyStatus? status,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? location)
    {
        var properties = await _propertyService.SearchPropertiesAsync(
            keyword,
            type,
            status,
            minPrice,
            maxPrice,
            location);
        return new ApiResponse<IEnumerable<PropertyDto>> { Data = properties };
    }

    /// <summary>
    /// 删除房源
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteProperty(int id)
    {
        await _propertyService.DeletePropertyAsync(id);
        return new ApiResponse { Message = "房源已删除" };
    }

    /// <summary>
    /// 更新房源状态
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse> UpdatePropertyStatus(int id, [FromQuery] PropertyStatus status)
    {
        await _propertyService.UpdatePropertyStatusAsync(id, status);
        return new ApiResponse { Message = "房源状态已更新" };
    }

    /// <summary>
    /// 获取房主的所有房源
    /// </summary>
    [HttpGet("owner/{ownerId}")]
    public async Task<ApiResponse<IEnumerable<PropertyDto>>> GetOwnerProperties(int ownerId)
    {
        var properties = await _propertyService.GetOwnerPropertiesAsync(ownerId);
        return new ApiResponse<IEnumerable<PropertyDto>> { Data = properties };
    }
} 