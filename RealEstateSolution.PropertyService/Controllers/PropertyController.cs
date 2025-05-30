using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Services;
using RealEstateSolution.PropertyService.Dtos;
using RealEstateSolution.Common.Utils;
using System.Security.Claims;

namespace RealEstateSolution.PropertyService.Controllers;

/// <summary>
/// 房源管理控制器
/// 提供房源相关的API接口，包括房源的增删改查、状态管理、图片管理和统计功能
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = "admin,broker")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="propertyService">房源服务</param>
    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    /// <returns>当前登录用户的ID</returns>
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
        return User.IsInRole("admin")?true:User.IsInRole("broker");
    }

    /// <summary>
    /// 获取房源列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页的房源列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedList<PropertyDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<PagedList<PropertyDto>>>> GetProperties([FromQuery] PropertyQueryDto query)
    {
        try
        {
            var result = await _propertyService.GetPropertiesAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedList<PropertyDto>>.Error($"获取房源列表失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 根据ID获取房源详细信息
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <returns>房源详细信息</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PropertyDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PropertyDto>>> GetProperty(int id)
    {
        try
        {
            var result = await _propertyService.GetPropertyByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PropertyDto>.Error($"获取房源详细信息失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 创建新房源
    /// </summary>
    /// <param name="propertyDto">房源信息DTO</param>
    /// <returns>创建成功的房源信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PropertyDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<PropertyDto>>> CreateProperty([FromBody] PropertyDto propertyDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _propertyService.CreatePropertyAsync(propertyDto, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<PropertyDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PropertyDto>.Error($"创建房源失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 更新房源信息
    /// </summary>
    /// <param name="id">要更新的房源ID</param>
    /// <param name="propertyDto">新的房源信息DTO</param>
    /// <returns>更新后的房源信息</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PropertyDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PropertyDto>>> UpdateProperty(int id, [FromBody] PropertyDto propertyDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _propertyService.UpdatePropertyAsync(id, propertyDto, userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<PropertyDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PropertyDto>.Error($"更新房源信息失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 删除房源
    /// </summary>
    /// <param name="id">要删除的房源ID</param>
    /// <returns>删除操作结果</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> DeleteProperty(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _propertyService.DeletePropertyAsync(id, userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Error($"删除房源失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 更新房源状态
    /// </summary>
    /// <param name="id">房源ID</param>
    /// <param name="statusDto">状态更新DTO</param>
    /// <returns>更新后的房源信息</returns>
    [HttpPost("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<PropertyDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PropertyDto>>> UpdatePropertyStatus(int id, [FromBody] PropertyStatusUpdateDto statusDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _propertyService.UpdatePropertyStatusAsync(id, statusDto, userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<PropertyDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PropertyDto>.Error($"更新房源状态失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 获取房源统计数据
    /// </summary>
    /// <returns>房源相关的统计信息</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PropertyStatsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<PropertyStatsDto>>> GetPropertyStats()
    {
        try
        {
            var result = await _propertyService.GetPropertyStatsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PropertyStatsDto>.Error($"获取房源统计数据失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 上传房源图片
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <param name="file">图片文件</param>
    /// <returns>上传成功的图片信息</returns>
    [HttpPost("{propertyId}/images")]
    [ProducesResponseType(typeof(ApiResponse<PropertyImageDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PropertyImageDto>>> UploadPropertyImage(int propertyId, IFormFile file)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _propertyService.UploadPropertyImageAsync(propertyId, file, userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<PropertyImageDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PropertyImageDto>.Error($"上传图片失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 删除房源图片
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <param name="imageId">图片ID</param>
    /// <returns>删除操作结果</returns>
    [HttpDelete("{propertyId}/images/{imageId}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> DeletePropertyImage(int propertyId, int imageId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _propertyService.DeletePropertyImageAsync(propertyId, imageId, userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Error($"删除图片失败：{ex.Message}"));
        }
    }

    /// <summary>
    /// 获取房源图片列表
    /// </summary>
    /// <param name="propertyId">房源ID</param>
    /// <returns>房源图片列表</returns>
    [HttpGet("{propertyId}/images")]
    [ProducesResponseType(typeof(ApiResponse<List<PropertyImageDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<List<PropertyImageDto>>>> GetPropertyImages(int propertyId)
    {
        try
        {
            var result = await _propertyService.GetPropertyImagesAsync(propertyId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<PropertyImageDto>>.Error($"获取房源图片列表失败：{ex.Message}"));
        }
    }
} 