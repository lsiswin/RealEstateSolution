<template>
  <!-- 根据路径类型渲染不同的链接形式 -->
  <!-- 如果是外部链接，使用 <a> 标签 -->
  <component :is="type" v-bind="linkProps(to)">
    <slot />
  </component>
</template>

<script setup>
import { computed } from 'vue'
import { isExternal } from '@/utils/validate'

/**
 * 定义组件接收的属性
 * to: 链接目标路径，可以是内部路由路径或外部URL
 */
const props = defineProps({
  to: {
    type: String,
    required: true
  }
})

/**
 * 根据链接类型返回不同的组件类型
 * 外部链接返回 'a'（HTML标签）
 * 内部链接返回 'router-link'（Vue Router组件）
 */
const type = computed(() => {
  return isExternal(props.to) ? 'a' : 'router-link'
})

/**
 * 根据链接类型返回不同的属性
 * @param {string} url - 链接路径
 * @returns {Object} - 链接属性对象
 */
const linkProps = (url) => {
  // 如果是外部链接
  if (isExternal(url)) {
    // 返回a标签所需属性：href, target, rel
    return {
      href: url,
      target: '_blank', // 在新标签页打开
      rel: 'noopener' // 安全属性，防止新窗口获取原窗口的引用
    }
  }
  
  // 如果是内部链接，返回router-link所需属性
  return {
    to: url
  }
}
</script> 