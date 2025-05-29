using RealEstateSolution.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.ContractService.Models;

/// <summary>
/// 合同模板DTO
/// </summary>
public class ContractTemplateDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "模板名称不能为空")]
    [StringLength(100, ErrorMessage = "模板名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "模板描述长度不能超过500个字符")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "模板类型不能为空")]
    public ContractType Type { get; set; }
    
    [Required(ErrorMessage = "模板内容不能为空")]
    public string Content { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
} 