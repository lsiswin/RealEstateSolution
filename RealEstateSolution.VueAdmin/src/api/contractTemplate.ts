import request from './request'

// 合同类型枚举
export enum ContractType {
  Sale = 0,       // 买卖合同
  Rent = 1,       // 租赁合同
  Commission = 2  // 委托合同
}

// 合同模板接口
export interface ContractTemplate {
  id: number
  name: string
  description?: string
  type: ContractType
  content: string
  version: string
  isActive: boolean
  fileSize: number
  createdBy: string
  createdAt: string
  updatedAt: string
}

// 合同模板查询接口
export interface ContractTemplateQuery {
  name?: string
  type?: ContractType
  isActive?: boolean
  pageIndex?: number
  pageSize?: number
}

// 合同模板创建/更新接口
export interface ContractTemplateCreate {
  name: string
  description?: string
  type: ContractType
  content: string
  version: string
  isActive: boolean
}

// 分页结果接口
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

// API响应接口
export interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message: string
}

// 获取模板列表
export const getContractTemplates = (params: ContractTemplateQuery):Promise<ApiResponse<PagedResult<ContractTemplate>>> => {
  return request.get('/api/contract/GetTemplates', { params })
}

// 获取模板详情
export const getContractTemplate = (id: number):Promise<ApiResponse<ContractTemplate>> => {
  return request.get(`/api/contract/GetTemplateById/${id}`)
}

// 获取启用的模板列表
export const getActiveContractTemplates = (type?: ContractType):Promise<ApiResponse<ContractTemplate[]>> => {
  return request.get('/api/contract/GetActiveTemplates', { 
    params: type !== undefined ? { type } : {} 
  })
}

// 创建模板
export const createContractTemplate = (data: ContractTemplateCreate):Promise<ApiResponse<ContractTemplate>> => {
  return request.post('/api/contract/CreateTemplate', data)
}

// 更新模板
export const updateContractTemplate = (id: number, data: ContractTemplateCreate):Promise<ApiResponse<ContractTemplate>> => {
  return request.put(`/api/contract/UpdateTemplate/${id}`, data)
}

// 删除模板
export const deleteContractTemplate = (id: number):Promise<ApiResponse> => {
  return request.delete(`/api/contract/DeleteTemplate/${id}`)
}

// 更新模板状态
export const updateContractTemplateStatus = (id: number, isActive: boolean):Promise<ApiResponse> => {
  return request.patch(`/api/contract/UpdateTemplateStatus/${id}`, isActive)
}

// 导入Word模板
export const importContractTemplate = (file: File, type: ContractType):Promise<ApiResponse<ContractTemplate>> => {
  const formData = new FormData()
  formData.append('file', file)
  formData.append('type', type.toString())
  
  return request.post('/api/contract/ImportFromWord', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

// 导出模板为Word
export const exportContractTemplate = (id: number) => {
  return request.get(`/api/contract/ExportToWord/${id}`, {
    responseType: 'blob'
  })
}

// 获取模板统计信息
export const getContractTemplateStats = ():Promise<ApiResponse<any>> => {
  return request.get('/api/contract/GetTemplateStats')
}

// 辅助函数：获取合同类型文本
export const getContractTypeText = (type: ContractType): string => {
  const typeMap: Record<ContractType, string> = {
    [ContractType.Sale]: '买卖合同',
    [ContractType.Rent]: '租赁合同',
    [ContractType.Commission]: '委托合同'
  }
  return typeMap[type] || '未知'
}

// 辅助函数：获取合同类型颜色
export const getContractTypeColor = (type: ContractType): string => {
  const colorMap: Record<ContractType, string> = {
    [ContractType.Sale]: 'success',
    [ContractType.Rent]: 'warning',
    [ContractType.Commission]: 'info'
  }
  return colorMap[type] || ''
} 