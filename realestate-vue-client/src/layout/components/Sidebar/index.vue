<template>
  <div :class="{'has-logo': showLogo}">
    <!-- Logo展示 -->
    <logo v-if="showLogo" :collapse="isCollapse" />
    
    <!-- 菜单 -->
    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapse"
        :unique-opened="false"
        :collapse-transition="false"
        mode="vertical"
        background-color="#304156"
        text-color="#bfcbd9"
        active-text-color="#409EFF"
      >
        <sidebar-item
          v-for="route in permission_routes"
          :key="route.path"
          :item="route"
          :base-path="route.path"
          :is-collapse="isCollapse"
        />
      </el-menu>
    </el-scrollbar>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useStore } from 'vuex'
import Logo from './Logo.vue'
import SidebarItem from './SidebarItem.vue'

const route = useRoute()
const store = useStore()

// 是否显示Logo
const showLogo = computed(() => true)

// 是否折叠侧边栏
const isCollapse = computed(() => !store.state.app.sidebar.opened)

// 权限路由
const permission_routes = computed(() => store.state.permission.routes)

// 当前激活的菜单
const activeMenu = computed(() => {
  const { meta, path } = route
  // 如果设置了activeMenu，则使用activeMenu
  if (meta.activeMenu) {
    return meta.activeMenu
  }
  return path
})
</script> 