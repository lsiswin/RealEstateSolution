using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealEstateSolution.ClientService.Data;
using RealEstateSolution.ClientService.Repository;
using RealEstateSolution.ClientService.Services;
using System.Text;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// 添加数据库上下文
builder.Services.AddDbContext<ClientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RealEstateClient")));

// 添加仓储和服务
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();


// 添加控制器
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();