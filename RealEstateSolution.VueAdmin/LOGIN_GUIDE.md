# 登录注册功能使用指南

## 🚀 功能概述

房产中介管理系统已集成完整的用户认证功能，包括：

- ✅ 用户登录
- ✅ 用户注册  
- ✅ 路由守卫
- ✅ 用户状态管理
- ✅ 自动登出

## 📱 页面访问

### 登录页面
- 访问地址：`http://localhost:3000/login`
- 功能：用户登录系统

### 注册页面  
- 访问地址：`http://localhost:3000/register`
- 功能：新用户注册账号

## 🔐 注册字段说明

根据后端 `CreateUserRequest.cs` 模型，注册时需要填写：

### 必填字段
- **用户名** (userName)：3-50个字符，只能包含字母、数字和下划线
- **密码** (password)：6-100个字符，必须包含大小写字母和数字
- **确认密码**：与密码一致
- **邮箱** (email)：有效的邮箱格式

### 可选字段
- **手机号码** (phoneNumber)：11位中国大陆手机号
- **真实姓名** (realName)：最多50个字符

### 默认设置
- **角色** (roles)：默认为 `["User"]`
- **状态** (isActive)：默认为 `true`

## 🔧 API接口

所有认证相关的API接口都在 `src/api/auth.ts` 中定义：

```typescript
// 登录
POST /api/auth/login
{
  "userName": "string",
  "password": "string"
}

// 注册
POST /api/auth/register
{
  "userName": "string",
  "password": "string", 
  "email": "string",
  "phoneNumber": "string", // 可选
  "realName": "string"     // 可选
}

// 刷新令牌
POST /api/auth/refresh
{
  "refreshToken": "string"
}

// 登出
POST /api/auth/logout
{
  "refreshToken": "string"
}

// 修改密码
POST /api/auth/change-password
{
  "currentPassword": "string",
  "newPassword": "string"
}

// 更新资料
PUT /api/auth/profile
{
  "userName": "string",
  "email": "string"
}
```

## 🛡️ 安全特性

### 前端验证
- 用户名格式验证
- 密码强度验证（大小写字母+数字）
- 邮箱格式验证
- 手机号格式验证
- 密码确认验证

### 路由守卫
- 未登录用户自动跳转到登录页
- 已登录用户访问登录页自动跳转到首页
- Token过期自动清除并跳转登录页

### 状态管理
- 使用Pinia管理用户状态
- 自动保存/恢复登录状态
- 安全的Token存储

## 🎨 UI设计

### 设计风格
- 简约现代的卡片式设计
- 渐变背景色彩
- 响应式布局
- Element Plus组件库

### 交互体验
- 表单实时验证
- 加载状态提示
- 错误信息展示
- 成功反馈

## 🔄 使用流程

1. **首次访问**：自动跳转到登录页
2. **新用户注册**：点击"立即注册"跳转注册页
3. **填写注册信息**：完成表单验证后提交
4. **注册成功**：自动跳转到登录页
5. **用户登录**：输入用户名密码登录
6. **进入系统**：登录成功后进入管理后台
7. **退出登录**：点击用户头像选择退出

## 🚨 注意事项

1. **后端服务**：确保后端API服务正常运行在 `http://localhost:5000`
2. **CORS配置**：后端需要配置CORS允许前端跨域访问
3. **Token有效期**：Token过期后会自动清除用户状态
4. **密码安全**：建议使用强密码，包含大小写字母和数字

## 🛠️ 开发说明

### 环境变量
```bash
VITE_API_BASE_URL=http://localhost:5000  # 后端API地址
```

### 启动项目
```bash
npm install
npm run dev
```

### 构建项目
```bash
npm run build
``` 