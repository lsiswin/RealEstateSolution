<template>
  <div class="client-list">
    <div class="page-header">
      <h2>客户管理</h2>
      <p>管理和查看所有客户信息</p>
    </div>
    
    <!-- 搜索和筛选区域 -->
    <el-card class="search-card">
      <el-form :model="searchForm" :inline="true" class="search-form">
        <el-form-item label="客户姓名">
          <el-input
            v-model="searchForm.name"
            placeholder="请输入客户姓名"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="联系电话">
          <el-input
            v-model="searchForm.phone"
            placeholder="请输入联系电话"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="客户类型">
          <el-select
            v-model="searchForm.type"
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
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :loading="loading">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
          <el-button type="success" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            新增客户
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 客户列表 -->
    <el-card class="table-card">
      <el-table
        :data="clientList"
        v-loading="loading"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="客户ID" width="80" />
        <el-table-column prop="name" label="客户姓名" width="120" />
        <el-table-column prop="phone" label="联系电话" width="130" />
        <el-table-column prop="email" label="邮箱" width="180" />
        <el-table-column prop="type" label="客户类型" width="100">
          <template #default="scope">
            <el-tag :type="getClientTypeColor(scope.row.type)">
              {{ getClientTypeText(scope.row.type) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="address" label="地址" min-width="150" show-overflow-tooltip />
       <el-table-column prop="createdAt" label="创建时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="255" fixed="right">
          <template #default="scope">
            <el-button
              type="primary"
              size="small"
              @click="handleView(scope.row)"
            >
              查看
            </el-button>
            <el-button
              type="warning"
              size="small"
              @click="handleEdit(scope.row)"
            >
              编辑
            </el-button>
            <el-button
              type="info"
              size="small"
              @click="handleRequirements(scope.row)"
            >
              需求
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(scope.row)"
            >
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

    <!-- 客户详情/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form
        ref="clientFormRef"
        :model="clientForm"
        :rules="clientRules"
        label-width="100px"
      >
        <el-form-item label="客户姓名" prop="name">
          <el-input v-model="clientForm.name" placeholder="请输入客户姓名" />
        </el-form-item>
        <el-form-item label="联系电话" prop="phone">
          <el-input v-model="clientForm.phone" placeholder="请输入联系电话" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="clientForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="客户类型" prop="type">
          <el-select v-model="clientForm.type" placeholder="请选择客户类型" style="width: 100%">
            <el-option
              v-for="(label, value) in clientTypeOptions"
              :key="value"
              :label="label"
              :value="Number(value)"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="地址">
          <el-input v-model="clientForm.address" placeholder="请输入地址" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input
            v-model="clientForm.notes"
            type="textarea"
            :rows="3"
            placeholder="请输入备注信息"
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
            v-if="!isViewMode"
          >
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Search, Refresh, Plus } from '@element-plus/icons-vue'
import {
  getClients,
  createClient,
  updateClient,
  deleteClient,
  ClientType,
  type Client,
  type GetClientsParams
} from '@/api/client'

const router = useRouter()

// 响应式数据
const loading = ref(false)
const submitLoading = ref(false)
const clientList = ref<Client[]>([])
const selectedClients = ref<Client[]>([])

// 搜索表单
const searchForm = reactive<GetClientsParams>({
  name: '',
  phone: '',
  type: undefined,
  pageIndex: 1,
  pageSize: 10
})

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

// 对话框相关
const dialogVisible = ref(false)
const dialogTitle = ref('')
const isViewMode = ref(false)
const clientFormRef = ref<FormInstance>()

// 客户表单
const clientForm = reactive<Partial<Client>>({
  name: '',
  phone: '',
  email: '',
  type: ClientType.Buyer,
  address: '',
  notes: '',
  agentId: 0
})

// 客户类型选项
const clientTypeOptions = {
  [ClientType.Buyer]: '买家',
  [ClientType.Seller]: '卖家',
  [ClientType.Tenant]: '租客',
  [ClientType.Landlord]: '房东'
}

// 表单验证规则
const clientRules: FormRules = {
  name: [
    { required: true, message: '请输入客户姓名', trigger: 'blur' },
    { min: 2, max: 20, message: '姓名长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  phone: [
    { required: true, message: '请输入联系电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  email: [
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择客户类型', trigger: 'change' }
  ]
}

// 计算属性
const getClientTypeText = (type: ClientType) => {
  return clientTypeOptions[type] || '未知'
}

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

// 获取客户列表
const fetchClientList = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    const response = await getClients(params)
    clientList.value = response.data.items
    pagination.total = response.data.totalCount
  } catch (error) {
    console.error('获取客户列表失败:', error)
    ElMessage.error('获取客户列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  fetchClientList()
}

// 重置
const handleReset = () => {
  Object.assign(searchForm, {
    name: '',
    phone: '',
    type: undefined
  })
  pagination.pageIndex = 1
  fetchClientList()
}

// 新增客户
const handleAdd = () => {
  dialogTitle.value = '新增客户'
  isViewMode.value = false
  resetClientForm()
  dialogVisible.value = true
}

// 查看客户
const handleView = (client: Client) => {
  dialogTitle.value = '客户详情'
  isViewMode.value = true
  Object.assign(clientForm, client)
  dialogVisible.value = true
}

// 编辑客户
const handleEdit = (client: Client) => {
  dialogTitle.value = '编辑客户'
  isViewMode.value = false
  Object.assign(clientForm, client)
  dialogVisible.value = true
}

// 客户需求
const handleRequirements = (client: Client) => {
  router.push(`/client/requirements/${client.id}`)
}

// 删除客户
const handleDelete = async (client: Client) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除客户 "${client.name}" 吗？`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteClient(client.id)
    ElMessage.success('删除成功')
    fetchClientList()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除客户失败:', error)
      ElMessage.error('删除失败')
    }
  }
}

// 表格选择变化
const handleSelectionChange = (selection: Client[]) => {
  selectedClients.value = selection
}

// 分页大小变化
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  fetchClientList()
}

// 当前页变化
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  fetchClientList()
}

// 提交表单
const handleSubmit = async () => {
  if (!clientFormRef.value) return
  
  try {
    await clientFormRef.value.validate()
    submitLoading.value = true
    
    if (clientForm.id) {
      // 编辑
      await updateClient(clientForm.id, clientForm as Client)
      ElMessage.success('更新成功')
    } else {
      // 新增
      await createClient(clientForm as Client)
      ElMessage.success('创建成功')
    }
    
    dialogVisible.value = false
    fetchClientList()
  } catch (error) {
    console.error('提交失败:', error)
    ElMessage.error('操作失败')
  } finally {
    submitLoading.value = false
  }
}

// 对话框关闭
const handleDialogClose = () => {
  clientFormRef.value?.resetFields()
  resetClientForm()
}

// 重置客户表单
const resetClientForm = () => {
  Object.assign(clientForm, {
    id: undefined,
    name: '',
    phone: '',
    email: '',
    type: ClientType.Buyer,
    address: '',
    notes: '',
    agentId: 0
  })
}

// 初始化
onMounted(() => {
  fetchClientList()
})
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

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style> 