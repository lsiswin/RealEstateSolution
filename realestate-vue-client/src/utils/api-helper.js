import { ElMessage, ElMessageBox } from 'element-plus'
import { ApiResponse } from '@/models/common'

/**
 * 处理API请求
 * @param {Promise} apiPromise API请求Promise
 * @param {Object} options 选项
 * @param {string} options.successMsg 成功消息
 * @param {string} options.errorMsg 错误消息
 * @param {Function} options.transform 数据转换函数
 * @param {boolean} options.showSuccessMsg 是否显示成功消息
 * @param {boolean} options.showErrorMsg 是否显示错误消息
 * @returns {Promise} 处理后的Promise
 */
export async function handleRequest(apiPromise, options = {}) {
  const {
    successMsg = '操作成功',
    errorMsg = '操作失败',
    transform = data => data,
    showSuccessMsg = false,
    showErrorMsg = true
  } = options

  try {
    const response = await apiPromise
    const apiResponse = new ApiResponse(response)
    
    if (apiResponse.isSuccess()) {
      if (showSuccessMsg) {
        ElMessage.success(apiResponse.message || successMsg)
      }
      return transform(apiResponse.data)
    } else {
      if (showErrorMsg) {
        ElMessage.error(apiResponse.getErrorMessage() || errorMsg)
      }
      return Promise.reject(new Error(apiResponse.getErrorMessage() || errorMsg))
    }
  } catch (error) {
    if (showErrorMsg) {
      ElMessage.error(error.message || errorMsg)
    }
    return Promise.reject(error)
  }
}

/**
 * 处理分页查询
 * @param {Promise} apiPromise API请求Promise
 * @param {Function} transform 数据项转换函数
 * @param {boolean} showErrorMsg 是否显示错误消息
 * @returns {Promise} 处理后的Promise
 */
export async function handlePagedQuery(apiPromise, transform = item => item, showErrorMsg = true) {
  try {
    const response = await apiPromise
    const apiResponse = new ApiResponse(response)
    
    if (apiResponse.isSuccess()) {
      // 转换列表数据
      const data = apiResponse.data
      if (data && data.items) {
        data.items = data.items.map(transform)
      }
      return data
    } else {
      if (showErrorMsg) {
        ElMessage.error(apiResponse.getErrorMessage() || '查询失败')
      }
      return Promise.reject(new Error(apiResponse.getErrorMessage() || '查询失败'))
    }
  } catch (error) {
    if (showErrorMsg) {
      ElMessage.error(error.message || '查询失败')
    }
    return Promise.reject(error)
  }
}

/**
 * 处理表单提交
 * @param {Promise} apiPromise API请求Promise
 * @param {string} successMsg 成功消息
 * @param {string} errorMsg 错误消息
 * @param {Function} transform 数据转换函数
 * @returns {Promise} 处理后的Promise
 */
export async function handleFormSubmit(apiPromise, successMsg = '提交成功', errorMsg = '提交失败', transform = data => data) {
  return handleRequest(apiPromise, {
    successMsg,
    errorMsg,
    transform,
    showSuccessMsg: true,
    showErrorMsg: true
  })
}

/**
 * 处理删除操作
 * @param {Promise} apiPromise API请求Promise
 * @param {string} successMsg 成功消息
 * @param {string} errorMsg 错误消息
 * @returns {Promise} 处理后的Promise
 */
export async function handleDelete(apiPromise, successMsg = '删除成功', errorMsg = '删除失败') {
  return handleRequest(apiPromise, {
    successMsg,
    errorMsg,
    showSuccessMsg: true,
    showErrorMsg: true
  })
}

/**
 * 确认操作
 * @param {string} message 确认消息
 * @param {string} title 标题
 * @param {Object} options 选项
 * @returns {Promise} 确认结果Promise
 */
export function confirm(message, title = '提示', options = {}) {
  const defaultOptions = {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }
  
  return ElMessageBox.confirm(
    message,
    title,
    {
      ...defaultOptions,
      ...options
    }
  )
}

/**
 * 确认删除操作
 * @param {string} message 确认消息
 * @returns {Promise} 确认结果Promise
 */
export function confirmDelete(message = '确定要删除所选记录吗？') {
  return confirm(message, '删除确认', { type: 'error' })
}

/**
 * 批量处理请求
 * @param {Array} promises Promise数组
 * @param {Object} options 选项
 * @returns {Promise} 处理后的Promise
 */
export async function handleBatchRequests(promises, options = {}) {
  const {
    successMsg = '批量操作成功',
    errorMsg = '批量操作部分失败',
    showSuccessMsg = true,
    continueOnError = false
  } = options
  
  try {
    if (continueOnError) {
      // 允许部分失败，返回所有结果
      const results = await Promise.allSettled(promises)
      const successCount = results.filter(r => r.status === 'fulfilled').length
      const failCount = results.length - successCount
      
      if (failCount > 0) {
        ElMessage({
          message: `${successCount}个操作成功，${failCount}个操作失败`,
          type: 'warning'
        })
      } else if (showSuccessMsg) {
        ElMessage.success(successMsg)
      }
      
      return results
    } else {
      // 要求全部成功
      const results = await Promise.all(promises)
      if (showSuccessMsg) {
        ElMessage.success(successMsg)
      }
      return results
    }
  } catch (error) {
    ElMessage.error(error.message || errorMsg)
    return Promise.reject(error)
  }
} 