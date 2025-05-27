# JWT验证架构说明

## 架构概述

本系统采用了**集中式JWT验证**的微服务架构，其中：

- **ApiGateway** 负责完整的JWT验证（签名、过期时间、撤销状态等）
- **各微服务** 只负责提取和验证用户身份信息，不进行完整的JWT验证

## 工作流程

### 1. 用户登录
```
用户 -> AuthService -> 生成JWT -> 返回给用户
```

### 2. 访问受保护资源
```
用户 -> ApiGateway -> JWT完整验证 -> 提取用户信息 -> 转发到微服务
```

### 3. 微服务处理
```
微服务 -> 接收请求头中的用户信息 -> 验证用户身份 -> 处理业务逻辑
```

## 关键组件

### ApiGateway (端口: 5000)

#### JWT验证配置
- 完整的JWT验证（签名、过期时间、发行者、受众）
- JWT撤销检查（通过Redis黑名单）
- 用户信息提取和转发

#### Ocelot配置 (ocelot.json)
```json
{
    "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
    },
    "DelegatingHandlers": ["JwtRevocationCheckHandler"],
    "AddHeadersToRequest": {
        "X-User-Id": "Claims[sub]",
        "X-User-Name": "Claims[name]",
        "X-User-Roles": "Claims[role]"
    }
}
```

#### JwtRevocationCheckHandler
- 检查JWT是否在黑名单中
- 提取用户信息用于转发
- 处理JWT验证错误

### PropertyService (端口: 5001)

#### 简化的JWT配置
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = false, // 不验证签名（ApiGateway已验证）
    ValidateIssuer = false,           // 不验证发行者（ApiGateway已验证）
    ValidateAudience = false,         // 不验证受众（ApiGateway已验证）
    ValidateLifetime = false,         // 不验证过期时间（ApiGateway已验证）
    RequireExpirationTime = false,    // 不要求过期时间
    RequireSignedTokens = false,      // 不要求签名令牌
    SignatureValidator = (token, parameters) => new JwtSecurityToken(token),
    NameClaimType = ClaimTypes.NameIdentifier
};
```

#### UserContextMiddleware
- 从ApiGateway传递的请求头中提取用户信息
- 创建用户身份上下文
- 支持回退到JWT认证（用于直接访问）

#### AutoMapper优化
- 使用AutoMapper简化属性更新
- 自动处理UpdateTime等字段
- 忽略不应更新的字段（Id、OwnerId、CreateTime）

## 安全特性

### 1. 集中式验证
- 所有JWT验证逻辑集中在ApiGateway
- 微服务不需要维护JWT密钥
- 统一的安全策略

### 2. 令牌撤销
- Redis黑名单机制
- 实时撤销检查
- 防止已撤销令牌的使用

### 3. 用户信息传递
- 通过HTTP头部安全传递用户信息
- 避免在微服务中重复解析JWT
- 支持角色和权限信息传递

## 配置要求

### 共同配置
所有服务需要相同的JWT配置：
```json
{
    "Jwt": {
        "Key": "lengshuang19971104coldforevering",
        "Issuer": "ls",
        "Audience": "cold",
        "RefreshTokenExpirationDays": 1,
        "AccessTokenExpirationMinutes": 10
    }
}
```

### Redis配置
```json
{
    "ConnectionStrings": {
        "Redis": "localhost,abortConnect=false"
    }
}
```

## 优势

1. **性能优化**: 微服务不需要进行复杂的JWT验证
2. **安全集中**: 所有安全逻辑集中管理
3. **易于维护**: JWT密钥只需在ApiGateway配置
4. **可扩展性**: 新增微服务只需简单配置
5. **一致性**: 统一的用户身份验证机制

## 使用示例

### 在PropertyService中获取用户信息
```csharp
public async Task<ApiResponse<Property>> UpdatePropertyAsync(int id, Property property, string userId)
{
    // userId 从 HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 获取
    // 或者从 UserContextMiddleware 设置的上下文中获取
    
    await ValidatePropertyAccess(id, userId);
    var existingProperty = await _propertyRepository.GetByIdAsync(id);
    
    // 使用AutoMapper更新属性
    _mapper.Map(property, existingProperty);
    
    // ... 其他业务逻辑
}
```

### 检查用户角色
```csharp
var isAgent = User.IsInRole("Agent");
var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
```

## 故障处理

### ApiGateway故障
- 微服务仍可通过JWT直接访问（降级模式）
- 需要确保微服务的JWT配置正确

### Redis故障
- JWT撤销检查会失败
- 建议实现Redis故障转移机制

### 微服务故障
- 不影响其他微服务
- ApiGateway会返回相应的错误信息 