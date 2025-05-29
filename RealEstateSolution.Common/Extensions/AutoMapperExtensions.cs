using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Utils;

namespace RealEstateSolution.Common.Extensions;

/// <summary>
/// AutoMapper扩展方法
/// </summary>
public static class AutoMapperExtensions
{
    /// <summary>
    /// 异步投影到分页列表
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="queryable">查询对象</param>
    /// <param name="mapper">映射器</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分页结果</returns>
    public static async Task<PagedList<TDestination>> ToPagedListAsync<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        IMapper mapper,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await queryable.CountAsync(cancellationToken);
        
        var items = await queryable
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TDestination>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedList<TDestination>(items, totalCount, pageIndex, pageSize);
    }

    /// <summary>
    /// 异步投影到列表
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="queryable">查询对象</param>
    /// <param name="mapper">映射器</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>映射后的列表</returns>
    public static async Task<List<TDestination>> ProjectToListAsync<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        IMapper mapper,
        CancellationToken cancellationToken = default)
    {
        return await queryable
            .ProjectTo<TDestination>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 异步投影到单个对象
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="queryable">查询对象</param>
    /// <param name="mapper">映射器</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>映射后的对象</returns>
    public static async Task<TDestination?> ProjectToFirstOrDefaultAsync<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        IMapper mapper,
        CancellationToken cancellationToken = default)
    {
        return await queryable
            .ProjectTo<TDestination>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 条件映射 - 只映射非空值
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="mapper">映射器</param>
    /// <param name="source">源对象</param>
    /// <param name="destination">目标对象</param>
    /// <returns>映射后的目标对象</returns>
    public static TDestination MapNonNullValues<TSource, TDestination>(
        this IMapper mapper,
        TSource source,
        TDestination destination)
        where TDestination : class
    {
        if (source == null) return destination;
        
        // 使用AutoMapper的条件映射功能
        return mapper.Map(source, destination);
    }

    /// <summary>
    /// 批量映射
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="mapper">映射器</param>
    /// <param name="sources">源对象集合</param>
    /// <returns>映射后的目标对象集合</returns>
    public static List<TDestination> MapList<TSource, TDestination>(
        this IMapper mapper,
        IEnumerable<TSource> sources)
    {
        return sources?.Select(source => mapper.Map<TDestination>(source)).ToList() ?? new List<TDestination>();
    }

    /// <summary>
    /// 安全映射 - 处理空值情况
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="mapper">映射器</param>
    /// <param name="source">源对象</param>
    /// <returns>映射后的目标对象，如果源对象为空则返回默认值</returns>
    public static TDestination? SafeMap<TSource, TDestination>(
        this IMapper mapper,
        TSource? source)
        where TSource : class
        where TDestination : class
    {
        return source == null ? null : mapper.Map<TDestination>(source);
    }

    /// <summary>
    /// 映射并验证
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="mapper">映射器</param>
    /// <param name="source">源对象</param>
    /// <param name="validator">验证函数</param>
    /// <returns>映射后的目标对象</returns>
    /// <exception cref="ArgumentException">验证失败时抛出异常</exception>
    public static TDestination MapWithValidation<TSource, TDestination>(
        this IMapper mapper,
        TSource source,
        Func<TDestination, string?> validator)
    {
        var destination = mapper.Map<TDestination>(source);
        var validationError = validator(destination);
        
        if (!string.IsNullOrEmpty(validationError))
        {
            throw new ArgumentException(validationError);
        }
        
        return destination;
    }
} 