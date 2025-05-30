using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealEstateSolution.ContractService.Data;
using RealEstateSolution.ContractService.Repository;
using RealEstateSolution.ContractService.Services;
using System.Text;
using AutoMapper;
using RealEstateSolution.Common.Utils;
using Serilog;
using System.Security.Claims;
using RealEstateSolution.ContractService.Extension.Mappings;
using RealEstateSolution.Common.Repository;

var builder = WebApplication.CreateBuilder(args);

// 添加数据库上下文
builder.Services.AddDbContext<ContractDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RealEstateClient")));

// 注册工作单元
builder.Services.AddScoped<IUnitOfWork<ContractDbContext>, UnitOfWork<ContractDbContext>>();

// 添加仓储和服务
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractTemplateRepository, ContractTemplateRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IContractTemplateService, ContractTemplateService>();

builder.Services.AddAutoMapper(option => 
{
    option.AddProfile<ContractMappingProfile>();
    option.AddProfile<ContractTemplateMappingProfile>();
});
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

// 添加控制器
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

// 配置Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "合同服务 API", Version = "v1" });
    
    // 添加JWT认证配置
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
app.MapControllers();

app.Run();