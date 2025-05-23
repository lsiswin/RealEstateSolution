using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.ClientService.Services;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Controllers;

/// <summary>
/// 客户控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// 创建客户
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] Client client)
    {
        var result = await _clientService.CreateClientAsync(client);
        return Ok(new { message = "客户创建成功", data = result });
    }

    /// <summary>
    /// 更新客户信息
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
    {
        if (id != client.Id)
        {
            return BadRequest(new { message = "ID不匹配" });
        }

        try
        {
            var result = await _clientService.UpdateClientAsync(client);
            return Ok(new { message = "客户信息更新成功", data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        if (client == null)
        {
            return NotFound(new { message = $"未找到ID为{id}的客户" });
        }

        return Ok(new { data = client });
    }

    /// <summary>
    /// 搜索客户
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchClients(
        [FromQuery] string? keyword,
        [FromQuery] ClientType? type,
        [FromQuery] ClientStatus? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var clients = await _clientService.SearchClientsAsync(keyword, type, status, startDate, endDate);
        return Ok(new { data = clients });
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            await _clientService.DeleteClientAsync(id);
            return Ok(new { message = "客户删除成功" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 获取客户购房需求
    /// </summary>
    [HttpGet("{clientId}/requirements")]
    public async Task<IActionResult> GetClientRequirements(int clientId)
    {
        var requirements = await _clientService.GetClientRequirementsAsync(clientId);
        if (requirements == null)
        {
            return NotFound(new { message = $"未找到ID为{clientId}的客户的购房需求" });
        }

        return Ok(new { data = requirements });
    }

    /// <summary>
    /// 更新客户购房需求
    /// </summary>
    [HttpPut("{clientId}/requirements")]
    public async Task<IActionResult> UpdateClientRequirements(int clientId, [FromBody] ClientRequirements requirements)
    {
        if (clientId != requirements.ClientId)
        {
            return BadRequest(new { message = "客户ID不匹配" });
        }

        var result = await _clientService.UpdateClientRequirementsAsync(requirements);
        return Ok(new { message = "客户购房需求更新成功", data = result });
    }

    /// <summary>
    /// 更新客户状态
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateClientStatus(int id, [FromBody] ClientStatus status)
    {
        await _clientService.UpdateClientStatusAsync(id, status);
        return Ok(new { message = "客户状态更新成功" });
    }
}