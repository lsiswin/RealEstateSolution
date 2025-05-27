# 房源管理API使用指南

## 🏠 功能概述

房产中介管理系统的房源管理模块已完成，包含以下功能：

- ✅ 房源列表查询（支持筛选、分页）
- ✅ 房源详情查看
- ✅ 房源添加/编辑
- ✅ 房源状态管理
- ✅ 房源删除（单个/批量）
- ✅ 房源统计数据

## 🔧 技术架构

### 后端架构
```
前端 (Vue:3000) → API网关 (Ocelot:5098) → PropertyService (5001)
```

### API路由配置
- **网关地址**: `http://localhost:5098`
- **房源API**: `/api/property/*`
- **后端服务**: PropertyService (端口5001)

## 📊 数据模型

### 房产类型 (PropertyType)
- `1` - 住宅 (Residential)
- `2` - 商业 (Commercial) 
- `3` - 办公 (Office)
- `4` - 工业 (Industrial)
- `5` - 土地 (Land)

### 装修类型 (DecorationType)
- `1` - 毛坯 (Rough)
- `2` - 简装 (Simple)
- `3` - 精装 (Fine)
- `4` - 豪装 (Luxury)

### 朝向类型 (OrientationType)
- `1` - 东 (East)
- `2` - 南 (South)
- `3` - 西 (West)
- `4` - 北 (North)
- `5` - 东南 (Southeast)
- `6` - 东北 (Northeast)
- `7` - 西南 (Southwest)
- `8` - 西北 (Northwest)

### 房产状态 (PropertyStatus)
- `1` - 待售 (ForSale)
- `2` - 已售 (Sold)
- `3` - 待租 (ForRent)
- `4` - 已租 (Rented)
- `5` - 下架 (Offline)
- `6` - 可用 (Available)

## 🗄️ 模拟数据

已创建10条真实的房源模拟数据，包含：
- 北京阳光花园三室两厅 (320万)
- 上海万科城市花园复式 (580万)
- 深圳CBD核心商业综合体 (1200万)
- 广州甲级写字楼整层 (850万)
- 杭州学区房两室一厅 (280万)
- 武汉江景豪宅别墅 (1500万)
- 苏州工业园区厂房 (350万)
- 成都商业用地 (2500万)
- 南京地铁口精装公寓 (120万)
- 西安临街商铺门面 (180万)

### 数据导入
```sql
-- 执行 mock-data/property-data.sql 文件
-- 包含完整的INSERT语句和枚举值说明
```

## 🔌 API接口

### 1. 查询房源列表
```typescript
GET /api/property/QueryProperties
参数: {
  type?: PropertyType,
  minPrice?: number,
  maxPrice?: number,
  minArea?: number,
  maxArea?: number,
  status?: PropertyStatus,
  keyword?: string,
  pageIndex?: number,
  pageSize?: number
}
```

### 2. 获取房源详情
```typescript
GET /api/property/GetProperty/{id}
```

### 3. 登记新房源
```typescript
POST /api/property/RegisterProperty
Body: Property对象
```

### 4. 修改房源信息
```typescript
PUT /api/property/UpdateProperty/{id}
Body: Property对象
```

### 5. 变更房源状态
```typescript
POST /api/property/ChangePropertyStatus/{id}
Body: { status: PropertyStatus }
```

### 6. 删除房源
```typescript
DELETE /api/property/DeleteProperty/{id}
```

### 7. 获取房源统计
```typescript
GET /api/property/GetPropertyStats
```

## 🎨 前端功能

### 房源列表页面 (`/property/list`)
- **搜索筛选**: 房产类型、状态、价格范围、面积范围、关键词
- **数据展示**: 表格形式，支持排序
- **批量操作**: 多选删除
- **分页**: 支持页面大小调整
- **操作**: 查看、编辑、删除

### 功能特色
- 🔍 **智能搜索**: 支持标题、地址关键词搜索
- 💰 **价格格式化**: 自动转换万元显示
- 🏷️ **状态标签**: 不同颜色区分房源状态
- 📱 **响应式设计**: 适配移动端
- ⚡ **实时加载**: Loading状态提示

## 🚀 使用流程

1. **启动服务**
   ```bash
   # 启动API网关
   cd RealEstateSolution.ApiGateway
   dotnet run
   
   # 启动房源服务
   cd RealEstateSolution.PropertyService  
   dotnet run
   
   # 启动前端
   cd RealEstateSolution.VueAdmin
   npm run dev
   ```

2. **访问系统**
   - 前端地址: `http://localhost:3000`
   - 登录系统后进入房源管理

3. **数据操作**
   - 查看房源列表
   - 使用筛选条件搜索
   - 添加/编辑房源信息
   - 管理房源状态

## 🛠️ 开发说明

### 环境变量
```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5098
```

### 依赖服务
- ✅ AuthService (5005) - 用户认证
- ✅ ApiGateway (5098) - API网关
- ✅ PropertyService (5001) - 房源服务
- ✅ SQL Server - 数据库
- ✅ Redis - 缓存服务

### 权限控制
- 需要登录认证
- 支持角色权限（Agent/User）
- JWT Token验证

## 🐛 问题排查

### 常见问题
1. **连接被拒绝**: 检查API网关是否启动在5098端口
2. **401未授权**: 确认用户已登录且Token有效
3. **数据为空**: 检查数据库是否导入模拟数据
4. **CORS错误**: 确认网关CORS配置正确

### 调试工具
- 浏览器开发者工具 (Network)
- Swagger UI: `http://localhost:5001/swagger`
- API网关日志

## 📈 后续扩展

- [ ] 房源图片上传
- [ ] 房源地图定位
- [ ] 房源推荐算法
- [ ] 房源收藏功能
- [ ] 房源分享功能
- [ ] 数据导入导出 