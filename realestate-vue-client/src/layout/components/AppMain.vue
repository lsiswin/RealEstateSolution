<template>
  <section class="app-main">
    <!-- 路由切换动画 -->
    <router-view v-slot="{ Component }">
      <transition name="fade-transform" mode="out-in">
        <keep-alive :include="cachedViews">
          <component :is="Component" :key="key" />
        </keep-alive>
      </transition>
    </router-view>
  </section>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useStore } from 'vuex'

const route = useRoute()
const store = useStore()

// 缓存的视图
const cachedViews = computed(() => store.state.tagsView.cachedViews)

// 根据路由变化生成key
const key = computed(() => route.path)
</script>

<style scoped>
.app-main {
  /*50 = navbar  */
  min-height: calc(100vh - 50px - 34px);
  width: 100%;
  position: relative;
  overflow: hidden;
  padding: 15px;
  background-color: #f0f2f5;
}

.fade-transform-enter-active,
.fade-transform-leave-active {
  transition: all 0.3s;
}

.fade-transform-enter-from {
  opacity: 0;
  transform: translateX(-30px);
}

.fade-transform-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style> 