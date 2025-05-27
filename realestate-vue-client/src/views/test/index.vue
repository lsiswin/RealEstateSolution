<template>
  <div class="test-container">
    <h1>测试页面</h1>
    <p>如果您看到这个页面，说明路由和布局组件正常工作</p>
    <el-button type="primary" @click="$router.push('/')">返回首页</el-button>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';

onMounted(() => {
  console.log('测试页面已加载');
});
</script>

<style scoped>
.test-container {
  padding: 20px;
  text-align: center;
}
</style> 