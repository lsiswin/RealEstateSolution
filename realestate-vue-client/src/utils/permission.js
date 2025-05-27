import store from '@/store'

/**
 * 判断是否有权限
 * @param {string} permission 权限字符串
 * @returns {boolean}
 */
export function hasPermission(permission) {
  // 从store中获取用户角色
  const roles = store.getters.roles
  
  // 如果是管理员角色，默认拥有所有权限
  if (roles.includes('admin')) {
    return true
  }
  
  return false
}

/**
 * 检查用户是否拥有指定角色
 * @param {string|Array} role 角色标识，可以是单个角色字符串或角色数组
 * @returns {boolean} 是否拥有角色
 */
export function hasRole(role) {
  const userRoles = store.getters.roles || []
  
  // 如果角色参数是数组，则检查是否拥有其中任意一个角色
  if (Array.isArray(role)) {
    return role.some(r => userRoles.includes(r))
  }
  
  // 如果角色参数是字符串，则直接检查
  return userRoles.includes(role)
}

/**
 * 检查用户是否是经纪人
 * @returns {boolean} 是否是经纪人
 */
export function isAgent() {
  return hasRole('Agent')
}

/**
 * 检查用户是否是管理员
 * @returns {boolean} 是否是管理员
 */
export function isAdmin() {
  return hasRole('admin')
}

/**
 * 检查用户是否有权限管理房源
 * @param {number} ownerId 房源所有者ID
 * @returns {boolean} 是否有权限管理
 */
export function canManageProperty(ownerId) {
  // 管理员或经纪人可以管理所有房源
  if (isAdmin() || isAgent()) {
    return true
  }
  
  // 普通用户只能管理自己的房源
  const userId = store.getters.userId
  return userId && ownerId && userId.toString() === ownerId.toString()
}

/**
 * 检查用户是否有权限管理客户
 * @param {number} agentId 客户所属经纪人ID
 * @returns {boolean} 是否有权限管理
 */
export function canManageClient(agentId) {
  // 管理员可以管理所有客户
  if (isAdmin()) {
    return true
  }
  
  // 经纪人只能管理自己的客户
  const userId = store.getters.userId
  return userId && agentId && userId.toString() === agentId.toString()
}

/**
 * 权限指令 - 用于控制DOM元素的显示
 */
export const permission = {
  mounted(el, binding) {
    const { value } = binding
    if (value && typeof value === 'string') {
      const hasAuth = hasPermission(value)
      if (!hasAuth) {
        el.parentNode?.removeChild(el)
      }
    } else {
      throw new Error(`权限值错误: ${value}`)
    }
  }
}

/**
 * 角色指令 - 用于基于角色控制DOM元素的显示
 */
export const role = {
  mounted(el, binding) {
    const { value } = binding
    if (value && (Array.isArray(value) || typeof value === 'string')) {
      // 从store中获取用户角色
      const roles = store.getters.roles
      
      // 将字符串转换为数组
      const requiredRoles = Array.isArray(value) ? value : [value]
      
      // 检查用户是否有必要的角色
      const hasRole = roles.some(role => requiredRoles.includes(role))
      
      if (!hasRole) {
        el.parentNode?.removeChild(el)
      }
    } else {
      throw new Error(`角色值错误: ${value}`)
    }
  }
} 