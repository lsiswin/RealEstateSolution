using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;
using RealEstateSolution.MatchingService.Services;

namespace RealEstateSolution.MatchingService.Controllers;

/// <summary>
/// 匹配服务控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;

    public MatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    /// <summary>
    /// 创建匹配
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<Matching>> CreateMatching([FromBody] Matching matching)
    {
        var result = await _matchingService.CreateMatchingAsync(matching);
        return new ApiResponse<Matching> { Data = result };
    }

    /// <summary>
    /// 更新匹配状态
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse> UpdateMatchingStatus(int id, [FromQuery] MatchingStatus status)
    {
        await _matchingService.UpdateMatchingStatusAsync(id, status);
        return new ApiResponse { Message = "匹配状态已更新" };
    }

    /// <summary>
    /// 获取匹配详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<Matching>> GetMatching(int id)
    {
        var matching = await _matchingService.GetMatchingByIdAsync(id);
        if (matching == null)
        {
            return new ApiResponse<Matching> { Message = "未找到匹配记录" };
        }
        return new ApiResponse<Matching> { Data = matching };
    }

    /// <summary>
    /// 搜索匹配记录
    /// </summary>
    [HttpGet("search")]
    public async Task<ApiResponse<IEnumerable<Matching>>> SearchMatchings(
        [FromQuery] string? keyword,
        [FromQuery] MatchingType? type,
        [FromQuery] MatchingStatus? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var matchings = await _matchingService.SearchMatchingsAsync(
            keyword,
            type,
            status,
            startDate,
            endDate);
        return new ApiResponse<IEnumerable<Matching>> { Data = matchings };
    }

    /// <summary>
    /// 获取客户的匹配记录
    /// </summary>
    [HttpGet("client/{clientId}")]
    public async Task<ApiResponse<IEnumerable<Matching>>> GetClientMatchings(int clientId)
    {
        var matchings = await _matchingService.GetClientMatchingsAsync(clientId);
        return new ApiResponse<IEnumerable<Matching>> { Data = matchings };
    }

    /// <summary>
    /// 获取房源的匹配记录
    /// </summary>
    [HttpGet("property/{propertyId}")]
    public async Task<ApiResponse<IEnumerable<Matching>>> GetPropertyMatchings(int propertyId)
    {
        var matchings = await _matchingService.GetPropertyMatchingsAsync(propertyId);
        return new ApiResponse<IEnumerable<Matching>> { Data = matchings };
    }

    /// <summary>
    /// 删除匹配记录
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteMatching(int id)
    {
        await _matchingService.DeleteMatchingAsync(id);
        return new ApiResponse { Message = "匹配记录已删除" };
    }

    /// <summary>
    /// 自动匹配
    /// </summary>
    [HttpPost("auto/{clientId}")]
    public async Task<ApiResponse<IEnumerable<Matching>>> AutoMatch(int clientId)
    {
        var matchings = await _matchingService.AutoMatchAsync(clientId);
        return new ApiResponse<IEnumerable<Matching>> { Data = matchings };
    }

    /// <summary>
    /// 手动匹配
    /// </summary>
    [HttpPost("manual")]
    public async Task<ApiResponse<Matching>> ManualMatch(
        [FromQuery] int clientId,
        [FromQuery] int propertyId)
    {
        var matching = await _matchingService.ManualMatchAsync(clientId, propertyId);
        return new ApiResponse<Matching> { Data = matching };
    }
} 