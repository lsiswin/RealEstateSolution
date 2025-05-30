using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Repository;

/// <summary>
/// 合同模板仓储接口
/// </summary>
public interface IContractTemplateRepository : IGenericRepository<ContractTemplate>
{
    /// <summary>
    /// 根据条件查询模板
    /// </summary>
    Task<IEnumerable<ContractTemplate>> SearchTemplatesAsync(
        string? name,
        ContractType? type,
        bool? isActive);

    /// <summary>
    /// 根据类型获取启用的模板
    /// </summary>
    Task<IEnumerable<ContractTemplate>> GetActiveTemplatesByTypeAsync(ContractType type);

    /// <summary>
    /// 检查模板名称是否存在
    /// </summary>
    Task<bool> IsNameExistsAsync(string name, int? excludeId = null);

    /// <summary>
    /// 更新模板状态
    /// </summary>
    Task UpdateStatusAsync(int templateId, bool isActive);

    /// <summary>
    /// 删除模板
    /// </summary>
    Task DeleteTemplateAsync(int templateId);

    /// <summary>
    /// 获取模板统计信息
    /// </summary>
    Task<(int Total, int Active, int Inactive)> GetTemplateStatsAsync();
} 