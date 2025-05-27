import request from './request'

// 房产类型枚举
export enum PropertyType {
  Residential = 1, // 住宅
  Commercial = 2,  // 商业
  Office = 3,      // 办公
  Industrial = 4,  // 工业
  Land = 5         // 土地
}

// 装修类型枚举
export enum DecorationType {
  Rough = 1,   // 毛坯
  Simple = 2,  // 简装
  Fine = 3,    // 精装
  Luxury = 4   // 豪装
}

// 朝向类型枚举
export enum OrientationType {
  East = 1,      // 东
  South = 2,     // 南
  West = 3,      // 西
  North = 4,     // 北
  Southeast = 5, // 东南
  Northeast = 6, // 东北
  Southwest = 7, // 西南
  Northwest = 8  // 西北
}

// 房产状态枚举
export enum PropertyStatus {
  ForSale = 1,   // 待售
  Sold = 2,      // 已售
  ForRent = 3,   // 待租
  Rented = 4,    // 已租
  Offline = 5,   // 下架
  Available = 6  // 可用
}

// 房产接口
export interface Property {
  id: number
  type: PropertyType
  title: string
  description?: string
  price: number
  area: number
  address: string
  decoration: DecorationType
  orientation: OrientationType
  floor: number
  totalFloors: number
  rooms: number
  bathrooms: number
  status: PropertyStatus
  createTime: string
  updateTime: string
  ownerId: string
  images?: PropertyImage[]
}

// 房产图片接口
export interface PropertyImage {
  id: number
  propertyId: number
  imageUrl: string
  description?: string
  isMain: boolean
  createTime: string
}

// 房产统计接口
export interface PropertyStats {
  totalCount: number
  forSaleCount: number
  soldCount: number
  forRentCount: number
  rentedCount: number
  averagePrice: number
  totalValue: number
}

// 分页列表接口
export interface PagedList<T> {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

// API响应接口
export interface ApiResponse<T = any> {
  success: boolean
  message: string
  data?: T
}

// 房产查询参数
export interface PropertyQueryParams {
  type?: PropertyType
  minPrice?: number
  maxPrice?: number
  minArea?: number
  maxArea?: number
  status?: PropertyStatus
  keyword?: string
  pageIndex?: number
  pageSize?: number
}

// 房产状态更新DTO
export interface PropertyStatusUpdateDto {
  status: PropertyStatus
}

// 登记新房源
export const registerProperty = (property: Omit<Property, 'id' | 'createTime' | 'updateTime'>): Promise<ApiResponse<Property>> => {
  return request.post('/api/property/RegisterProperty', property)
}

// 修改房源信息
export const updateProperty = (id: number, property: Partial<Property>): Promise<ApiResponse<Property>> => {
  return request.put(`/api/property/UpdateProperty/${id}`, property)
}

// 变更房源状态
export const changePropertyStatus = (id: number, status: PropertyStatus): Promise<ApiResponse<Property>> => {
  return request.post(`/api/property/ChangePropertyStatus/${id}`, { status })
}

// 获取房源详情
export const getProperty = (id: number): Promise<ApiResponse<Property>> => {
  return request.get(`/api/property/GetProperty/${id}`)
}

// 查询房源列表
export const queryProperties = (params: PropertyQueryParams = {}): Promise<ApiResponse<PagedList<Property>>> => {
  return request.get('/api/property/QueryProperties', { params })
}

// 删除房源
export const deleteProperty = (id: number): Promise<ApiResponse> => {
  return request.delete(`/api/property/DeleteProperty/${id}`)
}

// 获取房源统计数据
export const getPropertyStats = (): Promise<ApiResponse<PropertyStats>> => {
  return request.get('/api/property/GetPropertyStats')
}

// 枚举转换工具函数
export const getPropertyTypeText = (type: PropertyType): string => {
  const typeMap = {
    [PropertyType.Residential]: '住宅',
    [PropertyType.Commercial]: '商业',
    [PropertyType.Office]: '办公',
    [PropertyType.Industrial]: '工业',
    [PropertyType.Land]: '土地'
  }
  return typeMap[type] || '未知'
}

export const getDecorationTypeText = (decoration: DecorationType): string => {
  const decorationMap = {
    [DecorationType.Rough]: '毛坯',
    [DecorationType.Simple]: '简装',
    [DecorationType.Fine]: '精装',
    [DecorationType.Luxury]: '豪装'
  }
  return decorationMap[decoration] || '未知'
}

export const getOrientationTypeText = (orientation: OrientationType): string => {
  const orientationMap = {
    [OrientationType.East]: '东',
    [OrientationType.South]: '南',
    [OrientationType.West]: '西',
    [OrientationType.North]: '北',
    [OrientationType.Southeast]: '东南',
    [OrientationType.Northeast]: '东北',
    [OrientationType.Southwest]: '西南',
    [OrientationType.Northwest]: '西北'
  }
  return orientationMap[orientation] || '未知'
}

export const getPropertyStatusText = (status: PropertyStatus): string => {
  const statusMap = {
    [PropertyStatus.ForSale]: '待售',
    [PropertyStatus.Sold]: '已售',
    [PropertyStatus.ForRent]: '待租',
    [PropertyStatus.Rented]: '已租',
    [PropertyStatus.Offline]: '下架',
    [PropertyStatus.Available]: '可用'
  }
  return statusMap[status] || '未知'
}

export const getPropertyStatusColor = (status: PropertyStatus): string => {
  const colorMap = {
    [PropertyStatus.ForSale]: 'success',
    [PropertyStatus.Sold]: 'info',
    [PropertyStatus.ForRent]: 'warning',
    [PropertyStatus.Rented]: 'info',
    [PropertyStatus.Offline]: 'danger',
    [PropertyStatus.Available]: 'success'
  }
  return colorMap[status] || 'info'
} 