using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Repository;

/// <summary>
/// 客户仓储接口
/// </summary>
public interface IClientRepository : IGenericRepository<Client>
{
    /// <summary>
    /// 搜索客户
    /// </summary>
    Task<IEnumerable<Client>> SearchClientsAsync(
        string? keyword,
        ClientStatus? status,
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 获取客户需求
    /// </summary>
    Task<ClientRequirements?> GetClientRequirementsAsync(int clientId);

    /// <summary>
    /// 更新客户需求
    /// </summary>
    Task UpdateClientRequirementsAsync(ClientRequirements requirements);

    /// <summary>
    /// 更新客户状态
    /// </summary>
    Task UpdateClientStatusAsync(int clientId, ClientStatus status);
} 