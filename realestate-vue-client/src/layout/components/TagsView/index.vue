<template>
  <div class="tags-view-container">
    <!-- 标签页横向滚动区域 -->
    <scroll-pane ref="scrollPane" class="tags-view-wrapper">
      <router-link
        v-for="tag in visitedViews"
        :key="tag.path"
        :class="isActive(tag) ? 'active' : ''"
        :to="{ path: tag.path, query: tag.query, fullPath: tag.fullPath }"
        class="tags-view-item"
        @click.middle="!isAffix(tag) && closeSelectedTag(tag)"
        @contextmenu.prevent="openMenu(tag, $event)"
      >
        <!-- 标签页标题 -->
        <span>{{ tag.title }}</span>
        <!-- 非固定标签可关闭 -->
        <el-icon v-if="!isAffix(tag)" class="el-icon-close" @click.prevent.stop="closeSelectedTag(tag)">
          <Close />
        </el-icon>
      </router-link>
    </scroll-pane>
    
    <!-- 右键菜单 -->
    <ul v-show="visible" :style="{left: left+'px', top: top+'px'}" class="contextmenu">
      <li @click="refreshSelectedTag(selectedTag)">刷新</li>
      <li v-if="!isAffix(selectedTag)" @click="closeSelectedTag(selectedTag)">关闭</li>
      <li @click="closeOthersTags">关闭其他</li>
      <li @click="closeAllTags(selectedTag)">关闭所有</li>
    </ul>
  </div>
</template>

<script setup>
import { ref, computed, watch, nextTick, onMounted, onBeforeUnmount } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'
import ScrollPane from './ScrollPane.vue'

// 引入工具函数和状态管理
const store = useStore()
const route = useRoute()
const router = useRouter()

// 滚动区域的引用，用于控制标签页滚动位置
const scrollPane = ref(null)
// 右键菜单状态
const visible = ref(false)
const top = ref(0)
const left = ref(0)
const selectedTag = ref({})

// 从vuex获取已访问的视图列表
const visitedViews = computed(() => store.state.tagsView.visitedViews)
// 获取路由列表
const routes = computed(() => store.state.permission.routes)

/**
 * 初始化标签，添加当前页面到标签列表
 */
const initTags = () => {
  const affixTags = filterAffixTags(routes.value)
  for (const tag of affixTags) {
    if (tag.name) {
      store.dispatch('tagsView/addVisitedView', tag)
    }
  }
}

/**
 * 添加标签
 */
const addTags = () => {
  const { name } = route
  if (name) {
    store.dispatch('tagsView/addView', route)
  }
}

/**
 * 判断是否是固定标签
 * @param {Object} tag - 标签对象
 * @returns {boolean} - 是否是固定标签
 */
const isAffix = (tag) => {
  return tag.meta && tag.meta.affix
}

/**
 * 过滤出固定标签
 * @param {Array} routes - 路由列表
 * @param {Array} baseTags - 基础标签列表
 * @returns {Array} - 固定标签列表
 */
const filterAffixTags = (routes, baseTags = []) => {
  let tags = []
  
  // 递归查找固定标签
  routes.forEach(route => {
    if (route.meta && route.meta.affix) {
      const tagPath = route.path
      tags.push({
        fullPath: tagPath,
        path: tagPath,
        name: route.name,
        meta: { ...route.meta }
      })
    }
    
    if (route.children) {
      const childTags = filterAffixTags(route.children, baseTags)
      if (childTags.length >= 1) {
        tags = [...tags, ...childTags]
      }
    }
  })
  
  return tags
}

/**
 * 判断当前标签是否激活
 * @param {Object} tag - 标签对象
 * @returns {boolean} - 是否激活
 */
const isActive = (tag) => {
  return tag.path === route.path
}

/**
 * 刷新选中的标签
 * @param {Object} view - 标签对象
 */
const refreshSelectedTag = (view) => {
  store.dispatch('tagsView/delCachedView', view).then(() => {
    const { fullPath } = view
    nextTick(() => {
      router.replace({
        path: '/redirect' + fullPath
      })
    })
  })
}

/**
 * 关闭选中的标签
 * @param {Object} view - 标签对象
 */
const closeSelectedTag = (view) => {
  store.dispatch('tagsView/delView', view).then(({ visitedViews }) => {
    if (isActive(view)) {
      toLastView(visitedViews, view)
    }
  })
}

/**
 * 关闭其他标签
 */
const closeOthersTags = () => {
  router.push(selectedTag.value)
  store.dispatch('tagsView/delOthersViews', selectedTag.value).then(() => {
    moveToCurrentTag()
  })
}

/**
 * 关闭所有标签
 * @param {Object} view - 标签对象
 */
const closeAllTags = (view) => {
  store.dispatch('tagsView/delAllViews').then(({ visitedViews }) => {
    if (isAffix(view)) {
      toLastView(visitedViews, view)
    } else {
      router.push('/')
    }
  })
}

/**
 * 跳转到最后一个标签
 * @param {Array} visitedViews - 已访问标签列表
 * @param {Object} view - 当前标签对象
 */
const toLastView = (visitedViews, view) => {
  const latestView = visitedViews.slice(-1)[0]
  if (latestView) {
    router.push(latestView.fullPath)
  } else {
    // 如果没有标签，默认跳转到首页
    if (view.name === 'Dashboard') {
      router.replace({ path: '/redirect' + view.fullPath })
    } else {
      router.push('/')
    }
  }
}

/**
 * 移动到当前标签位置
 */
const moveToCurrentTag = () => {
  nextTick(() => {
    for (const tag of visitedViews.value) {
      if (tag.path === route.path) {
        scrollPane.value.moveToTarget(tag)
        // 如果当前标签不是活动标签，刷新它
        if (!isActive(tag)) {
          store.dispatch('tagsView/updateVisitedView', tag)
        }
        break
      }
    }
  })
}

/**
 * 打开右键菜单
 * @param {Object} tag - 标签对象
 * @param {Event} e - 鼠标事件
 */
const openMenu = (tag, e) => {
  const menuMinWidth = 105
  const offsetLeft = document.querySelector('.tags-view-container').getBoundingClientRect().left
  const offsetWidth = document.querySelector('.tags-view-container').offsetWidth
  const maxLeft = offsetWidth - menuMinWidth
  const left = e.clientX - offsetLeft + 15

  // 设置菜单位置
  if (left > maxLeft) {
    left.value = maxLeft
  } else {
    left.value = left
  }
  top.value = e.clientY

  // 设置菜单状态
  visible.value = true
  selectedTag.value = tag
}

/**
 * 关闭菜单
 */
const closeMenu = () => {
  visible.value = false
}

// 确保在组件挂载时初始化标签页，并且在路由变化时更新标签页
onMounted(() => {
  initTags()
  addTags()
})

// 监听路由变化，添加标签页
watch(route, () => {
  addTags()
  moveToCurrentTag()
})

// 监听点击事件，关闭右键菜单
const handleClickOutside = (e) => {
  const menuEl = document.querySelector('.contextmenu')
  if (menuEl && !menuEl.contains(e.target)) {
    closeMenu()
  }
}

// 添加和移除事件监听
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.tags-view-container {
  height: 34px;
  width: 100%;
  background: #fff;
  border-bottom: 1px solid #d8dce5;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.12), 0 0 3px 0 rgba(0, 0, 0, 0.04);
}

.tags-view-wrapper {
  .tags-view-item {
    display: inline-block;
    position: relative;
    cursor: pointer;
    height: 26px;
    line-height: 26px;
    border: 1px solid #d8dce5;
    color: #495060;
    background: #fff;
    padding: 0 8px;
    font-size: 12px;
    margin-left: 5px;
    margin-top: 4px;
    text-decoration: none;
  }

  .tags-view-item:first-of-type {
    margin-left: 15px;
  }

  .tags-view-item:last-of-type {
    margin-right: 15px;
  }

  .tags-view-item.active {
    background-color: #42b983;
    color: #fff;
    border-color: #42b983;
    &::before {
      content: '';
      background: #fff;
      display: inline-block;
      width: 8px;
      height: 8px;
      border-radius: 50%;
      position: relative;
      margin-right: 2px;
    }
  }
}

.contextmenu {
  margin: 0;
  background: #fff;
  z-index: 3000;
  position: absolute;
  list-style-type: none;
  padding: 5px 0;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 400;
  color: #333;
  box-shadow: 2px 2px 3px 0 rgba(0, 0, 0, 0.3);
  li {
    margin: 0;
    padding: 7px 16px;
    cursor: pointer;
    &:hover {
      background: #eee;
    }
  }
}
</style> 