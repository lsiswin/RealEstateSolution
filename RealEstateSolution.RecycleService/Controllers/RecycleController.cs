using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.Common.Models;
using RealEstateSolution.Database.Models;
using RealEstateSolution.RecycleService.Services;

namespace RealEstateSolution.RecycleService.Controllers;

/// <summary>
/// 回收站控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecycleController : ControllerBase
{
    private readonly IRecycleService _recycleService;

    public RecycleController(IRecycleService recycleService)
    {
        _recycleService = recycleService;
    }

    /// <summary>
    /// 将实体移至回收站
    /// </summary>
    [HttpPost("move")]
    public async Task<ApiResponse<RecycleBin>> MoveToRecycleBin(
        [FromBody] object entity,
        [FromQuery] string deleteReason,
        [FromQuery] int deletedBy)
    {
        var recycleBin = await _recycleService.MoveToRecycleBinAsync(entity, deleteReason, deletedBy);
        return new ApiResponse<RecycleBin> { Data = recycleBin };
    }

    /// <summary>
    /// 从回收站恢复实体
    /// </summary>
    [HttpPost("{id}/restore")]
    public async Task<ApiResponse<object>> RestoreFromRecycleBin(int id)
    {
        var entity = await _recycleService.RestoreFromRecycleBinAsync<object>(id);
        if (entity == null)
        {
            return new ApiResponse<object> { Message = "未找到可恢复的实体" };
        }
        return new ApiResponse<object> { Data = entity };
    }

    /// <summary>
    /// 永久删除回收站中的实体
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> PermanentlyDelete(int id)
    {
        await _recycleService.PermanentlyDeleteAsync(id);
        return new ApiResponse { Message = "实体已永久删除" };
    }

    /// <summary>
    /// 获取回收站记录详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<RecycleBin>> GetRecycleBin(int id)
    {
        var recycleBin = await _recycleService.GetRecycleBinByIdAsync(id);
        if (recycleBin == null)
        {
            return new ApiResponse<RecycleBin> { Message = "未找到回收站记录" };
        }
        return new ApiResponse<RecycleBin> { Data = recycleBin };
    }

    /// <summary>
    /// 搜索回收站记录
    /// </summary>
    [HttpGet("search")]
    public async Task<ApiResponse<IEnumerable<RecycleBin>>> SearchRecycleBins(
        [FromQuery] string? keyword,
        [FromQuery] string? entityType,
        [FromQuery] bool? isRestored,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var recycleBins = await _recycleService.SearchRecycleBinsAsync(
            keyword,
            entityType,
            isRestored,
            startDate,
            endDate);
        return new ApiResponse<IEnumerable<RecycleBin>> { Data = recycleBins };
    }

    /// <summary>
    /// 清空回收站
    /// </summary>
    [HttpDelete("clear")]
    public async Task<ApiResponse> ClearRecycleBin([FromQuery] DateTime? beforeDate)
    {
        await _recycleService.ClearRecycleBinAsync(beforeDate);
        return new ApiResponse { Message = "回收站已清空" };
    }

    /// <summary>
    /// 获取指定实体的回收站记录
    /// </summary>
    [HttpGet("entity")]
    public async Task<ApiResponse<RecycleBin>> GetEntityRecycleBin(
        [FromQuery] string entityType,
        [FromQuery] int entityId)
    {
        var recycleBin = await _recycleService.GetEntityRecycleBinAsync(entityType, entityId);
        if (recycleBin == null)
        {
            return new ApiResponse<RecycleBin> { Message = "未找到实体的回收站记录" };
        }
        return new ApiResponse<RecycleBin> { Data = recycleBin };
    }
} 