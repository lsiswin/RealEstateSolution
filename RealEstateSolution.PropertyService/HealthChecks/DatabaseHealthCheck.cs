using Microsoft.Extensions.Diagnostics.HealthChecks;
using RealEstateSolution.PropertyService.Data;

namespace RealEstateSolution.PropertyService.HealthChecks;

/// <summary>
/// 数据库健康检查
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly PropertyDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(PropertyDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // 尝试执行简单查询
            await _context.Database.CanConnectAsync(cancellationToken);

            return HealthCheckResult.Healthy("数据库连接正常");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据库健康检查失败");
            return HealthCheckResult.Unhealthy("数据库连接异常", ex);
        }
    }
} 