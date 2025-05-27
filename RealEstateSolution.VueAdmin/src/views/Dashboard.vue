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
            <div class="stat-number">{{ stats.totalContracts }}</div>
            <div class="stat-title">总合同数</div>
            <div class="stat-icon">
              <el-icon><Document /></el-icon>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-number">{{ stats.monthlyRevenue }}</div>
            <div class="stat-title">本月收入(万)</div>
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
            <span>月度成交趋势</span>
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
      
      <el-table :data="recentProperties" style="width: 100%">
        <el-table-column prop="id" label="房源编号" width="120" />
        <el-table-column prop="title" label="房源标题" />
        <el-table-column prop="type" label="房源类型" width="100" />
        <el-table-column prop="area" label="面积(㎡)" width="100" />
        <el-table-column prop="price" label="价格(万)" width="120" />
        <el-table-column prop="location" label="位置" width="150" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="getStatusType(scope.row.status)">
              {{ scope.row.status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="发布时间" width="150" />
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import * as echarts from 'echarts'

const router = useRouter()

// 统计数据
const stats = ref({
  totalProperties: 1256,
  totalClients: 892,
  totalContracts: 345,
  monthlyRevenue: 128.5
})

// 最新房源数据
const recentProperties = ref([
  {
    id: 'P001',
    title: '市中心精装三房',
    type: '住宅',
    area: 120,
    price: 280,
    location: '天河区',
    status: '在售',
    createTime: '2024-01-15'
  },
  {
    id: 'P002',
    title: '商业写字楼',
    type: '商业',
    area: 200,
    price: 450,
    location: '珠江新城',
    status: '在售',
    createTime: '2024-01-14'
  },
  {
    id: 'P003',
    title: '学区房两房一厅',
    type: '住宅',
    area: 85,
    price: 220,
    location: '越秀区',
    status: '已售',
    createTime: '2024-01-13'
  },
  {
    id: 'P004',
    title: '豪华别墅',
    type: '别墅',
    area: 350,
    price: 800,
    location: '番禺区',
    status: '在售',
    createTime: '2024-01-12'
  },
  {
    id: 'P005',
    title: '临街商铺',
    type: '商业',
    area: 60,
    price: 180,
    location: '荔湾区',
    status: '预售',
    createTime: '2024-01-11'
  }
])

// 图表引用
const pieChartRef = ref<HTMLDivElement>()
const lineChartRef = ref<HTMLDivElement>()

// 获取状态标签类型
const getStatusType = (status: string) => {
  switch (status) {
    case '在售':
      return 'success'
    case '已售':
      return 'info'
    case '预售':
      return 'warning'
    default:
      return 'info'
  }
}

// 跳转到房源列表
const viewAllProperties = () => {
  router.push('/property/list')
}

// 初始化饼图
const initPieChart = () => {
  if (!pieChartRef.value) return
  
  const chart = echarts.init(pieChartRef.value)
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
        data: [
          { value: 680, name: '住宅' },
          { value: 310, name: '商业' },
          { value: 156, name: '别墅' },
          { value: 110, name: '其他' }
        ],
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

// 初始化折线图
const initLineChart = () => {
  if (!lineChartRef.value) return
  
  const chart = echarts.init(lineChartRef.value)
  const option = {
    tooltip: {
      trigger: 'axis'
    },
    legend: {
      data: ['成交量', '成交额']
    },
    xAxis: {
      type: 'category',
      data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
    },
    yAxis: [
      {
        type: 'value',
        name: '成交量(套)',
        position: 'left'
      },
      {
        type: 'value',
        name: '成交额(万)',
        position: 'right'
      }
    ],
    series: [
      {
        name: '成交量',
        type: 'line',
        data: [28, 32, 45, 38, 52, 48, 55, 62, 58, 65, 72, 68],
        yAxisIndex: 0
      },
      {
        name: '成交额',
        type: 'line',
        data: [680, 780, 1120, 950, 1300, 1200, 1380, 1550, 1450, 1620, 1800, 1700],
        yAxisIndex: 1
      }
    ]
  }
  chart.setOption(option)
}

onMounted(() => {
  nextTick(() => {
    initPieChart()
    initLineChart()
  })
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