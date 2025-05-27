/**
 * @description 判断是否是外部链接
 * @param {string} path - 需要检查的路径
 * @returns {boolean} - 如果是外部链接返回true，否则返回false
 */
export function isExternal(path) {
  // 正则表达式：匹配以http:、https:、mailto:或tel:开头的字符串
  return /^(https?:|mailto:|tel:)/.test(path)
}

/**
 * @description 验证用户名
 * @param {string} str - 需要验证的用户名
 * @returns {boolean} - 如果用户名有效返回true，否则返回false
 */
export function validUsername(str) {
  // 这里可以根据实际需求修改正则表达式
  const validRegex = /^[a-zA-Z0-9_-]{4,16}$/
  return validRegex.test(str)
}

/**
 * @description 验证URL
 * @param {string} url - 需要验证的URL
 * @returns {boolean} - 如果URL有效返回true，否则返回false
 */
export function validURL(url) {
  const reg = /^(https?|ftp):\/\/([a-zA-Z0-9.-]+(:[a-zA-Z0-9.&%$-]+)*@)*((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])){3}|([a-zA-Z0-9-]+\.)*[a-zA-Z0-9-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(:[0-9]+)*(\/($|[a-zA-Z0-9.,?'\\+&%$#=~_-]+))*$/
  return reg.test(url)
}

/**
 * @description 验证小写字母
 * @param {string} str - 需要验证的字符串
 * @returns {boolean} - 如果全是小写字母返回true，否则返回false
 */
export function validLowerCase(str) {
  const reg = /^[a-z]+$/
  return reg.test(str)
}

/**
 * @description 验证大写字母
 * @param {string} str - 需要验证的字符串
 * @returns {boolean} - 如果全是大写字母返回true，否则返回false
 */
export function validUpperCase(str) {
  const reg = /^[A-Z]+$/
  return reg.test(str)
}

/**
 * @description 验证字母
 * @param {string} str - 需要验证的字符串
 * @returns {boolean} - 如果全是字母返回true，否则返回false
 */
export function validAlphabets(str) {
  const reg = /^[A-Za-z]+$/
  return reg.test(str)
}

/**
 * @description 验证邮箱
 * @param {string} email - 需要验证的邮箱
 * @returns {boolean} - 如果邮箱有效返回true，否则返回false
 */
export function validEmail(email) {
  const reg = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
  return reg.test(email)
}

/**
 * @description 验证手机号码
 * @param {string} phone - 需要验证的手机号码
 * @returns {boolean} - 如果手机号码有效返回true，否则返回false
 */
export function validPhone(phone) {
  const reg = /^1[3-9]\d{9}$/
  return reg.test(phone)
}

/**
 * @description 验证身份证号码
 * @param {string} idCard - 需要验证的身份证号码
 * @returns {boolean} - 如果身份证号码有效返回true，否则返回false
 */
export function validIDCard(idCard) {
  const reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/
  return reg.test(idCard)
} 