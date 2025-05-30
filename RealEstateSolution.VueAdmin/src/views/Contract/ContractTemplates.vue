<template>
  <div class="contract-templates">
    <div class="page-header">
      <h2>合同模板</h2>
      <p>管理和编辑合同模板文档</p>
    </div>
    
    <!-- 搜索和操作区域 -->
    <el-card class="search-card">
      <el-form :model="searchForm" :inline="true" class="search-form">
        <el-form-item label="模板名称">
          <el-input
            v-model="searchForm.name"
            placeholder="请输入模板名称"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="模板类型">
          <el-select
            v-model="searchForm.type"
            placeholder="请选择模板类型"
            clearable
            style="width: 150px"
          >
            <el-option label="买卖合同" :value="ContractType.Sale" />
            <el-option label="租赁合同" :value="ContractType.Rent" />
            <el-option label="委托合同" :value="ContractType.Commission" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select
            v-model="searchForm.status"
            placeholder="请选择状态"
            clearable
            style="width: 120px"
          >
            <el-option label="启用" value="active" />
            <el-option label="禁用" value="inactive" />
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
            新增模板
          </el-button>
          <el-button type="info" @click="handleImport">
            <el-icon><Upload /></el-icon>
            导入Word
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 模板列表 -->
    <el-card class="table-card">
      <el-table
        :data="templateList"
        v-loading="loading"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="模板ID" width="80" />
        <el-table-column prop="name" label="模板名称" min-width="200" show-overflow-tooltip />
        <el-table-column prop="type" label="模板类型" width="120">
          <template #default="scope">
            <el-tag :type="getTemplateTypeColor(scope.row.type)">
              {{ getTemplateTypeText(scope.row.type) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="description" label="描述" min-width="150" show-overflow-tooltip />
        <el-table-column prop="isActive" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.isActive ? 'success' : 'danger'">
              {{ scope.row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
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

    <!-- 文件上传对话框 -->
    <el-dialog
      v-model="uploadDialogVisible"
      title="导入Word文档"
      width="500px"
    >
      <el-upload
        ref="uploadRef"
        class="upload-demo"
        drag
        :auto-upload="false"
        :limit="1"
        accept=".doc,.docx"
        :on-change="handleFileChange"
        :on-exceed="handleExceed"
      >
        <el-icon class="el-icon--upload"><UploadFilled /></el-icon>
        <div class="el-upload__text">
          将Word文件拖到此处，或<em>点击上传</em>
        </div>
        <template #tip>
          <div class="el-upload__tip">
            只能上传.doc/.docx文件，且不超过10MB
          </div>
        </template>
      </el-upload>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="uploadDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleUpload" :loading="uploadLoading">
            确认上传
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox, type UploadInstance } from 'element-plus'
import { Search, Refresh, Plus, Upload, UploadFilled } from '@element-plus/icons-vue'
import { 
  getContractTemplates,
  deleteContractTemplate,
  exportContractTemplate,
  importContractTemplate,
  ContractType,
  ContractTemplate
} from '@/api/contractTemplate'

// 响应式数据
const router = useRouter()
const loading = ref(false)
const uploadLoading = ref(false)
const templateList = ref<ContractTemplate[]>([])
const selectedTemplates = ref<ContractTemplate[]>([])
const selectedFile = ref<File | null>(null)

// 对话框状态
const uploadDialogVisible = ref(false)
const uploadRef = ref<UploadInstance>()

// 搜索表单
const searchForm = reactive({
  name: '',
  type: undefined as ContractType | undefined,
  status: '',
  pageIndex: 1,
  pageSize: 10
})

// 分页信息
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

// 获取模板类型文本
const getTemplateTypeText = (type: ContractType) => {
  const typeMap: Record<ContractType, string> = {
    [ContractType.Sale]: '买卖合同',
    [ContractType.Rent]: '租赁合同',
    [ContractType.Commission]: '委托合同'
  }
  return typeMap[type] || '未知'
}

// 获取模板类型颜色
const getTemplateTypeColor = (type: ContractType) => {
  const colorMap: Record<ContractType, string> = {
    [ContractType.Sale]: 'success',
    [ContractType.Rent]: 'warning',
    [ContractType.Commission]: 'info'
  }
  return colorMap[type] || ''
}

// 格式化日期
const formatDate = (dateString: string) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 获取模板列表
const fetchTemplateList = async () => {
  loading.value = true
  try {
    // 构建查询参数
    const queryParams = {
      name: searchForm.name || undefined,
      type: searchForm.type,
      isActive: searchForm.status === 'active' ? true : searchForm.status === 'inactive' ? false : undefined,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    const response = await getContractTemplates(queryParams)
    
    // 检查响应数据结构
    const responseData = response.data as any
    
    if (responseData && responseData.items) {
      templateList.value = responseData.items
      pagination.total = responseData.totalCount
    } else if (responseData && responseData.success && responseData.data) {
      templateList.value = responseData.data.items
      pagination.total = responseData.data.totalCount
    } else {
      console.error('未知的响应数据格式:', responseData)
      ElMessage.error('获取模板列表失败：数据格式错误')
    }
  } catch (error: any) {
    console.error('获取模板列表异常:', error)
    // 检查是否是权限问题
    if (error.response?.status === 403) {
      ElMessage.error('您没有访问合同模板的权限，请联系管理员')
    } else if (error.response?.status === 401) {
      ElMessage.error('登录已过期，请重新登录')
    } else {
      ElMessage.error('获取模板列表失败，请稍后重试')
    }
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  fetchTemplateList()
}

// 重置
const handleReset = () => {
  Object.assign(searchForm, {
    name: '',
    type: undefined,
    status: ''
  })
  pagination.pageIndex = 1
  fetchTemplateList()
}

// 新增模板
const handleAdd = () => {
  router.push('/contract/templates/add')
}

// 查看模板
const handleView = (template: ContractTemplate) => {
  router.push(`/contract/templates/view/${template.id}`)
}

// 编辑模板
const handleEdit = (template: ContractTemplate) => {
  router.push(`/contract/templates/edit/${template.id}`)
}

// 下载模板
const handleDownload = async (template: ContractTemplate) => {
  try {
    ElMessage.info(`正在下载模板：${template.name}`)
    const response = await exportContractTemplate(template.id)
    
    // 对于blob响应，response.data就是blob数据
    const blob = response instanceof Blob ? response : new Blob([response.data], { 
      type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' 
    })
    
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `${template.name}.docx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    ElMessage.success('下载成功')   
  } catch (error: any) {
    console.error('下载失败:', error)
    if (error.response?.status === 404) {
      ElMessage.error('模板不存在')
    } else if (error.response?.status === 403) {
      ElMessage.error('没有下载权限')
    } else {
      ElMessage.error('下载失败，请稍后重试')
    }
  }
}

// 删除模板
const handleDelete = async (template: ContractTemplate) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除模板 "${template.name}" 吗？此操作不可恢复！`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // 调用删除模板API
    const response = await deleteContractTemplate(template.id)
    if (response.success) {
      ElMessage.success('删除成功')
      fetchTemplateList()
    } else {
      ElMessage.error(response.message || '删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除模板失败:', error)
      ElMessage.error('删除失败')
    }
  }
}

// 导入Word文档
const handleImport = () => {
  uploadDialogVisible.value = true
}

// 文件变化处理
const handleFileChange = (file: any) => {
  console.log('选择文件:', file)
  selectedFile.value = file.raw
}

// 文件超出限制
const handleExceed = () => {
  ElMessage.warning('只能上传一个文件')
}

// 上传文件
const handleUpload = async () => {
  if (!selectedFile.value) {
    ElMessage.warning('请选择要上传的文件')
    return
  }
  
  uploadLoading.value = true
  try {
    // 默认使用买卖合同类型，实际应用中可以让用户选择
    const response = await importContractTemplate(selectedFile.value, ContractType.Sale)
    
    if (response.success) {
      ElMessage.success('Word文档导入成功')
      uploadDialogVisible.value = false
      selectedFile.value = null
      fetchTemplateList()
    } else {
      ElMessage.error(response.message || '导入失败')
    }
  } catch (error) {
    console.error('导入失败:', error)
    ElMessage.error('导入失败')
  } finally {
    uploadLoading.value = false
  }
}

// 表格选择变化
const handleSelectionChange = (selection: ContractTemplate[]) => {
  selectedTemplates.value = selection
}

// 分页大小变化
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  fetchTemplateList()
}

// 当前页变化
const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  fetchTemplateList()
}

// 初始化
onMounted(() => {
  fetchTemplateList()
})
</script>

<style scoped>
.contract-templates {
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

:deep(.upload-demo) {
  width: 100%;
}
</style> 