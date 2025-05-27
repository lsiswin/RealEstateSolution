<template>
  <div class="client-list">
    <div class="page-header">
      <h2>客户列表</h2>
      <p>管理所有客户信息</p>
    </div>
    
    <el-card>
      <el-table :data="clients" style="width: 100%">
        <el-table-column prop="id" label="客户编号" width="120" />
        <el-table-column prop="name" label="客户姓名" />
        <el-table-column prop="phone" label="联系电话" width="150" />
        <el-table-column prop="email" label="邮箱" width="200" />
        <el-table-column prop="type" label="客户类型" width="100" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.status === '活跃' ? 'success' : 'info'">
              {{ scope.row.status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="scope">
            <el-button size="small" @click="editClient(scope.row)">编辑</el-button>
            <el-button size="small" type="danger" @click="deleteClient(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const clients = ref([
  {
    id: 'C001',
    name: '张三',
    phone: '13800138000',
    email: 'zhangsan@example.com',
    type: '买家',
    status: '活跃'
  },
  {
    id: 'C002',
    name: '李四',
    phone: '13800138001',
    email: 'lisi@example.com',
    type: '卖家',
    status: '活跃'
  }
])

const editClient = (client: any) => {
  console.log('编辑客户:', client)
}

const deleteClient = (client: any) => {
  console.log('删除客户:', client)
}
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
</style> 