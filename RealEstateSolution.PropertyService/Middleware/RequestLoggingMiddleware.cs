using System.Diagnostics;

namespace RealEstateSolution.PropertyService.Middleware;

/// <summary>
/// 请求日志中间件
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;
            var method = context.Request.Method;
            var path = context.Request.Path;
            var query = context.Request.QueryString;

            _logger.LogInformation(
                "请求 {Method} {Path}{Query} 完成，状态码: {StatusCode}，耗时: {Elapsed}ms",
                method, path, query, statusCode, elapsed);
        }
    }
} 