using RealEstateSolution.Database.Models;
namespace RealEstateSolution.ContractService.Services;

/// <summary>
/// 合同服务接口
/// </summary>
public interface IContractService
{
    /// <summary>
    /// 创建合同
    /// </summary>
    Task<Contract> CreateContractAsync(Contract contract);

    /// <summary>
    /// 更新合同
    /// </summary>
    Task<Contract> UpdateContractAsync(Contract contract);

    /// <summary>
    /// 获取合同详情
    /// </summary>
    Task<Contract?> GetContractByIdAsync(int id);

    /// <summary>
    /// 搜索合同
    /// </summary>
    Task<IEnumerable<Contract>> SearchContractsAsync(
        string? keyword,
        ContractType? type,
        ContractStatus? status,
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 更新合同状态
    /// </summary>
    Task UpdateContractStatusAsync(int contractId, ContractStatus status);

    /// <summary>
    /// 获取客户合同列表
    /// </summary>
    Task<IEnumerable<Contract>> GetClientContractsAsync(int clientId);

    /// <summary>
    /// 获取房产合同列表
    /// </summary>
    Task<IEnumerable<Contract>> GetPropertyContractsAsync(int propertyId);

    /// <summary>
    /// 删除合同
    /// </summary>
    Task DeleteContractAsync(int id);
} 