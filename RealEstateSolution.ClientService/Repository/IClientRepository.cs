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
    /// <param name="name">客户姓名（可选，模糊查询）</param>
    /// <param name="phone">客户电话（可选，模糊查询）</param>
    /// <param name="email">客户邮箱（可选，模糊查询）</param>
    /// <param name="type">客户类型（可选，精确匹配）</param>
    /// <param name="status">客户状态（可选，精确匹配）</param>
    /// <returns>符合条件的客户列表</returns>
    Task<IEnumerable<Client>> SearchClientsAsync(
        string? name,
        string? phone,
        string? email,
        ClientType? type,
        ClientStatus? status);

    /// <summary>
    /// 检查手机号是否已存在
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <param name="excludeId">排除的客户ID（用于更新时检查）</param>
    /// <returns>是否存在</returns>
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeId = null);

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="excludeId">排除的客户ID（用于更新时检查）</param>
    /// <returns>是否存在</returns>
    Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>删除操作的任务</returns>
    Task DeleteClientAsync(int id);

    /// <summary>
    /// 更新客户状态
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="status">新的客户状态</param>
    /// <returns>更新操作的任务</returns>
    Task UpdateStatusAsync(int id, ClientStatus status);

    /// <summary>
    /// 获取客户统计信息
    /// </summary>
    /// <returns>总数、活跃数、非活跃数、潜在客户数</returns>
    Task<(int total, int active, int inactive, int potential)> GetClientStatsAsync();

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
} 