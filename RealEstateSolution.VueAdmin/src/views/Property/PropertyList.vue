<template>
  <div class="property-list">
    <div class="page-header">
      <h2>房源列表</h2>
      <p>管理所有房源信息</p>
    </div>
    
    <el-card>
      <div class="toolbar">
        <el-button type="primary" @click="addProperty">
          <el-icon><Plus /></el-icon>
          添加房源
        </el-button>
      </div>
      
      <el-table :data="properties" style="width: 100%">
        <el-table-column prop="id" label="房源编号" width="120" />
        <el-table-column prop="title" label="房源标题" />
        <el-table-column prop="type" label="房源类型" width="100" />
        <el-table-column prop="area" label="面积(㎡)" width="100" />
        <el-table-column prop="price" label="价格(万)" width="120" />
        <el-table-column prop="location" label="位置" width="150" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="getStatusType(scope.row.status)">
              {{ scope.row.status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="scope">
            <el-button size="small" @click="editProperty(scope.row)">编辑</el-button>
            <el-button size="small" type="danger" @click="deleteProperty(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const properties = ref([
  {
    id: 'P001',
    title: '市中心精装三房',
    type: '住宅',
    area: 120,
    price: 280,
    location: '天河区',
    status: '在售'
  },
  // 更多房源数据...
])

const getStatusType = (status: string) => {
  switch (status) {
    case '在售':
      return 'success'
    case '已售':
      return 'info'
    case '预售':
      return 'warning'
    default:
      return 'info'
  }
}

const addProperty = () => {
  router.push('/property/add')
}

const editProperty = (property: any) => {
  console.log('编辑房源:', property)
}

const deleteProperty = (property: any) => {
  console.log('删除房源:', property)
}
</script>

<style scoped>
.property-list {
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

.toolbar {
  margin-bottom: 16px;
}
</style> 