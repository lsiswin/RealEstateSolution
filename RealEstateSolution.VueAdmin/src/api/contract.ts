import request from './request'

// 合同类型枚举
export enum ContractType {
  Sale = 0,       // 买卖合同
  Rent = 1,       // 租赁合同
  Commission = 2  // 委托合同
}

// 合同状态枚举
export enum ContractStatus {
  Draft = 0,      // 草稿
  Pending = 1,    // 待签署
  Signed = 2,     // 已签署
  Completed = 3,  // 已完成
  Cancelled = 4   // 已取消
}

// 合同接口 - 统一用于增删改查
export interface Contract {
  id: number
  contractNumber: string
  title: string
  type: ContractType
  status: ContractStatus
  propertyId: number
  propertyTitle?: string
  propertyAddress?: string
  partyAId: number
  partyAName?: string
  partyAPhone?: string
  partyBId: number
  partyBName?: string
  partyBPhone?: string
  amount: number
  signDate?: string
  startDate?: string
  endDate?: string
  paymentMethod?: string
  terms?: string
  notes?: string
  content?: string
  createdAt: string
  updatedAt: string
}

// 合同查询接口
export interface ContractQuery {
  pageIndex?: number
  pageSize?: number
  keyword?: string
  type?: ContractType
  status?: ContractStatus
  startDate?: string
  endDate?: string
  partyAId?: number
  partyBId?: number
  propertyId?: number
}

// 合同模板接口
export interface ContractTemplate {
  id: number
  name: string
  description: string
  type: ContractType
  content: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

// 合同统计接口
export interface ContractStats {
  totalContracts: number
  draftContracts: number
  pendingContracts: number
  signedContracts: number
  completedContracts: number
  cancelledContracts: number
  totalAmount: number
  monthlyAmount: number
}

// 获取合同列表
export const getContracts = (params: ContractQuery) => {
  return request.get('/api/Contract/GetContracts', { params })
}

// 获取合同详情
export const getContract = (id: number) => {
  return request.get(`/api/Contract/GetContract/${id}`)
}

// 创建合同
export const createContract = (data: Contract) => {
  return request.post('/api/Contract/CreateContract', data)
}

// 更新合同
export const updateContract = (id: number, data: Contract) => {
  return request.put(`/api/Contract/UpdateContract/${id}`, data)
}

// 删除合同
export const deleteContract = (id: number) => {
  return request.delete(`/api/Contract/DeleteContract/${id}`)
}

// 更新合同状态
export const updateContractStatus = (id: number, status: ContractStatus) => {
  return request.post(`/api/Contract/UpdateContractStatus/${id}`, status)
}

// 获取合同模板列表
export const getContractTemplates = (type?: ContractType) => {
  return request.get('/api/Contract/GetContractTemplates', { params: { type } })
}

// 根据模板创建合同
export const createContractFromTemplate = (templateId: number, data: Contract) => {
  return request.post(`/api/Contract/CreateContractFromTemplate/${templateId}`, data)
}

// 生成合同编号
export const generateContractNumber = (type: ContractType) => {
  return request.get('/api/Contract/GenerateContractNumber', { params: { type } })
}

// 获取合同统计
export const getContractStats = () => {
  return request.get('/api/Contract/GetContractStats')
} 