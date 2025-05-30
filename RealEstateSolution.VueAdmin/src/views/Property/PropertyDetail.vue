<template>
  <div class="property-detail">
    <!-- 页面标题 -->
    <div class="page-header">
      <el-button @click="goBack" type="text" class="back-btn">
        <el-icon><ArrowLeft /></el-icon>
        返回列表
      </el-button>
      <h2>房源详情</h2>
    </div>

    <div v-loading="loading">
      <div v-if="property" class="property-content">
        <!-- 基本信息卡片 -->
        <el-card class="info-card">
          <template #header>
            <div class="card-header">
              <span class="title">{{ property.title }}</span>
              <div class="actions">
                <el-button type="primary" @click="handleEdit">
                  <el-icon><Edit /></el-icon>
                  编辑
                </el-button>
                <el-button type="danger" @click="handleDelete" v-if="canDelete">
                  <el-icon><Delete /></el-icon>
                  删除
                </el-button>
                <el-button type="warning" @click="handleChangeStatus">
                  <el-icon><Switch /></el-icon>
                  更改状态
                </el-button>
              </div>
            </div>
          </template>

          <div class="property-info">
            <!-- 图片轮播 -->
            <div class="image-section">
              <el-carousel v-if="property.images && property.images.length > 0" height="400px" indicator-position="outside">
                <el-carousel-item v-for="(image, index) in property.images" :key="index">
                  <img :src="image" :alt="`房源图片${index + 1}`" class="property-image" />
                </el-carousel-item>
              </el-carousel>
              <div v-else class="no-image">
                <el-icon size="80"><Picture /></el-icon>
                <p>暂无图片</p>
              </div>
            </div>

            <!-- 详细信息 -->
            <div class="detail-section">
              <el-row :gutter="24">
                <el-col :span="12">
                  <div class="info-group">
                    <h3>基本信息</h3>
                    <el-descriptions :column="1" border>
                      <el-descriptions-item label="房源编号">{{ property.id }}</el-descriptions-item>
                      <el-descriptions-item label="房源类型">
                        {{ getPropertyTypeText(property.type) }}
                      </el-descriptions-item>
                      <el-descriptions-item label="房源状态">
                        <el-tag :type="getPropertyStatusColor(property.status)">
                          {{ getPropertyStatusText(property.status) }}
                        </el-tag>
                      </el-descriptions-item>
                      <el-descriptions-item label="价格">
                        <span class="price">{{ formatPrice(property.price) }}</span>
                      </el-descriptions-item>
                      <el-descriptions-item label="面积">{{ property.area }}㎡</el-descriptions-item>
                      <el-descriptions-item label="地址">{{ property.address }}</el-descriptions-item>
                      <el-descriptions-item label="城市">{{ property.city }}</el-descriptions-item>
                      <el-descriptions-item label="区域">{{ property.district }}</el-descriptions-item>
                    </el-descriptions>
                  </div>
                </el-col>
                <el-col :span="12">
                  <div class="info-group">
                    <h3>房屋详情</h3>
                    <el-descriptions :column="1" border>
                      <el-descriptions-item label="卧室数量" v-if="property.bedrooms">
                        {{ property.bedrooms }}室
                      </el-descriptions-item>
                      <el-descriptions-item label="卫生间数量" v-if="property.bathrooms">
                        {{ property.bathrooms }}卫
                      </el-descriptions-item>
                      <el-descriptions-item label="楼层" v-if="property.floor">
                        {{ property.floor }}层
                      </el-descriptions-item>
                      <el-descriptions-item label="总楼层" v-if="property.totalFloors">
                        共{{ property.totalFloors }}层
                      </el-descriptions-item>
                      <el-descriptions-item label="建造年份" v-if="property.yearBuilt">
                        {{ property.yearBuilt }}年
                      </el-descriptions-item>
                      <el-descriptions-item label="朝向" v-if="property.orientation">
                        {{ property.orientation }}
                      </el-descriptions-item>
                      <el-descriptions-item label="装修情况" v-if="property.decoration">
                        {{ property.decoration }}
                      </el-descriptions-item>
                    </el-descriptions>
                  </div>
                </el-col>
              </el-row>

              <!-- 房源描述 -->
              <div class="info-group" v-if="property.description">
                <h3>房源描述</h3>
                <div class="description">
                  {{ property.description }}
                </div>
              </div>

              <!-- 配套设施 -->
              <div class="info-group" v-if="property.facilities">
                <h3>配套设施</h3>
                <div class="facilities">
                  <el-tag v-for="facility in facilitiesList" :key="facility" class="facility-tag">
                    {{ facility }}
                  </el-tag>
                </div>
              </div>

              <!-- 时间信息 -->
              <div class="info-group">
                <h3>时间信息</h3>
                <el-descriptions :column="2" border>
                  <el-descriptions-item label="创建时间">
                    {{ formatDateTime(property.createTime) }}
                  </el-descriptions-item>
                  <el-descriptions-item label="更新时间">
                    {{ formatDateTime(property.updateTime) }}
                  </el-descriptions-item>
                </el-descriptions>
              </div>
            </div>
          </div>
        </el-card>
      </div>

      <div v-else-if="!loading" class="not-found">
        <el-result icon="warning" title="房源不存在" sub-title="您访问的房源可能已被删除或不存在">
          <template #extra>
            <el-button type="primary" @click="goBack">返回列表</el-button>
          </template>
        </el-result>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ArrowLeft, Edit, Delete, Switch, Picture } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'

// API imports
import {
  getPropertyById,
  deleteProperty,
  changePropertyStatus,
  getPropertyTypeText,
  getPropertyStatusText,
  getPropertyStatusColor,
  type Property
} from '@/api/property'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()

// 响应式数据
const loading = ref(false)
const property = ref<Property | null>(null)

// 计算属性
const propertyId = computed(() => Number(route.params.id))

// 检查是否可以删除（管理员或房源所有者）
const canDelete = computed(() => {
  if (!property.value || !userStore.userInfo) return false
  return userStore.isAdmin || property.value.ownerId === userStore.userInfo.id
})

// 配套设施列表
const facilitiesList = computed(() => {
  if (!property.value?.facilities) return []
  return property.value.facilities
})

// 获取房源详情
const fetchPropertyDetail = async () => {
  if (!propertyId.value || propertyId.value <= 0) {
    ElMessage.error('无效的房源ID')
    goBack()
    return
  }

  loading.value = true
  try {
    const response = await getPropertyById(propertyId.value)
    if (response.success) {
      property.value = response.data
    } else {
      ElMessage.error(response.message || '获取房源详情失败')
      property.value = null
    }
  } catch (error) {
    console.error('获取房源详情失败:', error)
    ElMessage.error('获取房源详情失败')
    property.value = null
  } finally {
    loading.value = false
  }
}

// 返回列表
const goBack = () => {
  router.replace('/property/list')
}

// 编辑房源
const handleEdit = () => {
  router.replace(`/property/edit/${propertyId.value}`)
}

// 删除房源
const handleDelete = async () => {
  if (!property.value) return

  try {
    await ElMessageBox.confirm(
      `确定要删除房源"${property.value.title}"吗？`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    loading.value = true
    const response = await deleteProperty(propertyId.value)
    if (response.success) {
      ElMessage.success('删除成功')
      goBack()
    } else {
      ElMessage.error(response.message || '删除失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除房源失败:', error)
      ElMessage.error('删除失败')
    }
  } finally {
    loading.value = false
  }
}

// 更改房源状态
const handleChangeStatus = async () => {
  if (!property.value) return

  try {
    await ElMessageBox.confirm(
      '确定要更改房源状态吗？',
      '确认操作',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    loading.value = true
    const response = await changePropertyStatus(propertyId.value)
    if (response.success) {
      ElMessage.success('状态更改成功')
      // 重新获取房源详情
      await fetchPropertyDetail()
    } else {
      ElMessage.error(response.message || '状态更改失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('更改状态失败:', error)
      ElMessage.error('状态更改失败')
    }
  } finally {
    loading.value = false
  }
}

// 格式化价格
const formatPrice = (price: number): string => {
  if (price >= 10000) {
    return `${(price / 10000).toFixed(1)}万元`
  }
  return `${price.toLocaleString()}元`
}

// 格式化日期时间
const formatDateTime = (dateTime: string): string => {
  return new Date(dateTime).toLocaleString('zh-CN')
}

// 初始化
onMounted(() => {
  fetchPropertyDetail()
})
</script>

<style scoped>
.property-detail {
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

.property-content {
  max-width: 1200px;
}

.info-card {
  margin-bottom: 24px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header .title {
  font-size: 20px;
  font-weight: 500;
  color: #333;
}

.property-info {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.image-section {
  width: 100%;
}

.property-image {
  width: 100%;
  height: 400px;
  object-fit: cover;
  border-radius: 8px;
}

.no-image {
  height: 400px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background-color: #f5f7fa;
  border-radius: 8px;
  color: #909399;
}

.detail-section {
  flex: 1;
}

.info-group {
  margin-bottom: 24px;
}

.info-group h3 {
  margin: 0 0 16px 0;
  color: #333;
  font-size: 16px;
  font-weight: 500;
  border-left: 4px solid #409eff;
  padding-left: 12px;
}

.price {
  color: #e6a23c;
  font-size: 18px;
  font-weight: 600;
}

.description {
  padding: 16px;
  background-color: #f5f7fa;
  border-radius: 8px;
  line-height: 1.6;
  color: #666;
}

.facilities {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.facility-tag {
  margin: 0;
}

.not-found {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

@media (max-width: 768px) {
  .property-info {
    flex-direction: column;
  }
  
  .card-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }
  
  .actions {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
  }
}
</style> 