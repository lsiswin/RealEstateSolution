import axios from 'axios'
import { ElMessage, ElMessageBox } from 'element-plus'
import store from '@/store'
import router from '@/router'

/**
 * 创建axios实例
 * 配置请求基础URL和超时时间
 */
const service = axios.create({
  // API基础URL，根据环境变量设置
  baseURL: process.env.VUE_APP_BASE_API || '/api',
  // 请求超时时间
  timeout: 30000
})

/**
 * 请求拦截器
 * 在请求发送前处理请求配置
 */
service.interceptors.request.use(
  config => {
    // 在发送请求之前做一些处理
    
    // 从Vuex中获取token
    const token = store.getters.token
    if (token) {
      // 让每个请求携带token
      config.headers['Authorization'] = 'Bearer ' + token
    }
    
    // 处理不同的请求方法
    if (config.method === 'get') {
      // 添加时间戳参数，避免缓存
      config.params = {
        ...config.params,
        _t: Date.now()
      }
    }
    
    return config
  },
  error => {
    // 请求错误处理
    console.error('请求拦截器错误:', error)
    return Promise.reject(error)
  }
)

/**
 * 响应拦截器
 * 在收到响应后统一处理响应数据和错误
 */
service.interceptors.response.use(
  // 处理响应数据
  response => {
    const res = response.data
    
    // 如果响应不包含code字段，直接返回数据
    if (res.code === undefined) {
      return res
    }
    
    // 如果返回的不是200，则判断为错误
    if (res.code !== 200) {
      // 显示错误消息
      ElMessage({
        message: res.message || '请求错误',
        type: 'error',
        duration: 5 * 1000
      })

      // 401: 未登录或token过期
      if (res.code === 401) {
        handleUnauthorized()
        return Promise.reject(new Error('未授权，请重新登录'))
      }
      
      // 403: 无权限
      if (res.code === 403) {
        router.push({ path: '/401' })
        return Promise.reject(new Error('无权访问该资源'))
      }
      
      return Promise.reject(new Error(res.message || '请求错误'))
    } else {
      return res
    }
  },
  // 处理响应错误
  error => {
    console.error('响应拦截器错误:', error)
    const { response } = error
    
    if (response) {
      // 根据响应状态码处理错误
      switch (response.status) {
        case 401:
          handleUnauthorized()
          break
        case 403:
          ElMessage({
            message: '无权访问该资源',
            type: 'error',
            duration: 5 * 1000
          })
          router.push({ path: '/401' })
          break
        case 404:
          ElMessage({
            message: '请求的资源不存在',
            type: 'error',
            duration: 5 * 1000
          })
          break
        case 500:
          ElMessage({
            message: '服务器内部错误',
            type: 'error',
            duration: 5 * 1000
          })
          break
        default:
          ElMessage({
            message: error.message || '请求错误',
            type: 'error',
            duration: 5 * 1000
          })
      }
    } else {
      // 处理断网或请求超时
      if (error.message.includes('timeout')) {
        ElMessage({
          message: '请求超时，请检查网络连接',
          type: 'error',
          duration: 5 * 1000
        })
      } else {
        ElMessage({
          message: '网络连接错误，请检查网络设置',
          type: 'error',
          duration: 5 * 1000
        })
      }
    }
    
    return Promise.reject(error)
  }
)

/**
 * 处理未授权情况
 * 当用户token失效或未登录时调用
 */
function handleUnauthorized() {
  // 如果不是登录页面，弹出重新登录提示
  if (router.currentRoute.value.path !== '/login') {
    ElMessageBox.confirm(
      '登录状态已过期，您可以继续留在该页面，或者重新登录',
      '系统提示',
      {
        confirmButtonText: '重新登录',
        cancelButtonText: '取消',
        type: 'warning'
      }
    ).then(() => {
      // 清除用户信息并跳转到登录页
      store.dispatch('user/resetToken').then(() => {
        // 刷新页面，重新获取路由
        location.reload()
      })
    }).catch(() => {
      // 取消操作，用户留在当前页面
    })
  } else {
    // 已在登录页，直接清除token
    store.dispatch('user/resetToken')
  }
}

// 模拟API响应数据
const mockResponses = {
  '/auth/login': (data) => {
    // 模拟登录接口
    if (data.username === 'admin' && data.password === '123456') {
      return {
        code: 200,
        data: {
          token: 'mock-token-' + Date.now()
        },
        message: '登录成功'
      }
    } else {
      return {
        code: 401,
        message: '用户名或密码错误'
      }
    }
  },
  '/auth/user/info': () => {
    // 模拟获取用户信息接口
    return {
      code: 200,
      data: {
        id: 1,
        username: 'admin',
        realName: '管理员',
        avatar: 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif',
        roles: ['admin'],
        permissions: ['*:*:*']
      },
      message: '获取用户信息成功'
    }
  },
  '/auth/logout': () => {
    // 模拟登出接口
    return {
      code: 200,
      data: null,
      message: '登出成功'
    }
  }
}

// 覆盖原始的请求方法，使用模拟数据
const originalRequest = service.request
service.request = function(config) {
  return new Promise((resolve, reject) => {
    const { url, method, data, params } = config
    
    // 模拟网络延迟
    setTimeout(() => {
      // 检查是否有对应的模拟数据处理函数
      if (mockResponses[url]) {
        const responseData = mockResponses[url](method === 'post' ? data : params)
        if (responseData.code === 200) {
          resolve({ data: responseData })
        } else {
          reject({ response: { status: responseData.code }, data: responseData })
        }
      } else {
        // 如果没有对应的模拟数据，调用原始请求方法
        originalRequest.call(service, config).then(resolve).catch(reject)
      }
    }, 500) // 模拟网络延迟500ms
  })
}

export default service 