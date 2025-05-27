import { createRouter, createWebHistory } from 'vue-router'
import Layout from '@/views/Layout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: Layout,
      redirect: '/dashboard',
      children: [
        {
          path: 'dashboard',
          name: 'Dashboard',
          component: () => import('@/views/Dashboard.vue'),
          meta: { title: '首页', icon: 'House' }
        },
        {
          path: 'property',
          name: 'Property',
          component: () => import('@/views/Property/index.vue'),
          meta: { title: '房源管理', icon: 'OfficeBuilding' },
          children: [
            {
              path: 'list',
              name: 'PropertyList',
              component: () => import('@/views/Property/PropertyList.vue'),
              meta: { title: '房源列表', icon: 'List' }
            },
            {
              path: 'add',
              name: 'PropertyAdd',
              component: () => import('@/views/Property/PropertyAdd.vue'),
              meta: { title: '添加房源', icon: 'Plus' }
            }
          ]
        },
        {
          path: 'client',
          name: 'Client',
          component: () => import('@/views/Client/index.vue'),
          meta: { title: '客户管理', icon: 'User' },
          children: [
            {
              path: 'list',
              name: 'ClientList',
              component: () => import('@/views/Client/ClientList.vue'),
              meta: { title: '客户列表', icon: 'List' }
            },
            {
              path: 'requirements',
              name: 'ClientRequirements',
              component: () => import('@/views/Client/ClientRequirements.vue'),
              meta: { title: '客户需求', icon: 'Document' }
            }
          ]
        },
        {
          path: 'contract',
          name: 'Contract',
          component: () => import('@/views/Contract/index.vue'),
          meta: { title: '合同管理', icon: 'Document' },
          children: [
            {
              path: 'list',
              name: 'ContractList',
              component: () => import('@/views/Contract/ContractList.vue'),
              meta: { title: '合同列表', icon: 'List' }
            },
            {
              path: 'templates',
              name: 'ContractTemplates',
              component: () => import('@/views/Contract/ContractTemplates.vue'),
              meta: { title: '合同模板', icon: 'Files' }
            }
          ]
        },
        {
          path: 'matching',
          name: 'Matching',
          component: () => import('@/views/Matching/index.vue'),
          meta: { title: '智能匹配', icon: 'Connection' },
          children: [
            {
              path: 'results',
              name: 'MatchingResults',
              component: () => import('@/views/Matching/MatchingResults.vue'),
              meta: { title: '匹配结果', icon: 'List' }
            },
            {
              path: 'settings',
              name: 'MatchingSettings',
              component: () => import('@/views/Matching/MatchingSettings.vue'),
              meta: { title: '匹配设置', icon: 'Setting' }
            }
          ]
        },
        {
          path: 'recycle',
          name: 'Recycle',
          component: () => import('@/views/Recycle/index.vue'),
          meta: { title: '回收站', icon: 'Delete' },
          children: [
            {
              path: 'list',
              name: 'RecycleList',
              component: () => import('@/views/Recycle/RecycleList.vue'),
              meta: { title: '回收站列表', icon: 'List' }
            }
          ]
        },
        {
          path: 'system',
          name: 'System',
          component: () => import('@/views/System/index.vue'),
          meta: { title: '系统管理', icon: 'Setting' },
          children: [
            {
              path: 'users',
              name: 'SystemUsers',
              component: () => import('@/views/System/SystemUsers.vue'),
              meta: { title: '用户管理', icon: 'User' }
            },
            {
              path: 'roles',
              name: 'SystemRoles',
              component: () => import('@/views/System/SystemRoles.vue'),
              meta: { title: '角色管理', icon: 'UserFilled' }
            },
            {
              path: 'logs',
              name: 'SystemLogs',
              component: () => import('@/views/System/SystemLogs.vue'),
              meta: { title: '系统日志', icon: 'Document' }
            }
          ]
        }
      ]
    }
  ]
})

export default router 