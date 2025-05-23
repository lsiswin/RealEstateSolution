using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.RecycleService.Data;

namespace RealEstateSolution.RecycleService.Repository;

/// <summary>
/// 回收站仓储实现类
/// </summary>
public class RecycleRepository : GenericRepository<RecycleBin>, IRecycleRepository
{
    public RecycleRepository(RecycleDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RecycleBin>> SearchRecycleBinsAsync(
        string? keyword,
        string? entityType,
        bool? isRestored,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(r => r.EntityData.Contains(keyword) || 
                                   (r.DeleteReason != null && r.DeleteReason.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(r => r.EntityType == entityType);
        }

        if (isRestored.HasValue)
        {
            query = query.Where(r => r.IsRestored == isRestored.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(r => r.DeleteTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.DeleteTime <= endDate.Value);
        }

        return await query.OrderByDescending(r => r.DeleteTime).ToListAsync();
    }

    public async Task<RecycleBin?> GetEntityRecycleBinAsync(string entityType, int entityId)
    {
        return await _dbSet.FirstOrDefaultAsync(r => 
            r.EntityType == entityType && 
            r.EntityId == entityId && 
            !r.IsRestored);
    }

    public async Task RestoreEntityAsync(int recycleBinId)
    {
        var recycleBin = await _dbSet.FindAsync(recycleBinId);
        if (recycleBin != null && !recycleBin.IsRestored)
        {
            recycleBin.IsRestored = true;
            recycleBin.RestoreTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task PermanentlyDeleteAsync(int recycleBinId)
    {
        var recycleBin = await _dbSet.FindAsync(recycleBinId);
        if (recycleBin != null)
        {
            _dbSet.Remove(recycleBin);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearRecycleBinAsync(DateTime? beforeDate = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (beforeDate.HasValue)
        {
            query = query.Where(r => r.DeleteTime <= beforeDate.Value);
        }

        var itemsToDelete = await query.ToListAsync();
        _dbSet.RemoveRange(itemsToDelete);
        await _context.SaveChangesAsync();
    }
} 