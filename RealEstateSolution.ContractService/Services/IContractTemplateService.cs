using RealEstateSolution.Common.Utils;
using RealEstateSolution.ContractService.Models;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Services;

/// <summary>
/// 合同模板服务接口
/// </summary>
public interface IContractTemplateService
{
    /// <summary>
    /// 获取模板列表
    /// </summary>
    Task<ApiResponse<PagedList<ContractTemplateDto>>> GetTemplatesAsync(ContractTemplateQueryDto query);

    /// <summary>
    /// 根据ID获取模板详情
    /// </summary>
    Task<ApiResponse<ContractTemplateDto>> GetTemplateByIdAsync(int id);

    /// <summary>
    /// 根据类型获取启用的模板
    /// </summary>
    Task<ApiResponse<List<ContractTemplateDto>>> GetActiveTemplatesByTypeAsync(ContractType? type = null);

    /// <summary>
    /// 创建模板
    /// </summary>
    Task<ApiResponse<ContractTemplateDto>> CreateTemplateAsync(ContractTemplateCreateDto templateDto, string userId);

    /// <summary>
    /// 更新模板
    /// </summary>
    Task<ApiResponse<ContractTemplateDto>> UpdateTemplateAsync(int id, ContractTemplateCreateDto templateDto, string userId);

    /// <summary>
    /// 删除模板
    /// </summary>
    Task<ApiResponse> DeleteTemplateAsync(int id, string userId);

    /// <summary>
    /// 更新模板状态
    /// </summary>
    Task<ApiResponse> UpdateTemplateStatusAsync(int id, bool isActive, string userId);

    /// <summary>
    /// 从Word文档导入模板
    /// </summary>
    Task<ApiResponse<ContractTemplateDto>> ImportFromWordAsync(Stream fileStream, string fileName, ContractType type, string userId);

    /// <summary>
    /// 导出模板为Word文档
    /// </summary>
    Task<ApiResponse<byte[]>> ExportToWordAsync(int id);

    /// <summary>
    /// 获取模板统计信息
    /// </summary>
    Task<ApiResponse<object>> GetTemplateStatsAsync();
} 