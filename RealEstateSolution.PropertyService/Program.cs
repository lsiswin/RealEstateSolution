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
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options => {
//        // 必须显式声明不验证但接受任意令牌
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = false,
//            SignatureValidator = (token, parameters) => new JwtSecurityToken(token), // 绕过签名验证
//            ValidateAudience = false,
//            ValidateIssuer = false,
//            ValidateLifetime = false
//        };

//        // 重要：处理空方案情况
//        options.Challenge = "Gateway";
//        options.ForwardDefaultSelector = ctx => JwtBearerDefaults.AuthenticationScheme;
//    });
// 配置JWT认证
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero,
        // 关键配置：确保声明映射正确
        NameClaimType = ClaimTypes.NameIdentifier // 对应ClaimTypes.NameIdentifier
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var redisService = context.HttpContext.RequestServices.GetRequiredService<IRedisService>();
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (await redisService.IsTokenBlacklistedAsync(token))
            {
                context.Fail("Token has been revoked");
            }
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
app.Use(async (context, next) => {
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("接收请求头：{Headers}", context.Request.Headers);
    await next();
});
app.UseHttpsRedirection();
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