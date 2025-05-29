import request from './request'

// 房产类型枚举
export enum PropertyType {
  Apartment = 0,    // 公寓
  House = 1,        // 别墅
  Commercial = 2,   // 商业
  Office = 3,       // 办公
  Shop = 4,         // 商铺
  Warehouse = 5     // 仓库
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
  Available = 0,    // 可售/可租
  Pending = 1,      // 待定
  Sold = 2,         // 已售
  Rented = 3,       // 已租
  Withdrawn = 4     // 已下架
}

// 房产接口
export interface Property {
  id: number
  title: string
  description: string
  type: PropertyType
  status: PropertyStatus
  price: number
  area: number
  address: string
  city: string
  district: string
  bedrooms?: number
  bathrooms?: number
  floor?: number
  totalFloors?: number
  yearBuilt?: number
  orientation?: string
  decoration?: string
  facilities?: string
  images?: string[]
  ownerId: string
  agentId?: string
  createTime: string
  updateTime: string
  isDeleted: boolean
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
  totalProperties: number
  forSaleProperties: number
  forRentProperties: number
  soldProperties: number
  rentedProperties: number
  offlineProperties: number
  newPropertiesLast30Days: number
  soldPropertiesLast30Days: number
  averagePrice: number
  averageArea: number
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
  data: T
  code?: number
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

// 房源类型文本映射
export const getPropertyTypeText = (type: PropertyType): string => {
  const typeMap = {
    [PropertyType.Apartment]: '公寓',
    [PropertyType.House]: '别墅',
    [PropertyType.Commercial]: '商业',
    [PropertyType.Office]: '办公',
    [PropertyType.Shop]: '商铺',
    [PropertyType.Warehouse]: '仓库'
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

// 房源状态文本映射
export const getPropertyStatusText = (status: PropertyStatus): string => {
  const statusMap = {
    [PropertyStatus.Available]: '可售',
    [PropertyStatus.Pending]: '待定',
    [PropertyStatus.Sold]: '已售',
    [PropertyStatus.Rented]: '已租',
    [PropertyStatus.Withdrawn]: '已下架'
  }
  return statusMap[status] || '未知'
}

// 房源状态颜色映射
export const getPropertyStatusColor = (status: PropertyStatus): string => {
  const colorMap = {
    [PropertyStatus.Available]: 'success',
    [PropertyStatus.Pending]: 'warning',
    [PropertyStatus.Sold]: 'info',
    [PropertyStatus.Rented]: 'info',
    [PropertyStatus.Withdrawn]: 'danger'
  }
  return colorMap[status] || 'info'
}

/**
 * 查询房源列表（分页）
 */
export const queryProperties = (params: PropertyQueryParams = {}): Promise<ApiResponse<PagedList<Property>>> => {
  return request.get('/api/property/GetProperties', { params })
}

/**
 * 获取房源详情
 */
export const getPropertyById = (id: number): Promise<ApiResponse<Property>> => {
  return request.get(`/api/property/GetProperty/${id}`)
}

/**
 * 创建房源
 */
export const createProperty = (property: Partial<Property>): Promise<ApiResponse<Property>> => {
  return request.post('/api/property/RegisterProperty', property)
}

/**
 * 更新房源
 */
export const updateProperty = (id: number, property: Partial<Property>): Promise<ApiResponse<Property>> => {
  return request.put(`/api/property/UpdateProperty/${id}`, property)
}

/**
 * 删除房源
 */
export const deleteProperty = (id: number): Promise<ApiResponse> => {
  return request.delete(`/api/property/DeleteProperty/${id}`)
}

/**
 * 更改房源状态
 */
export const changePropertyStatus = (id: number): Promise<ApiResponse<Property>> => {
  return request.post(`/api/property/ChangePropertyStatus/${id}`)
}

/**
 * 获取房源统计数据
 */
export const getPropertyStats = (): Promise<ApiResponse<PropertyStats>> => {
  return request.get('/api/property/GetPropertyStats')
} 