using Microsoft.EntityFrameworkCore;
using RealEstateSolution.ClientService.Data;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Repository;

/// <summary>
/// 客户仓储实现类
/// 继承通用仓储，提供客户相关的数据访问操作
/// </summary>
public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    private readonly ClientDbContext _context;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public ClientRepository(ClientDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// 搜索客户
    /// </summary>
    public async Task<IEnumerable<Client>> SearchClientsAsync(
        string? name,
        string? phone,
        string? email,
        ClientType? type,
        ClientStatus? status)
    {
        var query = _context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            query = query.Where(c => c.Phone.Contains(phone));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(c => c.Email != null && c.Email.Contains(email));
        }

        if (type.HasValue)
        {
            query = query.Where(c => c.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        return await query.OrderByDescending(c => c.CreateTime).ToListAsync();
    }

    /// <summary>
    /// 检查手机号是否已存在
    /// </summary>
    public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeId = null)
    {
        var query = _context.Clients.Where(c => c.Phone == phone);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Clients.Where(c => c.Email == email);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    public async Task DeleteClientAsync(int id)
    {
        var client = await GetByIdAsync(id);
        if (client != null)
        {
            // 先删除客户需求
            await DeleteClientRequirementsAsync(id);
            
            // 删除客户
            Delete(client);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 更新客户状态
    /// </summary>
    public async Task UpdateStatusAsync(int id, ClientStatus status)
    {
        var client = await GetByIdAsync(id);
        if (client != null)
        {
            client.Status = status;
            client.UpdateTime = DateTime.Now;
            Update(client);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 获取客户统计信息
    /// </summary>
    public async Task<(int total, int active, int inactive, int potential)> GetClientStatsAsync()
    {
        var total = await _context.Clients.CountAsync();
        var active = await _context.Clients.CountAsync(c => c.Status == ClientStatus.Active);
        var inactive = await _context.Clients.CountAsync(c => c.Status == ClientStatus.Invalid);
        var potential = await _context.Clients.CountAsync(c => c.Status == ClientStatus.Potential);

        return (total, active, inactive, potential);
    }

    /// <summary>
    /// 获取客户需求信息
    /// </summary>
    public async Task<ClientRequirements?> GetClientRequirementsAsync(int clientId)
    {
        return await _context.ClientRequirements
            .FirstOrDefaultAsync(r => r.ClientId == clientId);
    }

    /// <summary>
    /// 创建客户需求信息
    /// </summary>
    public async Task CreateClientRequirementsAsync(ClientRequirements requirements)
    {
        requirements.CreateTime = DateTime.Now;
        requirements.UpdateTime = DateTime.Now;
        
        _context.ClientRequirements.Add(requirements);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 更新客户需求信息
    /// </summary>
    public async Task UpdateClientRequirementsAsync(ClientRequirements requirements)
    {
        requirements.UpdateTime = DateTime.Now;
        
        _context.ClientRequirements.Update(requirements);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 删除客户需求信息
    /// </summary>
    public async Task DeleteClientRequirementsAsync(int clientId)
    {
        var requirements = await GetClientRequirementsAsync(clientId);
        if (requirements != null)
        {
            _context.ClientRequirements.Remove(requirements);
            await _context.SaveChangesAsync();
        }
    }
} 