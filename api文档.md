# 房地产管理系统 API 文档

## 目录
- [客户管理](#客户管理)
- [合同管理](#合同管理)
- [匹配管理](#匹配管理)
- [回收站管理](#回收站管理)

## 客户管理

### 数据模型

#### 客户类型枚举 (ClientType)
```typescript
enum ClientType {
  Buyer = 0,    // 买家
  Seller = 1,   // 卖家
  Tenant = 2,   // 租客
  Landlord = 3  // 房东
}
```

#### 客户模型 (Client)
```typescript
interface Client {
  id: number           // 客户ID
  name: string         // 姓名
  phone: string        // 电话
  email?: string       // 邮箱（可选）
  type: ClientType     // 客户类型
  address?: string     // 地址（可选）
  notes?: string       // 备注（可选）
  agentId: number      // 代理人ID
  createdAt: string    // 创建时间
  updatedAt: string    // 更新时间
  agentName?: string   // 代理人姓名（可选）
}
```

#### 客户需求模型 (ClientRequirement)
```typescript
interface ClientRequirement {
  id: number           // 需求ID
  clientId: number     // 客户ID
  minPrice?: number    // 最低价格
  maxPrice?: number    // 最高价格
  minArea?: number     // 最小面积
  maxArea?: number     // 最大面积
  location?: string    // 位置要求
  propertyType?: string // 房产类型
  otherRequirements?: string // 其他要求
  createdAt: string    // 创建时间
  updatedAt: string    // 更新时间
}
```

### API 接口

#### 获取客户列表
- 请求路径：`GET /api/client/GetClients`
- 请求参数：
  ```typescript
  interface GetClientsParams {
    name?: string      // 客户姓名（可选）
    phone?: string     // 客户电话（可选）
    type?: ClientType  // 客户类型（可选）
    pageIndex?: number // 页码
    pageSize?: number  // 每页记录数
  }
  ```
- 响应数据：`PagedList<Client>`

#### 获取客户详情
- 请求路径：`GET /api/client/GetClient/{id}`
- 请求参数：`id`（客户ID）
- 响应数据：`Client`

#### 创建客户
- 请求路径：`POST /api/client/CreateClient`
- 请求参数：`Client`
- 响应数据：`Client`

#### 更新客户
- 请求路径：`PUT /api/client/UpdateClient/{id}`
- 请求参数：
  - `id`：客户ID
  - Body：`Client`
- 响应数据：`Client`

#### 删除客户
- 请求路径：`DELETE /api/client/DeleteClient/{id}`
- 请求参数：`id`（客户ID）
- 响应数据：`void`

## 合同管理

### 数据模型

#### 合同类型枚举 (ContractType)
```typescript
enum ContractType {
  Sale = 0,    // 销售合同
  Rent = 1,    // 租赁合同
  Lease = 2    // 租约合同
}
```

#### 合同状态枚举 (ContractStatus)
```typescript
enum ContractStatus {
  Draft = 0,      // 草稿
  Pending = 1,    // 待审核
  Active = 2,     // 生效
  Completed = 3,  // 完成
  Cancelled = 4,  // 取消
  Expired = 5     // 过期
}
```

#### 合同模型 (Contract)
```typescript
interface Contract {
  id: number           // 合同ID
  contractNumber: string // 合同编号
  clientId: number     // 客户ID
  propertyId: number   // 房产ID
  type: ContractType   // 合同类型
  status: ContractStatus // 合同状态
  startDate: string    // 开始日期
  endDate: string      // 结束日期
  amount: number       // 金额
  deposit: number      // 押金
  commission: number   // 佣金
  terms: string        // 合同条款
  notes?: string       // 备注
  createdAt: string    // 创建时间
  updatedAt: string    // 更新时间
  clientName?: string  // 客户姓名
  propertyTitle?: string // 房产标题
  agentName?: string   // 代理人姓名
}
```

### API 接口

#### 创建合同
- 请求路径：`POST /api/contract/CreateContract`
- 请求参数：`Contract`
- 响应数据：`Contract`

#### 更新合同
- 请求路径：`PUT /api/contract/UpdateContract/{id}`
- 请求参数：
  - `id`：合同ID
  - Body：`Contract`
- 响应数据：`void`

#### 获取合同详情
- 请求路径：`GET /api/contract/GetContract/{id}`
- 请求参数：`id`（合同ID）
- 响应数据：`Contract`

## 匹配管理

### 数据模型

#### 匹配类型枚举 (MatchingType)
```typescript
enum MatchingType {
  Auto = 0,    // 自动匹配
  Manual = 1   // 手动匹配
}
```

#### 匹配状态枚举 (MatchingStatus)
```typescript
enum MatchingStatus {
  Pending = 0,  // 待处理
  Matched = 1,  // 已匹配
  Rejected = 2, // 已拒绝
  Expired = 3   // 已过期
}
```

#### 匹配模型 (Matching)
```typescript
interface Matching {
  id: number           // 匹配ID
  clientId: number     // 客户ID
  propertyId: number   // 房产ID
  type: MatchingType   // 匹配类型
  status: MatchingStatus // 匹配状态
  matchScore: number   // 匹配分数
  matchReason: string  // 匹配原因
  createdAt: string    // 创建时间
  updatedAt: string    // 更新时间
  clientName?: string  // 客户姓名
  propertyTitle?: string // 房产标题
}
```

### API 接口

#### 创建匹配
- 请求路径：`POST /api/matching/CreateMatching`
- 请求参数：`Matching`
- 响应数据：`Matching`

#### 更新匹配状态
- 请求路径：`PUT /api/matching/UpdateMatchingStatus/{id}/status`
- 请求参数：
  - `id`：匹配ID
  - Query：`status`（匹配状态）
- 响应数据：`void`

## 回收站管理

### 数据模型

#### 回收站记录模型 (RecycleBin)
```typescript
interface RecycleBin {
  id: number           // 记录ID
  entityType: string   // 实体类型
  entityId: number     // 实体ID
  entityData: string   // 实体数据
  deleteReason: string // 删除原因
  deletedBy: number    // 删除人ID
  deletedAt: string    // 删除时间
  isRestored: boolean  // 是否已恢复
  restoredBy?: number  // 恢复人ID
  restoredAt?: string  // 恢复时间
}
```

### API 接口

#### 移至回收站
- 请求路径：`POST /api/recycle/MoveToRecycleBin/move`
- 请求参数：
  ```typescript
  interface MoveToRecycleBinRequest {
    entity: any        // 实体对象
    deleteReason: string // 删除原因
    deletedBy: number  // 删除人ID
  }
  ```
- 响应数据：`RecycleBin`

#### 从回收站恢复
- 请求路径：`POST /api/recycle/RestoreFromRecycleBin/{id}/restore`
- 请求参数：`id`（记录ID）
- 响应数据：`any`

#### 永久删除
- 请求路径：`DELETE /api/recycle/PermanentlyDelete/{id}`
- 请求参数：`id`（记录ID）
- 响应数据：`void`

## 通用响应格式

所有API响应都使用统一的响应格式：

```typescript
interface ApiResponse<T> {
  data?: T           // 响应数据
  message?: string   // 响应消息
  success: boolean   // 是否成功
}
```

## 分页响应格式

分页列表的响应格式：

```typescript
interface PagedList<T> {
  items: T[]         // 数据项列表
  totalCount: number // 总记录数
  pageIndex: number  // 当前页码
  pageSize: number   // 每页大小
  totalPages: number // 总页数
}
``` 