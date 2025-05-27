<template>
  <div class="property-form-container" v-loading="loading">
    <div class="page-header">
      <el-page-header @back="goBack" :title="'返回房源列表'" :content="isEdit ? '编辑房源' : '添加房源'" />
    </div>

    <el-card class="form-card">
      <el-form
        ref="formRef"
        :model="propertyForm"
        :rules="rules"
        label-width="100px"
        label-position="right"
        status-icon
      >
        <el-divider content-position="left">基本信息</el-divider>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="房源名称" prop="title">
              <el-input v-model="propertyForm.title" placeholder="请输入房源名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="房源类型" prop="propertyType">
              <el-select v-model="propertyForm.propertyType" placeholder="请选择房源类型" style="width: 100%">
                <el-option label="住宅" value="住宅" />
                <el-option label="商铺" value="商铺" />
                <el-option label="写字楼" value="写字楼" />
                <el-option label="别墅" value="别墅" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="所在区域" prop="region">
              <el-select v-model="propertyForm.region" placeholder="请选择所在区域" style="width: 100%">
                <el-option label="朝阳区" value="朝阳区" />
                <el-option label="海淀区" value="海淀区" />
                <el-option label="西城区" value="西城区" />
                <el-option label="东城区" value="东城区" />
                <el-option label="丰台区" value="丰台区" />
                <el-option label="石景山区" value="石景山区" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="详细地址" prop="address">
              <el-input v-model="propertyForm.address" placeholder="请输入详细地址" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="建筑面积" prop="area">
              <el-input-number v-model="propertyForm.area" :min="1" controls-position="right" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="价格(万)" prop="price">
              <el-input-number v-model="propertyForm.price" :min="0" :precision="2" controls-position="right" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="状态" prop="status">
              <el-select v-model="propertyForm.status" placeholder="请选择状态" style="width: 100%">
                <el-option label="在售" value="在售" />
                <el-option label="已售" value="已售" />
                <el-option label="预售" value="预售" />
                <el-option label="待售" value="待售" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-divider content-position="left">房源详情</el-divider>
        
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="户型" prop="layout">
              <el-input v-model="propertyForm.layout" placeholder="如：3室2厅1卫" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="楼层" prop="floor">
              <el-input v-model="propertyForm.floor" placeholder="如：3/18层" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="朝向" prop="orientation">
              <el-select v-model="propertyForm.orientation" placeholder="请选择朝向" style="width: 100%">
                <el-option label="南北通透" value="南北通透" />
                <el-option label="东西朝向" value="东西朝向" />
                <el-option label="南向" value="南向" />
                <el-option label="北向" value="北向" />
                <el-option label="东向" value="东向" />
                <el-option label="西向" value="西向" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="装修" prop="decoration">
              <el-select v-model="propertyForm.decoration" placeholder="请选择装修情况" style="width: 100%">
                <el-option label="精装修" value="精装修" />
                <el-option label="简装修" value="简装修" />
                <el-option label="毛坯房" value="毛坯房" />
                <el-option label="豪华装修" value="豪华装修" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="建筑年代" prop="buildYear">
              <el-input v-model="propertyForm.buildYear" placeholder="如：2010年" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="产权年限" prop="propertyRightYears">
              <el-input v-model="propertyForm.propertyRightYears" placeholder="如：70年" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="配套设施" prop="facilities">
          <el-checkbox-group v-model="propertyForm.facilities">
            <el-checkbox label="电梯">电梯</el-checkbox>
            <el-checkbox label="暖气">暖气</el-checkbox>
            <el-checkbox label="天然气">天然气</el-checkbox>
            <el-checkbox label="宽带">宽带</el-checkbox>
            <el-checkbox label="热水器">热水器</el-checkbox>
            <el-checkbox label="冰箱">冰箱</el-checkbox>
            <el-checkbox label="洗衣机">洗衣机</el-checkbox>
            <el-checkbox label="空调">空调</el-checkbox>
            <el-checkbox label="电视">电视</el-checkbox>
            <el-checkbox label="厨房">厨房</el-checkbox>
            <el-checkbox label="阳台">阳台</el-checkbox>
            <el-checkbox label="卫生间">卫生间</el-checkbox>
            <el-checkbox label="车位">车位</el-checkbox>
          </el-checkbox-group>
        </el-form-item>

        <el-form-item label="详细描述" prop="description">
          <el-input
            v-model="propertyForm.description"
            type="textarea"
            :rows="4"
            placeholder="请输入房源详细描述"
          />
        </el-form-item>

        <el-divider content-position="left">联系信息</el-divider>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="联系人" prop="contactName">
              <el-input v-model="propertyForm.contactName" placeholder="请输入联系人姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="contactPhone">
              <el-input v-model="propertyForm.contactPhone" placeholder="请输入联系人电话" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="经纪人" prop="agentName">
              <el-input v-model="propertyForm.agentName" placeholder="请输入经纪人姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="经纪人电话" prop="agentPhone">
              <el-input v-model="propertyForm.agentPhone" placeholder="请输入经纪人电话" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="上传图片">
          <el-upload
            action="#"
            list-type="picture-card"
            :auto-upload="false"
            :file-list="fileList"
            :on-change="handleFileChange"
            :on-remove="handleFileRemove"
          >
            <el-icon><Plus /></el-icon>
          </el-upload>
          <div class="upload-tip">
            <el-text type="info">提示：首张图片将作为主图显示</el-text>
          </div>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="submitForm">保存</el-button>
          <el-button @click="resetForm">重置</el-button>
          <el-button @click="goBack">取消</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'

const route = useRoute()
const router = useRouter()
const formRef = ref(null)
const loading = ref(false)
const fileList = ref([])

// 判断是编辑模式还是添加模式
const isEdit = computed(() => {
  return !!route.params.id
})

// 表单数据
const propertyForm = reactive({
  id: '',
  title: '',
  propertyType: '',
  region: '',
  address: '',
  area: 0,
  price: 0,
  status: '在售',
  layout: '',
  floor: '',
  orientation: '',
  decoration: '',
  buildYear: '',
  propertyRightYears: '',
  facilities: [],
  description: '',
  contactName: '',
  contactPhone: '',
  agentName: '',
  agentPhone: '',
  mainImage: '',
  images: []
})

// 表单验证规则
const rules = reactive({
  title: [
    { required: true, message: '请输入房源名称', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  propertyType: [
    { required: true, message: '请选择房源类型', trigger: 'change' }
  ],
  region: [
    { required: true, message: '请选择所在区域', trigger: 'change' }
  ],
  address: [
    { required: true, message: '请输入详细地址', trigger: 'blur' }
  ],
  area: [
    { required: true, message: '请输入建筑面积', trigger: 'blur' }
  ],
  price: [
    { required: true, message: '请输入价格', trigger: 'blur' }
  ],
  status: [
    { required: true, message: '请选择状态', trigger: 'change' }
  ],
  contactName: [
    { required: true, message: '请输入联系人姓名', trigger: 'blur' }
  ],
  contactPhone: [
    { required: true, message: '请输入联系人电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ]
})

// 获取房源详情
const fetchPropertyDetail = async (id) => {
  loading.value = true
  try {
    // 模拟API调用
    // const response = await getPropertyDetail(id)
    // Object.assign(propertyForm, response.data)
    
    // 模拟图片列表
    // if (response.data.images && response.data.images.length > 0) {
    //   fileList.value = response.data.images.map((url, index) => ({
    //     name: `图片${index + 1}`,
    //     url
    //   }))
    // }

    // 模拟数据
    setTimeout(() => {
      Object.assign(propertyForm, {
        id: id,
        title: '阳光花园3室2厅精装修',
        propertyType: '住宅',
        region: '朝阳区',
        address: '北京市朝阳区阳光花园小区B栋2单元303',
        area: 120,
        price: 450,
        status: '在售',
        layout: '3室2厅1卫',
        floor: '3/18层',
        orientation: '南北通透',
        decoration: '精装修',
        buildYear: '2010年',
        propertyRightYears: '70年',
        facilities: ['电梯', '暖气', '天然气', '宽带', '热水器', '冰箱', '洗衣机', '空调'],
        description: '优质房源，南北通透，精装修，拎包入住。小区环境优美，绿化率高，周边配套设施齐全，交通便利。',
        contactName: '李先生',
        contactPhone: '13812345678',
        agentName: '张经理',
        agentPhone: '13987654321',
        mainImage: 'https://img.zcool.cn/community/01b4df5f21777711013e3187c8aba8.jpg@1280w_1l_2o_100sh.jpg',
        images: [
          'https://img.zcool.cn/community/01b4df5f21777711013e3187c8aba8.jpg@1280w_1l_2o_100sh.jpg',
          'https://img.zcool.cn/community/0196df5f2177dd11013e3187c2adb6.jpg@1280w_1l_2o_100sh.jpg',
          'https://img.zcool.cn/community/01d56c5f21777211013e3187c01ab8.jpg@1280w_1l_2o_100sh.jpg'
        ]
      })

      // 模拟图片列表
      fileList.value = [
        { name: '图片1', url: 'https://img.zcool.cn/community/01b4df5f21777711013e3187c8aba8.jpg@1280w_1l_2o_100sh.jpg' },
        { name: '图片2', url: 'https://img.zcool.cn/community/0196df5f2177dd11013e3187c2adb6.jpg@1280w_1l_2o_100sh.jpg' },
        { name: '图片3', url: 'https://img.zcool.cn/community/01d56c5f21777211013e3187c01ab8.jpg@1280w_1l_2o_100sh.jpg' }
      ]
      
      loading.value = false
    }, 500)
  } catch (error) {
    console.error('获取房源详情失败', error)
    ElMessage.error('获取房源详情失败')
    loading.value = false
  }
}

// 处理文件上传
const handleFileChange = (file) => {
  console.log('文件上传变化', file)
}

// 处理文件移除
const handleFileRemove = (file) => {
  console.log('文件移除', file)
}

// 提交表单
const submitForm = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid, fields) => {
    if (valid) {
      loading.value = true
      try {
        // 如果有文件上传，先处理文件上传
        // const uploadedFiles = await handleUpload()
        // propertyForm.images = uploadedFiles.map(file => file.url)
        // propertyForm.mainImage = propertyForm.images[0] || ''

        // 模拟API调用
        // if (isEdit.value) {
        //   await updateProperty(propertyForm)
        // } else {
        //   await createProperty(propertyForm)
        // }

        // 模拟保存成功
        setTimeout(() => {
          ElMessage.success(isEdit.value ? '编辑成功' : '添加成功')
          loading.value = false
          router.push('/property/list')
        }, 1000)
      } catch (error) {
        console.error('保存失败', error)
        ElMessage.error('保存失败')
        loading.value = false
      }
    } else {
      console.log('表单验证失败', fields)
      ElMessage.error('表单填写有误，请检查')
    }
  })
}

// 重置表单
const resetForm = () => {
  if (!formRef.value) return

  if (isEdit.value) {
    // 编辑模式下重置为原始数据
    ElMessageBox.confirm('确定要重置表单吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(() => {
      const id = route.params.id
      fetchPropertyDetail(id)
    }).catch(() => {
      // 取消重置，不做处理
    })
  } else {
    // 添加模式下清空表单
    formRef.value.resetFields()
    fileList.value = []
  }
}

// 返回列表
const goBack = () => {
  router.back()
}

// 组件挂载时获取数据
onMounted(() => {
  if (isEdit.value) {
    const id = route.params.id
    fetchPropertyDetail(id)
  }
})
</script>

<style scoped>
.property-form-container {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.form-card {
  margin-bottom: 20px;
}

.upload-tip {
  margin-top: 10px;
  line-height: 1.5;
}

.el-input-number {
  width: 100%;
}
</style> 