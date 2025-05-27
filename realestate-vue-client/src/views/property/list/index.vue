<template>
  <div class="property-list-container">
    <div class="property-list-header">
      <el-row :gutter="20">
        <el-col :span="18">
          <el-input
            v-model="searchQuery"
            placeholder="搜索房源（名称/地址/编号）"
            prefix-icon="Search"
            clearable
            @clear="handleSearch"
            @keyup.enter="handleSearch"
          />
        </el-col>
        <el-col :span="6" class="text-right">
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>添加房源
          </el-button>
        </el-col>
      </el-row>
      
      <el-row :gutter="20" class="filter-row">
        <el-col :span="6">
          <el-select v-model="filters.propertyType" placeholder="房源类型" clearable @change="handleSearch">
            <el-option label="住宅" value="住宅" />
            <el-option label="商铺" value="商铺" />
            <el-option label="写字楼" value="写字楼" />
            <el-option label="别墅" value="别墅" />
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-select v-model="filters.status" placeholder="状态" clearable @change="handleSearch">
            <el-option label="在售" value="在售" />
            <el-option label="已售" value="已售" />
            <el-option label="预售" value="预售" />
            <el-option label="待售" value="待售" />
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-select v-model="filters.area" placeholder="面积范围" clearable @change="handleSearch">
            <el-option label="50㎡以下" value="0-50" />
            <el-option label="50-100㎡" value="50-100" />
            <el-option label="100-150㎡" value="100-150" />
            <el-option label="150㎡以上" value="150-999" />
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-select v-model="filters.price" placeholder="价格范围" clearable @change="handleSearch">
            <el-option label="100万以下" value="0-100" />
            <el-option label="100-300万" value="100-300" />
            <el-option label="300-500万" value="300-500" />
            <el-option label="500万以上" value="500-9999" />
          </el-select>
        </el-col>
      </el-row>
    </div>

    <!-- 房源列表 -->
    <el-table
      v-loading="loading"
      :data="propertyList"
      border
      style="width: 100%"
      @sort-change="handleSortChange"
    >
      <el-table-column prop="id" label="ID" width="80" sortable />
      <el-table-column prop="title" label="房源名称" min-width="180">
        <template #default="{ row }">
          <el-link type="primary" @click="handleViewDetail(row.id)">{{ row.title }}</el-link>
        </template>
      </el-table-column>
      <el-table-column prop="address" label="地址" min-width="180" />
      <el-table-column prop="propertyType" label="类型" width="100" />
      <el-table-column prop="area" label="面积" width="100" sortable>
        <template #default="{ row }">
          {{ row.area }} ㎡
        </template>
      </el-table-column>
      <el-table-column prop="price" label="价格" width="120" sortable>
        <template #default="{ row }">
          {{ row.price }} 万元
        </template>
      </el-table-column>
      <el-table-column prop="status" label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusTagType(row.status)">{{ row.status }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="创建时间" width="180" sortable />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button size="small" type="primary" @click="handleEdit(row.id)">
            <el-icon><Edit /></el-icon>编辑
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
import { Plus, Edit, Delete, Search } from '@element-plus/icons-vue'
// 假设已经创建了房源API服务
// import { getPropertyList, deleteProperty } from '@/api/property'

const router = useRouter()
const loading = ref(false)
const searchQuery = ref('')
const propertyList = ref([])

// 筛选条件
const filters = reactive({
  propertyType: '',
  status: '',
  area: '',
  price: ''
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

// 获取房源列表数据
const fetchPropertyList = async () => {
  loading.value = true
  try {
    // 模拟API调用
    // const response = await getPropertyList({
    //   page: page.current,
    //   pageSize: page.size,
    //   query: searchQuery.value,
    //   ...filters,
    //   sortProp: sort.prop,
    //   sortOrder: sort.order
    // })
    // propertyList.value = response.data.list
    // page.total = response.data.total

    // 模拟数据
    setTimeout(() => {
      propertyList.value = [
        {
          id: 1,
          title: '阳光花园3室2厅',
          address: '北京市朝阳区阳光花园小区B栋2单元303',
          propertyType: '住宅',
          area: 120,
          price: 450,
          status: '在售',
          createTime: '2023-06-15 10:23:45'
        },
        {
          id: 2,
          title: '金融街写字楼A座',
          address: '北京市西城区金融街12号',
          propertyType: '写字楼',
          area: 300,
          price: 1200,
          status: '在售',
          createTime: '2023-05-20 14:30:22'
        },
        {
          id: 3,
          title: '滨江花园2室1厅',
          address: '上海市浦东新区滨江大道123号',
          propertyType: '住宅',
          area: 89,
          price: 380,
          status: '已售',
          createTime: '2023-04-10 09:15:36'
        }
      ]
      page.total = 3
      loading.value = false
    }, 500)
  } catch (error) {
    console.error('获取房源列表失败', error)
    ElMessage.error('获取房源列表失败')
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

// 搜索处理
const handleSearch = () => {
  page.current = 1
  fetchPropertyList()
}

// 添加房源
const handleAdd = () => {
  router.push('/property/add')
}

// 编辑房源
const handleEdit = (id) => {
  router.push(`/property/edit/${id}`)
}

// 查看详情
const handleViewDetail = (id) => {
  router.push(`/property/detail/${id}`)
}

// 删除房源
const handleDelete = (id) => {
  ElMessageBox.confirm('确定要删除该房源吗？', '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      // 模拟API调用
      // await deleteProperty(id)
      ElMessage.success('删除成功')
      fetchPropertyList()
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
  fetchPropertyList()
}

// 分页大小变化
const handleSizeChange = (val) => {
  page.size = val
  fetchPropertyList()
}

// 页码变化
const handleCurrentChange = (val) => {
  page.current = val
  fetchPropertyList()
}

// 组件挂载时获取数据
onMounted(() => {
  console.log('房源列表页面已加载')
  fetchPropertyList()
})
</script>

<style scoped>
.property-list-container {
  padding: 20px;
}

.property-list-header {
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

.el-select {
  width: 100%;
}
</style> 