using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RealEstateSolution.Common.Utils
{
    /// <summary>
    /// 用户上下文中间件 - 处理从ApiGateway传递的用户信息
    /// </summary>
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserContextMiddleware> _logger;

        public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("=== UserContextMiddleware 开始处理请求 ===");
                _logger.LogInformation("请求路径: {Path}", context.Request.Path);
                _logger.LogInformation("请求方法: {Method}", context.Request.Method);

                // 记录所有请求头
                foreach (var header in context.Request.Headers)
                {
                    _logger.LogInformation("请求头: {Key} = {Value}", header.Key, string.Join(", ", header.Value));
                }

                // 从ApiGateway传递的头部信息中提取用户信息
                if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
                {
                    var userId = userIdHeader.FirstOrDefault();
                    _logger.LogInformation("从ApiGateway接收到用户ID: {UserId}", userId);

                    if (!string.IsNullOrEmpty(userId))
                    {
                        // 创建用户身份信息
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId)
                        };

                        // 添加用户名
                        if (context.Request.Headers.TryGetValue("X-User-Name", out var userNameHeader))
                        {
                            var userName = userNameHeader.FirstOrDefault();
                            _logger.LogInformation("从ApiGateway接收到用户名: {UserName}", userName);
                            if (!string.IsNullOrEmpty(userName))
                            {
                                claims.Add(new Claim(ClaimTypes.Name, userName));
                            }
                        }

                        // 添加角色信息
                        if (context.Request.Headers.TryGetValue("X-User-Roles", out var rolesHeader))
                        {
                            var roles = rolesHeader.FirstOrDefault();
                            _logger.LogInformation("从ApiGateway接收到角色: {Roles}", roles);
                            if (!string.IsNullOrEmpty(roles))
                            {
                                // 角色可能是逗号分隔的字符串
                                var roleList = roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                foreach (var role in roleList)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                                }
                            }
                        }

                        // 创建身份和主体
                        var identity = new ClaimsIdentity(claims, "Gateway");
                        var principal = new ClaimsPrincipal(identity);

                        // 设置用户上下文
                        context.User = principal;

                        _logger.LogInformation("从ApiGateway设置用户上下文成功: UserId={UserId}, UserName={UserName}, Claims={ClaimsCount}",
                            userId,
                            claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                            claims.Count);
                    }
                }
                else
                {
                    _logger.LogInformation("未从ApiGateway接收到X-User-Id头部");

                    // 如果没有从ApiGateway传递用户信息，尝试从JWT中提取
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                    {
                        _logger.LogInformation("检测到Authorization头部，将依赖JWT认证");
                        var token = authHeader.Substring("Bearer ".Length).Trim();

                        // 尝试解析JWT令牌以获取调试信息
                        try
                        {
                            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                            if (handler.CanReadToken(token))
                            {
                                var jwt = handler.ReadJwtToken(token);
                                _logger.LogInformation("JWT令牌解析成功，Claims数量: {ClaimsCount}", jwt.Claims.Count());

                                foreach (var claim in jwt.Claims)
                                {
                                    _logger.LogInformation("JWT Claim: {Type} = {Value}", claim.Type, claim.Value);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("无法读取JWT令牌");
                            }
                        }
                        catch (Exception jwtEx)
                        {
                            _logger.LogError(jwtEx, "解析JWT令牌时发生错误");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("未检测到Authorization头部");
                    }
                }

                _logger.LogInformation("=== UserContextMiddleware 处理完成 ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理用户上下文时发生错误");
            }

            await _next(context);
        }
    }

    /// <summary>
    /// 用户上下文中间件扩展方法
    /// </summary>
    public static class UserContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserContext(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserContextMiddleware>();
        }
    }
}