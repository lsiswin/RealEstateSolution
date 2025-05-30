<template>
  <div class="template-edit">
    <div class="page-header">
      <h2>{{ isEdit ? '编辑模板' : '新增模板' }}</h2>
      <p>{{ isEdit ? '修改模板信息和内容' : '创建新的合同模板' }}</p>
    </div>
    
    <!-- 模板表单 -->
    <el-card class="form-card">
      <template #header>
        <span>基本信息</span>
      </template>
      
      <el-form
        ref="templateFormRef"
        :model="templateForm"
        :rules="templateRules"
        label-width="100px"
        class="template-form"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="模板名称" prop="name">
              <el-input v-model="templateForm.name" placeholder="请输入模板名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="模板类型" prop="type">
              <el-select v-model="templateForm.type" placeholder="请选择模板类型" style="width: 100%">
                <el-option label="买卖合同" :value="ContractType.Sale" />
                <el-option label="租赁合同" :value="ContractType.Rent" />
                <el-option label="委托合同" :value="ContractType.Commission" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="版本号" prop="version">
              <el-input v-model="templateForm.version" placeholder="请输入版本号，如：1.0" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态">
              <el-switch
                v-model="templateForm.isActive"
                active-text="启用"
                inactive-text="禁用"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="描述">
          <el-input
            v-model="templateForm.description"
            type="textarea"
            :rows="2"
            placeholder="请输入模板描述"
          />
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 文档内容编辑器 -->
    <el-card class="editor-card">
      <template #header>
        <div class="card-header">
          <span>模板内容</span>
          <div class="header-actions">
            <el-button size="small" @click="handleFormatDocument">
              <el-icon><Document /></el-icon>
              格式化
            </el-button>
            <el-button size="small" @click="handlePreview">
              <el-icon><View /></el-icon>
              预览
            </el-button>
          </div>
        </div>
      </template>
      
      <div class="editor-container">
        <div
          ref="editorRef"
          class="rich-editor"
          contenteditable
          v-html="templateContent"
          @input="handleContentChange"
          @paste="handlePaste"
        ></div>
      </div>
    </el-card>

    <!-- 操作按钮 -->
    <div class="action-buttons">
      <el-button @click="handleCancel">取消</el-button>
      <el-button type="primary" @click="handleSubmit" :loading="submitLoading">
        {{ isEdit ? '更新' : '保存' }}
      </el-button>
    </div>

    <!-- 预览对话框 -->
    <el-dialog
      v-model="previewDialogVisible"
      title="模板预览"
      width="80%"
      @close="handlePreviewClose"
    >
      <div class="preview-content" v-html="templateContent"></div>
      <template #footer>
        <el-button @click="previewDialogVisible = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Document, View } from '@element-plus/icons-vue'
import { 
  getContractTemplate, 
  createContractTemplate, 
  updateContractTemplate,
  type ContractTemplateCreate,
  ContractType
} from '@/api/contractTemplate'

// 响应式数据
const route = useRoute()
const router = useRouter()
const templateFormRef = ref<FormInstance>()
const editorRef = ref<HTMLElement>()
const submitLoading = ref(false)
const previewDialogVisible = ref(false)
const templateContent = ref('')
const loading = ref(false)

// 判断是否为编辑模式
const isEdit = computed(() => route.params.id !== undefined)

// 编辑表单
const templateForm = reactive<ContractTemplateCreate & { id?: number }>({
  id: undefined,
  name: '',
  type: ContractType.Sale,
  version: '1.0',
  isActive: true,
  description: '',
  content: ''
})

// 表单验证规则
const templateRules: FormRules = {
  name: [
    { required: true, message: '请输入模板名称', trigger: 'blur' },
    { min: 2, max: 100, message: '模板名称长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择模板类型', trigger: 'change' }
  ],
  version: [
    { required: true, message: '请输入版本号', trigger: 'blur' },
    { pattern: /^\d+\.\d+$/, message: '版本号格式应为：x.x', trigger: 'blur' }
  ]
}

// 获取模板详情（编辑模式）
const fetchTemplateDetail = async (id: number) => {
  try {
    loading.value = true
    const response = await getContractTemplate(id)
    if (response.success && response.data) {
      const template = response.data
      Object.assign(templateForm, {
        id: template.id,
        name: template.name,
        type: template.type,
        version: template.version,
        isActive: template.isActive,
        description: template.description || ''
      })
      templateContent.value = template.content
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

// 初始化新增模式的默认内容
const initDefaultContent = () => {
  templateContent.value = `
    <h2 style="text-align: center; margin-bottom: 30px;">合同标题</h2>
    
    <p><strong>甲方：</strong>_________________</p>
    <p><strong>身份证号：</strong>_________________</p>
    <p><strong>联系电话：</strong>_________________</p>
    <p><strong>地址：</strong>_________________</p>
    
    <br>
    
    <p><strong>乙方：</strong>_________________</p>
    <p><strong>身份证号：</strong>_________________</p>
    <p><strong>联系电话：</strong>_________________</p>
    <p><strong>地址：</strong>_________________</p>
    
    <br>
    
    <p>请在此处编辑合同内容...</p>
    
    <h3>第一条 合同条款</h3>
    <p>请输入具体条款内容...</p>
    
    <br><br>
    
    <div style="display: flex; justify-content: space-between;">
      <div>
        <p><strong>甲方（签字）：</strong>_________________</p>
        <p><strong>日期：</strong>_________年_____月_____日</p>
      </div>
      <div>
        <p><strong>乙方（签字）：</strong>_________________</p>
        <p><strong>日期：</strong>_________年_____月_____日</p>
      </div>
    </div>
  `
}

// 格式化文档
const handleFormatDocument = () => {
  if (editorRef.value) {
    // 简单的格式化处理
    let content = editorRef.value.innerHTML
    content = content.replace(/<p><\/p>/g, '<br>')
    content = content.replace(/\n/g, '<br>')
    content = content.replace(/\s+/g, ' ')
    editorRef.value.innerHTML = content
    handleContentChange()
  }
  ElMessage.success('文档格式化完成')
}

// 预览
const handlePreview = () => {
  previewDialogVisible.value = true
}

// 关闭预览
const handlePreviewClose = () => {
  previewDialogVisible.value = false
}

// 内容变化处理
const handleContentChange = () => {
  if (editorRef.value) {
    templateContent.value = editorRef.value.innerHTML
  }
}

// 粘贴处理
const handlePaste = (event: ClipboardEvent) => {
  event.preventDefault()
  const text = event.clipboardData?.getData('text/plain') || ''
  document.execCommand('insertText', false, text)
}

// 取消
const handleCancel = () => {
  router.push('/contract/templates')
}

// 提交表单
const handleSubmit = async () => {
  if (!templateFormRef.value) return
  
  try {
    await templateFormRef.value.validate()
    
    if (!templateContent.value.trim()) {
      ElMessage.error('请输入模板内容')
      return
    }
    
    submitLoading.value = true
    
    const templateData: ContractTemplateCreate = {
      name: templateForm.name,
      description: templateForm.description,
      type: templateForm.type,
      content: templateContent.value,
      version: templateForm.version,
      isActive: templateForm.isActive
    }
    
    let response
    if (isEdit.value && templateForm.id) {
      response = await updateContractTemplate(templateForm.id, templateData)
    } else {
      response = await createContractTemplate(templateData)
    }
    if (response.success) {
      ElMessage.success(isEdit.value ? '模板更新成功' : '模板创建成功')
      router.push('/contract/templates')
    } else {
      ElMessage.error(response.message || '操作失败')
    }
  } catch (error) {
    console.error('提交失败:', error)
    ElMessage.error('操作失败')
  } finally {
    submitLoading.value = false
  }
}

// 初始化
onMounted(() => {
  if (isEdit.value) {
    const templateId = route.params.id as string
    if (templateId && !isNaN(Number(templateId))) {
      fetchTemplateDetail(Number(templateId))
    } else {
      ElMessage.error('无效的模板ID')
      router.push('/contract/templates')
    }
  } else {
    initDefaultContent()
  }
})
</script>

<style scoped>
.template-edit {
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

.form-card {
  margin-bottom: 16px;
}

.template-form {
  margin-bottom: 0;
}

.editor-card {
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

.editor-container {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
}

.rich-editor {
  padding: 20px;
  min-height: 500px;
  border: none;
  outline: none;
  font-family: 'Microsoft YaHei', sans-serif;
  font-size: 14px;
  line-height: 1.8;
  color: #333;
  background: #fafafa;
}

.rich-editor:focus {
  background: #fff;
}

.rich-editor h2 {
  color: #333;
  margin-bottom: 20px;
}

.rich-editor h3 {
  color: #409eff;
  margin: 20px 0 10px 0;
}

.rich-editor p {
  margin-bottom: 10px;
}

.action-buttons {
  text-align: center;
  margin-top: 24px;
}

.action-buttons .el-button {
  margin: 0 8px;
  min-width: 100px;
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