using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.ContractService.Data;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Repository;

/// <summary>
/// 合同模板仓储实现类
/// </summary>
public class ContractTemplateRepository : GenericRepository<ContractTemplate>, IContractTemplateRepository
{
    public ContractTemplateRepository(ContractDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ContractTemplate>> SearchTemplatesAsync(
        string? name,
        ContractType? type,
        bool? isActive)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t => t.Name.Contains(name) ||
                                   (t.Description != null && t.Description.Contains(name)));
        }

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(t => t.IsActive == isActive.Value);
        }

        return await query.OrderByDescending(t => t.CreatedAt)
                         .ToListAsync();
    }

    public async Task<IEnumerable<ContractTemplate>> GetActiveTemplatesByTypeAsync(ContractType type)
    {
        return await _dbSet.Where(t => t.Type == type && t.IsActive)
                          .OrderByDescending(t => t.CreatedAt)
                          .ToListAsync();
    }

    public async Task<bool> IsNameExistsAsync(string name, int? excludeId = null)
    {
        var query = _dbSet.Where(t => t.Name == name);
        
        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task UpdateStatusAsync(int templateId, bool isActive)
    {
        var template = await _dbSet.FindAsync(templateId);
        if (template != null)
        {
            template.IsActive = isActive;
            template.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteTemplateAsync(int templateId)
    {
        var template = await _dbSet.FindAsync(templateId);
        if (template != null)
        {
            _dbSet.Remove(template);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(int Total, int Active, int Inactive)> GetTemplateStatsAsync()
    {
        var total = await _dbSet.CountAsync();
        var active = await _dbSet.CountAsync(t => t.IsActive);
        var inactive = total - active;

        return (total, active, inactive);
    }
} 