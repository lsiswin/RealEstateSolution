<template>
  <div class="property-edit">
    <!-- 页面标题 -->
    <div class="page-header">
      <el-button @click="goBack" type="text" class="back-btn">
        <el-icon><ArrowLeft /></el-icon>
        返回详情
      </el-button>
      <h2>{{ isEdit ? '编辑房源' : '新增房源' }}</h2>
    </div>

    <div v-loading="loading">
      <el-form
        ref="propertyFormRef"
        :model="propertyForm"
        :rules="propertyRules"
        label-width="120px"
        class="property-form"
      >
        <el-card class="form-card">
          <template #header>
            <span>基本信息</span>
          </template>
          
          <el-row :gutter="24">
            <el-col :span="12">
              <el-form-item label="房源标题" prop="title">
                <el-input v-model="propertyForm.title" placeholder="请输入房源标题" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="房源类型" prop="type">
                <el-select v-model="propertyForm.type" placeholder="请选择房源类型" style="width: 100%">
                  <el-option
                    v-for="(text, value) in propertyTypeOptions"
                    :key="value"
                    :label="text"
                    :value="Number(value)"
                  />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :span="12">
              <el-form-item label="价格(元)" prop="price">
                <el-input-number
                  v-model="propertyForm.price"
                  :min="0"
                  :max="999999999"
                  placeholder="请输入价格"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="面积(㎡)" prop="area">
                <el-input-number
                  v-model="propertyForm.area"
                  :min="0"
                  :max="99999"
                  placeholder="请输入面积"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :span="12">
              <el-form-item label="房源状态" prop="status">
                <el-select v-model="propertyForm.status" placeholder="请选择房源状态" style="width: 100%">
                  <el-option
                    v-for="(text, value) in propertyStatusOptions"
                    :key="value"
                    :label="text"
                    :value="Number(value)"
                  />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="城市" prop="city">
                <el-input v-model="propertyForm.city" placeholder="请输入城市" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :span="12">
              <el-form-item label="区域" prop="district">
                <el-input v-model="propertyForm.district" placeholder="请输入区域" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="详细地址" prop="address">
                <el-input v-model="propertyForm.address" placeholder="请输入详细地址" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-form-item label="房源描述" prop="description">
            <el-input
              v-model="propertyForm.description"
              type="textarea"
              :rows="4"
              placeholder="请输入房源描述"
            />
          </el-form-item>
        </el-card>

        <el-card class="form-card">
          <template #header>
            <span>房屋详情</span>
          </template>
          
          <el-row :gutter="24">
            <el-col :span="8">
              <el-form-item label="卧室数量">
                <el-input-number
                  v-model="propertyForm.bedrooms"
                  :min="0"
                  :max="20"
                  placeholder="卧室数量"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="卫生间数量">
                <el-input-number
                  v-model="propertyForm.bathrooms"
                  :min="0"
                  :max="20"
                  placeholder="卫生间数量"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="楼层">
                <el-input-number
                  v-model="propertyForm.floor"
                  :min="1"
                  :max="200"
                  placeholder="楼层"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :span="8">
              <el-form-item label="总楼层">
                <el-input-number
                  v-model="propertyForm.totalFloors"
                  :min="1"
                  :max="200"
                  placeholder="总楼层"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="建造年份">
                <el-input-number
                  v-model="propertyForm.yearBuilt"
                  :min="1900"
                  :max="2030"
                  placeholder="建造年份"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="朝向">
                <el-select v-model="propertyForm.orientation" placeholder="请选择朝向" style="width: 100%">
                  <el-option
                    v-for="(text, value) in orientationOptions"
                    :key="value"
                    :label="text"
                    :value="text"
                  />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :span="12">
              <el-form-item label="装修情况">
                <el-select v-model="propertyForm.decoration" placeholder="请选择装修情况" style="width: 100%">
                  <el-option
                    v-for="(text, value) in decorationOptions"
                    :key="value"
                    :label="text"
                    :value="text"
                  />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="配套设施">
                <el-input
                  v-model="facilitiesInput"
                  placeholder="请输入配套设施，用逗号分隔"
                />
              </el-form-item>
            </el-col>
          </el-row>
        </el-card>

        <el-card class="form-card">
          <template #header>
            <span>房源图片</span>
          </template>
          
          <el-upload
            v-model:file-list="fileList"
            action="#"
            list-type="picture-card"
            :auto-upload="false"
            :on-preview="handlePictureCardPreview"
            :on-remove="handleRemove"
            :before-upload="beforeUpload"
            multiple
          >
            <el-icon><Plus /></el-icon>
          </el-upload>

          <el-dialog v-model="dialogVisible">
            <img w-full :src="dialogImageUrl" alt="Preview Image" />
          </el-dialog>
        </el-card>

        <div class="form-actions">
          <el-button @click="goBack">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="submitLoading">
            {{ isEdit ? '更新' : '创建' }}
          </el-button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules, type UploadProps, type UploadUserFile } from 'element-plus'
import { ArrowLeft, Plus } from '@element-plus/icons-vue'

// API imports
import {
  getPropertyById,
  createProperty,
  updateProperty,
  PropertyType,
  PropertyStatus,
  type Property
} from '@/api/property'

const route = useRoute()
const router = useRouter()

// 响应式数据
const loading = ref(false)
const submitLoading = ref(false)
const propertyFormRef = ref<FormInstance>()
const dialogVisible = ref(false)
const dialogImageUrl = ref('')
const fileList = ref<UploadUserFile[]>([])
const facilitiesInput = ref('')

// 计算属性
const propertyId = computed(() => Number(route.params.id))
const isEdit = computed(() => propertyId.value > 0)

// 房源表单
const propertyForm = reactive<Partial<Property>>({
  title: '',
  description: '',
  type: PropertyType.Apartment,
  status: PropertyStatus.Available,
  price: 0,
  area: 0,
  address: '',
  city: '',
  district: '',
  bedrooms: undefined,
  bathrooms: undefined,
  floor: undefined,
  totalFloors: undefined,
  yearBuilt: undefined,
  orientation: '',
  decoration: '',
  facilities: [],
  images: []
})

// 选项数据
const propertyTypeOptions = {
  [PropertyType.Apartment]: '公寓',
  [PropertyType.House]: '别墅',
  [PropertyType.Commercial]: '商业',
  [PropertyType.Office]: '办公',
  [PropertyType.Shop]: '商铺',
  [PropertyType.Warehouse]: '仓库'
}

const propertyStatusOptions = {
  [PropertyStatus.Available]: '可售',
  [PropertyStatus.Pending]: '待定',
  [PropertyStatus.Sold]: '已售',
  [PropertyStatus.Rented]: '已租',
  [PropertyStatus.Withdrawn]: '已下架'
}

const orientationOptions = {
  '东': '东',
  '南': '南',
  '西': '西',
  '北': '北',
  '东南': '东南',
  '东北': '东北',
  '西南': '西南',
  '西北': '西北'
}

const decorationOptions = {
  '毛坯': '毛坯',
  '简装': '简装',
  '精装': '精装',
  '豪装': '豪装'
}

// 表单验证规则
const propertyRules: FormRules = {
  title: [
    { required: true, message: '请输入房源标题', trigger: 'blur' },
    { min: 2, max: 100, message: '标题长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择房源类型', trigger: 'change' }
  ],
  price: [
    { required: true, message: '请输入价格', trigger: 'blur' },
    { type: 'number', min: 0, message: '价格必须大于等于0', trigger: 'blur' }
  ],
  area: [
    { required: true, message: '请输入面积', trigger: 'blur' },
    { type: 'number', min: 0, message: '面积必须大于0', trigger: 'blur' }
  ],
  status: [
    { required: true, message: '请选择房源状态', trigger: 'change' }
  ],
  city: [
    { required: true, message: '请输入城市', trigger: 'blur' }
  ],
  district: [
    { required: true, message: '请输入区域', trigger: 'blur' }
  ],
  address: [
    { required: true, message: '请输入详细地址', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '请输入房源描述', trigger: 'blur' },
    { min: 10, max: 1000, message: '描述长度在 10 到 1000 个字符', trigger: 'blur' }
  ]
}

// 获取房源详情（编辑模式）
const fetchPropertyDetail = async () => {
  if (!isEdit.value) return

  loading.value = true
  try {
    const response = await getPropertyById(propertyId.value)
    if (response.success) {
      Object.assign(propertyForm, response.data)
      
      // 处理配套设施显示
      if (response.data.facilities && Array.isArray(response.data.facilities)) {
        facilitiesInput.value = response.data.facilities.join(', ')
      }
      
      // 处理图片列表
      if (response.data.images && response.data.images.length > 0) {
        fileList.value = response.data.images.map((url, index) => ({
          name: `image-${index}`,
          url: url,
          uid: index
        }))
      }
    } else {
      ElMessage.error(response.message || '获取房源详情失败')
      goBack()
    }
  } catch (error) {
    console.error('获取房源详情失败:', error)
    ElMessage.error('获取房源详情失败')
    goBack()
  } finally {
    loading.value = false
  }
}

// 返回上一页
const goBack = () => {
  if (isEdit.value) {
    router.push(`/property/detail/${propertyId.value}`)
  } else {
    router.push('/property/list')
  }
}

// 图片预览
const handlePictureCardPreview: UploadProps['onPreview'] = (uploadFile) => {
  dialogImageUrl.value = uploadFile.url!
  dialogVisible.value = true
}

// 移除图片
const handleRemove: UploadProps['onRemove'] = (uploadFile, uploadFiles) => {
  console.log(uploadFile, uploadFiles)
}

// 上传前检查
const beforeUpload = (file: File) => {
  const isImage = file.type.startsWith('image/')
  const isLt5M = file.size / 1024 / 1024 < 5

  if (!isImage) {
    ElMessage.error('只能上传图片文件!')
    return false
  }
  if (!isLt5M) {
    ElMessage.error('图片大小不能超过 5MB!')
    return false
  }
  return true
}

// 提交表单
const handleSubmit = async () => {
  if (!propertyFormRef.value) return

  try {
    await propertyFormRef.value.validate()
    submitLoading.value = true

    // 处理图片数据
    propertyForm.images = fileList.value.map(file => file.url || '').filter(url => url)

    // 处理配套设施
    propertyForm.facilities = facilitiesInput.value.split(',').map(item => item.trim())

    if (isEdit.value) {
      // 更新房源
      const response = await updateProperty(propertyId.value, propertyForm)
      if (response.success) {
        ElMessage.success('更新成功')
        router.push(`/property/detail/${propertyId.value}`)
      } else {
        ElMessage.error(response.message || '更新失败')
      }
    } else {
      // 创建房源
      const response = await createProperty(propertyForm)
      if (response.success) {
        ElMessage.success('创建成功')
        router.push('/property/list')
      } else {
        ElMessage.error(response.message || '创建失败')
      }
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
    fetchPropertyDetail()
  }
})
</script>

<style scoped>
.property-edit {
  padding: 0;
}

.page-header {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
}

.back-btn {
  margin-right: 16px;
  padding: 8px;
}

.page-header h2 {
  margin: 0;
  color: #333;
  font-size: 24px;
  font-weight: 500;
}

.property-form {
  max-width: 1000px;
}

.form-card {
  margin-bottom: 24px;
}

.form-actions {
  display: flex;
  justify-content: center;
  gap: 16px;
  margin-top: 32px;
  padding: 24px;
  background-color: #f5f7fa;
  border-radius: 8px;
}

:deep(.el-upload--picture-card) {
  width: 100px;
  height: 100px;
}

:deep(.el-upload-list--picture-card .el-upload-list__item) {
  width: 100px;
  height: 100px;
}
</style> 