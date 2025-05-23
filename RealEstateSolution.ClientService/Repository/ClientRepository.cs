using Microsoft.EntityFrameworkCore;
using RealEstateSolution.ClientService.Data;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Repository;

/// <summary>
/// 客户仓储实现类
/// </summary>
public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    private readonly ClientDbContext _context;

    public ClientRepository(ClientDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> SearchClientsAsync(
        string? keyword,
        ClientStatus? status,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.Name.Contains(keyword) || 
                                   c.Phone.Contains(keyword) || 
                                   c.Email.Contains(keyword) || 
                                   c.Address.Contains(keyword));
        }
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(c => c.CreateTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.CreateTime <= endDate.Value);
        }

        return await query.OrderByDescending(c => c.CreateTime).ToListAsync();
    }

    public async Task<ClientRequirements?> GetClientRequirementsAsync(int clientId)
    {
        return await _context.ClientRequirements
            .FirstOrDefaultAsync(r => r.ClientId == clientId);
    }

    public async Task UpdateClientRequirementsAsync(ClientRequirements requirements)
    {
        var existingRequirements = await _context.ClientRequirements
            .FirstOrDefaultAsync(r => r.ClientId == requirements.ClientId);

        if (existingRequirements == null)
        {
            requirements.CreateTime = DateTime.Now;
            requirements.UpdateTime = DateTime.Now;
            await _context.ClientRequirements.AddAsync(requirements);
        }
        else
        {
            existingRequirements.MinPrice = requirements.MinPrice;
            existingRequirements.MaxPrice = requirements.MaxPrice;
            existingRequirements.MinArea = requirements.MinArea;
            existingRequirements.MaxArea = requirements.MaxArea;
            existingRequirements.Location = requirements.Location;
            existingRequirements.PropertyType = requirements.PropertyType;
            existingRequirements.OtherRequirements = requirements.OtherRequirements;
            existingRequirements.UpdateTime = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateClientStatusAsync(int clientId, ClientStatus status)
    {
        var client = await _dbSet.FindAsync(clientId);
        if (client != null)
        {
            client.Status = status;
            client.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
} 