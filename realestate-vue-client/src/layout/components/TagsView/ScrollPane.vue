<template>
  <el-scrollbar
    ref="scrollContainer"
    :vertical="false"
    class="scroll-container"
    @wheel.prevent="handleScroll"
  >
    <slot />
  </el-scrollbar>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'

const scrollContainer = ref(null)
const scrollWrapper = ref(null)

/**
 * 处理滚轮滚动事件
 */
const handleScroll = (e) => {
  const eventDelta = e.wheelDelta || -e.deltaY * 40
  const $scrollWrapper = scrollWrapper.value
  $scrollWrapper.scrollLeft = $scrollWrapper.scrollLeft - eventDelta / 4
}

/**
 * 移动到目标标签位置
 */
const moveToTarget = (currentTag) => {
  const $container = scrollContainer.value.$el
  const $containerWidth = $container.offsetWidth
  const $scrollWrapper = scrollWrapper.value

  let firstTag = null
  let lastTag = null

  // 找到所有标签元素
  const tagList = $scrollWrapper.querySelectorAll('.tags-view-item')

  if (tagList.length > 0) {
    firstTag = tagList[0]
    lastTag = tagList[tagList.length - 1]
  }

  if (firstTag === null) return

  // 找到当前标签元素
  let currentEl = null
  for (let i = 0; i < tagList.length; i++) {
    if (tagList[i].dataset.path === currentTag.path) {
      currentEl = tagList[i]
      break
    }
  }

  if (currentEl === null) return

  // 标签元素的位置数据
  const offsetLeft = currentEl.offsetLeft
  const offsetWidth = currentEl.offsetWidth

  // 滚动区域的位置数据
  const scrollLeft = $scrollWrapper.scrollLeft
  const scrollWidth = $scrollWrapper.scrollWidth

  // 计算需要滚动的位置
  if (offsetLeft < scrollLeft) {
    // 目标在可视区域左侧
    $scrollWrapper.scrollLeft = offsetLeft - 10
  } else if (offsetLeft + offsetWidth > $containerWidth + scrollLeft) {
    // 目标在可视区域右侧
    $scrollWrapper.scrollLeft = offsetLeft + offsetWidth - $containerWidth + 10
  }
}

onMounted(() => {
  scrollWrapper.value = scrollContainer.value.$el.querySelector('.el-scrollbar__wrap')
})

// 暴露方法给父组件
defineExpose({
  moveToTarget
})
</script>

<style scoped>
.scroll-container {
  white-space: nowrap;
  position: relative;
  overflow: hidden;
  width: 100%;
}

.scroll-container :deep(.el-scrollbar__bar) {
  bottom: 0px;
}

.scroll-container :deep(.el-scrollbar__wrap) {
  height: 49px;
}
</style>