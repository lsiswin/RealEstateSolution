/**
 * API响应模型 - 对应后端的ApiResponse类
 */
export class ApiResponse {
  constructor(data = {}) {
    this.success = data.success !== undefined ? data.success : false;
    this.code = data.code || 0;
    this.message = data.message || '';
    this.data = data.data || null;
  }

  /**
   * 判断响应是否成功
   * @returns {boolean} 是否成功
   */
  isSuccess() {
    return this.success || this.code === 200;
  }

  /**
   * 获取错误消息
   * @returns {string} 错误消息
   */
  getErrorMessage() {
    return this.message || '请求失败';
  }
}

/**
 * 分页请求参数
 */
export class PagedRequest {
  constructor(data = {}) {
    this.pageIndex = data.pageIndex || 1;
    this.pageSize = data.pageSize || 10;
  }

  /**
   * 转换为请求参数对象
   * @returns {Object} 请求参数对象
   */
  toRequestParams() {
    return {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize
    };
  }
}

/**
 * 分页结果 - 对应后端的PagedList类
 */
export class PagedList {
  constructor(data = {}) {
    this.items = data.items || [];
    this.total = data.total || 0;
    this.pageIndex = data.pageIndex || 1;
    this.pageSize = data.pageSize || 10;
    this.totalPages = Math.ceil(this.total / this.pageSize) || 1;
  }

  /**
   * 判断是否有下一页
   * @returns {boolean} 是否有下一页
   */
  hasNextPage() {
    return this.pageIndex < this.totalPages;
  }

  /**
   * 判断是否有上一页
   * @returns {boolean} 是否有上一页
   */
  hasPreviousPage() {
    return this.pageIndex > 1;
  }
}

/**
 * 格式化日期
 * @param {Date|string|number} date 日期对象、日期字符串或时间戳
 * @param {string} fmt 格式字符串，默认为 'yyyy-MM-dd HH:mm:ss'
 * @returns {string} 格式化后的日期字符串
 */
export function formatDate(date, fmt = 'yyyy-MM-dd HH:mm:ss') {
  if (!date) return ''
  
  // 如果是时间戳或日期字符串，转换为Date对象
  if (!(date instanceof Date)) {
    date = new Date(date)
  }
  
  // 如果是无效日期，返回空字符串
  if (isNaN(date.getTime())) {
    return ''
  }
  
  const o = {
    'M+': date.getMonth() + 1, // 月份
    'd+': date.getDate(), // 日
    'H+': date.getHours(), // 小时
    'm+': date.getMinutes(), // 分
    's+': date.getSeconds(), // 秒
    'q+': Math.floor((date.getMonth() + 3) / 3), // 季度
    'S': date.getMilliseconds() // 毫秒
  }
  
  if (/(y+)/.test(fmt)) {
    fmt = fmt.replace(RegExp.$1, (date.getFullYear() + '').substr(4 - RegExp.$1.length))
  }
  
  for (const k in o) {
    if (new RegExp('(' + k + ')').test(fmt)) {
      fmt = fmt.replace(
        RegExp.$1,
        RegExp.$1.length === 1 ? o[k] : ('00' + o[k]).substr(('' + o[k]).length)
      )
    }
  }
  
  return fmt
}

/**
 * 格式化金额
 * @param {number} money 金额
 * @param {number} decimals 小数位数，默认为2
 * @param {string} separator 千位分隔符，默认为','
 * @returns {string} 格式化后的金额字符串
 */
export function formatMoney(money, decimals = 2, separator = ',') {
  if (money === undefined || money === null || isNaN(money)) {
    return '0.00'
  }
  
  // 将money转换为数字
  money = parseFloat(money)
  
  // 格式化为指定小数位
  const formatted = money.toFixed(decimals)
  
  // 分割整数部分和小数部分
  const parts = formatted.split('.')
  const integerPart = parts[0]
  const decimalPart = parts.length > 1 ? '.' + parts[1] : ''
  
  // 添加千位分隔符
  const regex = /(\d+)(\d{3})/
  let formattedIntegerPart = integerPart
  while (regex.test(formattedIntegerPart)) {
    formattedIntegerPart = formattedIntegerPart.replace(regex, '$1' + separator + '$2')
  }
  
  return formattedIntegerPart + decimalPart
}

/**
 * 防抖函数
 * @param {Function} fn 要执行的函数
 * @param {number} delay 延迟时间（毫秒）
 * @returns {Function} 防抖处理后的函数
 */
export function debounce(fn, delay) {
  let timer = null;
  return function() {
    const context = this;
    const args = arguments;
    clearTimeout(timer);
    timer = setTimeout(() => {
      fn.apply(context, args);
    }, delay);
  };
}

/**
 * 节流函数
 * @param {Function} fn 要执行的函数
 * @param {number} delay 延迟时间（毫秒）
 * @returns {Function} 节流处理后的函数
 */
export function throttle(fn, delay) {
  let lastCall = 0;
  return function() {
    const now = new Date().getTime();
    if (now - lastCall < delay) {
      return;
    }
    lastCall = now;
    return fn.apply(this, arguments);
  };
}

/**
 * 深拷贝对象
 * @param {Object} obj 要拷贝的对象
 * @returns {Object} 拷贝后的新对象
 */
export function deepClone(obj) {
  if (obj === null || typeof obj !== 'object') {
    return obj;
  }
  
  // 处理Date对象
  if (obj instanceof Date) {
    return new Date(obj.getTime());
  }
  
  // 处理Array对象
  if (Array.isArray(obj)) {
    return obj.map(item => deepClone(item));
  }
  
  // 处理普通对象
  const result = {};
  for (const key in obj) {
    if (Object.prototype.hasOwnProperty.call(obj, key)) {
      result[key] = deepClone(obj[key]);
    }
  }
  
  return result;
}

/**
 * 生成随机字符串
 * @param {number} length 字符串长度
 * @returns {string} 随机字符串
 */
export function randomString(length = 8) {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  let result = '';
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
} 