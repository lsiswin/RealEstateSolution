import { createRouter, createWebHistory } from 'vue-router'
import { ElMessage } from 'element-plus'
import Layout from '@/views/Layout.vue'
import { useUserStore } from '@/stores/user'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // 登录注册路由
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/Auth/Login.vue'),
      meta: { title: '登录', requiresAuth: false }
    },
    {
      path: '/register',
      name: 'Register',
      component: () => import('@/views/Auth/Register.vue'),
      meta: { title: '注册', requiresAuth: false }
    },
    // 主应用路由
    {
      path: '/',
      component: Layout,
      redirect: '/dashboard',
      meta: { requiresAuth: true },
      children: [
        {
          path: '/dashboard',
          name: 'Dashboard',
          component: () => import('@/views/Dashboard.vue').catch(
            (error) => {
              console.error('Dashboard loading error:', error)
              ElMessage.error('加载失败，请重试')
              return import('@/views/Dashboard.vue')
            }
          ),
          meta: { title: '首页', icon: 'House', requiresAuth: true }
        },
        // 房源管理
        {
          path: '/property',
          name: 'Property',
          component: () => import('@/views/Property/index.vue'),
          redirect: '/property/list',
          meta: { title: '房源管理', icon: 'OfficeBuilding', requiresAuth: true },
          children: [
            {
              path: 'list',
              name: 'PropertyList',
              component: () => import('@/views/Property/PropertyList.vue'),
              meta: { title: '房源列表', icon: 'List', requiresAuth: true }
            },
            {
              path: 'add',
              name: 'PropertyAdd',
              component: () => import('@/views/Property/PropertyAdd.vue'),
              meta: { title: '添加房源', icon: 'Plus', requiresAuth: true }
            }
          ]
        },
        // 客户管理
        {
          path: '/client',
          name: 'Client',
          component: () => import('@/views/Client/index.vue'),
          redirect: '/client/list',
          meta: { title: '客户管理', icon: 'User', requiresAuth: true },
          children: [
            {
              path: 'list',
              name: 'ClientList',
              component: () => import('@/views/Client/ClientList.vue'),
              meta: { title: '客户列表', icon: 'List', requiresAuth: true }
            },
            {
              path: 'requirements',
              name: 'ClientRequirements',
              component: () => import('@/views/Client/ClientRequirements.vue'),
              meta: { title: '客户需求', icon: 'Document', requiresAuth: true }
            }
          ]
        },
        // 合同管理
        {
          path: '/contract',
          name: 'Contract',
          component: () => import('@/views/Contract/index.vue'),
          redirect: '/contract/list',
          meta: { title: '合同管理', icon: 'Document', requiresAuth: true },
          children: [
            {
              path: 'list',
              name: 'ContractList',
              component: () => import('@/views/Contract/ContractList.vue'),
              meta: { title: '合同列表', icon: 'List', requiresAuth: true }
            },
            {
              path: 'templates',
              name: 'ContractTemplates',
              component: () => import('@/views/Contract/ContractTemplates.vue'),
              meta: { title: '合同模板', icon: 'Files', requiresAuth: true }
            }
          ]
        },
        // 智能匹配
        {
          path: '/matching',
          name: 'Matching',
          component: () => import('@/views/Matching/index.vue'),
          redirect: '/matching/results',
          meta: { title: '智能匹配', icon: 'Connection', requiresAuth: true },
          children: [
            {
              path: 'results',
              name: 'MatchingResults',
              component: () => import('@/views/Matching/MatchingResults.vue'),
              meta: { title: '匹配结果', icon: 'List', requiresAuth: true }
            },
            {
              path: 'settings',
              name: 'MatchingSettings',
              component: () => import('@/views/Matching/MatchingSettings.vue'),
              meta: { title: '匹配设置', icon: 'Setting', requiresAuth: true }
            }
          ]
        },
        // 回收站
        {
          path: '/recycle',
          name: 'Recycle',
          component: () => import('@/views/Recycle/index.vue'),
          redirect: '/recycle/list',
          meta: { title: '回收站', icon: 'Delete', requiresAuth: true },
          children: [
            {
              path: 'list',
              name: 'RecycleList',
              component: () => import('@/views/Recycle/RecycleList.vue'),
              meta: { title: '回收站列表', icon: 'List', requiresAuth: true }
            }
          ]
        },
        // 系统管理
        {
          path: '/system',
          name: 'System',
          component: () => import('@/views/System/index.vue'),
          redirect: '/system/users',
          meta: { title: '系统管理', icon: 'Setting', requiresAuth: true },
          children: [
            {
              path: 'users',
              name: 'SystemUsers',
              component: () => import('@/views/System/SystemUsers.vue'),
              meta: { title: '用户管理', icon: 'User', requiresAuth: true }
            },
            {
              path: 'roles',
              name: 'SystemRoles',
              component: () => import('@/views/System/SystemRoles.vue'),
              meta: { title: '角色管理', icon: 'UserFilled', requiresAuth: true }
            },
            {
              path: 'logs',
              name: 'SystemLogs',
              component: () => import('@/views/System/SystemLogs.vue'),
              meta: { title: '系统日志', icon: 'Document', requiresAuth: true }
            }
          ]
        }
      ]
    },
    // 404 页面
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('@/views/NotFound.vue'),
      meta: { title: '页面不存在', requiresAuth: false }
    }
  ]
})

// 路由守卫
router.beforeEach((to, from, next) => {
  const userStore = useUserStore()
  
  console.log('路由守卫检查:', {
    to: to.path,
    from: from.path,
    isLoggedIn: userStore.isLoggedIn,
    token: !!userStore.token,
    userInfo: !!userStore.userInfo,
    requiresAuth: to.meta.requiresAuth
  })
  
  // 检查是否需要认证
  if (to.meta.requiresAuth !== false) {
    if (!userStore.isLoggedIn) {
      console.log('用户未登录，重定向到登录页')
      ElMessage.warning('请先登录')
      next('/login')
      return
    }
  }
  
  // 如果已登录用户访问登录页，重定向到首页
  if (to.path === '/login' && userStore.isLoggedIn) {
    console.log('已登录用户访问登录页，重定向到首页')
    next('/')
    return
  }
  
  console.log('路由守卫通过，继续导航')
  next()
})

// 路由错误处理
router.onError((error) => {
  console.error('Router error:', error)
  ElMessage.error('页面加载失败，请刷新重试')
})

export default router 