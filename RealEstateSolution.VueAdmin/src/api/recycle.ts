import request from './request'

// 回收站记录接口
export interface RecycleBin {
  id: number
  entityType: string
  entityId: number
  entityData: string
  deleteReason: string
  deletedBy: number
  deletedAt: string
  isRestored: boolean
  restoredBy?: number
  restoredAt?: string
}

// 移至回收站参数
export interface MoveToRecycleBinRequest {
  entity: any
  deleteReason: string
  deletedBy: number
}

// 搜索回收站参数
export interface SearchRecycleBinParams {
  keyword?: string
  entityType?: string
  isRestored?: boolean
  startDate?: string
  endDate?: string
}

// 获取指定实体回收站记录参数
export interface GetEntityRecycleBinParams {
  entityType: string
  entityId: number
}

// 将实体移至回收站
export const moveToRecycleBin = (params: MoveToRecycleBinRequest): Promise<RecycleBin> => {
  return request.post('/api/recycle/MoveToRecycleBin/move', params.entity, {
    params: {
      deleteReason: params.deleteReason,
      deletedBy: params.deletedBy
    }
  })
}

// 从回收站恢复实体
export const restoreFromRecycleBin = (id: number): Promise<any> => {
  return request.post(`/api/recycle/RestoreFromRecycleBin/${id}/restore`)
}

// 永久删除回收站中的实体
export const permanentlyDelete = (id: number): Promise<void> => {
  return request.delete(`/api/recycle/PermanentlyDelete/${id}`)
}

// 获取回收站记录详情
export const getRecycleBin = (id: number): Promise<RecycleBin> => {
  return request.get(`/api/recycle/GetRecycleBin/${id}`)
}

// 搜索回收站记录
export const searchRecycleBins = (params: SearchRecycleBinParams): Promise<RecycleBin[]> => {
  return request.get('/api/recycle/SearchRecycleBins/search', { params })
}

// 清空回收站
export const clearRecycleBin = (beforeDate?: string): Promise<void> => {
  return request.delete('/api/recycle/ClearRecycleBin/clear', {
    params: beforeDate ? { beforeDate } : undefined
  })
}

// 获取指定实体的回收站记录
export const getEntityRecycleBin = (params: GetEntityRecycleBinParams): Promise<RecycleBin> => {
  return request.get('/api/recycle/GetEntityRecycleBin/entity', { params })
} 