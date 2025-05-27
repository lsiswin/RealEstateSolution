using RealEstateSolution.ClientService.Models;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;
using System.Threading.Tasks;

namespace RealEstateSolution.ClientService.Services;

/// <summary>
/// 客户服务接口
/// </summary>
public interface IClientService
{
    /// <summary>
    /// 获取客户列表
    /// </summary>
    Task<ApiResponse<PagedList<Client>>> GetClientsAsync(
        int userId,
        string name = null,
        string phone = null,
        ClientType? type = null,
        int pageIndex = 1,
        int pageSize = 10);

    /// <summary>
    /// 获取客户详情
    /// </summary>
    Task<ApiResponse<Client>> GetClientByIdAsync(int id, int userId);

    /// <summary>
    /// 创建客户
    /// </summary>
    Task<ApiResponse<Client>> CreateClientAsync(Client client, int userId);

    /// <summary>
    /// 更新客户信息
    /// </summary>
    Task<ApiResponse<Client>> UpdateClientAsync(int id, Client client, int userId);

    /// <summary>
    /// 删除客户
    /// </summary>
    Task<ApiResponse> DeleteClientAsync(int id, int userId);

    /// <summary>
    /// 获取客户需求
    /// </summary>
    Task<ApiResponse<ClientRequirement>> GetClientRequirementsAsync(int clientId, int userId);

    /// <summary>
    /// 更新客户需求
    /// </summary>
    Task<ApiResponse<ClientRequirement>> UpdateClientRequirementsAsync(int clientId, ClientRequirement requirements, int userId);

    /// <summary>
    /// 获取客户统计数据
    /// </summary>
    Task<ApiResponse<ClientStats>> GetClientStatsAsync(int userId);
} 