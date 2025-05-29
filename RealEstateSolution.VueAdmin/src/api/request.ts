import axios, { type AxiosInstance, type InternalAxiosRequestConfig, type AxiosResponse } from 'axios'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'

// 创建axios实例
const request: AxiosInstance = axios.create({
  baseURL: 'http://localhost:5098', // 默认认证服务地址
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求拦截器
request.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const userStore = useUserStore()
    
    // 添加认证token
    if (userStore.token) {
      config.headers = config.headers || {}
      config.headers.Authorization = `Bearer ${userStore.token}`
    }
    
    return config
  },
  (error) => {
    console.error('Request error:', error)
    return Promise.reject(error)
  }
)

// 响应拦截器
request.interceptors.response.use(
  (response: AxiosResponse) => {
    return response.data
  },
  (error) => {
    const userStore = useUserStore()
    
    if (error.response?.status === 401) {
      ElMessage.error('登录已过期，请重新登录')
      userStore.clearUserInfo()
      window.location.href = '/login'
    } else if (error.response?.status >= 500) {
      ElMessage.error('服务器错误，请稍后重试')
    } else if (error.response?.data?.message) {
      ElMessage.error(error.response.data.message)
    } else {
      ElMessage.error('网络错误，请检查网络连接')
    }
    
    return Promise.reject(error)
  }
)

// 扩展axios实例，支持创建不同服务的请求实例
const createInstance = (config?: any) => {
  const instance = axios.create({
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json'
    },
    ...config
  })
  
  // 为新实例添加相同的拦截器
  instance.interceptors.request.use(
    (requestConfig: InternalAxiosRequestConfig) => {
      const userStore = useUserStore()
      
      if (userStore.token) {
        requestConfig.headers = requestConfig.headers || {}
        requestConfig.headers.Authorization = `Bearer ${userStore.token}`
      }
      
      return requestConfig
    },
    (error) => {
      console.error('Request error:', error)
      return Promise.reject(error)
    }
  )
  
  instance.interceptors.response.use(
    (response: AxiosResponse) => {
      return response.data
    },
    (error) => {
      const userStore = useUserStore()
      
      if (error.response?.status === 401) {
        ElMessage.error('登录已过期，请重新登录')
        userStore.clearUserInfo()
        window.location.href = '/login'
      } else if (error.response?.status >= 500) {
        ElMessage.error('服务器错误，请稍后重试')
      } else if (error.response?.data?.message) {
        ElMessage.error(error.response.data.message)
      } else {
        ElMessage.error('网络错误，请检查网络连接')
      }
      
      return Promise.reject(error)
    }
  )
  
  return instance
}

// 添加create方法到request实例
;(request as any).create = createInstance

export default request 