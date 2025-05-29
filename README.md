# 房产中介管理系统

一个基于Vue 3 + .NET Core的现代化房产中介管理系统，提供房源管理、客户管理、合同管理等功能。

## 系统架构

### 前端技术栈
- **Vue 3** - 渐进式JavaScript框架
- **TypeScript** - 类型安全的JavaScript超集
- **Element Plus** - Vue 3组件库
- **Vue Router** - 官方路由管理器
- **Pinia** - 状态管理
- **Axios** - HTTP客户端
- **ECharts** - 数据可视化

### 后端技术栈
- **.NET Core 8** - 跨平台开发框架
- **Entity Framework Core** - ORM框架
- **SQL Server** - 数据库
- **JWT** - 身份认证
- **AutoMapper** - 对象映射

## 项目结构

```
RealEstateSolution/
├── RealEstateSolution.VueAdmin/          # Vue前端项目
│   ├── src/
│   │   ├── api/                          # API接口
│   │   ├── components/                   # 公共组件
│   │   ├── router/                       # 路由配置
│   │   ├── stores/                       # 状态管理
│   │   ├── views/                        # 页面组件
│   │   └── utils/                        # 工具函数
├── RealEstateSolution.PropertyService/   # 房源服务
├── RealEstateSolution.AuthService/       # 认证服务
├── RealEstateSolution.MatchingService/   # 智能匹配服务
├── RealEstateSolution.Database/          # 数据库项目
├── RealEstateSolution.Models/            # 共享模型
└── start-services.bat                    # 启动脚本
```

## 功能特性

### ✅ 已实现功能

#### 🏠 房源管理
- 房源列表查看（支持分页、搜索、筛选）
- 房源添加（完整表单验证）
- 房源编辑和删除
- 房源状态管理
- 房源统计数据

#### 📊 数据统计
- 房源类型分布图表
- 房源状态统计
- 实时数据展示
- 可视化图表（ECharts）

#### 🔐 用户认证
- 用户登录/注册
- JWT令牌认证
- 路由守卫
- 权限控制

#### 🎨 用户界面
- 响应式设计
- 现代化UI组件
- 友好的用户体验
- 错误处理和加载状态

### 🚧 开发中功能
- 客户管理
- 合同管理
- 智能匹配
- 系统管理
- 回收站

## 快速开始

### 环境要求
- Node.js 16+
- .NET Core 8 SDK
- SQL Server 2019+

### 安装步骤

1. **克隆项目**
   ```bash
   git clone <repository-url>
   cd RealEstateSolution
   ```

2. **安装前端依赖**
   ```bash
   cd RealEstateSolution.VueAdmin
   npm install
   ```

3. **配置数据库**
   - 修改各服务的`appsettings.json`中的数据库连接字符串
   - 运行数据库迁移

4. **启动服务**
   
   **方式一：使用启动脚本（推荐）**
   ```bash
   # 在项目根目录双击运行
   start-services.bat
   ```
   
   **方式二：手动启动**
   ```bash
   # 启动房源服务
   cd RealEstateSolution.PropertyService
   dotnet run --urls="http://localhost:5001"
   
   # 启动认证服务
   cd RealEstateSolution.AuthService
   dotnet run --urls="http://localhost:5098"
   
   # 启动前端
   cd RealEstateSolution.VueAdmin
   npm run dev
   ```

5. **访问系统**
   - 前端管理系统: http://localhost:3000
   - 服务API: http://localhost:5098

## API文档

### 房源服务 (localhost:5001)
- `GET /api/Property/QueryProperties` - 查询房源列表
- `GET /api/Property/GetProperty/{id}` - 获取房源详情
- `POST /api/Property/RegisterProperty` - 创建房源
- `PUT /api/Property/UpdateProperty/{id}` - 更新房源
- `DELETE /api/Property/DeleteProperty/{id}` - 删除房源
- `GET /api/Property/GetPropertyStats` - 获取房源统计

### 认证服务 (localhost:5098)
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/register` - 用户注册
- `GET /api/users/current` - 获取当前用户信息

## 开发指南

### 前端开发
```bash
cd RealEstateSolution.VueAdmin
npm run dev          # 开发模式
npm run build        # 生产构建
npm run preview      # 预览构建结果
```

### 后端开发
```bash
cd RealEstateSolution.PropertyService
dotnet run           # 运行服务
dotnet build         # 构建项目
dotnet test          # 运行测试
```

## 系统截图

### 登录页面
- 用户友好的登录界面
- 表单验证和错误提示

### 仪表板
- 房源统计数据
- 可视化图表
- 最新房源列表

### 房源管理
- 房源列表（分页、搜索、筛选）
- 房源添加表单
- 房源详情查看

## 技术亮点

1. **类型安全**: 全面使用TypeScript，提供完整的类型定义
2. **组件化**: 模块化的组件设计，易于维护和扩展
3. **响应式**: 适配各种屏幕尺寸的响应式设计
4. **错误处理**: 完善的错误处理和用户反馈机制
5. **性能优化**: 路由懒加载、组件按需导入
6. **代码规范**: 统一的代码风格和最佳实践

## 故障排除

### 常见问题

1. **端口冲突**
   - 确保端口5001、5098、5173没有被其他程序占用
   - 可以修改配置文件中的端口设置

2. **数据库连接失败**
   - 检查SQL Server是否正在运行
   - 验证连接字符串是否正确
   - 确保数据库已创建

3. **API调用失败**
   - 确保后端服务正在运行
   - 检查CORS配置
   - 验证API地址是否正确

## 贡献指南

1. Fork项目
2. 创建功能分支
3. 提交更改
4. 推送到分支
5. 创建Pull Request

## 许可证

MIT License

## 联系方式

如有问题或建议，请联系开发团队。 