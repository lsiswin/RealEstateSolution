import request from './request'

/**
 * 获取客户列表
 * @param {Object} query - 查询参数
 * @returns {Promise}
 */
export function getClients(query) {
  return request({
    url: '/client',
    method: 'get',
    params: query
  })
}

/**
 * 获取单个客户详情
 * @param {number} id - 客户ID
 * @returns {Promise}
 */
export function getClient(id) {
  return request({
    url: `/client/${id}`,
    method: 'get'
  })
}

/**
 * 创建客户
 * @param {Object} data - 客户信息
 * @returns {Promise}
 */
export function createClient(data) {
  return request({
    url: '/client',
    method: 'post',
    data
  })
}

/**
 * 更新客户
 * @param {number} id - 客户ID
 * @param {Object} data - 客户信息
 * @returns {Promise}
 */
export function updateClient(id, data) {
  return request({
    url: `/client/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除客户
 * @param {number} id - 客户ID
 * @returns {Promise}
 */
export function deleteClient(id) {
  return request({
    url: `/client/${id}`,
    method: 'delete'
  })
}

/**
 * 获取客户需求
 * @param {number} id - 客户ID
 * @returns {Promise}
 */
export function getClientRequirements(id) {
  return request({
    url: `/client/${id}/requirements`,
    method: 'get'
  })
}

/**
 * 更新客户需求
 * @param {number} id - 客户ID
 * @param {Object} data - 需求信息
 * @returns {Promise}
 */
export function updateClientRequirements(id, data) {
  return request({
    url: `/client/${id}/requirements`,
    method: 'put',
    data
  })
}

/**
 * 获取客户统计数据
 * @returns {Promise}
 */
export function getClientStats() {
  return request({
    url: '/client/stats',
    method: 'get'
  })
} 