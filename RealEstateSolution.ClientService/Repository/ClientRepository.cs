using Microsoft.EntityFrameworkCore;
using RealEstateSolution.ClientService.Data;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Repository;

/// <summary>
/// 客户仓储实现类
/// 继承通用仓储，实现客户相关的数据访问操作
/// </summary>
public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    private readonly ClientDbContext _context;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context">客户数据库上下文</param>
    public ClientRepository(ClientDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// 搜索客户
    /// 根据关键字、状态和时间范围搜索客户，支持多字段模糊查询
    /// </summary>
    /// <param name="keyword">搜索关键字（可选，支持姓名、电话、邮箱、地址模糊查询）</param>
    /// <param name="status">客户状态（可选，精确匹配）</param>
    /// <param name="startDate">开始日期（可选，按创建时间筛选）</param>
    /// <param name="endDate">结束日期（可选，按创建时间筛选）</param>
    /// <returns>符合条件的客户列表，按创建时间倒序排列</returns>
    public async Task<IEnumerable<Client>> SearchClientsAsync(
        string? keyword,
        ClientStatus? status,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _dbSet.AsQueryable();

        // 关键字模糊查询（姓名、电话、邮箱、地址）
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var trimmedKeyword = keyword.Trim();
            query = query.Where(c => c.Name.Contains(trimmedKeyword) || 
                                   c.Phone.Contains(trimmedKeyword) || 
                                   (c.Email != null && c.Email.Contains(trimmedKeyword)) || 
                                   (c.Address != null && c.Address.Contains(trimmedKeyword)));
        }

        // 状态精确查询
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        // 时间范围查询
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

    /// <summary>
    /// 获取客户需求信息
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <returns>客户需求信息，如果不存在则返回null</returns>
    public async Task<ClientRequirements?> GetClientRequirementsAsync(int clientId)
    {
        return await _context.ClientRequirements
            .FirstOrDefaultAsync(r => r.ClientId == clientId);
    }

    /// <summary>
    /// 创建客户需求信息
    /// 为指定客户创建新的需求记录
    /// </summary>
    /// <param name="requirements">客户需求信息</param>
    /// <returns>创建操作的任务</returns>
    public async Task CreateClientRequirementsAsync(ClientRequirements requirements)
    {
        if (requirements == null)
            throw new ArgumentNullException(nameof(requirements));

        requirements.CreateTime = DateTime.Now;
        requirements.UpdateTime = DateTime.Now;
        
        await _context.ClientRequirements.AddAsync(requirements);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 更新客户需求信息
    /// 如果需求不存在则创建新记录，否则更新现有记录
    /// </summary>
    /// <param name="requirements">要更新的客户需求信息</param>
    /// <returns>更新操作的任务</returns>
    public async Task UpdateClientRequirementsAsync(ClientRequirements requirements)
    {
        if (requirements == null)
            throw new ArgumentNullException(nameof(requirements));

        var existingRequirements = await _context.ClientRequirements
            .FirstOrDefaultAsync(r => r.ClientId == requirements.ClientId);

        if (existingRequirements == null)
        {
            // 如果不存在，创建新记录
            requirements.CreateTime = DateTime.Now;
            requirements.UpdateTime = DateTime.Now;
            await _context.ClientRequirements.AddAsync(requirements);
        }
        else
        {
            // 更新现有记录
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

    /// <summary>
    /// 删除客户需求信息
    /// 物理删除指定客户的需求记录
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <returns>删除操作的任务</returns>
    public async Task DeleteClientRequirementsAsync(int clientId)
    {
        var requirements = await _context.ClientRequirements
            .FirstOrDefaultAsync(r => r.ClientId == clientId);

        if (requirements != null)
        {
            _context.ClientRequirements.Remove(requirements);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 更新客户状态
    /// 更新指定客户的状态并记录更新时间
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <param name="status">新的客户状态</param>
    /// <returns>更新操作的任务</returns>
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