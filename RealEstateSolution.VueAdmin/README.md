# 房产中介管理系统 - 前端管理后台

基于 Vue 3 + TypeScript + Vite + Element Plus 构建的现代化房产中介管理系统前端。

## 技术栈

- **Vue 3.4.0** - 渐进式 JavaScript 框架
- **TypeScript** - JavaScript 的超集，提供类型安全
- **Vite 5.0** - 下一代前端构建工具
- **Element Plus 2.4.4** - 基于 Vue 3 的组件库
- **Vue Router 4** - Vue.js 官方路由管理器
- **Pinia** - Vue 的状态管理库
- **ECharts 5** - 数据可视化图表库
- **Axios** - HTTP 客户端

## 功能模块

### 🏠 首页 Dashboard
- 系统概览统计
- 房源类型分布图表
- 月度成交趋势图表
- 最新房源列表

### 🏢 房源管理
- 房源列表查看
- 添加新房源
- 房源信息编辑
- 房源状态管理

### 👥 客户管理
- 客户信息管理
- 客户需求跟踪
- 客户分类统计

### 📄 合同管理
- 合同列表管理
- 合同模板配置
- 合同状态跟踪

### 🔗 智能匹配
- 房源客户智能匹配
- 匹配结果查看
- 匹配参数设置

### 🗑️ 回收站
- 已删除数据管理
- 数据恢复功能

### ⚙️ 系统管理
- 用户权限管理
- 角色配置
- 系统操作日志

## 项目结构

```
src/
├── assets/          # 静态资源
├── components/      # 公共组件
├── router/          # 路由配置
├── stores/          # 状态管理
├── views/           # 页面组件
│   ├── Dashboard.vue    # 首页
│   ├── Layout.vue       # 主布局
│   ├── Property/        # 房源管理
│   ├── Client/          # 客户管理
│   ├── Contract/        # 合同管理
│   ├── Matching/        # 智能匹配
│   ├── Recycle/         # 回收站
│   └── System/          # 系统管理
├── utils/           # 工具函数
├── App.vue          # 根组件
├── main.ts          # 入口文件
└── env.d.ts         # 类型定义
```

## 开发指南

### 环境要求
- Node.js >= 16.0.0
- npm >= 8.0.0

### 安装依赖
```bash
npm install
```

### 启动开发服务器
```bash
npm run dev
```

### 构建生产版本
```bash
npm run build
```

### 预览生产构建
```bash
npm run preview
```

### 代码检查
```bash
npm run lint
```

## 设计特色

### 🎨 现代化 UI 设计
- 采用 Element Plus 组件库
- 深蓝色主题配色方案
- 响应式布局设计
- 优雅的交互动效

### 📊 数据可视化
- ECharts 图表集成
- 实时数据统计展示
- 多维度数据分析

### 🔧 开发体验
- TypeScript 类型安全
- 热模块替换 (HMR)
- 自动导入配置
- ESLint 代码规范

### 🚀 性能优化
- Vite 快速构建
- 路由懒加载
- 组件按需加载
- 生产环境优化

## 浏览器支持

- Chrome >= 87
- Firefox >= 78
- Safari >= 14
- Edge >= 88

## 许可证

MIT License 