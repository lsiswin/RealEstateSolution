using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Services;

/// <summary>
/// 客户服务接口
/// </summary>
public interface IClientService
{
    /// <summary>
    /// 创建客户
    /// </summary>
    Task<Client> CreateClientAsync(Client client);

    /// <summary>
    /// 更新客户信息
    /// </summary>
    Task<Client> UpdateClientAsync(Client client);

    /// <summary>
    /// 获取客户详情
    /// </summary>
    Task<Client?> GetClientByIdAsync(int id);

    /// <summary>
    /// 搜索客户
    /// </summary>
    Task<IEnumerable<Client>> SearchClientsAsync(
        string? keyword,
        ClientType? type,
        ClientStatus? status,
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 删除客户
    /// </summary>
    Task DeleteClientAsync(int id);

    /// <summary>
    /// 获取客户购房需求
    /// </summary>
    Task<ClientRequirements?> GetClientRequirementsAsync(int clientId);

    /// <summary>
    /// 更新客户购房需求
    /// </summary>
    Task<ClientRequirements> UpdateClientRequirementsAsync(ClientRequirements requirements);

    /// <summary>
    /// 更新客户状态
    /// </summary>
    Task UpdateClientStatusAsync(int clientId, ClientStatus status);
} 