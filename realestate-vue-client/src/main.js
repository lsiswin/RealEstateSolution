import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import router from './router'
import store from './store'
import { permission, role } from '@/utils/permission'
import { formatDate, formatMoney } from '@/models/common'

const app = createApp(App)

// 注册所有图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component)
}

// 注册自定义指令
app.directive('permission', permission)
app.directive('role', role)

// 注册全局过滤器
app.config.globalProperties.$filters = {
    formatDate,
    formatMoney
}

app.use(ElementPlus)
app.use(router)
app.use(store)

// 初始化时检查登录状态
const token = localStorage.getItem('token')
if (token) {
    // 如果有token，确保生成路由
    store.dispatch('permission/generateRoutes', store.state.user.roles || ['admin'])
}

app.mount('#app')
