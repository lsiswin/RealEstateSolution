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

// 配置AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
// 在PropertyService的Program.cs中添加
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false, // 不验证签名
            ValidateIssuer = false,           // 不验证颁发者
            ValidateAudience = false,         // 不验证受众
            ValidateLifetime = false,         // 不验证过期时间
            RequireExpirationTime = false     // 不要求过期时间
        };
    });
// 配置缓存
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

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