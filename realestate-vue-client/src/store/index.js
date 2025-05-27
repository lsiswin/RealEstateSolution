import { createStore } from 'vuex'
import getters from './getters'

// 导入模块
import app from './modules/app'
import user from './modules/user'
import permission from './modules/permission'
import tagsView from './modules/tagsView'

export default createStore({
  modules: {
    app,
    user,
    permission,
    tagsView
  },
  getters
}) 