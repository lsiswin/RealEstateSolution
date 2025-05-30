<template>
  <div class="template-view">
    <div class="page-header">
      <h2>查看模板</h2>
      <p>{{ template?.name || '模板详情' }}</p>
    </div>
    
    <!-- 模板基本信息 -->
    <el-card class="info-card">
      <template #header>
        <div class="card-header">
          <span>基本信息</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleEdit">编辑模板</el-button>
            <el-button @click="handleBack">返回列表</el-button>
          </div>
        </div>
      </template>
      
      <el-descriptions :column="3" border>
        <el-descriptions-item label="模板名称">{{ template?.name || '-' }}</el-descriptions-item>
        <el-descriptions-item label="模板类型">
          <el-tag :type="template?.type !== undefined ? getContractTypeColor(template.type) : ''">
            {{ template?.type !== undefined ? getContractTypeText(template.type) : '未知' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="版本">{{ template?.version || '-' }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="template?.isActive ? 'success' : 'danger'">
            {{ template?.isActive ? '启用' : '禁用' }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="文件大小">
          {{ formatFileSize(template?.fileSize || 0) }}
        </el-descriptions-item>
        <el-descriptions-item label="创建人">{{ template?.createdBy || '-' }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">
          {{ formatDate(template?.createdAt || '') }}
        </el-descriptions-item>
        <el-descriptions-item label="更新时间">
          {{ formatDate(template?.updatedAt || '') }}
        </el-descriptions-item>
        <el-descriptions-item label="描述" span="3">
          {{ template?.description || '-' }}
        </el-descriptions-item>
      </el-descriptions>
    </el-card>

    <!-- 模板内容 -->
    <el-card class="content-card">
      <template #header>
        <div class="card-header">
          <span>模板内容</span>
          <div class="header-actions">
            <el-button size="small" @click="handlePreview">
              <el-icon><View /></el-icon>
              预览
            </el-button>
            <el-button size="small" @click="handleDownload">
              <el-icon><Download /></el-icon>
              下载
            </el-button>
          </div>
        </div>
      </template>
      
      <div class="content-display" v-html="template?.content"></div>
    </el-card>

    <!-- 预览对话框 -->
    <el-dialog
      v-model="previewDialogVisible"
      title="模板预览"
      width="80%"
      @close="handlePreviewClose"
    >
      <div class="preview-content" v-html="template?.content"></div>
      <template #footer>
        <el-button @click="previewDialogVisible = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { View, Download } from '@element-plus/icons-vue'
import { 
  getContractTemplate, 
  exportContractTemplate,
  type ContractTemplate,
  getContractTypeText,
  getContractTypeColor
} from '@/api/contractTemplate'

// 响应式数据
const route = useRoute()
const router = useRouter()
const template = ref<ContractTemplate | null>(null)
const previewDialogVisible = ref(false)
const loading = ref(false)

// 格式化文件大小
const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// 格式化日期
const formatDate = (dateString: string) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 获取模板详情
const fetchTemplateDetail = async (id: number) => {
  try {
    loading.value = true
    const response = await getContractTemplate(id)
    if (response.success && response.data) {
      template.value = response.data
    } else {
      ElMessage.error(response.message || '获取模板详情失败')
      router.push('/contract/templates')
    }
  } catch (error) {
    console.error('获取模板详情失败:', error)
    ElMessage.error('获取模板详情失败')
    router.push('/contract/templates')
  } finally {
    loading.value = false
  }
}

// 编辑模板
const handleEdit = () => {
  if (template.value) {
    router.push(`/contract/templates/edit/${template.value.id}`)
  }
}

// 返回列表
const handleBack = () => {
  router.push('/contract/templates')
}

// 预览
const handlePreview = () => {
  previewDialogVisible.value = true
}

// 关闭预览
const handlePreviewClose = () => {
  previewDialogVisible.value = false
}

// 下载
const handleDownload = async () => {
  if (!template.value) return
  
  try {
    ElMessage.info(`正在下载模板：${template.value.name}`)
    const response = await exportContractTemplate(template.value.id)    
    // 对于blob响应，response.data就是blob数据
    const blob = response instanceof Blob ? response : new Blob([response.data], { 
      type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' 
    })      
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `${template.value.name}.docx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    
    ElMessage.success('下载成功')
  } catch (error) {
    console.error('下载失败:', error)
    ElMessage.error('下载失败')
  }
}

// 初始化
onMounted(() => {
  const templateId = route.params.id as string
  if (templateId && !isNaN(Number(templateId))) {
    fetchTemplateDetail(Number(templateId))
  } else {
    ElMessage.error('无效的模板ID')
    router.push('/contract/templates')
  }
})
</script>

<style scoped>
.template-view {
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

.info-card {
  margin-bottom: 16px;
}

.content-card {
  margin-bottom: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.content-display {
  padding: 20px;
  background: #fafafa;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  min-height: 400px;
  font-family: 'Microsoft YaHei', sans-serif;
  line-height: 1.8;
  color: #333;
}

.content-display h2 {
  color: #333;
  margin-bottom: 20px;
}

.content-display h3 {
  color: #409eff;
  margin: 20px 0 10px 0;
}

.content-display p {
  margin-bottom: 10px;
}

.preview-content {
  padding: 20px;
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  min-height: 400px;
  font-family: 'Microsoft YaHei', sans-serif;
  line-height: 1.8;
  color: #333;
}

.preview-content h2 {
  color: #333;
  margin-bottom: 20px;
}

.preview-content h3 {
  color: #409eff;
  margin: 20px 0 10px 0;
}

.preview-content p {
  margin-bottom: 10px;
}
</style> 