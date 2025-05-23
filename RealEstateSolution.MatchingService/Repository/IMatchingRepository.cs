using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.MatchingService.Repository;

/// <summary>
/// 匹配仓储接口
/// </summary>
public interface IMatchingRepository : IGenericRepository<Matching>
{
    /// <summary>
    /// 根据条件查询匹配
    /// </summary>
    Task<IEnumerable<Matching>> SearchMatchingsAsync(
        string? keyword,
        MatchingType? type,
        MatchingStatus? status,
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 更新匹配状态
    /// </summary>
    Task UpdateStatusAsync(int matchingId, MatchingStatus status);

    /// <summary>
    /// 获取客户的匹配列表
    /// </summary>
    Task<IEnumerable<Matching>> GetClientMatchingsAsync(int clientId);

    /// <summary>
    /// 获取房产的匹配列表
    /// </summary>
    Task<IEnumerable<Matching>> GetPropertyMatchingsAsync(int propertyId);
} 