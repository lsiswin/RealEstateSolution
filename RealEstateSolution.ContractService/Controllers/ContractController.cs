using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.ContractService.Models;
using RealEstateSolution.ContractService.Services;
using RealEstateSolution.Common.Utils;
using System.Security.Claims;
using DbModels = RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Controllers;

/// <summary>
/// 合同管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class ContractController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractController(IContractService contractService)
    {
        _contractService = contractService;
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    private string GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("无效的用户身份");
        }
        return userId;
    }

    /// <summary>
    /// 获取合同列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedList<ContractDto>>> GetContracts([FromQuery] ContractQueryDto query)
    {
        return await _contractService.GetContractsAsync(query);
    }

    /// <summary>
    /// 获取合同详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ContractDto>> GetContract(int id)
    {
        return await _contractService.GetContractByIdAsync(id);
    }

    /// <summary>
    /// 创建合同
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<ContractDto>> CreateContract([FromBody] ContractDto contractDto)
    {
        var userId = GetCurrentUserId();
        return await _contractService.CreateContractAsync(contractDto, userId);
    }

    /// <summary>
    /// 更新合同
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<ContractDto>> UpdateContract(int id, [FromBody] ContractDto contractDto)
    {
        var userId = GetCurrentUserId();
        return await _contractService.UpdateContractAsync(id, contractDto, userId);
    }

    /// <summary>
    /// 删除合同
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteContract(int id)
    {
        var userId = GetCurrentUserId();
        return await _contractService.DeleteContractAsync(id, userId);
    }

    /// <summary>
    /// 更新合同状态
    /// </summary>
    [HttpPost("{id}/status")]
    public async Task<ApiResponse<ContractDto>> UpdateContractStatus(int id, [FromBody] DbModels.ContractStatus status)
    {
        var userId = GetCurrentUserId();
        return await _contractService.UpdateContractStatusAsync(id, status, userId);
    }

    /// <summary>
    /// 获取合同模板列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<List<ContractTemplateDto>>> GetContractTemplates([FromQuery] DbModels.ContractType? type = null)
    {
        return await _contractService.GetContractTemplatesAsync(type);
    }

    /// <summary>
    /// 根据模板创建合同
    /// </summary>
    [HttpPost("{templateId}")]
    public async Task<ApiResponse<ContractDto>> CreateContractFromTemplate(int templateId, [FromBody] ContractDto contractDto)
    {
        var userId = GetCurrentUserId();
        return await _contractService.CreateContractFromTemplateAsync(templateId, contractDto, userId);
    }

    /// <summary>
    /// 生成合同编号
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<string>> GenerateContractNumber([FromQuery] DbModels.ContractType type)
    {
        var contractNumber = await _contractService.GenerateContractNumberAsync(type);
        return new ApiResponse<string> { Success = true, Data = contractNumber };
    }

    /// <summary>
    /// 获取合同统计
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<ContractStatsDto>> GetContractStats()
    {
        return await _contractService.GetContractStatsAsync();
    }
}