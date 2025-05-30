<template>
  <div class="contract-list">
    <div class="page-header">
      <h2>合同列表</h2>
      <p>管理所有合同文档</p>
    </div>
    
    <!-- 搜索和操作区域 -->
    <el-card class="search-card">
      <el-form :model="searchForm" :inline="true" class="search-form">
        <el-form-item label="合同名称">
          <el-input
            v-model="searchForm.keyword"
            placeholder="请输入合同名称"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="合同类型">
          <el-select
            v-model="searchForm.type"
            placeholder="请选择合同类型"
            clearable
            style="width: 150px"
          >
            <el-option label="买卖合同" :value="ContractType.Sale" />
            <el-option label="租赁合同" :value="ContractType.Rent" />
            <el-option label="委托合同" :value="ContractType.Commission" />
          </el-select>
        </el-form-item>
        <el-form-item label="合同状态">
          <el-select
            v-model="searchForm.status"
            placeholder="请选择合同状态"
            clearable
            style="width: 150px"
          >
            <el-option label="草稿" :value="ContractStatus.Draft" />
            <el-option label="待签署" :value="ContractStatus.Pending" />
            <el-option label="已签署" :value="ContractStatus.Signed" />
            <el-option label="已完成" :value="ContractStatus.Completed" />
            <el-option label="已取消" :value="ContractStatus.Cancelled" />
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
            新增合同
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 合同列表 -->
    <el-card class="table-card">
      <el-table
        :data="contractList"
        v-loading="loading"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="合同ID" width="80" />
        <el-table-column prop="title" label="合同名称" min-width="200" show-overflow-tooltip />
        <el-table-column prop="type" label="合同类型" width="120">
          <template #default="scope">
            <el-tag :type="getContractTypeColor(scope.row.type)">
              {{ getContractTypeText(scope.row.type) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="partyAName" label="甲方" width="150" show-overflow-tooltip />
        <el-table-column prop="partyBName" label="乙方" width="150" show-overflow-tooltip />
        <el-table-column prop="amount" label="合同金额" width="120">
          <template #default="scope">
            <span v-if="scope.row.amount">{{ formatAmount(scope.row.amount) }}</span>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="getContractStatusColor(scope.row.status)">
              {{ getContractStatusText(scope.row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="signDate" label="签署日期" width="120">
          <template #default="scope">
            <span v-if="scope.row.signDate">{{ formatDate(scope.row.signDate) }}</span>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="260" fixed="right">
          <template #default="scope">
            <el-button type="primary" size="small" @click="handleView(scope.row)">
              查看
            </el-button>
            <el-button type="warning" size="small" @click="handleEdit(scope.row)">
              编辑
            </el-button>
            <el-button type="success" size="small" @click="handleDownload(scope.row)">
              下载
            </el-button>
            <el-button type="danger" size="small" @click="handleDelete(scope.row)">
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

    <!-- 查看合同对话框 -->
    <el-dialog
      v-model="viewDialogVisible"
      title="查看合同"
      width="80%"
      :before-close="handleViewClose"
    >
      <div class="contract-view" v-if="selectedContract">
        <!-- 合同基本信息 -->
        <el-descriptions :column="3" border class="contract-info">
          <el-descriptions-item label="合同名称">{{ selectedContract.title }}</el-descriptions-item>
          <el-descriptions-item label="合同类型">
            <el-tag :type="getContractTypeColor(selectedContract.type)">
              {{ getContractTypeText(selectedContract.type) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="合同状态">
            <el-tag :type="getContractStatusColor(selectedContract.status)">
              {{ getContractStatusText(selectedContract.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="甲方">{{ selectedContract.partyAName }}</el-descriptions-item>
          <el-descriptions-item label="乙方">{{ selectedContract.partyBName }}</el-descriptions-item>
          <el-descriptions-item label="合同金额">{{ selectedContract.amount ? formatAmount(selectedContract.amount) : '-' }}</el-descriptions-item>
          <el-descriptions-item label="签署日期">{{ selectedContract.signDate ? formatDate(selectedContract.signDate) : '-' }}</el-descriptions-item>
          <el-descriptions-item label="生效日期">{{ selectedContract.startDate ? formatDate(selectedContract.startDate) : '-' }}</el-descriptions-item>
          <el-descriptions-item label="到期日期">{{ selectedContract.endDate ? formatDate(selectedContract.endDate) : '-' }}</el-descriptions-item>
          <el-descriptions-item label="备注" span="3">
            {{ selectedContract.notes || '-' }}
          </el-descriptions-item>
        </el-descriptions>
        
        <!-- 合同内容 -->
        <div class="contract-content">
          <h4>合同内容</h4>
          <div class="content-display" v-html="selectedContract.content"></div>
        </div>
      </div>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="handleViewClose">关闭</el-button>
          <el-button type="primary" @click="handleEdit(selectedContract!)">编辑</el-button>
          <el-button type="success" @click="handleDownload(selectedContract!)">下载</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 新增/编辑合同对话框 -->
    <el-dialog
      v-model="editDialogVisible"
      :title="editMode === 'add' ? '新增合同' : '编辑合同'"
      width="600px"
      @close="handleEditClose"
    >
      <el-form
        ref="contractFormRef"
        :model="contractForm"
        :rules="contractRules"
        label-width="100px"
      >
        <el-form-item label="合同名称" prop="title">
          <el-input v-model="contractForm.title" placeholder="请输入合同名称" />
        </el-form-item>
        <el-form-item label="合同类型" prop="type">
          <el-select v-model="contractForm.type" placeholder="请选择合同类型" style="width: 100%">
            <el-option label="买卖合同" :value="ContractType.Sale" />
            <el-option label="租赁合同" :value="ContractType.Rent" />
            <el-option label="委托合同" :value="ContractType.Commission" />
          </el-select>
        </el-form-item>
        <el-form-item label="甲方" prop="partyAId">
          <el-select v-model="contractForm.partyAId" placeholder="请选择甲方" style="width: 100%">
            <!-- 这里需要根据实际情况填充甲方的选项 -->
          </el-select>
        </el-form-item>
        <el-form-item label="乙方" prop="partyBId">
          <el-select v-model="contractForm.partyBId" placeholder="请选择乙方" style="width: 100%">
            <!-- 这里需要根据实际情况填充乙方的选项 -->
          </el-select>
        </el-form-item>
        <el-form-item label="合同金额">
          <el-input-number
            v-model="contractForm.amount"
            :min="0"
            :precision="2"
            placeholder="请输入合同金额"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="签署日期">
          <el-date-picker
            v-model="contractForm.signDate"
            type="date"
            placeholder="请选择签署日期"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="备注">
          <el-input
            v-model="contractForm.notes"
            type="textarea"
            :rows="3"
            placeholder="请输入备注信息"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="editDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="submitLoading">
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Search, Refresh, Plus } from '@element-plus/icons-vue'
import { 
  getContracts, 
  getContract, 
  createContract, 
  updateContract, 
  deleteContract, 
  generateContractNumber,
  type Contract, 
  type ContractQuery,
  ContractType, 
  ContractStatus 
} from '@/api/contract'

// 响应式数据
const loading = ref(false)
const submitLoading = ref(false)
const contractList = ref<Contract[]>([])
const selectedContracts = ref<Contract[]>([])
const selectedContract = ref<Contract | null>(null)

// 对话框状态
const viewDialogVisible = ref(false)
const editDialogVisible = ref(false)
const editMode = ref<'add' | 'edit'>('add')
const contractFormRef = ref<FormInstance>()

// 搜索表单
const searchForm = reactive<ContractQuery>({
  keyword: '',
  type: undefined,
  status: undefined,
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
const contractForm = reactive({
  id: 0,
  contractNumber: '',
  title: '',
  type: ContractType.Sale,
  status: ContractStatus.Draft,
  propertyId: 0,
  partyAId: 0,
  partyBId: 0,
  amount: 0,
  signDate: '',
  startDate: '',
  endDate: '',
  paymentMethod: '',
  terms: '',
  notes: '',
  content: '',
  createdAt: '',
  updatedAt: ''
})

// 表单验证规则
const contractRules: FormRules = {
  title: [
    { required: true, message: '请输入合同标题', trigger: 'blur' },
    { min: 2, max: 100, message: '合同标题长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择合同类型', trigger: 'change' }
  ],
  partyAId: [
    { required: true, message: '请选择甲方', trigger: 'change' }
  ],
  partyBId: [
    { required: true, message: '请选择乙方', trigger: 'change' }
  ],
  amount: [
    { required: true, message: '请输入合同金额', trigger: 'blur' }
  ]
}

// 获取合同类型文本
const getContractTypeText = (type: ContractType) => {
  const typeMap: Record<ContractType, string> = {
    [ContractType.Sale]: '买卖合同',
    [ContractType.Rent]: '租赁合同',
    [ContractType.Commission]: '委托合同'
  }
  return typeMap[type] || '未知'
}

// 获取合同类型颜色
const getContractTypeColor = (type: ContractType) => {
  const colorMap: Record<ContractType, string> = {
    [ContractType.Sale]: 'success',
    [ContractType.Rent]: 'warning',
    [ContractType.Commission]: 'info'
  }
  return colorMap[type] || ''
}

// 获取合同状态文本
const getContractStatusText = (status: ContractStatus) => {
  const statusMap: Record<ContractStatus, string> = {
    [ContractStatus.Draft]: '草稿',
    [ContractStatus.Pending]: '待签署',
    [ContractStatus.Signed]: '已签署',
    [ContractStatus.Completed]: '已完成',
    [ContractStatus.Cancelled]: '已取消'
  }
  return statusMap[status] || '未知'
}

// 获取合同状态颜色
const getContractStatusColor = (status: ContractStatus) => {
  const colorMap: Record<ContractStatus, string> = {
    [ContractStatus.Draft]: 'info',
    [ContractStatus.Pending]: 'warning',
    [ContractStatus.Signed]: 'success',
    [ContractStatus.Completed]: 'success',
    [ContractStatus.Cancelled]: 'danger'
  }
  return colorMap[status] || ''
}

// 格式化金额
const formatAmount = (amount: number) => {
  return `¥${amount.toLocaleString()}`
}

// 格式化日期
const formatDate = (dateString: string) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 获取合同列表
const fetchContractList = async () => {
  loading.value = true
  try {
    const response = await getContracts({
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    })
    console.log(response.data)
    if (response.data) {
      contractList.value = response.data.items || []
      pagination.total = response.data.total || 0
    }
  } catch (error: any) {
    console.error('获取合同列表失败:', error)
    
    // 检查是否是权限问题
    if (error.response?.status === 403) {
      ElMessage.error('您没有访问合同管理的权限，请联系管理员')
    } else if (error.response?.status === 401) {
      ElMessage.error('登录已过期，请重新登录')
    } else {
      ElMessage.error('获取合同列表失败，请稍后重试')
    }
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  fetchContractList()
}

// 重置
const handleReset = () => {
  Object.assign(searchForm, {
    keyword: '',
    type: undefined,
    status: undefined
  })
  pagination.pageIndex = 1
  fetchContractList()
}

// 新增合同
const handleAdd = async () => {
  editMode.value = 'add'
  resetContractForm()
  
  // 生成合同编号
  try {
    const response = await generateContractNumber(ContractType.Sale)
    contractForm.contractNumber = response.data
  } catch (error) {
    console.error('生成合同编号失败:', error)
  }
  
  editDialogVisible.value = true
}

// 查看合同
const handleView = async (contract: Contract) => {
  try {
    const response = await getContract(contract.id)
    selectedContract.value = response.data
    viewDialogVisible.value = true
  } catch (error) {
    console.error('获取合同详情失败:', error)
    ElMessage.error('获取合同详情失败')
  }
}

// 编辑合同
const handleEdit = async (contract: Contract) => {
  try {
    const response = await getContract(contract.id)
    const contractData = response.data
    
    editMode.value = 'edit'
    Object.assign(contractForm, {
      id: contractData.id,
      contractNumber: contractData.contractNumber,
      title: contractData.title,
      type: contractData.type,
      status: contractData.status,
      propertyId: contractData.propertyId,
      partyAId: contractData.partyAId,
      partyBId: contractData.partyBId,
      amount: contractData.amount,
      signDate: contractData.signDate,
      startDate: contractData.startDate,
      endDate: contractData.endDate,
      paymentMethod: contractData.paymentMethod,
      terms: contractData.terms,
      notes: contractData.notes,
      content: contractData.content
    })
    editDialogVisible.value = true
  } catch (error) {
    console.error('获取合同详情失败:', error)
    ElMessage.error('获取合同详情失败')
  }
}

// 下载合同
const handleDownload = (contract: Contract) => {
  ElMessage.info(`正在下载合同：${contract.title}`)
  // 这里实现下载逻辑
}

// 删除合同
const handleDelete = async (contract: Contract) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除合同 "${contract.title}" 吗？此操作不可恢复！`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteContract(contract.id)
    ElMessage.success('删除成功')
    fetchContractList()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除合同失败:', error)
      ElMessage.error('删除失败')
    }
  }
}

// 表格选择变化
const handleSelectionChange = (selection: Contract[]) => {
  selectedContracts.value = selection
}

// 分页大小变化
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  fetchContractList()
}

// 当前页变化
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  fetchContractList()
}

// 提交表单
const handleSubmit = async () => {
  if (!contractFormRef.value) return
  
  try {
    await contractFormRef.value.validate()
    submitLoading.value = true
    
    if (editMode.value === 'add') {
      await createContract(contractForm as Contract)
      ElMessage.success('新增成功')
    } else {
      await updateContract(contractForm.id, contractForm as Contract)
      ElMessage.success('更新成功')
    }
    
    editDialogVisible.value = false
    fetchContractList()
  } catch (error) {
    console.error('提交失败:', error)
    ElMessage.error('操作失败')
  } finally {
    submitLoading.value = false
  }
}

// 查看对话框关闭
const handleViewClose = () => {
  viewDialogVisible.value = false
  selectedContract.value = null
}

// 编辑对话框关闭
const handleEditClose = () => {
  contractFormRef.value?.resetFields()
  resetContractForm()
}

// 重置合同表单
const resetContractForm = () => {
  Object.assign(contractForm, {
    id: 0,
    contractNumber: '',
    title: '',
    type: ContractType.Sale,
    status: ContractStatus.Draft,
    propertyId: 0,
    partyAId: 0,
    partyBId: 0,
    amount: 0,
    signDate: '',
    startDate: '',
    endDate: '',
    paymentMethod: '',
    terms: '',
    notes: '',
    content: '',
    createdAt: '',
    updatedAt: ''
  })
}

// 初始化
onMounted(() => {
  fetchContractList()
})
</script>

<style scoped>
.contract-list {
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

.contract-view {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.contract-info {
  margin-bottom: 16px;
}

.contract-content {
  margin-top: 16px;
}

.contract-content h4 {
  margin-bottom: 8px;
  color: #333;
}

.content-display {
  background: #f5f5f5;
  padding: 16px;
  border-radius: 4px;
}

.content-display p {
  margin: 0 0 12px 0;
  color: #666;
  line-height: 1.6;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style> 