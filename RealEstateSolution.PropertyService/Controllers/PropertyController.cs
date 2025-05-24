using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Services;
using RealEstateSolution.Common.Utils;
using System.Security.Claims;

namespace RealEstateSolution.PropertyService.Controllers;

/// <summary>
/// 房源管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("无效的用户身份");
        }
        return userId;
    }

    /// <summary>
    /// 判断当前用户是否为经纪人
    /// </summary>
    private bool IsAgent()
    {
        return User.IsInRole("Agent");
    }

    /// <summary>
    /// 登记新房源
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<Property>> RegisterProperty([FromBody] Property property)
    {
        var userId = GetCurrentUserId();
        return await _propertyService.RegisterPropertyAsync(property, userId);
    }

    /// <summary>
    /// 修改房源信息
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<Property>> UpdateProperty(int id, [FromBody] Property property)
    {
        var userId = GetCurrentUserId();
        return await _propertyService.UpdatePropertyAsync(id, property, userId);
    }

    /// <summary>
    /// 变更房源状态
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ApiResponse<Property>> ChangePropertyStatus(int id, [FromBody] PropertyStatus status)
    {
        var userId = GetCurrentUserId();
        return await _propertyService.ChangePropertyStatusAsync(id, status, userId);
    }

    /// <summary>
    /// 获取房源详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<Property>> GetProperty(int id)
    {
        var userId = GetCurrentUserId();
        return await _propertyService.GetPropertyByIdAsync(id, userId);
    }

    /// <summary>
    /// 查询房源列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedList<Property>>> QueryProperties(
        [FromQuery] PropertyType? type,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] decimal? minArea,
        [FromQuery] decimal? maxArea,
        [FromQuery] PropertyStatus? status,
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        var isAgent = IsAgent();
        return await _propertyService.QueryPropertiesAsync(
            userId, isAgent, type, minPrice, maxPrice, minArea, maxArea, status, keyword, pageIndex, pageSize);
    }

    /// <summary>
    /// 删除房源
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteProperty(int id)
    {
        var userId = GetCurrentUserId();
        return await _propertyService.DeletePropertyAsync(id, userId);
    }
} 