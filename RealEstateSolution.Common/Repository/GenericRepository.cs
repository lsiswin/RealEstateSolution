using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RealEstateSolution.Common.Repository;

/// <summary>
/// 泛型仓储实现类
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// 获取查询对象
    /// </summary>
    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// 根据条件查询实体
    /// </summary>
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// 添加实体
    /// </summary>
    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// 批量添加实体
    /// </summary>
    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// 批量删除实体
    /// </summary>
    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    /// <summary>
    /// 判断是否存在满足条件的实体
    /// </summary>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    /// <summary>
    /// 获取满足条件的实体数量
    /// </summary>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }
} 