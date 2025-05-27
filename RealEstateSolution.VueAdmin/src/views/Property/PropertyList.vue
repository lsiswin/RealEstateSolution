<template>
  <div class="property-list-container">
    <!-- 搜索筛选区域 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="searchForm" :inline="true" class="search-form">
        <el-form-item label="房产类型">
          <el-select v-model="searchForm.type" placeholder="请选择房产类型" clearable>
            <el-option label="住宅" :value="PropertyType.Residential" />
            <el-option label="商业" :value="PropertyType.Commercial" />
            <el-option label="办公" :value="PropertyType.Office" />
            <el-option label="工业" :value="PropertyType.Industrial" />
            <el-option label="土地" :value="PropertyType.Land" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="房产状态">
          <el-select v-model="searchForm.status" placeholder="请选择房产状态" clearable>
            <el-option label="待售" :value="PropertyStatus.ForSale" />
            <el-option label="已售" :value="PropertyStatus.Sold" />
            <el-option label="待租" :value="PropertyStatus.ForRent" />
            <el-option label="已租" :value="PropertyStatus.Rented" />
            <el-option label="下架" :value="PropertyStatus.Offline" />
            <el-option label="可用" :value="PropertyStatus.Available" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="价格范围">
          <el-input-number v-model="searchForm.minPrice" placeholder="最低价格" :min="0" controls-position="right" style="width: 120px" />
          <span style="margin: 0 8px">-</span>
          <el-input-number v-model="searchForm.maxPrice" placeholder="最高价格" :min="0" controls-position="right" style="width: 120px" />
        </el-form-item>
        
        <el-form-item label="面积范围">
          <el-input-number v-model="searchForm.minArea" placeholder="最小面积" :min="0" controls-position="right" style="width: 120px" />
          <span style="margin: 0 8px">-</span>
          <el-input-number v-model="searchForm.maxArea" placeholder="最大面积" :min="0" controls-position="right" style="width: 120px" />
        </el-form-item>
        
        <el-form-item label="关键词">
          <el-input v-model="searchForm.keyword" placeholder="请输入标题或地址关键词" clearable style="width: 200px" />
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
    <el-card class="action-card" shadow="never">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加房源
      </el-button>
      <el-button type="danger" @click="handleBatchDelete" :disabled="selectedIds.length === 0">
        <el-icon><Delete /></el-icon>
        批量删除
      </el-button>
    </el-card>

    <!-- 房源列表 -->
    <el-card class="table-card" shadow="never">
      <el-table
        v-loading="loading"
        :data="propertyList"
        @selection-change="handleSelectionChange"
        stripe
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="title" label="标题" min-width="200" show-overflow-tooltip />
        <el-table-column prop="type" label="类型" width="80">
          <template #default="{ row }">
            {{ getPropertyTypeText(row.type) }}
          </template>
        </el-table-column>
        <el-table-column prop="price" label="价格" width="120">
          <template #default="{ row }">
            <span class="price-text">¥{{ formatPrice(row.price) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="area" label="面积" width="100">
          <template #default="{ row }">
            {{ row.area }}㎡
          </template>
        </el-table-column>
        <el-table-column prop="address" label="地址" min-width="200" show-overflow-tooltip />
        <el-table-column prop="decoration" label="装修" width="80">
          <template #default="{ row }">
            {{ getDecorationTypeText(row.decoration) }}
          </template>
        </el-table-column>
        <el-table-column prop="orientation" label="朝向" width="80">
          <template #default="{ row }">
            {{ getOrientationTypeText(row.orientation) }}
          </template>
        </el-table-column>
        <el-table-column prop="rooms" label="房间" width="80">
          <template #default="{ row }">
            {{ row.rooms }}室{{ row.bathrooms }}卫
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="getPropertyStatusColor(row.status)">
              {{ getPropertyStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.createTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">
              <el-icon><View /></el-icon>
              查看
            </el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">
              <el-icon><Edit /></el-icon>
              编辑
            </el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">
              <el-icon><Delete /></el-icon>
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
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
import { Search, Refresh, Plus, Delete, View, Edit } from '@element-plus/icons-vue'
import {
  queryProperties,
  deleteProperty,
  PropertyType,
  PropertyStatus,
  Property,
  PropertyQueryParams,
  getPropertyTypeText,
  getDecorationTypeText,
  getOrientationTypeText,
  getPropertyStatusText,
  getPropertyStatusColor
} from '@/api/property'

const router = useRouter()

// 响应式数据
const loading = ref(false)
const propertyList = ref<Property[]>([])
const selectedIds = ref<number[]>([])

// 搜索表单
const searchForm = reactive<PropertyQueryParams>({
  type: undefined,
  status: undefined,
  minPrice: undefined,
  maxPrice: undefined,
  minArea: undefined,
  maxArea: undefined,
  keyword: ''
})

// 分页数据
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

// 获取房源列表
const fetchPropertyList = async () => {
  try {
    loading.value = true
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
  } catch (error: any) {
    ElMessage.error(error.message || '获取房源列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  fetchPropertyList()
}

// 重置
const handleReset = () => {
  Object.assign(searchForm, {
    type: undefined,
    status: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    minArea: undefined,
    maxArea: undefined,
    keyword: ''
  })
  pagination.pageIndex = 1
  fetchPropertyList()
}

// 添加房源
const handleAdd = () => {
  router.push('/property/add')
}

// 查看房源
const handleView = (row: Property) => {
  router.push(`/property/detail/${row.id}`)
}

// 编辑房源
const handleEdit = (row: Property) => {
  router.push(`/property/edit/${row.id}`)
}

// 删除房源
const handleDelete = async (row: Property) => {
  try {
    await ElMessageBox.confirm(`确定要删除房源"${row.title}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    const response = await deleteProperty(row.id)
    if (response.success) {
      ElMessage.success('删除成功')
      fetchPropertyList()
    } else {
      ElMessage.error(response.message || '删除失败')
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || '删除失败')
    }
  }
}

// 批量删除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(`确定要删除选中的${selectedIds.value.length}个房源吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    // 批量删除逻辑
    for (const id of selectedIds.value) {
      await deleteProperty(id)
    }
    
    ElMessage.success('批量删除成功')
    selectedIds.value = []
    fetchPropertyList()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || '批量删除失败')
    }
  }
}

// 选择变化
const handleSelectionChange = (selection: Property[]) => {
  selectedIds.value = selection.map(item => item.id)
}

// 分页大小变化
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  fetchPropertyList()
}

// 当前页变化
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  fetchPropertyList()
}

// 格式化价格
const formatPrice = (price: number): string => {
  if (price >= 10000) {
    return (price / 10000).toFixed(1) + '万'
  }
  return price.toLocaleString()
}

// 格式化日期时间
const formatDateTime = (dateTime: string): string => {
  return new Date(dateTime).toLocaleString('zh-CN')
}

// 组件挂载时获取数据
onMounted(() => {
  fetchPropertyList()
})
</script>

<style scoped>
.property-list-container {
  padding: 20px;
}

.search-card,
.action-card,
.table-card {
  margin-bottom: 20px;
}

.search-form {
  margin-bottom: 0;
}

.price-text {
  color: #e6a23c;
  font-weight: bold;
}

.pagination-container {
  margin-top: 20px;
  text-align: right;
}
</style> 