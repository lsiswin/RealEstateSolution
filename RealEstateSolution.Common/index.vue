<!-- 代码已包含 CSS：使用 TailwindCSS , 安装 TailwindCSS 后方可看到布局样式效果 -->

<template>
  <div class="min-h-screen bg-gray-50">
    <div class="max-w-[1440px] mx-auto">
      <!-- 顶部导航 -->
      <nav class="h-16 bg-white shadow-sm flex items-center justify-between px-8">
        <div class="flex items-center gap-8">
          <img src="https://ai-public.mastergo.com/ai/img_res/149a3543927da3c58c6a9d93a8382373.jpg" 
               alt="Logo" class="h-8 object-contain" />
          <div class="flex gap-6">
            <a href="#" class="text-gray-600 hover:text-blue-600">首页</a>
            <a href="#" class="text-gray-600 hover:text-blue-600">房源管理</a>
            <a href="#" class="text-gray-600 hover:text-blue-600">客户管理</a>
            <a href="#" class="text-gray-600 hover:text-blue-600">数据报表</a>
          </div>
        </div>
        <div class="flex items-center gap-4">
          <el-input
            v-model="searchText"
            placeholder="搜索房源/客户"
            class="w-64"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
          <el-avatar :size="32" :src="userAvatar" />
        </div>
      </nav>

      <!-- 数据看板 -->
      <div class="grid grid-cols-4 gap-6 p-8">
        <div v-for="(stat, index) in statistics" :key="index"
             class="bg-white rounded-lg p-6 shadow-sm">
          <div class="text-gray-500 mb-2">{{ stat.label }}</div>
          <div class="text-2xl font-semibold">{{ stat.value }}</div>
          <div class="text-sm mt-2" :class="stat.trend > 0 ? 'text-green-500' : 'text-red-500'">
            {{ stat.trend > 0 ? '+' : '' }}{{ stat.trend }}% 较上月
          </div>
        </div>
      </div>

      <!-- 快捷功能区 -->
      <div class="grid grid-cols-3 gap-6 px-8 mb-8">
        <div v-for="(action, index) in quickActions" :key="index"
             class="bg-white rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
          <div class="flex items-center gap-4">
            <el-icon :size="24" class="text-blue-600">
              <component :is="action.icon" />
            </el-icon>
            <div>
              <div class="font-medium mb-1">{{ action.title }}</div>
              <div class="text-sm text-gray-500">{{ action.desc }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- 房源列表 -->
      <div class="px-8 mb-8">
        <div class="flex justify-between items-center mb-6">
          <h2 class="text-xl font-semibold">最新房源</h2>
          <el-button type="primary" class="!rounded-button">
            <el-icon class="mr-1"><Plus /></el-icon>
            发布房源
          </el-button>
        </div>
        
        <div class="grid grid-cols-3 gap-6">
          <div v-for="(house, index) in houses" :key="index"
               class="bg-white rounded-lg overflow-hidden shadow-sm hover:shadow-md transition-shadow">
            <div class="relative h-48">
              <img :src="house.image" :alt="house.title" class="w-full h-full object-cover object-top" />
              <div class="absolute top-2 left-2 bg-red-500 text-white px-2 py-1 text-sm rounded"
                   v-if="house.isUrgent">
                急售
              </div>
            </div>
            <div class="p-4">
              <h3 class="font-medium mb-2">{{ house.title }}</h3>
              <div class="flex gap-2 mb-2">
                <span class="text-sm text-gray-500">{{ house.area }}m² </span>
                <span class="text-sm text-gray-500">{{ house.rooms }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-red-500 font-semibold">{{ house.price }}万</span>
                <el-button type="primary" plain size="small" class="!rounded-button">查看详情</el-button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { Search, Plus, House, Calendar, MessageBox } from '@element-plus/icons-vue';

const searchText = ref('');
const userAvatar = 'https://ai-public.mastergo.com/ai/img_res/29bae3ee8b62e1003070da2a4c9b242c.jpg';

const statistics = [
  { label: '本月新增房源', value: '128', trend: 12.5 },
  { label: '带看预约数', value: '56', trend: 8.3 },
  { label: '成交总额', value: '￥3,680万', trend: 15.2 },
  { label: '新增客户', value: '86', trend: -5.4 },
];

const quickActions = [
  {
    icon: House,
    title: '登记房源',
    desc: '快速录入新房源信息'
  },
  {
    icon: Calendar,
    title: '预约管理',
    desc: '查看和安排看房日程'
  },
  {
    icon: MessageBox,
    title: '客户跟进',
    desc: '及时响应客户需求'
  }
];

const houses = [
  {
    image: 'https://ai-public.mastergo.com/ai/img_res/fcd34b9d2caa9e9f6083194632e3956d.jpg',
    title: '华府骏苑 3室2厅',
    area: 128,
    rooms: '3室2厅2卫',
    price: 580,
    isUrgent: true
  },
  {
    image: 'https://ai-public.mastergo.com/ai/img_res/439ed01260d209d0b8a455075c99e081.jpg',
    title: '碧桂园 4室2厅',
    area: 156,
    rooms: '4室2厅2卫',
    price: 680,
    isUrgent: false
  },
  {
    image: 'https://ai-public.mastergo.com/ai/img_res/9f7148cbded1b3a72c9ea8546a8cc105.jpg',
    title: '万科城 2室2厅',
    area: 89,
    rooms: '2室2厅1卫',
    price: 320,
    isUrgent: true
  },
  {
    image: 'https://ai-public.mastergo.com/ai/img_res/8ebe83728aa243bcb4ac03ce28dba5c6.jpg',
    title: '绿城桂语江南 3室2厅',
    area: 142,
    rooms: '3室2厅2卫',
    price: 620,
    isUrgent: false
  },
  {
    image: 'https://ai-public.mastergo.com/ai/img_res/294a9a3fe354226f318636a9c3d95bcf.jpg',
    title: '保利香槟国际 4室2厅',
    area: 168,
    rooms: '4室2厅2卫',
    price: 750,
    isUrgent: true
  },
  {
    image: 'https://ai-public.mastergo.com/ai/img_res/e9eda276fa757e6501d593fa4f26763f.jpg',
    title: '龙湖天街 3室2厅',
    area: 135,
    rooms: '3室2厅2卫',
    price: 598,
    isUrgent: false
  }
];
</script>

<style scoped>
.el-input :deep(.el-input__wrapper) {
  border-radius: 8px;
  box-shadow: 0 0 0 1px #e5e7eb;
}

.el-input :deep(.el-input__wrapper.is-focus) {
  box-shadow: 0 0 0 1px #3b82f6;
}

.el-input :deep(.el-input__inner) {
  font-size: 14px;
}
</style>

