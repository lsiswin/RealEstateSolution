using RealEstateSolution.Common.Utils;
using RealEstateSolution.ContractService.Models;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Services;

/// <summary>
/// 合同服务接口
/// </summary>
public interface IContractService
{
    /// <summary>
    /// 获取合同列表
    /// </summary>
    Task<ApiResponse<PagedList<ContractDto>>> GetContractsAsync(ContractQueryDto query);

    /// <summary>
    /// 根据ID获取合同详情
    /// </summary>
    Task<ApiResponse<ContractDto>> GetContractByIdAsync(int id);

    /// <summary>
    /// 创建合同
    /// </summary>
    Task<ApiResponse<ContractDto>> CreateContractAsync(ContractDto contractDto, string userId);

    /// <summary>
    /// 更新合同
    /// </summary>
    Task<ApiResponse<ContractDto>> UpdateContractAsync(int id, ContractDto contractDto, string userId);

    /// <summary>
    /// 删除合同
    /// </summary>
    Task<ApiResponse> DeleteContractAsync(int id, string userId);

    /// <summary>
    /// 更新合同状态
    /// </summary>
    Task<ApiResponse<ContractDto>> UpdateContractStatusAsync(int id, ContractStatus status, string userId);

    /// <summary>
    /// 获取合同模板列表
    /// </summary>
    Task<ApiResponse<List<ContractTemplateDto>>> GetContractTemplatesAsync(ContractType? type = null);

    /// <summary>
    /// 根据模板创建合同
    /// </summary>
    Task<ApiResponse<ContractDto>> CreateContractFromTemplateAsync(int templateId, ContractDto contractDto, string userId);

    /// <summary>
    /// 生成合同编号
    /// </summary>
    Task<string> GenerateContractNumberAsync(ContractType type);

    /// <summary>
    /// 获取合同统计
    /// </summary>
    Task<ApiResponse<ContractStatsDto>> GetContractStatsAsync();
} 