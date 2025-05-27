<template>
  <div class="client-list-container">
    <div class="client-list-header">
      <el-row :gutter="20">
        <el-col :span="18">
          <el-input
            v-model="searchQuery"
            placeholder="搜索客户（姓名/电话/邮箱）"
            prefix-icon="Search"
            clearable
            @clear="handleSearch"
            @keyup.enter="handleSearch"
          />
        </el-col>
        <el-col :span="6" class="text-right">
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>添加客户
          </el-button>
        </el-col>
      </el-row>
      
      <el-row :gutter="20" class="filter-row">
        <el-col :span="8">
          <el-select v-model="filters.clientType" placeholder="客户类型" clearable @change="handleSearch">
            <el-option label="个人" value="个人" />
            <el-option label="企业" value="企业" />
            <el-option label="中介" value="中介" />
          </el-select>
        </el-col>
        <el-col :span="8">
          <el-select v-model="filters.intentionLevel" placeholder="意向等级" clearable @change="handleSearch">
            <el-option label="高" value="高" />
            <el-option label="中" value="中" />
            <el-option label="低" value="低" />
          </el-select>
        </el-col>
        <el-col :span="8">
          <el-date-picker
            v-model="filters.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="YYYY-MM-DD"
            @change="handleSearch"
          />
        </el-col>
      </el-row>
    </div>

    <!-- 客户列表 -->
    <el-table
      v-loading="loading"
      :data="clientList"
      border
      style="width: 100%"
      @sort-change="handleSortChange"
    >
      <el-table-column prop="id" label="ID" width="80" sortable />
      <el-table-column prop="name" label="姓名" min-width="120">
        <template #default="{ row }">
          <el-link type="primary" @click="handleViewDetail(row.id)">{{ row.name }}</el-link>
        </template>
      </el-table-column>
      <el-table-column prop="phone" label="联系电话" width="140" />
      <el-table-column prop="email" label="电子邮箱" min-width="180" />
      <el-table-column prop="clientType" label="客户类型" width="100" />
      <el-table-column prop="intentionLevel" label="意向等级" width="100">
        <template #default="{ row }">
          <el-tag :type="getIntentionLevelType(row.intentionLevel)">{{ row.intentionLevel }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="intentionProperty" label="意向房源" min-width="160" />
      <el-table-column prop="followupTime" label="最近跟进" width="180" sortable />
      <el-table-column prop="createTime" label="创建时间" width="180" sortable />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button size="small" type="primary" @click="handleEdit(row.id)">
            <el-icon><Edit /></el-icon>编辑
          </el-button>
          <el-button size="small" type="success" @click="handleFollowup(row.id)">
            <el-icon><ChatLineRound /></el-icon>跟进
          </el-button>
          <el-button size="small" type="danger" @click="handleDelete(row.id)">
            <el-icon><Delete /></el-icon>删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <div class="pagination-container">
      <el-pagination
        v-model:current-page="page.current"
        v-model:page-size="page.size"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        :total="page.total"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Edit, Delete, Search, ChatLineRound } from '@element-plus/icons-vue'

const router = useRouter()
const loading = ref(false)
const searchQuery = ref('')
const clientList = ref([])

// 筛选条件
const filters = reactive({
  clientType: '',
  intentionLevel: '',
  dateRange: []
})

// 分页参数
const page = reactive({
  current: 1,
  size: 10,
  total: 0
})

// 排序参数
const sort = reactive({
  prop: '',
  order: ''
})

// 获取客户列表数据
const fetchClientList = async () => {
  loading.value = true
  try {
    // 模拟API调用
    // const response = await getClientList({
    //   page: page.current,
    //   pageSize: page.size,
    //   query: searchQuery.value,
    //   ...filters,
    //   startDate: filters.dateRange && filters.dateRange[0],
    //   endDate: filters.dateRange && filters.dateRange[1],
    //   sortProp: sort.prop,
    //   sortOrder: sort.order
    // })
    // clientList.value = response.data.list
    // page.total = response.data.total

    // 模拟数据
    setTimeout(() => {
      clientList.value = [
        {
          id: 1,
          name: '张三',
          phone: '13812345678',
          email: 'zhangsan@example.com',
          clientType: '个人',
          intentionLevel: '高',
          intentionProperty: '三居室，朝阳区，100-120㎡',
          followupTime: '2023-06-20 14:30:00',
          createTime: '2023-06-15 10:23:45'
        },
        {
          id: 2,
          name: '李四企业',
          phone: '13987654321',
          email: 'lisi@company.com',
          clientType: '企业',
          intentionLevel: '中',
          intentionProperty: '写字楼，西城区，300㎡以上',
          followupTime: '2023-06-18 16:45:00',
          createTime: '2023-05-20 14:30:22'
        },
        {
          id: 3,
          name: '王五',
          phone: '13700001111',
          email: 'wangwu@example.com',
          clientType: '个人',
          intentionLevel: '低',
          intentionProperty: '两居室，海淀区，70-90㎡',
          followupTime: '2023-06-15 11:20:00',
          createTime: '2023-04-10 09:15:36'
        }
      ]
      page.total = 3
      loading.value = false
    }, 500)
  } catch (error) {
    console.error('获取客户列表失败', error)
    ElMessage.error('获取客户列表失败')
    loading.value = false
  }
}

// 根据意向等级获取标签类型
const getIntentionLevelType = (level) => {
  const levelMap = {
    '高': 'success',
    '中': 'warning',
    '低': 'info'
  }
  return levelMap[level] || ''
}

// 搜索处理
const handleSearch = () => {
  page.current = 1
  fetchClientList()
}

// 添加客户
const handleAdd = () => {
  router.push('/client/add')
}

// 编辑客户
const handleEdit = (id) => {
  router.push(`/client/edit/${id}`)
}

// 客户跟进
const handleFollowup = (id) => {
  router.push(`/client/followup/${id}`)
}

// 查看详情
const handleViewDetail = (id) => {
  router.push(`/client/detail/${id}`)
}

// 删除客户
const handleDelete = (id) => {
  ElMessageBox.confirm('确定要删除该客户吗？', '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      // 模拟API调用
      // await deleteClient(id)
      ElMessage.success('删除成功')
      fetchClientList()
    } catch (error) {
      console.error('删除失败', error)
      ElMessage.error('删除失败')
    }
  }).catch(() => {
    // 取消删除，不做处理
  })
}

// 排序变化
const handleSortChange = ({ prop, order }) => {
  sort.prop = prop
  sort.order = order
  fetchClientList()
}

// 分页大小变化
const handleSizeChange = (val) => {
  page.size = val
  fetchClientList()
}

// 页码变化
const handleCurrentChange = (val) => {
  page.current = val
  fetchClientList()
}

// 组件挂载时获取数据
onMounted(() => {
  fetchClientList()
  console.log('客户列表页面已加载')
})
</script>

<style scoped>
.client-list-container {
  padding: 20px;
}

.client-list-header {
  margin-bottom: 20px;
}

.filter-row {
  margin-top: 15px;
}

.text-right {
  text-align: right;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: center;
}

.el-select, .el-date-picker {
  width: 100%;
}
</style> 