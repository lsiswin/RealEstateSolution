using Microsoft.EntityFrameworkCore;

namespace RealEstateSolution.Common.Repository;

/// <summary>
/// 工作单元接口
/// </summary>
/// <typeparam name="TContext">数据库上下文类型</typeparam>
public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
{
    /// <summary>
    /// 获取数据库上下文
    /// </summary>
    TContext Context { get; }

    /// <summary>
    /// 获取指定类型的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储实例</returns>
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

    /// <summary>
    /// 开始事务
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// 回滚事务
    /// </summary>
    Task RollbackAsync();

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <returns>受影响的行数</returns>
    Task<int> SaveChangesAsync();
} 