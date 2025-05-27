<template>
  <div class="layout-container">
    <!-- 顶部系统信息栏 -->
    <el-header class="layout-header">
      <div class="header-content">
        <div class="header-left">
          <el-icon class="header-icon"><OfficeBuilding /></el-icon>
          <span class="system-title">房产中介管理系统</span>
        </div>
        <div class="header-right">
          <el-dropdown>
            <span class="user-info">
              <el-icon><User /></el-icon>
              <span>管理员</span>
              <el-icon class="el-icon--right"><arrow-down /></el-icon>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item>个人中心</el-dropdown-item>
                <el-dropdown-item>修改密码</el-dropdown-item>
                <el-dropdown-item divided>退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </div>
    </el-header>

    <!-- 主体内容区域 -->
    <el-container class="layout-main">
      <!-- 左侧导航栏 -->
      <el-aside class="layout-sidebar">
        <el-menu
          :default-active="$route.path"
          class="sidebar-menu"
          background-color="#001529"
          text-color="#fff"
          active-text-color="#1890ff"
          router
        >
          <template v-for="route in menuRoutes" :key="route.path">
            <!-- 有子菜单的情况 -->
            <el-sub-menu v-if="route.children && route.children.length > 0" :index="route.path">
              <template #title>
                <el-icon><component :is="route.meta?.icon" /></el-icon>
                <span>{{ route.meta?.title }}</span>
              </template>
              <el-menu-item
                v-for="child in route.children"
                :key="child.path"
                :index="route.path + '/' + child.path"
              >
                <el-icon><component :is="child.meta?.icon" /></el-icon>
                <span>{{ child.meta?.title }}</span>
              </el-menu-item>
            </el-sub-menu>
            <!-- 没有子菜单的情况 -->
            <el-menu-item v-else :index="route.path">
              <el-icon><component :is="route.meta?.icon" /></el-icon>
              <span>{{ route.meta?.title }}</span>
            </el-menu-item>
          </template>
        </el-menu>
      </el-aside>

      <!-- 右侧内容区域 -->
      <el-main class="layout-content">
        <router-view />
      </el-main>
    </el-container>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import router from '@/router'

const route = useRoute()

// 获取菜单路由
const menuRoutes = computed(() => {
  const routes = router.getRoutes()
  const mainRoute = routes.find(r => r.path === '/')
  return mainRoute?.children?.filter(child => child.meta?.title) || []
})
</script>

<style scoped>
.layout-container {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.layout-header {
  height: 60px;
  background: #fff;
  border-bottom: 1px solid #e4e7ed;
  box-shadow: 0 1px 4px rgba(0,21,41,.08);
  padding: 0 20px;
  display: flex;
  align-items: center;
}

.header-content {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left {
  display: flex;
  align-items: center;
}

.header-icon {
  font-size: 24px;
  color: #1890ff;
  margin-right: 12px;
}

.system-title {
  font-size: 20px;
  font-weight: bold;
  color: #333;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.user-info:hover {
  background-color: #f5f5f5;
}

.user-info .el-icon {
  margin-right: 8px;
}

.layout-main {
  flex: 1;
  overflow: hidden;
}

.layout-sidebar {
  width: 200px;
  background: #001529;
  overflow-y: auto;
}

.sidebar-menu {
  border-right: none;
  height: 100%;
}

.sidebar-menu .el-menu-item,
.sidebar-menu .el-sub-menu__title {
  height: 50px;
  line-height: 50px;
}

.layout-content {
  background: #f0f2f5;
  overflow-y: auto;
  padding: 20px;
}
</style> 