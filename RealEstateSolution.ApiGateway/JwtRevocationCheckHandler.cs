
using System.Net;
using Ocelot.Logging;
using System.IdentityModel.Tokens.Jwt;
using RealEstateSolution.Common.Redis;

public class JwtRevocationCheckHandler : DelegatingHandler
{
    private readonly IOcelotLogger _logger;
    private readonly IRedisService redisService;

    public JwtRevocationCheckHandler(IRedisService redisService,
        IOcelotLoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<JwtRevocationCheckHandler>();
        this.redisService = redisService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 获取JWT
        if (!request.Headers.Authorization?.Scheme.Equals("Bearer") ?? true)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var token = request.Headers.Authorization!.Parameter;
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            return CreateUnauthorizedResponse("Invalid token format");
        }

        var jwt = handler.ReadJwtToken(token);
        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        // 检查黑名单
        if (!string.IsNullOrEmpty(jti))
        {
            
            var isRevoked = await redisService.IsTokenBlacklistedAsync(jti);

            if (isRevoked)
            {
                _logger.LogWarning($"Blocked revoked token: {jti}");
                return CreateUnauthorizedResponse("Token revoked");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private HttpResponseMessage CreateUnauthorizedResponse(string message)
    {
        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(message)
        };
    }
}