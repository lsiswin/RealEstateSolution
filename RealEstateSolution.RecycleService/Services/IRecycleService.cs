

using RealEstateSolution.Database.Models;

namespace RealEstateSolution.RecycleService.Services;

/// <summary>
/// 回收站服务接口
/// </summary>
public interface IRecycleService
{
    /// <summary>
    /// 将实体移至回收站
    /// </summary>
    Task<RecycleBin> MoveToRecycleBinAsync<T>(T entity, string deleteReason, int deletedBy) where T : class;

    /// <summary>
    /// 从回收站恢复实体
    /// </summary>
    Task<T?> RestoreFromRecycleBinAsync<T>(int recycleBinId) where T : class;

    /// <summary>
    /// 永久删除回收站中的实体
    /// </summary>
    Task PermanentlyDeleteAsync(int recycleBinId);

    /// <summary>
    /// 获取回收站记录详情
    /// </summary>
    Task<RecycleBin?> GetRecycleBinByIdAsync(int id);

    /// <summary>
    /// 搜索回收站记录
    /// </summary>
    Task<IEnumerable<RecycleBin>> SearchRecycleBinsAsync(
        string? keyword,
        string? entityType,
        bool? isRestored,
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 清空回收站
    /// </summary>
    Task ClearRecycleBinAsync(DateTime? beforeDate = null);

    /// <summary>
    /// 获取指定实体的回收站记录
    /// </summary>
    Task<RecycleBin?> GetEntityRecycleBinAsync(string entityType, int entityId);
} 