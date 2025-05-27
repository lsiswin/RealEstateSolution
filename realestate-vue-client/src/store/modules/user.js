import { ElMessage } from 'element-plus'

// 获取token
const getToken = () => {
  return localStorage.getItem('token')
}

// 设置token
const setToken = (token) => {
  localStorage.setItem('token', token)
}

// 移除token
const removeToken = () => {
  localStorage.removeItem('token')
}

// 初始状态
const state = {
  token: getToken(),
  username: '',
  realName: '',
  avatar: '',
  roles: []
}

// mutations
const mutations = {
  SET_TOKEN: (state, token) => {
    state.token = token
  },
  SET_USERNAME: (state, username) => {
    state.username = username
  },
  SET_REALNAME: (state, realName) => {
    state.realName = realName
  },
  SET_AVATAR: (state, avatar) => {
    state.avatar = avatar
  },
  SET_ROLES: (state, roles) => {
    state.roles = roles
  }
}

// actions
const actions = {
  // 用户登录
  login({ commit }, userInfo) {
    const { username, password } = userInfo
    
    return new Promise((resolve, reject) => {
      // 模拟登录接口
      if (username === 'admin' && password === '123456') {
        // 登录成功
        const token = 'admin-token'
        commit('SET_TOKEN', token)
        setToken(token)
        
        // 设置用户信息
        commit('SET_USERNAME', username)
        commit('SET_REALNAME', '管理员')
        commit('SET_AVATAR', 'https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif')
        commit('SET_ROLES', ['admin'])
        
        resolve()
      } else {
        reject(new Error('用户名或密码错误'))
      }
    })
  },

  // 退出登录
  logout({ commit }) {
    return new Promise(resolve => {
      commit('SET_TOKEN', '')
      commit('SET_ROLES', [])
      removeToken()
      resolve()
    })
  },

  // 重置token
  resetToken({ commit }) {
    return new Promise(resolve => {
      commit('SET_TOKEN', '')
      commit('SET_ROLES', [])
      removeToken()
      resolve()
    })
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
} 