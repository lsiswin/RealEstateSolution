using System.Net;
using Ocelot.Logging;
using System.IdentityModel.Tokens.Jwt;
using RealEstateSolution.Common.Redis;
using System.Security.Claims;

public class JwtRevocationCheckHandler : DelegatingHandler
{
    private readonly IOcelotLogger _logger;
    private readonly IRedisService _redisService;

    public JwtRevocationCheckHandler(IRedisService redisService,
        IOcelotLoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<JwtRevocationCheckHandler>();
        _redisService = redisService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("=== JwtRevocationCheckHandler 开始处理请求 ===");
        _logger.LogInformation($"请求URL: {request.RequestUri}");
        _logger.LogInformation($"请求方法: {request.Method}");

        // 记录所有请求头
        foreach (var header in request.Headers)
        {
            _logger.LogInformation($"请求头: {header.Key} = {string.Join("",header.Value)}" );
        }

        // 获取JWT
        if (!request.Headers.Authorization?.Scheme.Equals("Bearer") ?? true)
        {
            _logger.LogInformation("未检测到Bearer认证头部，跳过JWT验证");
            return await base.SendAsync(request, cancellationToken);
        }

        var token = request.Headers.Authorization!.Parameter;
        _logger.LogInformation($"检测到JWT令牌，长度: {token?.Length ?? 0}");
        
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            _logger.LogWarning("无效的令牌格式");
            return CreateUnauthorizedResponse("Invalid token format");
        }

        try
        {
            var jwt = handler.ReadJwtToken(token);
            _logger.LogInformation($"JWT令牌解析成功，Claims数量: {jwt.Claims.Count()}");
            
            // 记录所有Claims
            foreach (var claim in jwt.Claims)
            {
                _logger.LogInformation($"JWT Claim: {claim.Type} = {claim.Value}");
            }
            
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            _logger.LogInformation($"JWT ID (jti): {jti}");

            // 检查黑名单
            if (!string.IsNullOrEmpty(jti))
            {
                var isRevoked = await _redisService.IsTokenBlacklistedAsync(jti);
                _logger.LogInformation($"令牌撤销检查结果: {isRevoked}");

                if (isRevoked)
                {
                    _logger.LogWarning($"阻止已撤销的令牌: {jti}");
                    return CreateUnauthorizedResponse("Token revoked");
                }
            }

            // 提取用户信息并添加到请求头中（这些信息将被Ocelot自动添加到下游请求）
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
            var userName = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value;
            var roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value);

            _logger.LogInformation($"JWT验证成功，用户ID: {userId}, 用户名: {userName}, 角色: {string.Join(",", roles)}");

            // 注意：这里不需要手动添加头部，因为Ocelot会根据配置自动添加
            // AddHeadersToRequest 配置会自动处理 Claims 到头部的映射
            
            _logger.LogInformation("=== JwtRevocationCheckHandler 处理完成，准备转发请求 ===");
        }
        catch (Exception ex)
        {
            _logger.LogError($"JWT处理过程中发生错误: {ex.Message}", ex);
            return CreateUnauthorizedResponse("Token processing error");
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private HttpResponseMessage CreateUnauthorizedResponse(string message)
    {
        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent($"{{\"error\": \"{message}\", \"timestamp\": \"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}\"}}")
            {
                Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json") }
            }
        };
    }
}