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

// 角色接口
export interface Role {
  id: string
  name: string
  description?: string
  permissions: Permission[]
}

// 权限接口
export interface Permission {
  id: string
  name: string
  description?: string
  module: string
}

// 分页参数接口
export interface PageParams {
  pageNum: number
  pageSize: number
  username?: string
  realName?: string
  roleId?: number
}

// 分页结果接口
export interface PageResult<T> {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

// 登录请求参数
export interface LoginRequest {
  userName: string
  password: string
}

// 注册请求参数
export interface RegisterRequest {
  username: string
  password: string
  email: string
  realName?: string
}

// 修改密码参数
export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}

// 更新个人信息参数
export interface UpdateProfileRequest {
  userName?: string
  email?: string
}

// 创建用户参数
export interface CreateUserRequest {
  username: string
  password: string
  email: string
  realName?: string
  roleIds?: string[]
}

// 更新用户参数
export interface UpdateUserRequest {
  username?: string
  email?: string
  realName?: string
  roleIds?: string[]
  isActive?: boolean
}

// 创建角色参数
export interface CreateRoleRequest {
  name: string
  description?: string
}

// 更新角色参数
export interface UpdateRoleRequest {
  name?: string
  description?: string
}

// 分配权限参数
export interface AssignPermissionsRequest {
  permissionIds: string[]
}

// 认证响应
export interface AuthResponse {
  success: boolean
  message?: string
  accessToken?: string
  refreshToken?: string
  user?: User
}

// 刷新令牌请求
export interface RefreshTokenRequest {
  refreshToken: string
}

// 登出请求
export interface LogoutRequest {
  refreshToken: string
}

// ===== 认证相关API =====

// 用户登录
export const login = (data: LoginRequest): Promise<AuthResponse> => {
  return request.post('/api/auth/login', data)
}

// 用户注册
export const register = (data: RegisterRequest): Promise<AuthResponse> => {
  return request.post('/api/auth/register', data)
}

// 刷新令牌
export const refreshToken = (refreshToken: string): Promise<AuthResponse> => {
  return request.post('/api/auth/refresh', { refreshToken })
}

// 用户登出
export const logout = (refreshToken: string): Promise<AuthResponse> => {
  return request.post('/api/auth/logout', { refreshToken })
}

// 修改密码
export const changePassword = (data: ChangePasswordRequest): Promise<AuthResponse> => {
  return request.post('/api/auth/change-password', data)
}

// 更新用户资料
export const updateProfile = (data: UpdateProfileRequest): Promise<AuthResponse> => {
  return request.put('/api/auth/profile', data)
} 

// ===== 用户管理API =====

// 获取当前用户信息
export const getCurrentUser = (): Promise<User> => {
  return request.get('/api/auth/user/info')
}

// 获取用户分页列表
export const getUserList = (params: PageParams): Promise<PageResult<User>> => {
  return request.get('/api/auth/users', { params })
}

// 获取单个用户信息
export const getUser = (id: string): Promise<User> => {
  return request.get(`/api/auth/users/${id}`)
}

// 创建用户
export const createUser = (params: CreateUserRequest): Promise<User> => {
  return request.post('/api/auth/users', params)
}

// 更新用户
export const updateUser = (id: string, params: UpdateUserRequest): Promise<User> => {
  return request.put(`/api/auth/users/${id}`, params)
}

// 删除用户
export const deleteUser = (id: string): Promise<void> => {
  return request.delete(`/api/auth/users/${id}`)
}

// ===== 角色管理API =====

// 获取所有角色
export const getRoles = (): Promise<Role[]> => {
  return request.get('/api/auth/roles')
}

// 获取单个角色
export const getRole = (id: string): Promise<Role> => {
  return request.get(`/api/auth/roles/${id}`)
}

// 创建角色
export const createRole = (params: CreateRoleRequest): Promise<Role> => {
  return request.post('/api/auth/roles', params)
}

// 更新角色
export const updateRole = (id: string, params: UpdateRoleRequest): Promise<Role> => {
  return request.put(`/api/auth/roles/${id}`, params)
}

// 删除角色
export const deleteRole = (id: string): Promise<void> => {
  return request.delete(`/api/auth/roles/${id}`)
}

// 获取所有权限
export const getPermissions = (): Promise<Permission[]> => {
  return request.get('/api/auth/permissions')
}

// 获取角色的权限
export const getRolePermissions = (roleId: string): Promise<Permission[]> => {
  return request.get(`/api/auth/roles/${roleId}/permissions`)
}

// 为角色分配权限
export const assignRolePermissions = (roleId: string, params: AssignPermissionsRequest): Promise<void> => {
  return request.post(`/api/auth/roles/${roleId}/permissions`, params)
}

// 获取用户统计信息
export const getUserStats = (): Promise<{
  totalUsers: number
  activeUsers: number
  newUsersThisMonth: number
}> => {
  return request.get('/api/auth/users/stats')
} 