import { createRouter, createWebHistory } from 'vue-router'
import Layout from '@/layout/index.vue'
import store from '@/store'

/**
 * 路由配置
 * 
 * meta: {
 *   title: 'title'           // 页面标题，显示在浏览器标签上
 *   icon: 'icon-name'        // 图标名称
 *   requiresAuth: true       // 是否需要登录
 *   roles: ['admin']         // 角色权限控制
 * }
 */

// 基础路由 - 不需要布局的路由
export const constantRoutes = [
  {
    path: '/login',
    component: () => import('@/views/login/index.vue'),
    meta: { title: '登录' },
    hidden: true
  },
  {
    path: '/test',
    component: () => import('@/views/test/index.vue'),
    meta: { title: '测试页面' },
    hidden: true
  },
  {
    path: '/404',
    component: () => import('@/views/error/404.vue'),
    meta: { title: '404' },
    hidden: true
  },
  {
    path: '/redirect',
    component: Layout,
    hidden: true,
    children: [
      {
        path: '/redirect/:path(.*)',
        component: () => import('@/views/redirect/index.vue')
      }
    ]
  }
]

// 主要路由 - 需要布局的路由
export const asyncRoutes = [
  {
    path: '/',
    component: Layout,
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/index.vue'),
        meta: { title: '首页', icon: 'HomeFilled', requiresAuth: true }
      }
    ]
  },
  // 用户权限管理
  {
    path: '/system',
    component: Layout,
    redirect: '/system/users',
    name: 'System',
    meta: { title: '系统管理', icon: 'Setting', roles: ['admin'], requiresAuth: true },
    children: [
      {
        path: 'users',
        name: 'Users',
        component: () => import('@/views/system/users/index.vue'),
        meta: { title: '用户管理', icon: 'User', requiresAuth: true }
      },
      {
        path: 'roles',
        name: 'Roles',
        component: () => import('@/views/system/roles/index.vue'),
        meta: { title: '角色权限', icon: 'Lock', requiresAuth: true }
      }
    ]
  },
  // 房源管理
  {
    path: '/property',
    component: Layout,
    redirect: '/property/list',
    name: 'Property',
    meta: { title: '房源管理', icon: 'House', requiresAuth: true },
    children: [
      {
        path: 'list',
        name: 'PropertyList',
        component: () => import('@/views/property/list/index.vue'),
        meta: { title: '房源列表', icon: 'List', requiresAuth: true }
      },
      {
        path: 'add',
        name: 'PropertyAdd',
        component: () => import('@/views/property/form/index.vue'),
        meta: { title: '添加房源', icon: 'Plus', requiresAuth: true }
      },
      {
        path: 'edit/:id',
        name: 'PropertyEdit',
        component: () => import('@/views/property/form/index.vue'),
        meta: { title: '编辑房源', icon: 'Edit', requiresAuth: true },
        hidden: true
      },
      {
        path: 'detail/:id',
        name: 'PropertyDetail',
        component: () => import('@/views/property/detail/index.vue'),
        meta: { title: '房源详情', icon: 'InfoFilled', requiresAuth: true },
        hidden: true
      }
    ]
  },
  // 客户管理
  {
    path: '/client',
    component: Layout,
    redirect: '/client/list',
    name: 'Client',
    meta: { title: '客户管理', icon: 'User', requiresAuth: true },
    children: [
      {
        path: 'list',
        name: 'ClientList',
        component: () => import('@/views/client/list/index.vue'),
        meta: { title: '客户列表', icon: 'List', requiresAuth: true }
      }
    ]
  },
  // 匹配度为100%时使用
  { path: '/:pathMatch(.*)*', redirect: '/404', hidden: true }
]

// 合并所有路由
const routes = [...constantRoutes, ...asyncRoutes]

const router = createRouter({
  history: createWebHistory(),
  routes: routes,
  scrollBehavior: () => ({ top: 0 })
})

// 全局前置守卫
router.beforeEach(async (to, from, next) => {
  // 设置页面标题
  document.title = to.meta.title || '房产管理系统'
  
  // 获取token，判断是否登录
  const token = localStorage.getItem('token')
  
  // 判断是否需要登录权限
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!token) {
      // 未登录，重定向到登录页
      next({
        path: '/login',
        query: { redirect: to.fullPath } // 将要访问的路径作为参数，登录成功后跳转到该路径
      })
    } else {
      // 检查是否有权限路由
      if (store.state.permission.routes.length === 0) {
        // 没有权限路由，需要重新生成
        try {
          // 生成路由
          await store.dispatch('permission/generateRoutes', store.state.user.roles || ['admin'])
          // 必须确保路由跳转在路由生成之后
          next({ ...to, replace: true })
        } catch (error) {
          // 出错时重定向到登录页
          localStorage.removeItem('token')
          next('/login')
        }
      } else {
        next()
      }
    }
  } else {
    // 已登录状态下，访问登录页则重定向到首页
    if (to.path === '/login' && token) {
      next('/')
    } else {
      next()
    }
  }
})

export default router 