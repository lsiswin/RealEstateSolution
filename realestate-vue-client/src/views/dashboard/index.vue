<template>
  <div class="dashboard-container">
    <h1>欢迎使用房产管理系统</h1>
    <p>您好，{{ username }}！祝您工作愉快。</p>
    
    <el-row :gutter="20" class="stat-row">
      <el-col :span="8">
        <el-card shadow="hover">
          <div class="stat-item">
            <h3>房源总数</h3>
            <div class="stat-value">58</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card shadow="hover">
          <div class="stat-item">
            <h3>客户总数</h3>
            <div class="stat-value">126</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card shadow="hover">
          <div class="stat-item">
            <h3>本月成交</h3>
            <div class="stat-value">15</div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <div class="dashboard-links">
      <el-button type="primary" @click="$router.push('/property/list')">查看房源</el-button>
      <el-button type="success" @click="$router.push('/client/list')">查看客户</el-button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const username = computed(() => store.state.user.username || '管理员')
</script>

<style scoped>
.dashboard-container {
  padding: 20px;
}

.stat-row {
  margin: 20px 0;
}

.stat-item {
  text-align: center;
  padding: 20px;
}

.stat-item h3 {
  margin: 0 0 10px 0;
  font-size: 16px;
  color: #606266;
}

.stat-value {
  font-size: 28px;
  font-weight: bold;
  color: #303133;
}

.dashboard-links {
  margin-top: 20px;
  text-align: center;
}

.dashboard-links .el-button {
  margin: 0 10px;
}
</style> 