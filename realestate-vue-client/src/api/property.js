import request from './request'

/**
 * 获取房源列表
 * @param {Object} query - 查询参数
 * @returns {Promise}
 */
export function getProperties(query) {
  return request({
    url: '/property',
    method: 'get',
    params: query
  })
}

/**
 * 获取单个房源详情
 * @param {number} id - 房源ID
 * @returns {Promise}
 */
export function getProperty(id) {
  return request({
    url: `/property/${id}`,
    method: 'get'
  })
}

/**
 * 创建房源
 * @param {Object} data - 房源信息
 * @returns {Promise}
 */
export function createProperty(data) {
  return request({
    url: '/property',
    method: 'post',
    data
  })
}

/**
 * 更新房源
 * @param {number} id - 房源ID
 * @param {Object} data - 房源信息
 * @returns {Promise}
 */
export function updateProperty(id, data) {
  return request({
    url: `/property/${id}`,
    method: 'put',
    data
  })
}

/**
 * 删除房源
 * @param {number} id - 房源ID
 * @returns {Promise}
 */
export function deleteProperty(id) {
  return request({
    url: `/property/${id}`,
    method: 'delete'
  })
}

/**
 * 上传房源图片
 * @param {number} id - 房源ID
 * @param {FormData} formData - 包含图片的表单数据
 * @returns {Promise}
 */
export function uploadPropertyImage(id, formData) {
  return request({
    url: `/property/${id}/images`,
    method: 'post',
    data: formData,
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 删除房源图片
 * @param {number} propertyId - 房源ID
 * @param {number} imageId - 图片ID
 * @returns {Promise}
 */
export function deletePropertyImage(propertyId, imageId) {
  return request({
    url: `/property/${propertyId}/images/${imageId}`,
    method: 'delete'
  })
}

/**
 * 获取房源统计数据
 * @returns {Promise}
 */
export function getPropertyStats() {
  return request({
    url: '/property/stats',
    method: 'get'
  })
} 