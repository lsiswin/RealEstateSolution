<template>
  <div class="error-container">
    <div class="error-content">
      <h1>404</h1>
      <h2>页面不存在</h2>
      <p>抱歉，您访问的页面不存在或已被移除</p>
      <el-button type="primary" @click="goHome">返回首页</el-button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

// 获取路由实例
const router = useRouter()

/**
 * 返回首页
 * 当用户点击返回首页按钮时，跳转到应用的首页
 */
const goHome = () => {
  router.push('/')
}
</script>

<style scoped>
.error-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f0f2f5;
}

.error-content {
  text-align: center;
}

.error-content h1 {
  font-size: 120px;
  color: #409EFF;
  margin: 0;
  line-height: 1.2;
}

.error-content h2 {
  font-size: 30px;
  color: #303133;
  margin: 10px 0;
}

.error-content p {
  color: #606266;
  margin-bottom: 30px;
}
</style> 