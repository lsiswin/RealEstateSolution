using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RealEstateSolution.Common.Repository;

/// <summary>
/// 泛型仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// 获取查询对象
    /// </summary>
    /// <returns>IQueryable对象</returns>
    IQueryable<TEntity> Query();

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>实体对象</returns>
    Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>实体集合</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// 根据条件查询实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体集合</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// 批量添加实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    void Update(TEntity entity);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    void Delete(TEntity entity);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体集合</param>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// 判断是否存在满足条件的实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>是否存在</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 获取满足条件的实体数量
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>实体数量</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
} 