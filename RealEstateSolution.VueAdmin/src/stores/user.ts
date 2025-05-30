import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

// 用户信息接口
export interface UserInfo {
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

export const useUserStore = defineStore('user', () => {
  // 状态
  const token = ref<string>('')
  const refreshToken = ref<string>('')
  const userInfo = ref<UserInfo | null>(null)
  const isLoggedIn = computed(() => !!token.value && !!userInfo.value)

  // 初始化用户信息（从localStorage恢复）
  const initUserInfo = () => {
    const savedToken = localStorage.getItem('token')
    const savedRefreshToken = localStorage.getItem('refreshToken')
    const savedUserInfo = localStorage.getItem('userInfo')

    if (savedToken) {
      token.value = savedToken
    }
    if (savedRefreshToken) {
      refreshToken.value = savedRefreshToken
    }
    if (savedUserInfo) {
      try {
        userInfo.value = JSON.parse(savedUserInfo)
      } catch (error) {
        console.error('解析用户信息失败:', error)
        clearUserInfo()
      }
    }
  }

  // 设置用户信息
  const setUserInfo = (newToken: string, newRefreshToken: string, newUserInfo: UserInfo) => {
    token.value = newToken
    refreshToken.value = newRefreshToken
    userInfo.value = newUserInfo

    // 保存到localStorage
    localStorage.setItem('token', newToken)
    localStorage.setItem('refreshToken', newRefreshToken)
    localStorage.setItem('userInfo', JSON.stringify(newUserInfo))
  }

  // 清除用户信息
  const clearUserInfo = () => {
    token.value = ''
    refreshToken.value = ''
    userInfo.value = null

    // 清除localStorage
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('userInfo')
  }

  // 更新token
  const updateToken = (newToken: string, newRefreshToken?: string) => {
    token.value = newToken
    localStorage.setItem('token', newToken)
    
    if (newRefreshToken) {
      refreshToken.value = newRefreshToken
      localStorage.setItem('refreshToken', newRefreshToken)
    }
  }

  // 检查是否有特定角色
  const hasRole = (role: string): boolean => {
    return userInfo.value?.roles.includes(role) || false
  }

  // 检查是否有任意一个角色
  const hasAnyRole = (roles: string[]): boolean => {
    if (!userInfo.value?.roles) return false
    return roles.some(role => userInfo.value!.roles.includes(role))
  }

  // 检查是否是管理员
  const isAdmin = computed(() => hasRole('admin'))

  return {
    // 状态
    token,
    refreshToken,
    userInfo,
    isLoggedIn,
    isAdmin,
    
    // 方法
    initUserInfo,
    setUserInfo,
    clearUserInfo,
    updateToken,
    hasRole,
    hasAnyRole
  }
}) 