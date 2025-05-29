// API接口统一导出
export { default as request } from './request'

// 统一导出所有API模块
export * from './user'
export * from './client'
export * from './contract'
export * from './matching'
export * from './recycle'

// 也可以按模块导出
export * as userApi from './user'
export * as clientApi from './client'
export * as contractApi from './contract'
export * as matchingApi from './matching'
export * as recycleApi from './recycle' 