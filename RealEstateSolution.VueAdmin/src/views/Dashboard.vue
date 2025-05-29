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
            <div class="stat-number">{{ stats.totalUsers }}</div>
            <div class="stat-title">总用户数</div>
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
            <div class="stat-number">{{ stats.averagePrice }}</div>
            <div class="stat-title">平均价格(万)</div>
            <div class="stat-icon">
              <el-icon><Money /></el-icon>
            </div>
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
            <span>房源状态分布</span>
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

    <!-- 最新用户列表 -->
    <el-card class="user-list-card">
      <template #header>
        <div class="card-header">
          <span>最新用户</span>
          <el-button type="primary" size="small" @click="viewAllUsers">
            查看全部
          </el-button>
        </div>
      </template>
      
      <el-table :data="recentUsers" style="width: 100%" v-loading="userLoading">
        <el-table-column prop="id" label="用户ID" width="120" />
        <el-table-column prop="userName" label="用户名" />
        <el-table-column prop="realName" label="真实姓名" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="phone" label="电话" />
        <el-table-column prop="isActive" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.isActive ? 'success' : 'danger'">
              {{ scope.row.isActive ? '活跃' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="注册时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createTime) }}
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
  getUserList, 
  getUserStats,
  type User as UserType
} from '@/api/user'

const router = useRouter()

// 加载状态
const loading = ref(false)
const userLoading = ref(false)

// 统计数据
const stats = ref({
  totalProperties: 0,
  totalUsers: 0,
  forSaleCount: 0,
  averagePrice: 0
})

// 最新房源数据
const recentProperties = ref<Property[]>([])

// 最新用户数据
const recentUsers = ref<UserType[]>([])

// 图表引用
const pieChartRef = ref<HTMLDivElement>()
const lineChartRef = ref<HTMLDivElement>()

// 房源类型统计数据
const propertyTypeStats = ref<{[key: string]: number}>({})

// 房源状态统计数据
const propertyStatusStats = ref<{[key: string]: number}>({})

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 跳转到房源列表
const viewAllProperties = () => {
  router.push('/property/list')
}

// 跳转到用户列表
const viewAllUsers = () => {
  router.push('/system/users')
}

// 获取房源统计数据
const fetchPropertyStats = async () => {
  try {
    const response = await getPropertyStats()
    if (response.success && response.data) {
      const propertyStats = response.data
      stats.value.totalProperties = propertyStats.totalCount
      stats.value.forSaleCount = propertyStats.forSaleCount
      stats.value.averagePrice = Math.round(propertyStats.averagePrice / 10000) // 转换为万元
    }
  } catch (error) {
    console.error('获取房源统计失败:', error)
  }
}

// 获取用户统计数据
const fetchUserStats = async () => {
  try {
    const userStats = await getUserStats()
    stats.value.totalUsers = userStats.totalUsers
  } catch (error) {
    console.error('获取用户统计失败:', error)
  }
}

// 获取最新房源列表
const fetchRecentProperties = async () => {
  loading.value = true
  try {
    const response = await queryProperties({
      pageIndex: 1,
      pageSize: 5
    })
    if (response.success && response.data) {
      recentProperties.value = response.data.items
      
      // 统计房源类型分布
      const typeStats: {[key: string]: number} = {}
      const statusStats: {[key: string]: number} = {}
      
      response.data.items.forEach(property => {
        const typeText = getPropertyTypeText(property.type)
        const statusText = getPropertyStatusText(property.status)
        
        typeStats[typeText] = (typeStats[typeText] || 0) + 1
        statusStats[statusText] = (statusStats[statusText] || 0) + 1
      })
      
      propertyTypeStats.value = typeStats
      propertyStatusStats.value = statusStats
    }
  } catch (error) {
    console.error('获取房源列表失败:', error)
    ElMessage.error('获取房源列表失败')
  } finally {
    loading.value = false
  }
}

// 获取最新用户列表
const fetchRecentUsers = async () => {
  userLoading.value = true
  try {
    const response = await getUserList({
      pageIndex: 1,
      pageSize: 5
    })
    recentUsers.value = response.items
  } catch (error) {
    console.error('获取用户列表失败:', error)
    ElMessage.error('获取用户列表失败')
  } finally {
    userLoading.value = false
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
  const data = Object.entries(propertyStatusStats.value).map(([name, value]) => ({
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
        name: '房源状态',
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
    fetchUserStats(),
    fetchRecentProperties(),
    fetchRecentUsers()
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

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style> 