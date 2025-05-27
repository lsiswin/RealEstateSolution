using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Services;
using RealEstateSolution.Common.Utils;
using System.Security.Claims;
using RealEstateSolution.PropertyService.Models;

namespace RealEstateSolution.PropertyService.Controllers;

/// <summary>
/// 房源管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
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
    private string GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
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
    [HttpPost("{id}/status")]
    public async Task<ApiResponse<Property>> ChangePropertyStatus(int id, [FromBody] PropertyStatusUpdateDto statusUpdate)
    {
        var userId = GetCurrentUserId();
        return await _propertyService.ChangePropertyStatusAsync(id, statusUpdate.Status, userId);
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

    /// <summary>
    /// 获取房源统计数据
    /// </summary>
    [HttpGet("stats")]
    public async Task<ApiResponse<PropertyStats>> GetPropertyStats()
    {
        var userId = GetCurrentUserId();
        var isAgent = IsAgent();
        return await _propertyService.GetPropertyStatsAsync(userId, isAgent);
    }
    
    ///// <summary>
    ///// 上传房源图片
    ///// </summary>
    //[HttpPost("{id}/images")]
    //public async Task<ApiResponse<PropertyImage>> UploadPropertyImage(int id, IFormFile file)
    //{
    //    var userId = GetCurrentUserId();
    //    return await _propertyService.UploadPropertyImageAsync(id, file, userId);
    //}
    
    /// <summary>
    /// 删除房源图片
    /// </summary>
    //[HttpDelete("{propertyId}/images/{imageId}")]
    //public async Task<ApiResponse> DeletePropertyImage(int propertyId, int imageId)
    //{
    //    var userId = GetCurrentUserId();
    //    return await _propertyService.DeletePropertyImageAsync(propertyId, imageId, userId);
    //}
} 