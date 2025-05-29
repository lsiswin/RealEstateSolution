import request from './request'

// 匹配类型枚举
export enum MatchingType {
  Auto = 0,
  Manual = 1
}

// 匹配状态枚举
export enum MatchingStatus {
  Pending = 0,
  Matched = 1,
  Rejected = 2,
  Expired = 3
}

// 匹配记录接口
export interface Matching {
  id: number
  clientId: number
  propertyId: number
  type: MatchingType
  status: MatchingStatus
  matchScore: number
  matchReason: string
  createdAt: string
  updatedAt: string
  clientName?: string
  propertyTitle?: string
}

// 搜索匹配记录参数
export interface SearchMatchingParams {
  keyword?: string
  type?: MatchingType
  status?: MatchingStatus
  startDate?: string
  endDate?: string
}

// 手动匹配参数
export interface ManualMatchParams {
  clientId: number
  propertyId: number
}

// 创建匹配
export const createMatching = (matching: Matching): Promise<Matching> => {
  return request.post('/api/matching/CreateMatching', matching)
}

// 更新匹配状态
export const updateMatchingStatus = (id: number, status: MatchingStatus): Promise<void> => {
  return request.put(`/api/matching/UpdateMatchingStatus/${id}/status`, null, {
    params: { status }
  })
}

// 获取匹配详情
export const getMatching = (id: number): Promise<Matching> => {
  return request.get(`/api/matching/GetMatching/${id}`)
}

// 搜索匹配记录
export const searchMatchings = (params: SearchMatchingParams): Promise<Matching[]> => {
  return request.get('/api/matching/SearchMatchings', { params })
}

// 获取客户的匹配记录
export const getClientMatchings = (clientId: number): Promise<Matching[]> => {
  return request.get(`/api/matching/GetClientMatchings/client/${clientId}`)
}

// 获取房源的匹配记录
export const getPropertyMatchings = (propertyId: number): Promise<Matching[]> => {
  return request.get(`/api/matching/GetPropertyMatchings/property/${propertyId}`)
}

// 删除匹配记录
export const deleteMatching = (id: number): Promise<void> => {
  return request.delete(`/api/matching/DeleteMatching/${id}`)
}

// 自动匹配
export const autoMatch = (clientId: number): Promise<Matching[]> => {
  return request.post(`/api/matching/AutoMatch/auto/${clientId}`)
}

// 手动匹配
export const manualMatch = (params: ManualMatchParams): Promise<Matching> => {
  return request.post('/api/matching/ManualMatch/manual', null, {
    params
  })
} 