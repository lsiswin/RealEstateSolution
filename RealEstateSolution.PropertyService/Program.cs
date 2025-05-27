using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Common.Redis;
using RealEstateSolution.PropertyService.Data;
using RealEstateSolution.PropertyService.Services;
using Serilog;
using StackExchange.Redis;
using RealEstateSolution.PropertyService.Extension.Middleware;
using RealEstateSolution.PropertyService.Extension.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RealEstateSolution.Common.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RealEstateSolution.PropertyService.Extension.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置数据库
builder.Services.AddDbContext<PropertyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册工作单元
builder.Services.AddScoped<IUnitOfWork<PropertyDbContext>, UnitOfWork<PropertyDbContext>>();

// 注册服务
builder.Services.AddScoped<IPropertyService, PropertyService>();

// 注册HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 注册Redis服务
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost"));
builder.Services.AddScoped<IRedisService, RedisService>();
// 修改 AutoMapper 注册代码以消除二义性  
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>()); // 添加映射配置文件  
// 在PropertyService的Program.cs中添加
// 配置JWT设置
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// 配置JWT认证 - 只验证用户身份，不进行完整JWT验证（因为ApiGateway已经验证过）
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // 方案1：简化的JWT验证配置
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "default-key-for-property-service");
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = false, // 不验证签名（ApiGateway已验证）
        IssuerSigningKey = new SymmetricSecurityKey(key), // 提供密钥但不验证
        ValidateIssuer = false,           // 不验证发行者（ApiGateway已验证）
        ValidateAudience = false,         // 不验证受众（ApiGateway已验证）
        ValidateLifetime = false,         // 不验证过期时间（ApiGateway已验证）
        RequireExpirationTime = false,    // 不要求过期时间
        RequireSignedTokens = false,      // 不要求签名令牌
        // 关键配置：确保声明映射正确
        NameClaimType = ClaimTypes.NameIdentifier
    };

    // 不需要检查令牌撤销，因为ApiGateway已经检查过
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // 记录认证失败但不阻止请求（因为可能是从ApiGateway转发的有效请求）
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("JWT认证失败: {Exception}", context.Exception?.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // 记录成功验证的用户信息
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            logger.LogInformation("用户身份验证成功: {UserId}", userId);
            return Task.CompletedTask;
        }
    };
});

// 配置健康检查
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database_health_check");

// 配置日志
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/property-service-.log", rollingInterval: RollingInterval.Day));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 使用请求日志中间件
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

// 使用用户上下文中间件 - 必须在认证中间件之前
app.UseUserContext();

app.Use(async (context, next) => {
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("接收请求头：{Headers}", context.Request.Headers);
    await next();
});

app.UseAuthentication();
app.UseRouting();
// 必须位于Routing之后
app.UseAuthorization();

// 配置健康检查端点
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration
            })
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

app.MapControllers();

app.Run();