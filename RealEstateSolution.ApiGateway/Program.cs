using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using RealEstateSolution.Common.Redis;
using RealEstateSolution.Common.Utils;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
// 添加 CORS 策略（开发环境允许所有）
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // 前端地址
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // 如果使用 Cookie 或 Authorization Header
    });
});// 配置JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer",options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,    // 检查过期时间,
        ValidIssuer = jwtSettings.Issuer,
        ClockSkew = TimeSpan.Zero
    };
});
// 添加配置
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
// 添加Redis服务
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddSingleton<IRedisService, RedisService>();
// 添加Ocelot
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<JwtRevocationCheckHandler>();
// 添加CORS服务

var app = builder.Build();

// 启用CORS
app.UseCors("DevCors");

app.UseAuthentication(); // 必须放在Ocelot之前
app.UseAuthorization();
app.UseOcelot().Wait();

app.Run();