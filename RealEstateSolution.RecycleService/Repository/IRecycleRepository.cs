using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.RecycleService.Repository;

/// <summary>
/// 回收站仓储接口
/// </summary>
public interface IRecycleRepository : IGenericRepository<RecycleBin>
{
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
    /// 获取指定实体的回收站记录
    /// </summary>
    Task<RecycleBin?> GetEntityRecycleBinAsync(string entityType, int entityId);

    /// <summary>
    /// 恢复实体
    /// </summary>
    Task RestoreEntityAsync(int recycleBinId);

    /// <summary>
    /// 永久删除实体
    /// </summary>
    Task PermanentlyDeleteAsync(int recycleBinId);

    /// <summary>
    /// 清空回收站
    /// </summary>
    Task ClearRecycleBinAsync(DateTime? beforeDate = null);
} 