import request from './request'

// 登录请求接口
export interface LoginRequest {
  userName: string
  password: string
}

// 注册请求接口
export interface RegisterRequest {
  userName: string
  password: string
  email: string
  phoneNumber?: string
  realName?: string
}

// 修改密码请求接口
export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}

// 更新资料请求接口
export interface UpdateProfileRequest {
  userName: string
  email: string
}

// 认证响应接口
export interface AuthResponse {
  success: boolean
  message: string
  accessToken?: string
  refreshToken?: string
  expiresAt?: string
  user?: {
    id: string
    userName: string
    email: string
    realName?: string
    roles?: string[]
  }
}

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