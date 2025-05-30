using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.ContractService.Models;
using RealEstateSolution.ContractService.Services;
using RealEstateSolution.Database.Models;
using RealEstateSolution.Common.Utils;
using System.Security.Claims;

namespace RealEstateSolution.ContractService.Controllers;

/// <summary>
/// 合同模板控制器
/// </summary>
[ApiController]
[Route("api/contract/[action]")]
[Authorize(Roles = "admin,broker")]
public class ContractTemplateController : ControllerBase
{
    private readonly IContractTemplateService _templateService;

    public ContractTemplateController(IContractTemplateService templateService)
    {
        _templateService = templateService;
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
    /// 获取模板列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedList<ContractTemplateDto>>> GetTemplates([FromQuery] ContractTemplateQueryDto query)
    {
        return await _templateService.GetTemplatesAsync(query);
    }

    /// <summary>
    /// 根据ID获取模板详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ContractTemplateDto>> GetTemplateById(int id)
    {
        return await _templateService.GetTemplateByIdAsync(id);
    }

    /// <summary>
    /// 根据类型获取启用的模板
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<List<ContractTemplateDto>>> GetActiveTemplates([FromQuery] ContractType? type = null)
    {
        return await _templateService.GetActiveTemplatesByTypeAsync(type);
    }

    /// <summary>
    /// 创建模板
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<ContractTemplateDto>> CreateTemplate([FromBody] ContractTemplateCreateDto templateDto)
    {
        var userId = GetCurrentUserId();
        return await _templateService.CreateTemplateAsync(templateDto, userId);
    }

    /// <summary>
    /// 更新模板
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<ContractTemplateDto>> UpdateTemplate(int id, [FromBody] ContractTemplateCreateDto templateDto)
    {
        var userId = GetCurrentUserId();
        return await _templateService.UpdateTemplateAsync(id, templateDto, userId);
    }

    /// <summary>
    /// 删除模板
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteTemplate(int id)
    {
        var userId = GetCurrentUserId();
        return await _templateService.DeleteTemplateAsync(id, userId);
    }

    /// <summary>
    /// 更新模板状态
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ApiResponse> UpdateTemplateStatus(int id, [FromBody] bool isActive)
    {
        var userId = GetCurrentUserId();
        return await _templateService.UpdateTemplateStatusAsync(id, isActive, userId);
    }

    /// <summary>
    /// 从Word文档导入模板
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<ContractTemplateDto>> ImportFromWord(IFormFile file, [FromForm] ContractType type)
    {
        if (file == null || file.Length == 0)
        {
            return new ApiResponse<ContractTemplateDto>
            {
                Success = false,
                Message = "请选择要上传的文件"
            };
        }

        if (!file.FileName.EndsWith(".doc") && !file.FileName.EndsWith(".docx"))
        {
            return new ApiResponse<ContractTemplateDto>
            {
                Success = false,
                Message = "只支持Word文档格式(.doc, .docx)"
            };
        }

        var userId = GetCurrentUserId();
        
        using var stream = file.OpenReadStream();
        return await _templateService.ImportFromWordAsync(stream, file.FileName, type, userId);
    }

    /// <summary>
    /// 导出模板为Word文档
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> ExportToWord(int id)
    {
        try
        {
            var result = await _templateService.ExportToWordAsync(id);
            
            if (!result.Success || result.Data == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = result.Message
                });
            }

            // 获取模板信息用于文件名
            var templateResult = await _templateService.GetTemplateByIdAsync(id);
            var fileName = templateResult.Success && templateResult.Data != null 
                ? $"{templateResult.Data.Name}.docx" 
                : $"合同模板_{id}.docx";

            return File(
                result.Data, 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"导出失败: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 获取模板统计信息
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<object>> GetTemplateStats()
    {
        return await _templateService.GetTemplateStatsAsync();
    }
} 