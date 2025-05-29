<template>
  <div class="client-list">
    <div class="page-header">
      <h2>客户列表</h2>
      <p>管理和查看所有客户信息</p>
    </div>
    
    <el-card>
      <div class="empty-state">
        <el-empty description="客户管理功能开发中..." />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
// 客户列表页面 - 待开发
</script>

<style scoped>
.client-list {
  padding: 0;
}

.page-header {
  margin-bottom: 24px;
}

.page-header h2 {
  margin: 0 0 8px 0;
  color: #333;
  font-size: 24px;
  font-weight: 500;
}

.page-header p {
  margin: 0;
  color: #666;
  font-size: 14px;
}

.empty-state {
  padding: 40px;
  text-align: center;
}
</style> 