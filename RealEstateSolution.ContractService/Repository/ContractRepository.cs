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
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.ContractNumber.Contains(keyword) ||
                                   c.Terms.Contains(keyword) ||
                                   c.Remark.Contains(keyword));
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
            query = query.Where(c => c.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.EndDate <= endDate.Value);
        }

        return await query.Include(c => c.Property)
                         .Include(c => c.Client)
                         .ToListAsync();
    }

    public async Task UpdateStatusAsync(int contractId, ContractStatus status)
    {
        var contract = await _dbSet.FindAsync(contractId);
        if (contract != null)
        {
            contract.Status = status;
            contract.UpdateTime = DateTime.Now;

            switch (status)
            {
                case ContractStatus.Signed:
                    contract.SignTime = DateTime.Now;
                    break;
                case ContractStatus.Completed:
                    contract.CompleteTime = DateTime.Now;
                    break;
                case ContractStatus.Cancelled:
                    contract.CancelTime = DateTime.Now;
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Contract>> GetClientContractsAsync(int clientId)
    {
        return await _dbSet.Include(c => c.Property)
                          .Where(c => c.ClientId == clientId)
                          .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetPropertyContractsAsync(int propertyId)
    {
        return await _dbSet.Include(c => c.Client)
                          .Where(c => c.PropertyId == propertyId)
                          .ToListAsync();
    }

    public async Task<string> GenerateContractNumberAsync()
    {
        var date = DateTime.Now.ToString("yyyyMMdd");
        var count = await _dbSet.CountAsync(c => c.ContractNumber.StartsWith(date)) + 1;
        return $"{date}{count:D4}";
    }
} 