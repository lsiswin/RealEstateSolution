using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Repository;

/// <summary>
/// 合同仓储接口
/// </summary>
public interface IContractRepository : IGenericRepository<Contract>
{
    /// <summary>
    /// 根据条件查询合同
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
    Task UpdateStatusAsync(int contractId, ContractStatus status);

    /// <summary>
    /// 获取客户的合同列表
    /// </summary>
    Task<IEnumerable<Contract>> GetClientContractsAsync(int clientId);

    /// <summary>
    /// 获取房产的合同列表
    /// </summary>
    Task<IEnumerable<Contract>> GetPropertyContractsAsync(int propertyId);

    /// <summary>
    /// 生成合同编号
    /// </summary>
    Task<string> GenerateContractNumberAsync();
} 