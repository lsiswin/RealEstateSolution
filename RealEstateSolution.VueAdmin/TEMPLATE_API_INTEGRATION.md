# 合同模板API集成修改说明

## 修改概述

本次修改将合同模板的前端组件从使用模拟数据改为使用真实的API请求，实现了完整的前后端数据交互。

## 修改的文件

### 1. 新增API文件
- **文件**: `src/api/contractTemplate.ts`
- **功能**: 定义了合同模板相关的所有API接口和类型定义
- **包含内容**:
  - ContractType枚举（Sale=0, Rent=1, Commission=2）
  - ContractTemplate、ContractTemplateQuery、ContractTemplateCreate等接口
  - 所有CRUD操作的API函数
  - 辅助函数：getContractTypeText、getContractTypeColor

### 2. 修改模板查看组件
- **文件**: `src/views/Contract/TemplateView.vue`
- **主要修改**:
  - 导入真实的API函数和类型
  - 替换模拟数据获取逻辑为API调用
  - 实现真实的文件下载功能
  - 删除重复的辅助函数
  - 添加错误处理和加载状态

### 3. 修改模板编辑组件
- **文件**: `src/views/Contract/TemplateEdit.vue`
- **主要修改**:
  - 导入真实的API函数和类型
  - 替换模拟数据获取逻辑为API调用
  - 实现真实的创建和更新功能
  - 修正表单字段映射（status -> isActive）
  - 使用ContractType枚举值
  - 添加错误处理和加载状态

## 技术细节

### API接口设计
```typescript
// 获取模板列表（分页）
getContractTemplates(params: ContractTemplateQuery)

// 获取模板详情
getContractTemplate(id: number)

// 创建模板
createContractTemplate(data: ContractTemplateCreate)

// 更新模板
updateContractTemplate(id: number, data: ContractTemplateCreate)

// 删除模板
deleteContractTemplate(id: number)

// 导出Word文档
exportContractTemplate(id: number)
```

### 数据类型映射
- 前端使用ContractType枚举（0=买卖, 1=租赁, 2=委托）
- 状态字段从字符串改为布尔值（isActive）
- 添加了完整的TypeScript类型支持

### 错误处理
- 所有API调用都包含try-catch错误处理
- 显示用户友好的错误消息
- 无效参数时自动跳转到列表页

## 测试状态

- ✅ 前端项目编译成功
- ✅ TypeScript类型检查通过
- ✅ 所有导入和依赖正确
- ⏳ 需要后端API服务运行进行功能测试

## 后续工作

1. 启动后端API服务
2. 测试模板的增删改查功能
3. 测试文件上传下载功能
4. 验证错误处理和边界情况
5. 优化用户体验和性能

## 注意事项

- 确保后端API服务正常运行
- 检查API路径是否与后端控制器匹配
- 验证权限控制是否正确配置
- 测试文件上传下载的MIME类型处理 