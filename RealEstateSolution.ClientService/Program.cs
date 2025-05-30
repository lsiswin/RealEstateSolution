using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstateSolution.ClientService.Data;
using RealEstateSolution.ClientService.Repository;
using RealEstateSolution.ClientService.Services;
using System.Text;
using RealEstateSolution.Common.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using RealEstateSolution.PropertyService.Extension.HealthChecks;
using Serilog;
using RealEstateSolution.ClientService.Mappings;
using RealEstateSolution.Common.Repository;

var builder = WebApplication.CreateBuilder(args);

// 配置JWT设置
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// 配置JWT认证 - 支持从API网关传递的Claims和直接JWT验证
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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.Zero,
        // 关键配置：确保声明映射正确
        NameClaimType = ClaimTypes.NameIdentifier
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("JWT认证失败: {Exception}", context.Exception?.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            logger.LogInformation("用户身份验证成功: {UserId}", userId);
            return Task.CompletedTask;
        }
    };
});

// 配置日志
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/client-service-.log", rollingInterval: RollingInterval.Day));

// 添加数据库上下文
builder.Services.AddDbContext<ClientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RealEstateClient")));

// 注册工作单元
builder.Services.AddScoped<IUnitOfWork<ClientDbContext>, UnitOfWork<ClientDbContext>>();

// 添加仓储和服务
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();

// 添加控制器
builder.Services.AddControllers();
builder.Services.AddAutoMapper(option=> option.AddProfile<ClientMappingProfile>());
// 配置健康检查
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database_health_check");
// 添加Swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// 使用用户上下文中间件 - 必须在认证中间件之前
app.UseUserContext();
app.UseAuthentication();
app.UseRouting();

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