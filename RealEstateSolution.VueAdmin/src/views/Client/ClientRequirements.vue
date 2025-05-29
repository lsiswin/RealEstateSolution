<template>
  <div class="client-requirements">
    <div class="page-header">
      <h2>客户需求管理</h2>
      <p>管理客户的房源需求信息</p>
    </div>
    
    <!-- 客户信息卡片 -->
    <el-card class="client-info-card" v-if="clientInfo">
      <template #header>
        <span>客户信息</span>
      </template>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="客户姓名">{{ clientInfo.name }}</el-descriptions-item>
        <el-descriptions-item label="联系电话">{{ clientInfo.phone }}</el-descriptions-item>
        <el-descriptions-item label="邮箱">{{ clientInfo.email || '-' }}</el-descriptions-item>
        <el-descriptions-item label="客户类型">
          <el-tag :type="getClientTypeColor(clientInfo.type)">
            {{ getClientTypeText(clientInfo.type) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="地址">{{ clientInfo.address || '-' }}</el-descriptions-item>
        <el-descriptions-item label="经纪人">{{ clientInfo.agentName || '-' }}</el-descriptions-item>
      </el-descriptions>
    </el-card>

    <!-- 需求信息卡片 -->
    <el-card class="requirements-card">
      <template #header>
        <div class="card-header">
          <span>需求信息</span>
          <el-button 
            type="primary" 
            @click="handleEdit"
            :disabled="!clientInfo"
          >
            {{ requirements ? '编辑需求' : '添加需求' }}
          </el-button>
        </div>
      </template>
      
      <div v-if="requirements" class="requirements-content">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="价格范围">
            {{ formatPriceRange(requirements.minPrice, requirements.maxPrice) }}
          </el-descriptions-item>
          <el-descriptions-item label="面积范围">
            {{ formatAreaRange(requirements.minArea, requirements.maxArea) }}
          </el-descriptions-item>
          <el-descriptions-item label="期望位置">
            {{ requirements.location || '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="房源类型">
            {{ requirements.propertyType || '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="其他要求" span="2">
            {{ requirements.otherRequirements || '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="创建时间">
            {{ formatDate(requirements.createdAt) }}
          </el-descriptions-item>
          <el-descriptions-item label="更新时间">
            {{ formatDate(requirements.updatedAt) }}
          </el-descriptions-item>
        </el-descriptions>
      </div>
      
      <div v-else class="empty-requirements">
        <el-empty description="暂无需求信息">
          <el-button type="primary" @click="handleEdit">添加需求</el-button>
        </el-empty>
      </div>
    </el-card>

    <!-- 编辑需求对话框 -->
    <el-dialog
      v-model="dialogVisible"
      title="编辑客户需求"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form
        ref="requirementsFormRef"
        :model="requirementsForm"
        :rules="requirementsRules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="最低价格" prop="minPrice">
              <el-input-number
                v-model="requirementsForm.minPrice"
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
                v-model="requirementsForm.maxPrice"
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
                v-model="requirementsForm.minArea"
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
                v-model="requirementsForm.maxArea"
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
            v-model="requirementsForm.location"
            placeholder="请输入期望位置"
          />
        </el-form-item>
        
        <el-form-item label="房源类型">
          <el-input
            v-model="requirementsForm.propertyType"
            placeholder="请输入房源类型，如：住宅、商铺、写字楼等"
          />
        </el-form-item>
        
        <el-form-item label="其他要求">
          <el-input
            v-model="requirementsForm.otherRequirements"
            type="textarea"
            :rows="4"
            placeholder="请输入其他具体要求"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button
            type="primary"
            @click="handleSubmit"
            :loading="submitLoading"
          >
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import {
  getClient,
  getClientRequirements,
  updateClientRequirements,
  ClientType,
  type Client,
  type ClientRequirement,
  type ClientRequirementDto
} from '@/api/client'

const route = useRoute()
const router = useRouter()

// 响应式数据
const loading = ref(false)
const submitLoading = ref(false)
const clientInfo = ref<Client | null>(null)
const requirements = ref<ClientRequirement | null>(null)

// 对话框相关
const dialogVisible = ref(false)
const requirementsFormRef = ref<FormInstance>()

// 需求表单
const requirementsForm = reactive<ClientRequirementDto>({
  minPrice: undefined,
  maxPrice: undefined,
  minArea: undefined,
  maxArea: undefined,
  location: '',
  propertyType: '',
  otherRequirements: ''
})

// 表单验证规则
const requirementsRules: FormRules = {
  minPrice: [
    {
      validator: (rule, value, callback) => {
        if (value !== undefined && requirementsForm.maxPrice !== undefined && value > requirementsForm.maxPrice) {
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
      validator: (rule, value, callback) => {
        if (value !== undefined && requirementsForm.minPrice !== undefined && value < requirementsForm.minPrice) {
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
      validator: (rule, value, callback) => {
        if (value !== undefined && requirementsForm.maxArea !== undefined && value > requirementsForm.maxArea) {
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
      validator: (rule, value, callback) => {
        if (value !== undefined && requirementsForm.minArea !== undefined && value < requirementsForm.minArea) {
          callback(new Error('最大面积不能小于最小面积'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 客户类型选项
const clientTypeOptions = {
  [ClientType.Buyer]: '买家',
  [ClientType.Seller]: '卖家',
  [ClientType.Tenant]: '租客',
  [ClientType.Landlord]: '房东'
}

// 获取客户类型文本
const getClientTypeText = (type: ClientType) => {
  return clientTypeOptions[type] || '未知'
}

// 获取客户类型颜色
const getClientTypeColor = (type: ClientType) => {
  const colorMap = {
    [ClientType.Buyer]: 'success',
    [ClientType.Seller]: 'warning',
    [ClientType.Tenant]: 'info',
    [ClientType.Landlord]: 'danger'
  }
  return colorMap[type] || ''
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

// 获取客户信息
const fetchClientInfo = async (clientId: number) => {
  try {
    const client = await getClient(clientId)
    clientInfo.value = client
  } catch (error) {
    console.error('获取客户信息失败:', error)
    ElMessage.error('获取客户信息失败')
  }
}

// 获取客户需求
const fetchClientRequirements = async (clientId: number) => {
  try {
    const clientRequirements = await getClientRequirements(clientId)
    requirements.value = clientRequirements
  } catch (error) {
    console.error('获取客户需求失败:', error)
    // 如果是404错误，说明还没有需求信息，这是正常的
    if (error?.response?.status !== 404) {
      ElMessage.error('获取客户需求失败')
    }
  }
}

// 编辑需求
const handleEdit = () => {
  if (requirements.value) {
    // 编辑现有需求
    Object.assign(requirementsForm, {
      minPrice: requirements.value.minPrice,
      maxPrice: requirements.value.maxPrice,
      minArea: requirements.value.minArea,
      maxArea: requirements.value.maxArea,
      location: requirements.value.location,
      propertyType: requirements.value.propertyType,
      otherRequirements: requirements.value.otherRequirements
    })
  } else {
    // 新增需求
    resetRequirementsForm()
  }
  dialogVisible.value = true
}

// 提交表单
const handleSubmit = async () => {
  if (!requirementsFormRef.value || !clientInfo.value) return
  
  try {
    await requirementsFormRef.value.validate()
    submitLoading.value = true
    
    await updateClientRequirements(clientInfo.value.id, requirementsForm)
    ElMessage.success('保存成功')
    
    dialogVisible.value = false
    await fetchClientRequirements(clientInfo.value.id)
  } catch (error) {
    console.error('保存失败:', error)
    ElMessage.error('保存失败')
  } finally {
    submitLoading.value = false
  }
}

// 对话框关闭
const handleDialogClose = () => {
  requirementsFormRef.value?.resetFields()
  resetRequirementsForm()
}

// 重置需求表单
const resetRequirementsForm = () => {
  Object.assign(requirementsForm, {
    minPrice: undefined,
    maxPrice: undefined,
    minArea: undefined,
    maxArea: undefined,
    location: '',
    propertyType: '',
    otherRequirements: ''
  })
}

// 初始化
onMounted(async () => {
  const clientId = Number(route.params.id)
  if (!clientId) {
    ElMessage.error('客户ID无效')
    router.push('/client/list')
    return
  }
  
  await Promise.all([
    fetchClientInfo(clientId),
    fetchClientRequirements(clientId)
  ])
})
</script>

<style scoped>
.client-requirements {
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

.client-info-card {
  margin-bottom: 16px;
}

.requirements-card {
  margin-bottom: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.requirements-content {
  padding: 0;
}

.empty-requirements {
  padding: 40px;
  text-align: center;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>