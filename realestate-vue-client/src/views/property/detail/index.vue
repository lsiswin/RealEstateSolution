<template>
  <div class="property-detail-container" v-loading="loading">
    <div class="page-header">
      <el-page-header @back="goBack" :title="'返回房源列表'" :content="property.title || '房源详情'" />
    </div>

    <!-- 基本信息卡片 -->
    <el-card class="detail-card">
      <template #header>
        <div class="card-header">
          <span>基本信息</span>
          <el-button type="primary" size="small" @click="handleEdit(property.id)">
            <el-icon><Edit /></el-icon>编辑
          </el-button>
        </div>
      </template>
      
      <el-row :gutter="20">
        <el-col :span="16">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="房源ID">{{ property.id }}</el-descriptions-item>
            <el-descriptions-item label="房源名称">{{ property.title }}</el-descriptions-item>
            <el-descriptions-item label="地址">{{ property.address }}</el-descriptions-item>
            <el-descriptions-item label="所在区域">{{ property.region }}</el-descriptions-item>
            <el-descriptions-item label="房源类型">{{ property.propertyType }}</el-descriptions-item>
            <el-descriptions-item label="建筑面积">{{ property.area }} ㎡</el-descriptions-item>
            <el-descriptions-item label="价格">{{ property.price }} 万元</el-descriptions-item>
            <el-descriptions-item label="单价">{{ Math.round(property.price * 10000 / property.area) }} 元/㎡</el-descriptions-item>
            <el-descriptions-item label="户型">{{ property.layout }}</el-descriptions-item>
            <el-descriptions-item label="楼层">{{ property.floor }}</el-descriptions-item>
            <el-descriptions-item label="朝向">{{ property.orientation }}</el-descriptions-item>
            <el-descriptions-item label="装修">{{ property.decoration }}</el-descriptions-item>
            <el-descriptions-item label="建筑年代">{{ property.buildYear }}</el-descriptions-item>
            <el-descriptions-item label="产权年限">{{ property.propertyRightYears }}</el-descriptions-item>
            <el-descriptions-item label="状态">
              <el-tag :type="getStatusTagType(property.status)">{{ property.status }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="上架时间">{{ property.publishTime }}</el-descriptions-item>
          </el-descriptions>
        </el-col>
        <el-col :span="8">
          <div class="property-images">
            <el-image 
              v-if="property.mainImage" 
              :src="property.mainImage" 
              fit="cover"
              class="main-image"
              :preview-src-list="property.images || []"
            />
            <div v-else class="image-placeholder">
              <el-icon><Picture /></el-icon>
              <span>暂无图片</span>
            </div>
          </div>
        </el-col>
      </el-row>
    </el-card>

    <!-- 详细描述卡片 -->
    <el-card class="detail-card">
      <template #header>
        <div class="card-header">
          <span>详细描述</span>
        </div>
      </template>
      <div class="property-description" v-html="property.description || '暂无详细描述'"></div>
    </el-card>

    <!-- 房源配套设施 -->
    <el-card class="detail-card">
      <template #header>
        <div class="card-header">
          <span>配套设施</span>
        </div>
      </template>
      <div class="facilities">
        <el-tag 
          v-for="facility in property.facilities" 
          :key="facility" 
          class="facility-tag"
        >
          {{ facility }}
        </el-tag>
        <div v-if="!property.facilities || property.facilities.length === 0" class="no-data">
          暂无配套设施信息
        </div>
      </div>
    </el-card>

    <!-- 联系人信息 -->
    <el-card class="detail-card">
      <template #header>
        <div class="card-header">
          <span>联系人信息</span>
        </div>
      </template>
      <el-descriptions :column="2" border>
        <el-descriptions-item label="联系人">{{ property.contactName }}</el-descriptions-item>
        <el-descriptions-item label="联系电话">{{ property.contactPhone }}</el-descriptions-item>
        <el-descriptions-item label="所属经纪人">{{ property.agentName }}</el-descriptions-item>
        <el-descriptions-item label="经纪人电话">{{ property.agentPhone }}</el-descriptions-item>
      </el-descriptions>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Edit, Picture } from '@element-plus/icons-vue'

const route = useRoute()
const router = useRouter()
const loading = ref(false)
const property = ref({})
const propertyId = route.params.id

// 获取房源详情
const fetchPropertyDetail = async (id) => {
  loading.value = true
  try {
    // 模拟API调用
    // const response = await getPropertyDetail(id)
    // property.value = response.data

    // 模拟数据
    setTimeout(() => {
      property.value = {
        id: id,
        title: '阳光花园3室2厅精装修',
        address: '北京市朝阳区阳光花园小区B栋2单元303',
        region: '朝阳区',
        propertyType: '住宅',
        area: 120,
        price: 450,
        layout: '3室2厅1卫',
        floor: '3/18层',
        orientation: '南北通透',
        decoration: '精装修',
        buildYear: '2010年',
        propertyRightYears: '70年',
        status: '在售',
        publishTime: '2023-06-15 10:23:45',
        description: `
          <p>优质房源，南北通透，精装修，拎包入住。</p>
          <p>小区环境优美，绿化率高，周边配套设施齐全，交通便利。</p>
          <p>距离地铁5号线仅500米，周边有大型超市、医院、学校等。</p>
          <p>房屋布局合理，采光充足，通风良好。</p>
        `,
        facilities: ['电梯', '暖气', '天然气', '宽带', '热水器', '冰箱', '洗衣机', '空调'],
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
      }
      loading.value = false
    }, 500)
  } catch (error) {
    console.error('获取房源详情失败', error)
    ElMessage.error('获取房源详情失败')
    loading.value = false
  }
}

// 根据状态获取标签类型
const getStatusTagType = (status) => {
  const statusMap = {
    '在售': 'success',
    '已售': 'info',
    '预售': 'warning',
    '待售': 'danger'
  }
  return statusMap[status] || ''
}

// 返回列表
const goBack = () => {
  router.back()
}

// 编辑房源
const handleEdit = (id) => {
  router.push(`/property/edit/${id}`)
}

// 组件挂载时获取数据
onMounted(() => {
  if (propertyId) {
    fetchPropertyDetail(propertyId)
  } else {
    ElMessage.error('房源ID不存在')
    router.push('/property/list')
  }
  console.log('房源详情页面已加载，ID:', propertyId)
})
</script>

<style scoped>
.property-detail-container {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.detail-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.property-images {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
}

.main-image {
  width: 100%;
  height: auto;
  max-height: 300px;
  border-radius: 4px;
  cursor: pointer;
}

.image-placeholder {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 300px;
  background-color: #f5f7fa;
  border-radius: 4px;
  color: #909399;
}

.image-placeholder .el-icon {
  font-size: 48px;
  margin-bottom: 10px;
}

.property-description {
  line-height: 1.8;
}

.facilities {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.facility-tag {
  margin-right: 10px;
  margin-bottom: 10px;
}

.no-data {
  color: #909399;
  font-size: 14px;
  padding: 20px 0;
  text-align: center;
}
</style> 