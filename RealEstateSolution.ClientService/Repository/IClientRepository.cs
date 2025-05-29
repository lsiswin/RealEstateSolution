using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Repository;

/// <summary>
/// 客户仓储接口
/// 继承通用仓储接口，提供客户相关的数据访问操作
/// </summary>
public interface IClientRepository : IGenericRepository<Client>
{
    /// <summary>
    /// 搜索客户
    /// 根据关键字、状态和时间范围搜索客户
    /// </summary>
    /// <param name="keyword">搜索关键字（可选，支持姓名、电话模糊查询）</param>
    /// <param name="status">客户状态（可选，精确匹配）</param>
    /// <param name="startDate">开始日期（可选，按创建时间筛选）</param>
    /// <param name="endDate">结束日期（可选，按创建时间筛选）</param>
    /// <returns>符合条件的客户列表</returns>
    Task<IEnumerable<Client>> SearchClientsAsync(
        string? keyword,
        ClientStatus? status,
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 获取客户需求信息
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <returns>客户需求信息，如果不存在则返回null</returns>
    Task<ClientRequirements?> GetClientRequirementsAsync(int clientId);

    /// <summary>
    /// 创建客户需求信息
    /// </summary>
    /// <param name="requirements">客户需求信息</param>
    /// <returns>创建操作的任务</returns>
    Task CreateClientRequirementsAsync(ClientRequirements requirements);

    /// <summary>
    /// 更新客户需求信息
    /// </summary>
    /// <param name="requirements">要更新的客户需求信息</param>
    /// <returns>更新操作的任务</returns>
    Task UpdateClientRequirementsAsync(ClientRequirements requirements);

    /// <summary>
    /// 删除客户需求信息
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <returns>删除操作的任务</returns>
    Task DeleteClientRequirementsAsync(int clientId);

    /// <summary>
    /// 更新客户状态
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <param name="status">新的客户状态</param>
    /// <returns>更新操作的任务</returns>
    Task UpdateClientStatusAsync(int clientId, ClientStatus status);
} 