import request from './request'
import { ApiResponse } from './property'

// 客户类型枚举
export enum ClientType {
  Buyer = 0,
  Seller = 1,
  Tenant = 2,
  Landlord = 3
}

// 客户接口
export interface Client {
  id: number
  name: string
  phone: string
  email?: string
  type: ClientType
  address?: string
  notes?: string
  agentId: number
  createdAt: string
  updatedAt: string
  agentName?: string
}

// 客户需求接口
export interface ClientRequirement {
  id: number
  clientId: number
  minPrice?: number
  maxPrice?: number
  minArea?: number
  maxArea?: number
  location?: string
  propertyType?: string
  otherRequirements?: string
  createdAt: string
  updatedAt: string
}

// 客户需求DTO
export interface ClientRequirementDto {
  minPrice?: number
  maxPrice?: number
  minArea?: number
  maxArea?: number
  location?: string
  propertyType?: string
  otherRequirements?: string
}

// 客户统计数据
export interface ClientStats {
  totalClients: number
  newClientsLast30Days: number
  activeClients: number
}

// 获取客户列表参数
export interface GetClientsParams {
  name: '',
  phone: '',
  type: undefined,
  pageIndex?: number
  pageSize?: number
}

// 分页结果
export interface PagedList<T> {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

// 获取客户列表
export const getClients = (params: GetClientsParams): Promise<ApiResponse<PagedList<Client>>> => {
  return request.get('/api/client/GetClients', { params })
}

// 获取客户详情
export const getClient = (id: number): Promise<ApiResponse<Client>> => {
  return request.get(`/api/client/GetClient/${id}`)
}

// 创建客户
export const createClient = (client: Client): Promise<ApiResponse<Client>> => {
  return request.post('/api/client/CreateClient', client)
}

// 更新客户信息
export const updateClient = (id: number, client: Client): Promise<ApiResponse<Client>> => {
  return request.put(`/api/client/UpdateClient/${id}`, client)
}

// 删除客户
export const deleteClient = (id: number): Promise<void> => {
  return request.delete(`/api/client/DeleteClient/${id}`)
}

// 获取客户需求
export const getClientRequirements = (id: number): Promise<ApiResponse<ClientRequirement>> => {
  return request.get(`/api/client/GetClientRequirements/${id}/requirements`)
}

// 更新客户需求
export const updateClientRequirements = (id: number, requirement: ClientRequirementDto): Promise<ApiResponse<ClientRequirement>> => {
  return request.put(`/api/client/UpdateClientRequirements/${id}/requirements`, requirement)
}

// 获取客户统计数据
export const getClientStats = (): Promise<ApiResponse<ClientStats>> => {
  return request.get('/api/client/GetClientStats/stats')
} 