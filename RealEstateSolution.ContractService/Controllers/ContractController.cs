using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.Common.Models;
using RealEstateSolution.Database.Models;
using RealEstateSolution.ContractService.Services;

namespace RealEstateSolution.ContractService.Controllers;

/// <summary>
/// 合同服务控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractController(IContractService contractService)
    {
        _contractService = contractService;
    }

    /// <summary>
    /// 创建合同
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<Contract>> CreateContract([FromBody] Contract contract)
    {
        var result = await _contractService.CreateContractAsync(contract);
        return new ApiResponse<Contract> { Data = result };
    }

    /// <summary>
    /// 更新合同
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateContract(int id, [FromBody] Contract contract)
    {
        if (id != contract.Id)
        {
            return new ApiResponse { Message = "ID不匹配" };
        }
        await _contractService.UpdateContractAsync(contract);
        return new ApiResponse { Message = "合同已更新" };
    }

    /// <summary>
    /// 获取合同详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<Contract>> GetContract(int id)
    {
        var contract = await _contractService.GetContractByIdAsync(id);
        if (contract == null)
        {
            return new ApiResponse<Contract> { Message = "未找到合同" };
        }
        return new ApiResponse<Contract> { Data = contract };
    }

    /// <summary>
    /// 搜索合同
    /// </summary>
    [HttpGet("search")]
    public async Task<ApiResponse<IEnumerable<Contract>>> SearchContracts(
        [FromQuery] string? keyword,
        [FromQuery] ContractType? type,
        [FromQuery] ContractStatus? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var contracts = await _contractService.SearchContractsAsync(
            keyword,
            type,
            status,
            startDate,
            endDate);
        return new ApiResponse<IEnumerable<Contract>> { Data = contracts };
    }

    /// <summary>
    /// 删除合同
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteContract(int id)
    {
        await _contractService.DeleteContractAsync(id);
        return new ApiResponse { Message = "合同已删除" };
    }

    /// <summary>
    /// 更新合同状态
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse> UpdateContractStatus(int id, [FromQuery] ContractStatus status)
    {
        await _contractService.UpdateContractStatusAsync(id, status);
        return new ApiResponse { Message = "合同状态已更新" };
    }

    /// <summary>
    /// 获取客户的所有合同
    /// </summary>
    [HttpGet("client/{clientId}")]
    public async Task<ApiResponse<IEnumerable<Contract>>> GetClientContracts(int clientId)
    {
        var contracts = await _contractService.GetClientContractsAsync(clientId);
        return new ApiResponse<IEnumerable<Contract>> { Data = contracts };
    }

    /// <summary>
    /// 获取房源的所有合同
    /// </summary>
    [HttpGet("property/{propertyId}")]
    public async Task<ApiResponse<IEnumerable<Contract>>> GetPropertyContracts(int propertyId)
    {
        var contracts = await _contractService.GetPropertyContractsAsync(propertyId);
        return new ApiResponse<IEnumerable<Contract>> { Data = contracts };
    }
}