<template>
  <div class="not-found">
    <div class="not-found-content">
      <div class="not-found-icon">
        <el-icon size="120"><DocumentDelete /></el-icon>
      </div>
      <h1>404</h1>
      <p>抱歉，您访问的页面不存在</p>
      <el-button type="primary" @click="goHome">
        返回首页
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { DocumentDelete } from '@element-plus/icons-vue'

const router = useRouter()

const goHome = () => {
  router.push('/')
}
</script>

<style scoped>
.not-found {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f5f5f5;
}

.not-found-content {
  text-align: center;
}

.not-found-icon {
  color: #ccc;
  margin-bottom: 20px;
}

h1 {
  font-size: 72px;
  color: #666;
  margin: 20px 0;
}

p {
  font-size: 18px;
  color: #999;
  margin-bottom: 30px;
}
</style> 