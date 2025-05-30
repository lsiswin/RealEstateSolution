<template>
  <div class="client-requirements-list">
    <div class="page-header">
      <h2>客户需求管理</h2>
      <p>查看和管理所有客户的房源需求信息</p>
    </div>
    
    <!-- 搜索和筛选区域 -->
    <el-card class="search-card">
      <el-form :model="searchForm" :inline="true" class="search-form">
        <el-form-item label="客户姓名">
          <el-input
            v-model="searchForm.clientName"
            placeholder="请输入客户姓名"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="客户类型">
          <el-select
            v-model="searchForm.clientType"
            placeholder="请选择客户类型"
            clearable
            style="width: 150px"
          >
            <el-option
              v-for="(label, value) in clientTypeOptions"
              :key="value"
              :label="label"
              :value="Number(value)"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="价格范围">
          <el-input
            v-model.number="searchForm.minPrice"
            placeholder="最低价格"
            type="number"
            style="width: 100px"
          />
          <span style="margin: 0 8px">-</span>
          <el-input
            v-model.number="searchForm.maxPrice"
            placeholder="最高价格"
            type="number"
            style="width: 100px"
          />
        </el-form-item>
        <el-form-item label="期望位置">
          <el-input
            v-model="searchForm.location"
            placeholder="请输入期望位置"
            clearable
            style="width: 200px"
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

    <!-- 需求列表 -->
    <el-card class="table-card">
      <el-table
        :data="requirementsList"
        v-loading="loading"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="需求ID" width="80" />
        <el-table-column prop="clientName" label="客户姓名" width="120" />
        <el-table-column prop="clientPhone" label="联系电话" width="130" />
        <el-table-column prop="clientType" label="客户类型" width="100">
          <template #default="scope">
            <el-tag :type="getClientTypeColor(scope.row.clientType)">
              {{ getClientTypeText(scope.row.clientType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="价格范围" width="150">
          <template #default="scope">
            {{ formatPriceRange(scope.row.minPrice, scope.row.maxPrice) }}
          </template>
        </el-table-column>
        <el-table-column label="面积范围" width="150">
          <template #default="scope">
            {{ formatAreaRange(scope.row.minArea, scope.row.maxArea) }}
          </template>
        </el-table-column>
        <el-table-column prop="location" label="期望位置" width="150" show-overflow-tooltip />
        <el-table-column prop="propertyType" label="房源类型" width="120" />
        <el-table-column prop="createdAt" label="创建时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="scope">
            <el-button type="primary" size="small" @click="handleViewDetail(scope.row)">
              查看详情
            </el-button>
            <el-button type="success" size="small" @click="handleMatchProperties(scope.row)">
              匹配房源
            </el-button>
            <el-button type="warning" size="small" @click="handleEdit(scope.row)">
              编辑
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

    <!-- 需求详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="需求详情"
      width="800px"
    >
      <div v-if="selectedRequirement" class="requirement-detail">
        <!-- 客户信息 -->
        <el-card class="detail-card">
          <template #header>
            <span>客户信息</span>
          </template>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="客户姓名">{{ selectedRequirement.clientName }}</el-descriptions-item>
            <el-descriptions-item label="联系电话">{{ selectedRequirement.clientPhone }}</el-descriptions-item>
            <el-descriptions-item label="邮箱">{{ selectedRequirement.clientEmail || '-' }}</el-descriptions-item>
            <el-descriptions-item label="客户类型">
              <el-tag :type="getClientTypeColor(selectedRequirement.clientType)">
                {{ getClientTypeText(selectedRequirement.clientType) }}
              </el-tag>
            </el-descriptions-item>
          </el-descriptions>
        </el-card>

        <!-- 需求信息 -->
        <el-card class="detail-card">
          <template #header>
            <span>需求信息</span>
          </template>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="价格范围">
              {{ formatPriceRange(selectedRequirement.minPrice, selectedRequirement.maxPrice) }}
            </el-descriptions-item>
            <el-descriptions-item label="面积范围">
              {{ formatAreaRange(selectedRequirement.minArea, selectedRequirement.maxArea) }}
            </el-descriptions-item>
            <el-descriptions-item label="期望位置">{{ selectedRequirement.location || '-' }}</el-descriptions-item>
            <el-descriptions-item label="房源类型">{{ selectedRequirement.propertyType || '-' }}</el-descriptions-item>
            <el-descriptions-item label="其他要求" span="2">
              {{ selectedRequirement.otherRequirements || '-' }}
            </el-descriptions-item>
            <el-descriptions-item label="创建时间">
              {{ formatDate(selectedRequirement.createdAt) }}
            </el-descriptions-item>
            <el-descriptions-item label="更新时间">
              {{ formatDate(selectedRequirement.updatedAt) }}
            </el-descriptions-item>
          </el-descriptions>
        </el-card>
      </div>
    </el-dialog>

    <!-- 编辑需求对话框 -->
    <el-dialog
      v-model="editDialogVisible"
      title="编辑客户需求"
      width="600px"
      @close="handleEditDialogClose"
    >
      <el-form
        ref="requirementFormRef"
        :model="requirementForm"
        :rules="requirementRules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="最低价格" prop="minPrice">
              <el-input-number
                v-model="requirementForm.minPrice"
                :min="0"
                :max="99999999"
                placeholder="最低价格"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="最高价格" prop="maxPrice">
              <el-input-number
                v-model="requirementForm.maxPrice"
                :min="0"
                :max="99999999"
                placeholder="最高价格"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="最小面积" prop="minArea">
              <el-input-number
                v-model="requirementForm.minArea"
                :min="0"
                :max="9999"
                placeholder="最小面积"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="最大面积" prop="maxArea">
              <el-input-number
                v-model="requirementForm.maxArea"
                :min="0"
                :max="9999"
                placeholder="最大面积"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="期望位置">
          <el-input
            v-model="requirementForm.location"
            placeholder="请输入期望位置"
          />
        </el-form-item>
        
        <el-form-item label="房源类型">
          <el-input
            v-model="requirementForm.propertyType"
            placeholder="请输入房源类型，如：住宅、商铺、写字楼等"
          />
        </el-form-item>
        
        <el-form-item label="其他要求">
          <el-input
            v-model="requirementForm.otherRequirements"
            type="textarea"
            :rows="4"
            placeholder="请输入其他具体要求"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="editDialogVisible = false">取消</el-button>
          <el-button
            type="primary"
            @click="handleSubmitEdit"
            :loading="submitLoading"
          >
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 匹配房源弹窗 -->
    <el-dialog
      v-model="matchDialogVisible"
      title="匹配房源"
      width="80%"
      :before-close="handleCloseMatchDialog"
    >
      <div v-if="selectedRequirement" class="match-content">
        <div class="requirement-info">
          <h3>客户需求信息</h3>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="客户姓名">{{ selectedRequirement.clientName }}</el-descriptions-item>
            <el-descriptions-item label="客户类型">{{ getClientTypeText(selectedRequirement.clientType) }}</el-descriptions-item>
            <el-descriptions-item label="联系电话">{{ selectedRequirement.clientPhone }}</el-descriptions-item>
            <el-descriptions-item label="价格范围">
              {{ formatPriceRange(selectedRequirement.minPrice, selectedRequirement.maxPrice) }}
            </el-descriptions-item>
            <el-descriptions-item label="面积范围">
              {{ formatAreaRange(selectedRequirement.minArea, selectedRequirement.maxArea) }}
            </el-descriptions-item>
            <el-descriptions-item label="期望位置">{{ selectedRequirement.location || '不限' }}</el-descriptions-item>
          </el-descriptions>
        </div>

        <div class="properties-section">
          <h3>匹配的房源列表</h3>
          <el-table
            :data="matchedProperties"
            v-loading="matchLoading"
            @selection-change="handlePropertySelection"
            style="width: 100%"
          >
            <el-table-column type="selection" width="55" />
            <el-table-column prop="title" label="房源标题" min-width="200" show-overflow-tooltip />
            <el-table-column prop="price" label="价格(万)" width="120">
              <template #default="scope">
                {{ (scope.row.price / 10000).toFixed(1) }}
              </template>
            </el-table-column>
            <el-table-column prop="area" label="面积(㎡)" width="100" />
            <el-table-column prop="address" label="地址" min-width="200" show-overflow-tooltip />
            <el-table-column prop="type" label="类型" width="100">
              <template #default="scope">
                {{ getPropertyTypeText(scope.row.type) }}
              </template>
            </el-table-column>
            <el-table-column prop="status" label="状态" width="100">
              <template #default="scope">
                <el-tag :type="getPropertyStatusColor(scope.row.status)">
                  {{ getPropertyStatusText(scope.row.status) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="匹配度" width="100">
              <template #default="scope">
                <el-progress
                  :percentage="calculateMatchScore(scope.row)"
                  :color="getMatchColor(calculateMatchScore(scope.row))"
                  :stroke-width="8"
                />
              </template>
            </el-table-column>
          </el-table>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="handleCloseMatchDialog">取消</el-button>
          <el-button 
            type="primary" 
            @click="handleConfirmMatch"
            :disabled="selectedProperties.length === 0"
          >
            确认匹配 ({{ selectedProperties.length }})
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Search, Refresh } from '@element-plus/icons-vue'

// 导入房源相关的API和工具函数
import {
  queryProperties,
  getPropertyTypeText,
  getPropertyStatusText,
  getPropertyStatusColor,
  type Property
} from '@/api/property'

// 导入匹配相关的API
import {
  manualMatch,
  type ManualMatchParams
} from '@/api/matching'

// 这里需要创建相应的API接口
interface ClientRequirementWithClient {
  id: number
  clientId: number
  clientName: string
  clientPhone: string
  clientEmail?: string
  clientType: number
  minPrice?: number
  maxPrice?: number
  minArea?: number
  maxArea?: number
  location?: string
  propertyType?: string
  otherRequirements?: string
  createdAt: string
  updatedAt: string
}

interface SearchForm {
  clientName?: string
  clientType?: number
  minPrice?: number
  maxPrice?: number
  location?: string
  pageIndex: number
  pageSize: number
}

// 响应式数据
const loading = ref(false)
const submitLoading = ref(false)
const requirementsList = ref<ClientRequirementWithClient[]>([])
const selectedRequirements = ref<ClientRequirementWithClient[]>([])
const selectedRequirement = ref<ClientRequirementWithClient | null>(null)

// 对话框状态
const detailDialogVisible = ref(false)
const editDialogVisible = ref(false)
const requirementFormRef = ref<FormInstance>()
const matchDialogVisible = ref(false)
const matchedProperties = ref<any[]>([])
const selectedProperties = ref<any[]>([])
const matchLoading = ref(false)

// 搜索表单
const searchForm = reactive<SearchForm>({
  clientName: '',
  clientType: undefined,
  minPrice: undefined,
  maxPrice: undefined,
  location: '',
  pageIndex: 1,
  pageSize: 10
})

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

// 编辑表单
const requirementForm = reactive({
  minPrice: undefined as number | undefined,
  maxPrice: undefined as number | undefined,
  minArea: undefined as number | undefined,
  maxArea: undefined as number | undefined,
  location: '',
  propertyType: '',
  otherRequirements: ''
})

// 客户类型选项
const clientTypeOptions = {
  0: '买家',
  1: '卖家',
  2: '租客',
  3: '房东'
}

// 表单验证规则
const requirementRules: FormRules = {
  minPrice: [
    {
      validator: (_rule, value, callback) => {
        if (value !== undefined && requirementForm.maxPrice !== undefined && value > requirementForm.maxPrice) {
          callback(new Error('最低价格不能大于最高价格'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  maxPrice: [
    {
      validator: (_rule, value, callback) => {
        if (value !== undefined && requirementForm.minPrice !== undefined && value < requirementForm.minPrice) {
          callback(new Error('最高价格不能小于最低价格'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  minArea: [
    {
      validator: (_rule, value, callback) => {
        if (value !== undefined && requirementForm.maxArea !== undefined && value > requirementForm.maxArea) {
          callback(new Error('最小面积不能大于最大面积'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  maxArea: [
    {
      validator: (_rule, value, callback) => {
        if (value !== undefined && requirementForm.minArea !== undefined && value < requirementForm.minArea) {
          callback(new Error('最大面积不能小于最小面积'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 获取客户类型文本
const getClientTypeText = (type: number) => {
  return clientTypeOptions[type as keyof typeof clientTypeOptions] || '未知'
}

// 获取客户类型颜色
const getClientTypeColor = (type: number) => {
  const colorMap = {
    0: 'success',
    1: 'warning',
    2: 'info',
    3: 'danger'
  }
  return colorMap[type as keyof typeof colorMap] || ''
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 格式化价格范围
const formatPriceRange = (minPrice?: number, maxPrice?: number) => {
  if (!minPrice && !maxPrice) return '-'
  if (minPrice && maxPrice) return `${minPrice}万 - ${maxPrice}万`
  if (minPrice) return `${minPrice}万以上`
  if (maxPrice) return `${maxPrice}万以下`
  return '-'
}

// 格式化面积范围
const formatAreaRange = (minArea?: number, maxArea?: number) => {
  if (!minArea && !maxArea) return '-'
  if (minArea && maxArea) return `${minArea}㎡ - ${maxArea}㎡`
  if (minArea) return `${minArea}㎡以上`
  if (maxArea) return `${maxArea}㎡以下`
  return '-'
}

// 获取需求列表
const fetchRequirementsList = async () => {
  loading.value = true
  try {
    // 这里需要实现API调用
    // const response = await getClientRequirementsList(searchForm)
    // requirementsList.value = response.data.items
    // pagination.total = response.data.totalCount
    
    // 模拟数据
    requirementsList.value = [
      {
        id: 1,
        clientId: 1,
        clientName: '张三',
        clientPhone: '13800138001',
        clientEmail: 'zhangsan@example.com',
        clientType: 0,
        minPrice: 100,
        maxPrice: 200,
        minArea: 80,
        maxArea: 120,
        location: '朝阳区',
        propertyType: '住宅',
        otherRequirements: '靠近地铁站，周边配套设施齐全',
        createdAt: '2024-01-15T10:30:00',
        updatedAt: '2024-01-15T10:30:00'
      }
    ]
    pagination.total = 1
  } catch (error) {
    console.error('获取需求列表失败:', error)
    ElMessage.error('获取需求列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  fetchRequirementsList()
}

// 重置
const handleReset = () => {
  Object.assign(searchForm, {
    clientName: '',
    clientType: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    location: ''
  })
  pagination.pageIndex = 1
  fetchRequirementsList()
}

// 查看详情
const handleViewDetail = (requirement: ClientRequirementWithClient) => {
  selectedRequirement.value = requirement
  detailDialogVisible.value = true
}

// 编辑需求
const handleEdit = (requirement: ClientRequirementWithClient) => {
  selectedRequirement.value = requirement
  Object.assign(requirementForm, {
    minPrice: requirement.minPrice,
    maxPrice: requirement.maxPrice,
    minArea: requirement.minArea,
    maxArea: requirement.maxArea,
    location: requirement.location,
    propertyType: requirement.propertyType,
    otherRequirements: requirement.otherRequirements
  })
  editDialogVisible.value = true
}

// 匹配房源
const handleMatchProperties = async (requirement: ClientRequirementWithClient) => {
  selectedRequirement.value = requirement
  matchDialogVisible.value = true
  
  // 获取匹配的房源
  await fetchMatchedProperties(requirement)
}

// 获取匹配的房源
const fetchMatchedProperties = async (requirement: ClientRequirementWithClient) => {
  matchLoading.value = true
  try {
    const params = {
      minPrice: requirement.minPrice,
      maxPrice: requirement.maxPrice,
      minArea: requirement.minArea,
      maxArea: requirement.maxArea,
      keyword: requirement.location,
      pageIndex: 1,
      pageSize: 50 // 获取更多房源用于匹配
    }
    
    const response = await queryProperties(params)
    if (response.success && response.data) {
      matchedProperties.value = response.data.items
    } else {
      ElMessage.error('获取匹配房源失败')
      matchedProperties.value = []
    }
  } catch (error) {
    console.error('获取匹配房源失败:', error)
    ElMessage.error('获取匹配房源失败')
    matchedProperties.value = []
  } finally {
    matchLoading.value = false
  }
}

// 表格选择变化
const handleSelectionChange = (selection: ClientRequirementWithClient[]) => {
  selectedRequirements.value = selection
}

// 分页大小变化
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  fetchRequirementsList()
}

// 当前页变化
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  fetchRequirementsList()
}

// 提交编辑
const handleSubmitEdit = async () => {
  if (!requirementFormRef.value || !selectedRequirement.value) return
  
  try {
    await requirementFormRef.value.validate()
    submitLoading.value = true
    
    // 这里需要实现API调用
    // await updateClientRequirements(selectedRequirement.value.clientId, requirementForm)
    
    ElMessage.success('保存成功')
    editDialogVisible.value = false
    fetchRequirementsList()
  } catch (error) {
    console.error('保存失败:', error)
    ElMessage.error('保存失败')
  } finally {
    submitLoading.value = false
  }
}

// 编辑对话框关闭
const handleEditDialogClose = () => {
  requirementFormRef.value?.resetFields()
  Object.assign(requirementForm, {
    minPrice: undefined,
    maxPrice: undefined,
    minArea: undefined,
    maxArea: undefined,
    location: '',
    propertyType: '',
    otherRequirements: ''
  })
}

// 匹配房源对话框关闭
const handleCloseMatchDialog = () => {
  matchDialogVisible.value = false
  selectedProperties.value = []
}

// 处理房源选择
const handlePropertySelection = (selection: any[]) => {
  selectedProperties.value = selection
}

// 确认匹配
const handleConfirmMatch = async () => {
  if (!selectedRequirement.value || selectedProperties.value.length === 0) {
    ElMessage.warning('请选择要匹配的房源')
    return
  }

  try {
    // 批量创建匹配记录
    const matchPromises = selectedProperties.value.map(property => {
      const params: ManualMatchParams = {
        clientId: selectedRequirement.value!.clientId,
        propertyId: property.id
      }
      return manualMatch(params)
    })

    await Promise.all(matchPromises)
    ElMessage.success(`成功匹配 ${selectedProperties.value.length} 个房源`)
    matchDialogVisible.value = false
    selectedProperties.value = []
  } catch (error) {
    console.error('房源匹配失败:', error)
    ElMessage.error('房源匹配失败')
  }
}

// 计算匹配度
const calculateMatchScore = (property: Property): number => {
  if (!selectedRequirement.value) return 0
  
  let score = 0
  let totalCriteria = 0
  
  // 价格匹配
  if (selectedRequirement.value.minPrice || selectedRequirement.value.maxPrice) {
    totalCriteria++
    const minPrice = selectedRequirement.value.minPrice || 0
    const maxPrice = selectedRequirement.value.maxPrice || Infinity
    if (property.price >= minPrice && property.price <= maxPrice) {
      score++
    }
  }
  
  // 面积匹配
  if (selectedRequirement.value.minArea || selectedRequirement.value.maxArea) {
    totalCriteria++
    const minArea = selectedRequirement.value.minArea || 0
    const maxArea = selectedRequirement.value.maxArea || Infinity
    if (property.area >= minArea && property.area <= maxArea) {
      score++
    }
  }
  
  // 位置匹配
  if (selectedRequirement.value.location) {
    totalCriteria++
    if (property.address.includes(selectedRequirement.value.location) || 
        property.city.includes(selectedRequirement.value.location) ||
        property.district.includes(selectedRequirement.value.location)) {
      score++
    }
  }
  
  return totalCriteria > 0 ? Math.round((score / totalCriteria) * 100) : 0
}

// 获取匹配度颜色
const getMatchColor = (score: number): string => {
  if (score >= 80) return '#67c23a'
  if (score >= 60) return '#e6a23c'
  if (score >= 40) return '#f56c6c'
  return '#909399'
}

// 初始化
onMounted(() => {
  fetchRequirementsList()
})
</script>

<style scoped>
.client-requirements-list {
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

.search-form {
  margin-bottom: 0;
}

.table-card {
  margin-bottom: 16px;
}

.pagination-container {
  margin-top: 16px;
  text-align: right;
}

.requirement-detail {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.detail-card {
  margin-bottom: 0;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.match-content {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.requirement-info {
  margin-bottom: 16px;
}

.properties-section {
  margin-bottom: 16px;
}

.properties-section h3 {
  margin-bottom: 8px;
}
</style> 