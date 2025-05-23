using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.MatchingService.Data;

namespace RealEstateSolution.MatchingService.Repository;

/// <summary>
/// 匹配仓储实现类
/// </summary>
public class MatchingRepository : GenericRepository<Matching>, IMatchingRepository
{
    public MatchingRepository(MatchingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Matching>> SearchMatchingsAsync(
        string? keyword,
        MatchingType? type,
        MatchingStatus? status,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(m => m.Reason != null && m.Reason.Contains(keyword));
        }

        if (type.HasValue)
        {
            query = query.Where(m => m.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(m => m.CreateTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(m => m.CreateTime <= endDate.Value);
        }

        return await query.Include(m => m.Property)
                         .Include(m => m.Client)
                         .ToListAsync();
    }

    public async Task UpdateStatusAsync(int matchingId, MatchingStatus status)
    {
        var matching = await _dbSet.FindAsync(matchingId);
        if (matching != null)
        {
            matching.Status = status;
            matching.UpdateTime = DateTime.Now;

            if (status == MatchingStatus.Cancelled)
            {
                matching.ExpireTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Matching>> GetClientMatchingsAsync(int clientId)
    {
        return await _dbSet.Include(m => m.Property)
                          .Where(m => m.ClientId == clientId)
                          .ToListAsync();
    }

    public async Task<IEnumerable<Matching>> GetPropertyMatchingsAsync(int propertyId)
    {
        return await _dbSet.Include(m => m.Client)
                          .Where(m => m.PropertyId == propertyId)
                          .ToListAsync();
    }
} 