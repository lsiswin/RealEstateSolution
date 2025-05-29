<template>
  <div class="property-add">
    <div class="page-header">
      <h2>添加房源</h2>
      <p>填写房源基本信息</p>
    </div>
    
    <el-card class="form-card">
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="120px"
        v-loading="loading"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="房源标题" prop="title">
              <el-input v-model="form.title" placeholder="请输入房源标题" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="房源类型" prop="type">
              <el-select v-model="form.type" placeholder="请选择房源类型" style="width: 100%">
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

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="价格(元)" prop="price">
              <el-input-number
                v-model="form.price"
                :min="0"
                :step="10000"
                placeholder="请输入价格"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="面积(㎡)" prop="area">
              <el-input-number
                v-model="form.area"
                :min="0"
                :step="1"
                placeholder="请输入面积"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="城市" prop="city">
              <el-input v-model="form.city" placeholder="请输入城市" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="区域" prop="district">
              <el-input v-model="form.district" placeholder="请输入区域" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="详细地址" prop="address">
          <el-input v-model="form.address" placeholder="请输入详细地址" />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="卧室数量">
              <el-input-number
                v-model="form.bedrooms"
                :min="0"
                placeholder="卧室数量"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="卫生间数量">
              <el-input-number
                v-model="form.bathrooms"
                :min="0"
                placeholder="卫生间数量"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="楼层">
              <el-input-number
                v-model="form.floor"
                :min="1"
                placeholder="楼层"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="总楼层">
              <el-input-number
                v-model="form.totalFloors"
                :min="1"
                placeholder="总楼层"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="建造年份">
              <el-input-number
                v-model="form.yearBuilt"
                :min="1900"
                :max="new Date().getFullYear()"
                placeholder="建造年份"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="朝向">
              <el-input v-model="form.orientation" placeholder="如：南北通透" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="装修情况">
          <el-input v-model="form.decoration" placeholder="如：精装修、简装、毛坯" />
        </el-form-item>

        <el-form-item label="配套设施">
          <el-input
            v-model="form.facilities"
            type="textarea"
            :rows="3"
            placeholder="请描述房源的配套设施，如：停车位、电梯、花园等"
          />
        </el-form-item>

        <el-form-item label="房源描述" prop="description">
          <el-input
            v-model="form.description"
            type="textarea"
            :rows="4"
            placeholder="请详细描述房源特点、周边环境等"
          />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="handleSubmit" :loading="loading">
            <el-icon><Check /></el-icon>
            提交
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
          <el-button @click="handleCancel">
            <el-icon><Close /></el-icon>
            取消
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Check, Refresh, Close } from '@element-plus/icons-vue'

// API imports
import {
  createProperty,
  PropertyType,
  PropertyStatus,
  type Property
} from '@/api/property'

const router = useRouter()

// 加载状态
const loading = ref(false)

// 表单引用
const formRef = ref<FormInstance>()

// 表单数据
const form = reactive({
  title: '',
  description: '',
  type: undefined as PropertyType | undefined,
  price: undefined as number | undefined,
  area: undefined as number | undefined,
  address: '',
  city: '',
  district: '',
  bedrooms: undefined as number | undefined,
  bathrooms: undefined as number | undefined,
  floor: undefined as number | undefined,
  totalFloors: undefined as number | undefined,
  yearBuilt: undefined as number | undefined,
  orientation: '',
  decoration: '',
  facilities: ''
})

// 房源类型选项
const propertyTypeOptions = {
  [PropertyType.Apartment]: '公寓',
  [PropertyType.House]: '别墅',
  [PropertyType.Commercial]: '商业',
  [PropertyType.Office]: '办公',
  [PropertyType.Shop]: '商铺',
  [PropertyType.Warehouse]: '仓库'
}

// 表单验证规则
const rules: FormRules = {
  title: [
    { required: true, message: '请输入房源标题', trigger: 'blur' },
    { min: 2, max: 100, message: '标题长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择房源类型', trigger: 'change' }
  ],
  price: [
    { required: true, message: '请输入价格', trigger: 'blur' },
    { type: 'number', min: 1, message: '价格必须大于0', trigger: 'blur' }
  ],
  area: [
    { required: true, message: '请输入面积', trigger: 'blur' },
    { type: 'number', min: 1, message: '面积必须大于0', trigger: 'blur' }
  ],
  address: [
    { required: true, message: '请输入详细地址', trigger: 'blur' },
    { min: 5, max: 200, message: '地址长度在 5 到 200 个字符', trigger: 'blur' }
  ],
  city: [
    { required: true, message: '请输入城市', trigger: 'blur' }
  ],
  district: [
    { required: true, message: '请输入区域', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '请输入房源描述', trigger: 'blur' },
    { min: 10, max: 1000, message: '描述长度在 10 到 1000 个字符', trigger: 'blur' }
  ]
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    
    loading.value = true
    
    const propertyData: Partial<Property> = {
      ...form,
      status: PropertyStatus.Available, // 默认状态为可售
      ownerId: '', // 后端会自动设置当前用户ID
      createTime: new Date().toISOString(),
      updateTime: new Date().toISOString(),
      isDeleted: false
    }
    
    const response = await createProperty(propertyData)
    if (response.success) {
      ElMessage.success('房源添加成功')
      router.push('/property/list')
    } else {
      ElMessage.error(response.message || '添加失败')
    }
  } catch (error) {
    console.error('添加房源失败:', error)
    ElMessage.error('添加失败')
  } finally {
    loading.value = false
  }
}

// 重置表单
const handleReset = () => {
  formRef.value?.resetFields()
}

// 取消操作
const handleCancel = () => {
  router.back()
}
</script>

<style scoped>
.property-add {
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
  margin-bottom: 24px;
}
</style> 