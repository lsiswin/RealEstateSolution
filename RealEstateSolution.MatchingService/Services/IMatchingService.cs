using RealEstateSolution.Database.Models;

namespace RealEstateSolution.MatchingService.Services;

/// <summary>
/// 匹配服务接口
/// </summary>
public interface IMatchingService
{
    /// <summary>
    /// 创建匹配
    /// </summary>
    Task<Matching> CreateMatchingAsync(Matching matching);

    /// <summary>
    /// 更新匹配状态
    /// </summary>
    Task UpdateMatchingStatusAsync(int matchingId, MatchingStatus status);

    /// <summary>
    /// 获取匹配详情
    /// </summary>
    Task<Matching?> GetMatchingByIdAsync(int id);

    /// <summary>
    /// 搜索匹配
    /// </summary>
    Task<IEnumerable<Matching>> SearchMatchingsAsync(
        string? keyword,
        MatchingType? type,
        MatchingStatus? status,
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 获取客户匹配列表
    /// </summary>
    Task<IEnumerable<Matching>> GetClientMatchingsAsync(int clientId);

    /// <summary>
    /// 获取房产匹配列表
    /// </summary>
    Task<IEnumerable<Matching>> GetPropertyMatchingsAsync(int propertyId);

    /// <summary>
    /// 删除匹配
    /// </summary>
    Task DeleteMatchingAsync(int id);

    /// <summary>
    /// 自动匹配
    /// </summary>
    Task<IEnumerable<Matching>> AutoMatchAsync(int clientId);

    /// <summary>
    /// 手动匹配
    /// </summary>
    Task<Matching> ManualMatchAsync(int clientId, int propertyId);

    /// <summary>
    /// 计算匹配分数
    /// </summary>
    Task<double> CalculateMatchingScoreAsync(Client client, Property property);
} 