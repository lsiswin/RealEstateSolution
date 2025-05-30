<template>
  <div class="dashboard">
    <!-- 页面标题 -->
    <div class="page-header">
      <h2>系统概览</h2>
      <p>房产中介管理系统数据统计</p>
    </div>

    <!-- 统计卡片 -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-number">{{ stats.totalProperties }}</div>
            <div class="stat-title">总房源数</div>
            <div class="stat-icon">
              <el-icon><OfficeBuilding /></el-icon>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-number">{{ stats.totalClients }}</div>
            <div class="stat-title">总客户数</div>
            <div class="stat-icon">
              <el-icon><User /></el-icon>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-number">{{ stats.forSaleCount }}</div>
            <div class="stat-title">在售房源</div>
            <div class="stat-icon">
              <el-icon><Document /></el-icon>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-number">{{ stats.SoldCount }}</div>
            <div class="stat-title">已售房源</div>
            <div class="stat-icon">
              <el-icon><Money /></el-icon>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 用户信息调试卡片 -->
    <el-row :gutter="20" class="debug-row" style="margin-top: 20px;">
      <el-col :span="24">
        <el-card class="debug-card">
          <template #header>
            <span>当前用户信息（调试用）</span>
          </template>
          <div class="debug-content">
            <p><strong>用户名:</strong> {{ userStore.userInfo?.userName || '未登录' }}</p>
            <p><strong>真实姓名:</strong> {{ userStore.userInfo?.realName || '未设置' }}</p>
            <p><strong>用户角色:</strong> 
              <el-tag v-for="role in userStore.userInfo?.roles" :key="role" style="margin-right: 8px;">
                {{ role }}
              </el-tag>
              <span v-if="!userStore.userInfo?.roles || userStore.userInfo.roles.length === 0">无角色</span>
            </p>
            <p><strong>是否管理员:</strong> {{ userStore.hasRole('admin') ? '是' : '否' }}</p>
            <p><strong>是否经纪人:</strong> {{ userStore.hasRole('broker') ? '是' : '否' }}</p>
            <p><strong>可访问合同管理:</strong> {{ userStore.hasAnyRole(['admin', 'broker']) ? '是' : '否' }}</p>
            <p><strong>Token状态:</strong> {{ userStore.token ? '已设置' : '未设置' }}</p>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 图表区域 -->
    <el-row :gutter="20" class="charts-row">
      <el-col :span="12">
        <el-card class="chart-card">
          <template #header>
            <span>房源类型分布</span>
          </template>
          <div ref="pieChartRef" class="chart-container"></div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card class="chart-card">
          <template #header>
            <span>客户类型分布</span>
          </template>
          <div ref="lineChartRef" class="chart-container"></div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 最新房源列表 -->
    <el-card class="property-list-card">
      <template #header>
        <div class="card-header">
          <span>最新房源</span>
          <el-button type="primary" size="small" @click="viewAllProperties">
            查看全部
          </el-button>
        </div>
      </template>
      
      <el-table :data="recentProperties" style="width: 100%" v-loading="loading">
        <el-table-column prop="id" label="房源编号" width="120" />
        <el-table-column prop="title" label="房源标题" />
        <el-table-column prop="type" label="房源类型" width="100">
          <template #default="scope">
            {{ getPropertyTypeText(scope.row.type) }}
          </template>
        </el-table-column>
        <el-table-column prop="area" label="面积(㎡)" width="100" />
        <el-table-column prop="price" label="价格(万)" width="120" />
        <el-table-column prop="address" label="位置" width="150" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="getPropertyStatusColor(scope.row.status)">
              {{ getPropertyStatusText(scope.row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="发布时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createTime) }}
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 最新客户列表 -->
    <el-card class="client-list-card">
      <template #header>
        <div class="card-header">
          <span>最新客户</span>
          <el-button type="primary" size="small" @click="viewAllClients">
            查看全部
          </el-button>
        </div>
      </template>
      
      <el-table :data="recentClients" style="width: 100%" v-loading="clientLoading">
        <el-table-column prop="id" label="客户ID" width="120" />
        <el-table-column prop="name" label="客户姓名" />
        <el-table-column prop="phone" label="联系电话" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="type" label="客户类型" width="100">
          <template #default="scope">
            <el-tag :type="getClientTypeColor(scope.row.type)">
              {{ getClientTypeText(scope.row.type) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="agentName" label="经纪人" width="120" />
        <el-table-column prop="createdAt" label="创建时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createdAt) }}
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import * as echarts from 'echarts'
import {Document, Money, OfficeBuilding, User} from "@element-plus/icons-vue"
import { useUserStore } from '@/stores/user'

// API imports
import { 
  queryProperties, 
  getPropertyStats,
  getPropertyTypeText, 
  getPropertyStatusText, 
  getPropertyStatusColor,
  type Property
} from '@/api/property'
import { 
  getClients, 
  getClientStats,
  ClientType,
  type Client
} from '@/api/client'

const router = useRouter()
const userStore = useUserStore()

// 初始化用户信息
userStore.initUserInfo()

// 加载状态
const loading = ref(false)
const clientLoading = ref(false)

// 统计数据
const stats = ref({
  totalProperties: 0,
  totalClients: 0,
  forSaleCount: 0,
  SoldCount: 0
})

// 最新房源数据
const recentProperties = ref<Property[]>([])

// 最新客户数据
const recentClients = ref<Client[]>([])

// 图表引用
const pieChartRef = ref<HTMLDivElement>()
const lineChartRef = ref<HTMLDivElement>()

// 房源类型统计数据
const propertyTypeStats = ref<{[key: string]: number}>({})

// 客户类型统计数据
const clientTypeStats = ref<{[key: string]: number}>({})

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 获取客户类型文本
const getClientTypeText = (type: ClientType) => {
  const typeMap = {
    [ClientType.Buyer]: '买家',
    [ClientType.Seller]: '卖家',
    [ClientType.Tenant]: '租客',
    [ClientType.Landlord]: '房东'
  }
  return typeMap[type] || '未知'
}

// 获取客户类型颜色
const getClientTypeColor = (type: ClientType) => {
  const colorMap = {
    [ClientType.Buyer]: 'success',
    [ClientType.Seller]: 'warning',
    [ClientType.Tenant]: 'info',
    [ClientType.Landlord]: 'danger'
  }
  return colorMap[type] || ''
}

// 跳转到房源列表
const viewAllProperties = () => {
  router.push('/property/list')
}

// 跳转到客户列表
const viewAllClients = () => {
  router.push('/client/list')
}

// 获取房源统计数据
const fetchPropertyStats = async () => {
  try {
    console.log('开始获取房源统计数据...')
    const response = await getPropertyStats()
    console.log('房源统计数据响应:', response)
    
    // 响应拦截器已经返回了response.data，所以response就是ApiResponse<PropertyStats>
    if (response && response.success && response.data) {
      const propertyStats = response.data
      console.log('房源统计数据更新完成:', propertyStats,stats.value)
      stats.value.totalProperties = propertyStats.totalProperties
      stats.value.forSaleCount = propertyStats.forSaleProperties
      stats.value.SoldCount = propertyStats.soldProperties
      console.log('房源统计数据更新完成:', stats.value)
    } else {
      console.warn('房源统计数据格式异常:', response)
      // 设置默认值
      stats.value.totalProperties = 0
      stats.value.forSaleCount = 0
      stats.value.SoldCount = 0
    }
  } catch (error) {
    console.error('获取房源统计失败:', error)
    // 设置默认值
    stats.value.totalProperties = 0
    stats.value.forSaleCount = 0
    stats.value.SoldCount = 0
    ElMessage.warning('房源数据加载失败')
  }
}

// 获取客户统计数据
const fetchClientStats = async () => {
  try {
    console.log('开始获取客户统计数据...')
    const response = await getClientStats()
    console.log('客户统计数据响应:', response)
    
    // 响应拦截器已经返回了response.data，所以response就是ApiResponse<ClientStats>
    if (response && response.success && response.data) {
      stats.value.totalClients = response.data.totalClients||0
      console.log('客户统计数据更新完成:', response.data)
    } else {
      console.warn('客户统计数据格式异常:', response)
      stats.value.totalClients = 0
    }
  } catch (error) {
    console.error('获取客户统计失败:', error)
    // 设置默认值
    stats.value.totalClients = 0
    ElMessage.warning('客户数据加载失败')
  }
}

// 获取最新房源列表
const fetchRecentProperties = async () => {
  loading.value = true
  try {
    console.log('开始获取最新房源列表...')
    const response = await queryProperties({
      pageIndex: 1,
      pageSize: 5
    })
    console.log('房源列表响应:', response)
    
    if (response && response.success && response.data) {
      recentProperties.value = response.data.items || []
      
      // 统计房源类型分布
      const typeStats: {[key: string]: number} = {}
      
      recentProperties.value.forEach(property => {
        const typeText = getPropertyTypeText(property.type)
        typeStats[typeText] = (typeStats[typeText] || 0) + 1
      })
      
      propertyTypeStats.value = typeStats
      console.log('房源列表更新完成，共', recentProperties.value.length, '条记录')
    } else {
      console.warn('房源列表数据格式异常:', response)
      recentProperties.value = []
      propertyTypeStats.value = {}
    }
  } catch (error) {
    console.error('获取房源列表失败:', error)
    recentProperties.value = []
    propertyTypeStats.value = {}
    ElMessage.warning('房源列表加载失败')
  } finally {
    loading.value = false
  }
}

// 获取最新客户列表
const fetchRecentClients = async () => {
  clientLoading.value = true
  try {
    console.log('开始获取最新客户列表...')
    const response = await getClients({
      name: '',
      phone: '',
      type: undefined,
      pageIndex: 1,
      pageSize: 5
    })
    console.log('客户列表响应:', response)
    
    if (response && response.success && response.data) {
      recentClients.value = response.data.items || []
      
      // 统计客户类型分布
      const typeStats: {[key: string]: number} = {}
      
      recentClients.value.forEach(client => {
        const typeText = getClientTypeText(client.type)
        typeStats[typeText] = (typeStats[typeText] || 0) + 1
      })
      
      clientTypeStats.value = typeStats
      console.log('客户列表更新完成，共', recentClients.value.length, '条记录')
    } else {
      console.warn('客户列表数据格式异常:', response)
      recentClients.value = []
      clientTypeStats.value = {}
    }
  } catch (error) {
    console.error('获取客户列表失败:', error)
    recentClients.value = []
    clientTypeStats.value = {}
    ElMessage.warning('客户列表加载失败')
  } finally {
    clientLoading.value = false
  }
}

// 初始化饼图
const initPieChart = () => {
  if (!pieChartRef.value) return
  
  const chart = echarts.init(pieChartRef.value)
  const data = Object.entries(propertyTypeStats.value).map(([name, value]) => ({
    name,
    value
  }))
  
  const option = {
    tooltip: {
      trigger: 'item'
    },
    legend: {
      orient: 'vertical',
      left: 'left'
    },
    series: [
      {
        name: '房源类型',
        type: 'pie',
        radius: '50%',
        data,
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        }
      }
    ]
  }
  chart.setOption(option)
}

// 初始化柱状图
const initLineChart = () => {
  if (!lineChartRef.value) return
  
  const chart = echarts.init(lineChartRef.value)
  const data = Object.entries(clientTypeStats.value).map(([name, value]) => ({
    name,
    value
  }))
  
  const option = {
    tooltip: {
      trigger: 'axis'
    },
    xAxis: {
      type: 'category',
      data: data.map(item => item.name)
    },
    yAxis: {
      type: 'value',
      name: '数量'
    },
    series: [
      {
        name: '客户类型',
        type: 'bar',
        data: data.map(item => item.value),
        itemStyle: {
          color: '#1890ff'
        }
      }
    ]
  }
  chart.setOption(option)
}

// 初始化数据
const initData = async () => {
  await Promise.all([
    fetchPropertyStats(),
    fetchClientStats(),
    fetchRecentProperties(),
    fetchRecentClients()
  ])
  
  // 等待DOM更新后初始化图表
  nextTick(() => {
    initPieChart()
    initLineChart()
  })
}

onMounted(() => {
  initData()
})
</script>

<style scoped>
.dashboard {
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

.stats-row {
  margin-bottom: 24px;
}

.stat-card {
  height: 120px;
}

.stat-content {
  position: relative;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.stat-number {
  font-size: 32px;
  font-weight: bold;
  color: #1890ff;
  margin-bottom: 8px;
}

.stat-title {
  color: #666;
  font-size: 14px;
}

.stat-icon {
  position: absolute;
  right: 20px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 40px;
  color: #e6f7ff;
}

.charts-row {
  margin-bottom: 24px;
}

.chart-card {
  height: 400px;
}

.chart-container {
  height: 320px;
}

.property-list-card {
  margin-bottom: 24px;
}

.client-list-card {
  margin-bottom: 24px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.debug-row {
  margin-top: 20px;
}

.debug-card {
  height: 200px;
}

.debug-content {
  padding: 16px;
}
</style> 