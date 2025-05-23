using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace RealEstateSolution.Common.Repository;

/// <summary>
/// 工作单元实现类
/// </summary>
/// <typeparam name="TContext">数据库上下文类型</typeparam>
public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;
    private readonly ConcurrentDictionary<Type, object> _repositories;

    public UnitOfWork(TContext context)
    {
        _context = context;
        _repositories = new ConcurrentDictionary<Type, object>();
    }

    /// <summary>
    /// 获取数据库上下文
    /// </summary>
    public TContext Context => _context;

    /// <summary>
    /// 获取指定类型的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储实例</returns>
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        return _repositories.GetOrAdd(typeof(TEntity), _ => new GenericRepository<TEntity>(_context)) as IGenericRepository<TEntity>;
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <returns>受影响的行数</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            foreach (var repository in _repositories.Values)
            {
                if (repository is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        _disposed = true;
    }
} 