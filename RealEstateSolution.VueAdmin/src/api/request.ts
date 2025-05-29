import axios, { type AxiosInstance, type InternalAxiosRequestConfig, type AxiosResponse } from 'axios'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/stores/user'

// 创建axios实例
const request: AxiosInstance = axios.create({
  baseURL: 'https://localhost:5098', // 使用vite代理
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求拦截器
request.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const userStore = useUserStore()
    
    console.log('发送请求:', {
      url: config.url,
      method: config.method,
      baseURL: config.baseURL,
      data: config.data
    })
    
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
    console.log('收到响应:', {
      status: response.status,
      data: response.data,
      url: response.config.url
    })
    return response.data
  },
  (error) => {
    console.error('响应错误详情:', {
      message: error.message,
      code: error.code,
      status: error.response?.status,
      statusText: error.response?.statusText,
      data: error.response?.data,
      url: error.config?.url
    })
    
    const userStore = useUserStore()
    
    if (error.response?.status === 401) {
      ElMessage.error('登录已过期，请重新登录')
      userStore.clearUserInfo()
      window.location.href = '/login'
    } else if (error.response?.status >= 500) {
      ElMessage.error('服务器错误，请稍后重试')
    } else if (error.response?.data?.message) {
      ElMessage.error(error.response.data.message)
    } else if (error.code === 'ECONNABORTED') {
      ElMessage.error('请求超时，请检查网络连接')
    } else if (error.code === 'ERR_NETWORK') {
      ElMessage.error('无法连接到服务器，请检查服务是否启动')
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