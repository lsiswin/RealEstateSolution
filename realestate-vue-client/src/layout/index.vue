<template>
  <div class="app-wrapper">
    <!-- 侧边栏 -->
    <Sidebar class="sidebar-container" />
    
    <!-- 主区域 -->
    <div class="main-container">
      <!-- 顶部导航栏 -->
      <div class="navbar-container">
        <Navbar />
      </div>
      
      <!-- 标签页导航 -->
      <div class="tags-view-container">
        <TagsView />
      </div>
      
      <!-- 主内容区域 -->
      <AppMain />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useStore } from 'vuex'
import Navbar from './components/Navbar.vue'
import Sidebar from './components/Sidebar/index.vue'
import AppMain from './components/AppMain.vue'
import TagsView from './components/TagsView/index.vue'

const store = useStore()
const device = computed(() => store.state.app.device)
const classObj = computed(() => ({
  hideSidebar: !store.state.app.sidebar.opened,
  openSidebar: store.state.app.sidebar.opened,
  withoutAnimation: false,
  mobile: device.value === 'mobile'
}))
</script>

<style scoped>
.app-wrapper {
  position: relative;
  height: 100%;
  width: 100%;
  display: flex;
}

.sidebar-container {
  width: 210px;
  height: 100%;
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  overflow: hidden;
  z-index: 1001;
  transition: width 0.28s;
  background-color: #304156;
}

.main-container {
  min-height: 100%;
  transition: margin-left 0.28s;
  margin-left: 210px;
  position: relative;
  width: calc(100% - 210px);
}

.navbar-container {
  height: 50px;
  overflow: hidden;
  position: relative;
  background: #fff;
  box-shadow: 0 1px 4px rgba(0,21,41,.08);
}

.tags-view-container {
  height: 34px;
  width: 100%;
  background: #fff;
  border-bottom: 1px solid #d8dce5;
  box-shadow: 0 1px 3px 0 rgba(0,0,0,.12), 0 0 3px 0 rgba(0,0,0,.04);
}

/* 响应式 */
@media (max-width: 768px) {
  .app-wrapper {
    flex-direction: column;
  }
  
  .sidebar-container {
    width: 54px;
  }
  
  .main-container {
    margin-left: 54px;
    width: calc(100% - 54px);
  }
}
</style> 