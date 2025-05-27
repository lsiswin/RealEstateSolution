import request from './request'

/**
 * 用户登录
 * @param {Object} data - 登录信息 {username, password}
 * @returns {Promise}
 */
export function login(data) {
  return request({
    url: '/auth/login',
    method: 'post',
    data
  })
}

/**
 * 用户登出
 * @returns {Promise}
 */
export function logout() {
  return request({
    url: '/auth/logout',
    method: 'post',
    data: {} // 添加空对象作为请求体，因为后端需要一个LogoutRequest参数
  })
}

/**
 * 获取用户信息
 * @returns {Promise}
 */
export function getUserInfo() {
  return request({
    url: '/auth/user/info',
    method: 'get'
  })
}

/**
 * 获取所有用户列表
 * @param {Object} query - 查询参数
 * @returns {Promise}
 */
export function getUsers(query) {
  return request({
    url: '/auth/users',
    method: 'get',
    params: query
  })
}

/**
 * 获取单个用户信息
 * @param {number} id - 用户ID
 * @returns {Promise}
 */
export function getUser(id) {
  return request({
    url: `/auth/users/${id}`,
    method: 'get'
  })
}

/**
 * 创建用户
 * @param {Object} data - 用户信息
 * @returns {Promise}
 */
export function createUser(data) {
  return request({
    url: '/auth/users',
    method: 'post',
    data
  })
}

/**
 * 更新用户
 * @param {number} id - 用户ID
 * @param {Object} data - 用户信息
 * @returns {Promise}
 */
export function updateUser(id, data) {
  return request({
    url: `/auth/users/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除用户
 * @param {number} id - 用户ID
 * @returns {Promise}
 */
export function deleteUser(id) {
  return request({
    url: `/auth/users/${id}`,
    method: 'delete'
  })
}

/**
 * 获取所有角色
 * @returns {Promise}
 */
export function getRoles() {
  return request({
    url: '/auth/roles',
    method: 'get'
  })
}

/**
 * 获取所有权限
 * @returns {Promise}
 */
export function getPermissions() {
  return request({
    url: '/auth/permissions',
    method: 'get'
  })
}

/**
 * 获取角色的权限
 * @param {string} roleId - 角色ID
 * @returns {Promise}
 */
export function getRolePermissions(roleId) {
  return request({
    url: `/auth/roles/${roleId}/permissions`,
    method: 'get'
  })
}

/**
 * 为角色分配权限
 * @param {string} roleId - 角色ID
 * @param {Array} permissionIds - 权限ID数组
 * @returns {Promise}
 */
export function assignRolePermissions(roleId, permissionIds) {
  return request({
    url: `/auth/roles/${roleId}/permissions`,
    method: 'post',
    data: { permissionIds }
  })
}

/**
 * 创建角色
 * @param {Object} data - 角色信息
 * @returns {Promise}
 */
export function createRole(data) {
  return request({
    url: '/auth/roles',
    method: 'post',
    data
  })
}

/**
 * 更新角色
 * @param {string} id - 角色ID
 * @param {Object} data - 角色信息
 * @returns {Promise}
 */
export function updateRole(id, data) {
  return request({
    url: `/auth/roles/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除角色
 * @param {string} id - 角色ID
 * @returns {Promise}
 */
export function deleteRole(id) {
  return request({
    url: `/auth/roles/${id}`,
    method: 'delete'
  })
}

/**
 * 修改密码
 * @param {Object} data - {oldPassword, newPassword}
 * @returns {Promise}
 */
export function changePassword(data) {
  return request({
    url: '/auth/change-password',
    method: 'post',
    data
  })
}

/**
 * 更新个人资料
 * @param {Object} data - 用户资料
 * @returns {Promise}
 */
export function updateProfile(data) {
  return request({
    url: '/auth/profile',
    method: 'put',
    data
  })
} 