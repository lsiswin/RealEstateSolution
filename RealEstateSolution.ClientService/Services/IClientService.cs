using RealEstateSolution.ClientService.Dtos;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;
using System.Threading.Tasks;

namespace RealEstateSolution.ClientService.Services;

/// <summary>
/// 客户服务接口
/// 提供客户管理相关的业务逻辑操作，包括客户的增删改查、需求管理和统计功能
/// </summary>
public interface IClientService
{
    /// <summary>
    /// 获取客户列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页的客户列表</returns>
    Task<ApiResponse<PagedList<ClientDto>>> GetClientsAsync(ClientQueryDto query);

    /// <summary>
    /// 根据ID获取客户详细信息
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详细信息</returns>
    Task<ApiResponse<ClientDto>> GetClientByIdAsync(int id);

    /// <summary>
    /// 创建新客户
    /// </summary>
    /// <param name="clientDto">客户信息DTO</param>
    /// <param name="userId">创建人ID</param>
    /// <returns>创建成功的客户信息</returns>
    Task<ApiResponse<ClientDto>> CreateClientAsync(ClientDto clientDto, string userId);

    /// <summary>
    /// 更新客户信息
    /// </summary>
    /// <param name="id">要更新的客户ID</param>
    /// <param name="clientDto">新的客户信息DTO</param>
    /// <param name="userId">更新人ID</param>
    /// <returns>更新后的客户信息</returns>
    Task<ApiResponse<ClientDto>> UpdateClientAsync(int id, ClientDto clientDto, string userId);

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">要删除的客户ID</param>
    /// <param name="userId">删除人ID</param>
    /// <returns>删除操作结果</returns>
    Task<ApiResponse> DeleteClientAsync(int id, string userId);

    /// <summary>
    /// 获取客户需求信息
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <returns>客户需求详细信息</returns>
    Task<ApiResponse<ClientRequirementDto>> GetClientRequirementsAsync(int clientId);

    /// <summary>
    /// 更新或创建客户需求信息
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <param name="requirementDto">客户需求信息DTO</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>更新后的客户需求信息</returns>
    Task<ApiResponse<ClientRequirementDto>> UpdateClientRequirementsAsync(int clientId, ClientRequirementDto requirementDto, string userId);

    /// <summary>
    /// 获取客户统计数据
    /// </summary>
    /// <returns>客户相关的统计信息</returns>
    Task<ApiResponse<ClientStatsDto>> GetClientStatsAsync();
} 