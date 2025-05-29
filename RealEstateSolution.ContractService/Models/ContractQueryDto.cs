using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Models;

/// <summary>
/// 合同查询DTO
/// </summary>
public class ContractQueryDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 合同类型
    /// </summary>
    public ContractType? Type { get; set; }

    /// <summary>
    /// 合同状态
    /// </summary>
    public ContractStatus? Status { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 甲方ID
    /// </summary>
    public int? PartyAId { get; set; }

    /// <summary>
    /// 乙方ID
    /// </summary>
    public int? PartyBId { get; set; }

    /// <summary>
    /// 房源ID
    /// </summary>
    public int? PropertyId { get; set; }
} 