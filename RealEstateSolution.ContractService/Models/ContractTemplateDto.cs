using RealEstateSolution.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.ContractService.Models;

/// <summary>
/// 合同模板DTO
/// </summary>
public class ContractTemplateDto
{
    /// <summary>
    /// 模板ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 模板名称
    /// </summary>
    [Required(ErrorMessage = "模板名称不能为空")]
    [StringLength(100, ErrorMessage = "模板名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 模板描述
    /// </summary>
    [StringLength(500, ErrorMessage = "模板描述长度不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 模板类型
    /// </summary>
    [Required(ErrorMessage = "模板类型不能为空")]
    public ContractType Type { get; set; }

    /// <summary>
    /// 模板内容
    /// </summary>
    [Required(ErrorMessage = "模板内容不能为空")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 合同模板查询DTO
/// </summary>
public class ContractTemplateQueryDto
{
    /// <summary>
    /// 模板名称关键字
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 模板类型
    /// </summary>
    public ContractType? Type { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// 合同模板创建/更新DTO
/// </summary>
public class ContractTemplateCreateDto
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 模板描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 模板类型
    /// </summary>
    public ContractType Type { get; set; }

    /// <summary>
    /// 模板内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;
} 