<template>
  <div class="property-list">
    <!-- 页面标题 -->
    <div class="page-header">
      <h2>房源列表</h2>
      <p>管理和查看所有房源信息</p>
    </div>

    <!-- 搜索和筛选区域 -->
    <el-card class="search-card">
      <el-form :model="searchForm" :inline="true" label-width="80px">
        <el-form-item label="关键词">
          <el-input
            v-model="searchForm.keyword"
            placeholder="请输入房源标题或地址"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="房源类型">
          <el-select v-model="searchForm.type" placeholder="请选择" clearable style="width: 150px">
            <el-option
              v-for="(text, value) in propertyTypeOptions"
              :key="value"
              :label="text"
              :value="Number(value)"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="房源状态">
          <el-select v-model="searchForm.status" placeholder="请选择" clearable style="width: 150px">
            <el-option
              v-for="(text, value) in propertyStatusOptions"
              :key="value"
              :label="text"
              :value="Number(value)"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="价格范围">
          <el-input
            v-model.number="searchForm.minPrice"
            placeholder="最低价"
            type="number"
            style="width: 100px"
          />
          <span style="margin: 0 8px">-</span>
          <el-input
            v-model.number="searchForm.maxPrice"
            placeholder="最高价"
            type="number"
            style="width: 100px"
          />
        </el-form-item>
        <el-form-item label="面积范围">
          <el-input
            v-model.number="searchForm.minArea"
            placeholder="最小面积"
            type="number"
            style="width: 100px"
          />
          <span style="margin: 0 8px">-</span>
          <el-input
            v-model.number="searchForm.maxArea"
            placeholder="最大面积"
            type="number"
            style="width: 100px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :loading="loading">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 操作按钮区域 -->
    <div class="action-bar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加房源
      </el-button>
      <el-button type="danger" @click="handleBatchDelete" :disabled="selectedIds.length === 0">
        <el-icon><Delete /></el-icon>
        批量删除
      </el-button>
    </div>

    <!-- 房源列表表格 -->
    <el-card class="table-card">
      <el-table
        :data="propertyList"
        v-loading="loading"
        @selection-change="handleSelectionChange"
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="房源编号" width="100" />
        <el-table-column prop="title" label="房源标题" min-width="200" show-overflow-tooltip />
        <el-table-column prop="type" label="房源类型" width="100">
          <template #default="scope">
            {{ getPropertyTypeText(scope.row.type) }}
          </template>
        </el-table-column>
        <el-table-column prop="price" label="价格(万)" width="120">
          <template #default="scope">
            {{ (scope.row.price / 10000).toFixed(1) }}
          </template>
        </el-table-column>
        <el-table-column prop="area" label="面积(㎡)" width="100" />
        <el-table-column prop="address" label="地址" min-width="200" show-overflow-tooltip />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="getPropertyStatusColor(scope.row.status)">
              {{ getPropertyStatusText(scope.row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="scope">
            <el-button type="primary" size="small" @click="handleView(scope.row)">
              查看
            </el-button>
            <el-button type="warning" size="small" @click="handleEdit(scope.row)">
              编辑
            </el-button>
            <el-button type="danger" size="small" @click="handleDelete(scope.row)">
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页组件 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.pageIndex"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Plus, Delete } from '@element-plus/icons-vue'

// API imports
import {
  queryProperties,
  deleteProperty,
  getPropertyTypeText,
  getPropertyStatusText,
  getPropertyStatusColor,
  PropertyType,
  PropertyStatus,
  type Property,
  type PropertyQueryParams
} from '@/api/property'

const router = useRouter()

// 加载状态
const loading = ref(false)

// 搜索表单
const searchForm = reactive<PropertyQueryParams>({
  keyword: '',
  type: undefined,
  status: undefined,
  minPrice: undefined,
  maxPrice: undefined,
  minArea: undefined,
  maxArea: undefined
})

// 房源列表
const propertyList = ref<Property[]>([])

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

// 选中的房源ID
const selectedIds = ref<number[]>([])

// 房源类型选项
const propertyTypeOptions = {
  [PropertyType.Apartment]: '公寓',
  [PropertyType.House]: '别墅',
  [PropertyType.Commercial]: '商业',
  [PropertyType.Office]: '办公',
  [PropertyType.Shop]: '商铺',
  [PropertyType.Warehouse]: '仓库'
}

// 房源状态选项
const propertyStatusOptions = {
  [PropertyStatus.Available]: '可售',
  [PropertyStatus.Pending]: '待定',
  [PropertyStatus.Sold]: '已售',
  [PropertyStatus.Rented]: '已租',
  [PropertyStatus.Withdrawn]: '已下架'
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 获取房源列表
const fetchPropertyList = async () => {
  loading.value = true
  try {
    const params: PropertyQueryParams = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    
    const response = await queryProperties(params)
    if (response.success && response.data) {
      propertyList.value = response.data.items
      pagination.total = response.data.totalCount
    } else {
      ElMessage.error(response.message || '获取房源列表失败')
    }
  } catch (error) {
    console.error('获取房源列表失败:', error)
    ElMessage.error('获取房源列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  fetchPropertyList()
}

// 重置搜索
const handleReset = () => {
  Object.assign(searchForm, {
    keyword: '',
    type: undefined,
    status: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    minArea: undefined,
    maxArea: undefined
  })
  pagination.pageIndex = 1
  fetchPropertyList()
}

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  fetchPropertyList()
}

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  fetchPropertyList()
}

// 选择改变
const handleSelectionChange = (selection: Property[]) => {
  selectedIds.value = selection.map(item => item.id)
}

// 添加房源
const handleAdd = () => {
  router.push('/property/add')
}

// 查看房源
const handleView = (property: Property) => {
  router.replace(`/property/detail/${property.id}`)
}

// 编辑房源
const handleEdit = (property: Property) => {
  router.replace(`/property/edit/${property.id}`)
}

// 删除房源
const handleDelete = async (property: Property) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除房源"${property.title}"吗？`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const response = await deleteProperty(property.id)
    if (response.success) {
      ElMessage.success('删除成功')
      fetchPropertyList()
    } else {
      ElMessage.error(response.message || '删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除房源失败:', error)
      ElMessage.error('删除失败')
    }
  }
}

// 批量删除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(
      `确定要删除选中的 ${selectedIds.value.length} 个房源吗？`,
      '确认批量删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // 批量删除逻辑
    const deletePromises = selectedIds.value.map(id => deleteProperty(id))
    await Promise.all(deletePromises)
    
    ElMessage.success('批量删除成功')
    selectedIds.value = []
    fetchPropertyList()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('批量删除失败:', error)
      ElMessage.error('批量删除失败')
    }
  }
}

// 初始化
onMounted(() => {
  fetchPropertyList()
})
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

.search-card {
  margin-bottom: 16px;
}

.action-bar {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 24px;
}

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: 20px;
}
</style> 