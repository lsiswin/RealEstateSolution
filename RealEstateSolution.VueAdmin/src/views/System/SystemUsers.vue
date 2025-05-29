<template>
  <div class="system-users">
    <div class="page-header">
      <h2>用户管理</h2>
      <p>管理系统用户账户和权限</p>
    </div>
    
    <!-- 搜索和筛选区域 -->
    <el-card class="search-card">
      <el-form :model="searchForm" :inline="true" class="search-form">
        <el-form-item label="用户名">
          <el-input
            v-model="searchForm.username"
            placeholder="请输入用户名"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="真实姓名">
          <el-input
            v-model="searchForm.realName"
            placeholder="请输入真实姓名"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="角色">
          <el-select
            v-model="searchForm.roleId"
            placeholder="请选择角色"
            clearable
            style="width: 150px"
          >
            <el-option
              v-for="role in roleList"
              :key="role.id"
              :label="role.name"
              :value="role.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch" :loading="loading">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
          <el-button type="success" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            新增用户
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 用户列表 -->
    <el-card class="table-card">
      <el-table
        :data="userList"
        v-loading="loading"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="用户ID" width="80" />
        <el-table-column prop="userName" label="用户名" width="120" />
        <el-table-column prop="realName" label="真实姓名" width="120" />
        <el-table-column prop="email" label="邮箱" width="180" />
        <el-table-column prop="phone" label="电话" width="130" />
        <el-table-column prop="roles" label="角色" width="150">
          <template #default="scope">
            <el-tag
              v-for="role in scope.row.roles"
              :key="role"
              style="margin-right: 4px;"
              size="small"
            >
              {{ role }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="isActive" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.isActive ? 'success' : 'danger'">
              {{ scope.row.isActive ? '活跃' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间" width="150">
          <template #default="scope">
            {{ formatDate(scope.row.createTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="lastLoginTime" label="最后登录" width="150">
          <template #default="scope">
            {{ scope.row.lastLoginTime ? formatDate(scope.row.lastLoginTime) : '-' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="scope">
            <el-button
              type="primary"
              size="small"
              @click="handleView(scope.row)"
            >
              查看
            </el-button>
            <el-button
              type="warning"
              size="small"
              @click="handleEdit(scope.row)"
            >
              编辑
            </el-button>
            <el-button
              :type="scope.row.isActive ? 'danger' : 'success'"
              size="small"
              @click="handleToggleStatus(scope.row)"
            >
              {{ scope.row.isActive ? '禁用' : '启用' }}
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(scope.row)"
              v-if="scope.row.id !== currentUserId"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.pageNum"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 用户详情/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form
        ref="userFormRef"
        :model="userForm"
        :rules="userRules"
        label-width="100px"
      >
        <el-form-item label="用户名" prop="username">
          <el-input 
            v-model="userForm.username" 
            placeholder="请输入用户名"
            :disabled="isEditMode"
          />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="!isEditMode">
          <el-input 
            v-model="userForm.password" 
            type="password"
            placeholder="请输入密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="真实姓名" prop="realName">
          <el-input v-model="userForm.realName" placeholder="请输入真实姓名" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="userForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="电话">
          <el-input v-model="userForm.phone" placeholder="请输入电话号码" />
        </el-form-item>
        <el-form-item label="角色" prop="roleIds">
          <el-select 
            v-model="userForm.roleIds" 
            multiple 
            placeholder="请选择角色"
            style="width: 100%"
          >
            <el-option
              v-for="role in roleList"
              :key="role.id"
              :label="role.name"
              :value="role.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="状态" v-if="isEditMode">
          <el-switch
            v-model="userForm.isActive"
            active-text="启用"
            inactive-text="禁用"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button
            type="primary"
            @click="handleSubmit"
            :loading="submitLoading"
            v-if="!isViewMode"
          >
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Search, Refresh, Plus } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import {
  getUserList,
  createUser,
  updateUser,
  deleteUser,
  getRoles,
  type User,
  type Role,
  type PageParams,
  type PageResult,
  type CreateUserRequest,
  type UpdateUserRequest
} from '@/api/user'

const userStore = useUserStore()

// 响应式数据
const loading = ref(false)
const submitLoading = ref(false)
const userList = ref<User[]>([])
const roleList = ref<Role[]>([])
const selectedUsers = ref<User[]>([])

// 当前用户ID（不能删除自己）
const currentUserId = computed(() => userStore.userInfo?.id)

// 搜索表单
const searchForm = reactive<PageParams>({
  pageNum: 1,
  pageSize: 10,
  username: '',
  realName: '',
  roleId: undefined
})

// 分页信息
const pagination = reactive({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

// 对话框相关
const dialogVisible = ref(false)
const dialogTitle = ref('')
const isViewMode = ref(false)
const isEditMode = ref(false)
const userFormRef = ref<FormInstance>()

// 用户表单
const userForm = reactive<Partial<CreateUserRequest & UpdateUserRequest & { id?: string; isActive?: boolean }>>({
  username: '',
  password: '',
  email: '',
  realName: '',
  phone: '',
  roleIds: [],
  isActive: true
})

// 表单验证规则
const userRules: FormRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '用户名长度在 3 到 20 个字符', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  realName: [
    { min: 2, max: 20, message: '真实姓名长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  roleIds: [
    { required: true, message: '请选择角色', trigger: 'change' }
  ]
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 获取用户列表
const fetchUserList = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      pageNum: pagination.pageNum,
      pageSize: pagination.pageSize
    }
    const response: PageResult<User> = await getUserList(params)
    userList.value = response.items
    pagination.total = response.totalCount
  } catch (error) {
    console.error('获取用户列表失败:', error)
    ElMessage.error('获取用户列表失败')
  } finally {
    loading.value = false
  }
}

// 获取角色列表
const fetchRoleList = async () => {
  try {
    const roles = await getRoles()
    roleList.value = roles
  } catch (error) {
    console.error('获取角色列表失败:', error)
    ElMessage.error('获取角色列表失败')
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageNum = 1
  fetchUserList()
}

// 重置
const handleReset = () => {
  Object.assign(searchForm, {
    username: '',
    realName: '',
    roleId: undefined
  })
  pagination.pageNum = 1
  fetchUserList()
}

// 新增用户
const handleAdd = () => {
  dialogTitle.value = '新增用户'
  isViewMode.value = false
  isEditMode.value = false
  resetUserForm()
  dialogVisible.value = true
}

// 查看用户
const handleView = (user: User) => {
  dialogTitle.value = '用户详情'
  isViewMode.value = true
  isEditMode.value = true
  Object.assign(userForm, {
    ...user,
    roleIds: [] // 需要根据用户角色设置
  })
  dialogVisible.value = true
}

// 编辑用户
const handleEdit = (user: User) => {
  dialogTitle.value = '编辑用户'
  isViewMode.value = false
  isEditMode.value = true
  Object.assign(userForm, {
    ...user,
    roleIds: [] // 需要根据用户角色设置
  })
  dialogVisible.value = true
}

// 切换用户状态
const handleToggleStatus = async (user: User) => {
  try {
    const action = user.isActive ? '禁用' : '启用'
    await ElMessageBox.confirm(
      `确定要${action}用户 "${user.userName}" 吗？`,
      '状态切换确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await updateUser(user.id, { isActive: !user.isActive })
    ElMessage.success(`${action}成功`)
    fetchUserList()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('切换用户状态失败:', error)
      ElMessage.error('操作失败')
    }
  }
}

// 删除用户
const handleDelete = async (user: User) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除用户 "${user.userName}" 吗？此操作不可恢复！`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteUser(user.id)
    ElMessage.success('删除成功')
    fetchUserList()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除用户失败:', error)
      ElMessage.error('删除失败')
    }
  }
}

// 表格选择变化
const handleSelectionChange = (selection: User[]) => {
  selectedUsers.value = selection
}

// 分页大小变化
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageNum = 1
  fetchUserList()
}

// 当前页变化
const handleCurrentChange = (page: number) => {
  pagination.pageNum = page
  fetchUserList()
}

// 提交表单
const handleSubmit = async () => {
  if (!userFormRef.value) return
  
  try {
    await userFormRef.value.validate()
    submitLoading.value = true
    
    if (isEditMode.value && userForm.id) {
      // 编辑
      const updateData: UpdateUserRequest = {
        username: userForm.username,
        email: userForm.email,
        realName: userForm.realName,
        roleIds: userForm.roleIds,
        isActive: userForm.isActive
      }
      await updateUser(userForm.id, updateData)
      ElMessage.success('更新成功')
    } else {
      // 新增
      const createData: CreateUserRequest = {
        username: userForm.username!,
        password: userForm.password!,
        email: userForm.email!,
        realName: userForm.realName,
        roleIds: userForm.roleIds
      }
      await createUser(createData)
      ElMessage.success('创建成功')
    }
    
    dialogVisible.value = false
    fetchUserList()
  } catch (error) {
    console.error('提交失败:', error)
    ElMessage.error('操作失败')
  } finally {
    submitLoading.value = false
  }
}

// 对话框关闭
const handleDialogClose = () => {
  userFormRef.value?.resetFields()
  resetUserForm()
}

// 重置用户表单
const resetUserForm = () => {
  Object.assign(userForm, {
    id: undefined,
    username: '',
    password: '',
    email: '',
    realName: '',
    phone: '',
    roleIds: [],
    isActive: true
  })
}

// 初始化
onMounted(() => {
  fetchUserList()
  fetchRoleList()
})
</script>

<style scoped>
.system-users {
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

.search-card {
  margin-bottom: 16px;
}

.search-form {
  margin-bottom: 0;
}

.table-card {
  margin-bottom: 16px;
}

.pagination-container {
  margin-top: 16px;
  text-align: right;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style> 