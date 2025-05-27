<template>
  <div v-if="!item.hidden" class="sidebar-item-container">
    <!-- 无子菜单的情况 -->
    <template v-if="hasOneShowingChild(item.children, item) && (!onlyOneChild.children || onlyOneChild.noShowingChildren) && !item.alwaysShow">
      <app-link v-if="onlyOneChild.meta" :to="resolvePath(onlyOneChild.path)">
        <el-menu-item :index="resolvePath(onlyOneChild.path)" :class="{'submenu-title-noDropdown': !isNest}">
          <!-- 菜单图标 -->
          <el-icon v-if="onlyOneChild.meta && onlyOneChild.meta.icon">
            <component :is="onlyOneChild.meta.icon" />
          </el-icon>
          <!-- 菜单标题 -->
          <template #title>
            <span>{{ onlyOneChild.meta.title }}</span>
          </template>
        </el-menu-item>
      </app-link>
    </template>

    <!-- 有子菜单的情况 -->
    <el-sub-menu v-else ref="subMenu" :index="resolvePath(item.path)" popper-append-to-body>
      <!-- 父级菜单标题 -->
      <template #title>
        <el-icon v-if="item.meta && item.meta.icon">
          <component :is="item.meta.icon" />
        </el-icon>
        <span>{{ item.meta.title }}</span>
      </template>
      
      <!-- 子菜单递归渲染 -->
      <sidebar-item
        v-for="child in item.children"
        :key="child.path"
        :is-nest="true"
        :item="child"
        :base-path="resolvePath(child.path)"
        :is-collapse="isCollapse"
        class="nest-menu"
      />
    </el-sub-menu>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { isExternal } from '@/utils/validate'
import AppLink from './Link.vue'
import path from 'path-browserify'

// 定义组件接收的属性
const props = defineProps({
  // 菜单项数据
  item: {
    type: Object,
    required: true
  },
  // 基础路径
  basePath: {
    type: String,
    default: ''
  },
  // 是否是嵌套菜单
  isNest: {
    type: Boolean,
    default: false
  },
  // 是否折叠状态
  isCollapse: {
    type: Boolean,
    default: false
  }
})

// 保存唯一子菜单的状态
const onlyOneChild = ref(null)

/**
 * 判断是否只有一个可显示的子菜单
 * @param {Array} children - 子菜单数组
 * @param {Object} parent - 父菜单对象
 * @returns {Boolean} - 是否只有一个显示的子菜单
 */
const hasOneShowingChild = (children = [], parent) => {
  // 筛选出不隐藏的子菜单
  const showingChildren = children.filter(item => {
    if (item.hidden) {
      return false
    } else {
      // 将唯一的子菜单保存到状态中
      onlyOneChild.value = item
      return true
    }
  })

  // 当只有一个子菜单可显示时
  if (showingChildren.length === 1) {
    return true
  }

  // 没有子菜单时，显示父菜单
  if (showingChildren.length === 0) {
    onlyOneChild.value = { ...parent, path: '', noShowingChildren: true }
    return true
  }

  return false
}

/**
 * 解析菜单路径
 * @param {String} routePath - 路由路径
 * @returns {String} - 完整的路由路径
 */
const resolvePath = (routePath) => {
  // 如果是外部链接，直接返回
  if (isExternal(routePath)) {
    return routePath
  }
  // 如果是外部链接，直接返回
  if (isExternal(props.basePath)) {
    return props.basePath
  }
  // 拼接基础路径和路由路径
  return path.resolve(props.basePath, routePath)
}
</script>

<style scoped>
.sidebar-item-container {
  /* 过渡效果 */
  transition: background-color 0.3s;
}

/* 鼠标悬停效果 */
.el-menu-item:hover, .el-sub-menu__title:hover {
  background-color: rgba(255, 255, 255, 0.06) !important;
}

/* 菜单项激活效果 */
.el-menu-item.is-active {
  color: #409EFF !important;
  background-color: rgba(64, 158, 255, 0.1) !important;
}

/* 图标样式 */
.el-icon {
  margin-right: 12px;
  font-size: 16px;
}

/* 折叠时的样式 */
.is-collapse .el-icon {
  margin-right: 0;
}
</style> 