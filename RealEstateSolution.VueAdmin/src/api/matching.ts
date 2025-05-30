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
export const createMatching = (matching: Matching) => {
  return request.post('/api/Matching/CreateMatching', matching)
}

// 更新匹配状态
export const updateMatchingStatus = (id: number, status: MatchingStatus) => {
  return request.put(`/api/Matching/UpdateMatchingStatus/${id}/status`, null, {
    params: { status }
  })
}

// 获取匹配详情
export const getMatching = (id: number) => {
  return request.get(`/api/Matching/GetMatching/${id}`)
}

// 搜索匹配记录
export const searchMatchings = (params: SearchMatchingParams) => {
  return request.get('/api/Matching/SearchMatchings/search', { params })
}

// 获取客户的匹配记录
export const getClientMatchings = (clientId: number) => {
  return request.get(`/api/Matching/GetClientMatchings/client/${clientId}`)
}

// 获取房源的匹配记录
export const getPropertyMatchings = (propertyId: number) => {
  return request.get(`/api/Matching/GetPropertyMatchings/property/${propertyId}`)
}

// 删除匹配记录
export const deleteMatching = (id: number) => {
  return request.delete(`/api/Matching/DeleteMatching/${id}`)
}

// 自动匹配
export const autoMatch = (clientId: number) => {
  return request.post(`/api/Matching/AutoMatch/auto/${clientId}`)
}

// 手动匹配
export const manualMatch = (params: ManualMatchParams) => {
  return request.post('/api/Matching/ManualMatch/manual', null, {
    params
  })
} 