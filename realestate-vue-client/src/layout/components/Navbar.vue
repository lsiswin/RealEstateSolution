<template>
  <div class="navbar">
    <!-- 折叠按钮 -->
    <div class="hamburger-container" @click="toggleSideBar">
      <el-icon :class="{'is-active': sidebar.opened}">
        <Fold v-if="sidebar.opened" />
        <Expand v-else />
      </el-icon>
    </div>
    
    <!-- 面包屑导航 -->
    <breadcrumb class="breadcrumb-container" />
    
    <div class="right-menu">
      <!-- 全屏按钮 -->
      <div class="right-menu-item hover-effect" @click="toggleFullScreen">
        <el-tooltip content="全屏" :effect="'dark'" placement="bottom">
          <el-icon><FullScreen /></el-icon>
        </el-tooltip>
      </div>
      
      <!-- 用户头像下拉菜单 -->
      <el-dropdown class="avatar-container right-menu-item hover-effect" trigger="click">
        <div class="avatar-wrapper">
          <img :src="avatar || defaultAvatar" class="user-avatar">
          <span class="user-name">{{ realName || username }}</span>
          <el-icon><ArrowDown /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <router-link to="/profile">
              <el-dropdown-item>个人中心</el-dropdown-item>
            </router-link>
            <el-dropdown-item divided @click="logout">
              <span style="display:block;">退出登录</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import { ElMessageBox } from 'element-plus'
import Breadcrumb from './Breadcrumb.vue'

const store = useStore()
const router = useRouter()
const defaultAvatar = ref('https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif')

// 从store获取状态
const sidebar = computed(() => store.state.app.sidebar)
const avatar = computed(() => store.state.user.avatar)
const username = computed(() => store.state.user.username)
const realName = computed(() => store.state.user.realName)

// 切换侧边栏
const toggleSideBar = () => {
  store.dispatch('app/toggleSideBar')
}

// 切换全屏
const toggleFullScreen = () => {
  if (!document.fullscreenElement) {
    document.documentElement.requestFullscreen()
  } else {
    if (document.exitFullscreen) {
      document.exitFullscreen()
    }
  }
}

// 退出登录
const logout = () => {
  ElMessageBox.confirm('确定要退出登录吗?', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    store.dispatch('user/logout').then(() => {
      router.push(`/login?redirect=${encodeURIComponent(router.currentRoute.value.fullPath)}`)
    })
  })
}
</script>

<style scoped>
.navbar {
  height: 50px;
  overflow: hidden;
  position: relative;
  background: #fff;
  box-shadow: 0 1px 4px rgba(0,21,41,.08);
  display: flex;
  align-items: center;
}

.hamburger-container {
  line-height: 46px;
  height: 100%;
  float: left;
  cursor: pointer;
  transition: background .3s;
  -webkit-tap-highlight-color: transparent;
  padding: 0 15px;
}

.hamburger-container:hover {
  background: rgba(0, 0, 0, .025);
}

.breadcrumb-container {
  float: left;
}

.right-menu {
  float: right;
  height: 100%;
  display: flex;
  align-items: center;
  margin-left: auto;
  padding-right: 15px;
}

.right-menu-item {
  display: inline-block;
  padding: 0 8px;
  height: 100%;
  font-size: 18px;
  color: #5a5e66;
  vertical-align: middle;
}

.hover-effect {
  cursor: pointer;
}

.avatar-container {
  margin-right: 10px;
}

.avatar-wrapper {
  display: flex;
  align-items: center;
  margin-top: 5px;
  position: relative;
}

.user-avatar {
  cursor: pointer;
  width: 35px;
  height: 35px;
  border-radius: 50%;
  margin-right: 5px;
}

.user-name {
  font-size: 14px;
  margin-right: 5px;
}

.is-active {
  transform: rotate(180deg);
}
</style> 