import request from './request'

// 用户信息接口
export interface User {
  id: string
  userName: string
  email: string
  realName?: string
  phone?: string
  roles: string[]
  isActive: boolean
  createTime: string
  lastLoginTime?: string
}

// 分页参数接口
export interface PageParams {
  pageIndex: number
  pageSize: number
  keyword?: string
}

// 分页结果接口
export interface PageResult<T> {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

// 修改密码参数
export interface ChangePasswordParams {
  oldPassword: string
  newPassword: string
  confirmPassword: string
}

// 更新个人信息参数
export interface UpdateProfileParams {
  realName?: string
  phone?: string
  email?: string
}

// 获取用户分页列表
export const getUserList = (params: PageParams): Promise<PageResult<User>> => {
  return request.get('/api/users', { params })
}

// 获取当前用户信息
export const getCurrentUser = (): Promise<User> => {
  return request.get('/api/users/current')
}

// 修改密码
export const changePassword = (params: ChangePasswordParams): Promise<void> => {
  return request.post('/api/users/change-password', params)
}

// 更新个人信息
export const updateProfile = (params: UpdateProfileParams): Promise<User> => {
  return request.put('/api/users/profile', params)
}

// 获取用户统计信息
export const getUserStats = (): Promise<{
  totalUsers: number
  activeUsers: number
  newUsersThisMonth: number
}> => {
  return request.get('/api/users/stats')
} 