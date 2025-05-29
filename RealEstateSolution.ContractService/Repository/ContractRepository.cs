using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.ContractService.Data;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Repository;

/// <summary>
/// 合同仓储实现类
/// </summary>
public class ContractRepository : GenericRepository<Contract>, IContractRepository
{
    public ContractRepository(ContractDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Contract>> SearchContractsAsync(
        string? keyword,
        ContractType? type,
        ContractStatus? status,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _dbSet.AsQueryable().Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.ContractNumber.Contains(keyword) ||
                                   c.Title.Contains(keyword) ||
                                   c.Content.Contains(keyword) ||
                                   (c.Notes != null && c.Notes.Contains(keyword)));
        }

        if (type.HasValue)
        {
            query = query.Where(c => c.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(c => c.EffectiveDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.ExpiryDate <= endDate.Value);
        }

        return await query.Include(c => c.Property)
                         .Include(c => c.PartyA)
                         .Include(c => c.PartyB)
                         .ToListAsync();
    }

    public async Task UpdateStatusAsync(int contractId, ContractStatus status)
    {
        var contract = await _dbSet.FindAsync(contractId);
        if (contract != null)
        {
            contract.Status = status;
            contract.UpdatedAt = DateTime.Now;

            // 如果状态变为已签署，设置签署日期
            if (status == ContractStatus.Signed && !contract.SignDate.HasValue)
            {
                contract.SignDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Contract>> GetClientContractsAsync(int clientId)
    {
        return await _dbSet.Include(c => c.Property)
                          .Include(c => c.PartyA)
                          .Include(c => c.PartyB)
                          .Where(c => !c.IsDeleted && (c.PartyAId == clientId || c.PartyBId == clientId))
                          .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetPropertyContractsAsync(int propertyId)
    {
        return await _dbSet.Include(c => c.PartyA)
                          .Include(c => c.PartyB)
                          .Where(c => !c.IsDeleted && c.PropertyId == propertyId)
                          .ToListAsync();
    }

    public async Task<string> GenerateContractNumberAsync()
    {
        var date = DateTime.Now.ToString("yyyyMMdd");
        var count = await _dbSet.CountAsync(c => c.ContractNumber.StartsWith(date)) + 1;
        return $"{date}{count:D4}";
    }
} 