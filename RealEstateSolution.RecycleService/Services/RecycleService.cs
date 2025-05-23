using System.Text.Json;
using RealEstateSolution.Database.Models;
using RealEstateSolution.RecycleService.Repository;

namespace RealEstateSolution.RecycleService.Services;

/// <summary>
/// 回收站服务实现类
/// </summary>
public class RecycleService : IRecycleService
{
    private readonly IRecycleRepository _recycleRepository;

    public RecycleService(IRecycleRepository recycleRepository)
    {
        _recycleRepository = recycleRepository;
    }

    public async Task<RecycleBin> MoveToRecycleBinAsync<T>(T entity, string deleteReason, int deletedBy) where T : class
    {
        var entityType = typeof(T).Name;
        var entityId = GetEntityId(entity);
        var entityData = JsonSerializer.Serialize(entity);

        var recycleBin = new RecycleBin
        {
            EntityType = entityType,
            EntityId = entityId,
            EntityData = entityData,
            DeleteReason = deleteReason,
            DeletedBy = deletedBy,
            DeleteTime = DateTime.Now,
            IsRestored = false
        };

        await _recycleRepository.AddAsync(recycleBin);
        return recycleBin;
    }

    public async Task<T?> RestoreFromRecycleBinAsync<T>(int recycleBinId) where T : class
    {
        var recycleBin = await _recycleRepository.GetByIdAsync(recycleBinId);
        if (recycleBin == null || recycleBin.IsRestored)
        {
            return null;
        }

        try
        {
            var entity = JsonSerializer.Deserialize<T>(recycleBin.EntityData);
            if (entity != null)
            {
                await _recycleRepository.RestoreEntityAsync(recycleBinId);
            }
            return entity;
        }
        catch
        {
            return null;
        }
    }

    public async Task PermanentlyDeleteAsync(int recycleBinId)
    {
        await _recycleRepository.PermanentlyDeleteAsync(recycleBinId);
    }

    public async Task<RecycleBin?> GetRecycleBinByIdAsync(int id)
    {
        return await _recycleRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<RecycleBin>> SearchRecycleBinsAsync(
        string? keyword,
        string? entityType,
        bool? isRestored,
        DateTime? startDate,
        DateTime? endDate)
    {
        return await _recycleRepository.SearchRecycleBinsAsync(
            keyword,
            entityType,
            isRestored,
            startDate,
            endDate);
    }

    public async Task ClearRecycleBinAsync(DateTime? beforeDate = null)
    {
        await _recycleRepository.ClearRecycleBinAsync(beforeDate);
    }

    public async Task<RecycleBin?> GetEntityRecycleBinAsync(string entityType, int entityId)
    {
        return await _recycleRepository.GetEntityRecycleBinAsync(entityType, entityId);
    }

    private int GetEntityId<T>(T entity) where T : class
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var idValue = idProperty.GetValue(entity);
            if (idValue != null)
            {
                return Convert.ToInt32(idValue);
            }
        }
        throw new InvalidOperationException("实体必须包含Id属性");
    }
}