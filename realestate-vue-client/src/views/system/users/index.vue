<template>
  <div class="app-container">
    <h1>用户管理</h1>
    <p>这是用户管理页面</p>
    <!-- 搜索和操作栏 -->
    <div class="filter-container">
      <el-form :inline="true" :model="queryParams" class="form-inline">
        <el-form-item label="用户名">
          <el-input v-model="queryParams.username" placeholder="请输入用户名" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="真实姓名">
          <el-input v-model="queryParams.realName" placeholder="请输入真实姓名" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="角色">
          <el-select v-model="queryParams.roleId" placeholder="请选择角色" clearable>
            <el-option v-for="item in roleOptions" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查询</el-button>
          <el-button @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>
      <div class="right-toolbar">
        <el-button type="primary" icon="Plus" @click="handleAdd">新增用户</el-button>
      </div>
    </div>

    <!-- 用户列表 -->
    <el-table
      v-loading="loading"
      :data="userList"
      border
      style="width: 100%"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="用户ID" prop="id" width="80" align="center" />
      <el-table-column label="用户名" prop="username" />
      <el-table-column label="真实姓名" prop="realName" />
      <el-table-column label="角色" align="center">
        <template #default="scope">
          <el-tag
            v-for="role in scope.row.roles"
            :key="role.id"
            :type="role.name === 'admin' ? 'danger' : ''"
            class="role-tag"
          >
            {{ role.name }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="状态" align="center" width="100">
        <template #default="scope">
          <el-switch
            v-model="scope.row.active"
            :active-value="true"
            :inactive-value="false"
            @change="handleStatusChange(scope.row)"
          />
        </template>
      </el-table-column>
      <el-table-column label="创建时间" prop="createdAt" width="180" />
      <el-table-column label="操作" align="center" width="200">
        <template #default="scope">
          <el-button type="primary" link icon="Edit" @click="handleEdit(scope.row)">编辑</el-button>
          <el-button type="primary" link icon="Key" @click="handleResetPwd(scope.row)">重置密码</el-button>
          <el-button type="danger" link icon="Delete" @click="handleDelete(scope.row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <el-pagination
      v-if="total > 0"
      :current-page="queryParams.pageNum"
      :page-sizes="[10, 20, 50, 100]"
      :page-size="queryParams.pageSize"
      layout="total, sizes, prev, pager, next, jumper"
      :total="total"
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
    />

    <!-- 添加或修改用户对话框 -->
    <el-dialog :title="title" v-model="open" width="600px" append-to-body>
      <el-form ref="userForm" :model="form" :rules="rules" label-width="100px">
        <el-form-item label="用户名" prop="username">
          <el-input v-model="form.username" placeholder="请输入用户名" :disabled="form.id !== undefined" />
        </el-form-item>
        <el-form-item label="真实姓名" prop="realName">
          <el-input v-model="form.realName" placeholder="请输入真实姓名" />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="!form.id">
          <el-input v-model="form.password" placeholder="请输入密码" type="password" />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword" v-if="!form.id">
          <el-input v-model="form.confirmPassword" placeholder="请确认密码" type="password" />
        </el-form-item>
        <el-form-item label="角色" prop="roleIds">
          <el-select v-model="form.roleIds" multiple placeholder="请选择角色">
            <el-option v-for="item in roleOptions" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-switch v-model="form.active" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="cancel">取 消</el-button>
          <el-button type="primary" @click="submitForm">确 定</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 重置密码对话框 -->
    <el-dialog title="重置密码" v-model="resetPwdOpen" width="500px" append-to-body>
      <el-form ref="resetPwdForm" :model="resetPwdForm" :rules="resetPwdRules" label-width="100px">
        <el-form-item label="新密码" prop="password">
          <el-input v-model="resetPwdForm.password" placeholder="请输入新密码" type="password" />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input v-model="resetPwdForm.confirmPassword" placeholder="请确认新密码" type="password" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="resetPwdOpen = false">取 消</el-button>
          <el-button type="primary" @click="submitResetPwd">确 定</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getUsers, getUser, createUser, updateUser, deleteUser, getRoles } from '@/api/auth'
import { validUsername } from '@/utils/validate'

// 加载状态
const loading = ref(false)
// 总条数
const total = ref(0)
// 用户列表
const userList = ref([])
// 角色选项
const roleOptions = ref([])
// 选中的用户列表
const selectedUsers = ref([])
// 对话框标题
const title = ref('')
// 是否显示对话框
const open = ref(false)
// 是否显示重置密码对话框
const resetPwdOpen = ref(false)
// 当前操作的用户信息
const currentUser = ref(null)

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  username: '',
  realName: '',
  roleId: undefined
})

// 表单参数
const form = reactive({
  id: undefined,
  username: '',
  realName: '',
  password: '',
  confirmPassword: '',
  roleIds: [],
  active: true
})

// 重置密码表单
const resetPwdForm = reactive({
  userId: null,
  password: '',
  confirmPassword: ''
})

// 表单校验规则
const rules = reactive({
  username: [
    { required: true, message: '用户名不能为空', trigger: 'blur' },
    { validator: validateUsername, trigger: 'blur' }
  ],
  realName: [
    { required: true, message: '真实姓名不能为空', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '密码不能为空', trigger: 'blur' },
    { min: 6, max: 18, message: '密码长度应为6-18个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '确认密码不能为空', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== form.password) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  roleIds: [
    { required: true, message: '请选择角色', trigger: 'blur' }
  ]
})

// 重置密码校验规则
const resetPwdRules = reactive({
  password: [
    { required: true, message: '新密码不能为空', trigger: 'blur' },
    { min: 6, max: 18, message: '密码长度应为6-18个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '确认密码不能为空', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== resetPwdForm.password) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
})

// 表单引用
const userForm = ref(null)
const resetPwdFormRef = ref(null)

/**
 * 组件挂载时获取数据
 */
onMounted(() => {
  console.log('用户管理页面已加载')
  // 获取用户列表
  getList()
  // 获取角色列表
  getRoleList()
})

/**
 * 获取用户列表
 */
function getList() {
  loading.value = true
  getUsers(queryParams).then(response => {
    userList.value = response.data.items
    total.value = response.data.total
    loading.value = false
  }).catch(() => {
    // 模拟数据
    userList.value = [
      {
        id: 1,
        username: 'admin',
        realName: '系统管理员',
        roles: [{ id: 1, name: 'admin' }],
        active: true,
        createdAt: '2023-05-01 09:00:00'
      },
      {
        id: 2,
        username: 'zhangsan',
        realName: '张三',
        roles: [{ id: 2, name: 'manager' }],
        active: true,
        createdAt: '2023-05-02 10:30:00'
      },
      {
        id: 3,
        username: 'lisi',
        realName: '李四',
        roles: [{ id: 3, name: 'sales' }],
        active: true,
        createdAt: '2023-05-03 11:45:00'
      }
    ]
    total.value = 3
    loading.value = false
  })
}

/**
 * 获取角色列表
 */
function getRoleList() {
  getRoles().then(response => {
    roleOptions.value = response.data
  }).catch(() => {
    // 模拟数据
    roleOptions.value = [
      { id: 1, name: 'admin', description: '系统管理员' },
      { id: 2, name: 'manager', description: '经理' },
      { id: 3, name: 'sales', description: '销售人员' }
    ]
  })
}

/**
 * 查询按钮操作
 */
function handleSearch() {
  queryParams.pageNum = 1
  getList()
}

/**
 * 重置查询操作
 */
function resetQuery() {
  queryParams.username = ''
  queryParams.realName = ''
  queryParams.roleId = undefined
  handleSearch()
}

/**
 * 多选框选中数据
 */
function handleSelectionChange(selection) {
  selectedUsers.value = selection
}

/**
 * 改变页码
 */
function handleSizeChange(val) {
  queryParams.pageSize = val
  getList()
}

/**
 * 改变页数
 */
function handleCurrentChange(val) {
  queryParams.pageNum = val
  getList()
}

/**
 * 重置表单
 */
function resetForm() {
  form.id = undefined
  form.username = ''
  form.realName = ''
  form.password = ''
  form.confirmPassword = ''
  form.roleIds = []
  form.active = true
}

/**
 * 添加用户
 */
function handleAdd() {
  resetForm()
  title.value = '添加用户'
  open.value = true
}

/**
 * 编辑用户
 */
function handleEdit(row) {
  resetForm()
  const userId = row.id
  getUser(userId).then(response => {
    Object.assign(form, response.data)
    form.roleIds = response.data.roles.map(role => role.id)
    title.value = '编辑用户'
    open.value = true
  }).catch(() => {
    // 模拟数据
    form.id = row.id
    form.username = row.username
    form.realName = row.realName
    form.roleIds = row.roles.map(role => role.id)
    form.active = row.active
    title.value = '编辑用户'
    open.value = true
  })
}

/**
 * 提交表单
 */
function submitForm() {
  userForm.value.validate(valid => {
    if (valid) {
      if (form.id) {
        // 编辑
        updateUser(form.id, form).then(() => {
          ElMessage.success('修改成功')
          open.value = false
          getList()
        })
      } else {
        // 新增
        createUser(form).then(() => {
          ElMessage.success('新增成功')
          open.value = false
          getList()
        })
      }
    }
  })
}

/**
 * 取消按钮
 */
function cancel() {
  open.value = false
  resetForm()
}

/**
 * 删除用户
 */
function handleDelete(row) {
  const userId = row.id
  ElMessageBox.confirm('确认删除该用户吗？', '警告', {
    confirmButtonText: '确认',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    deleteUser(userId).then(() => {
      ElMessage.success('删除成功')
      getList()
    })
  }).catch(() => {})
}

/**
 * 重置密码
 */
function handleResetPwd(row) {
  resetPwdForm.userId = row.id
  resetPwdForm.password = ''
  resetPwdForm.confirmPassword = ''
  resetPwdOpen.value = true
  currentUser.value = row
}

/**
 * 提交重置密码
 */
function submitResetPwd() {
  resetPwdFormRef.value.validate(valid => {
    if (valid) {
      const data = {
        userId: resetPwdForm.userId,
        password: resetPwdForm.password
      }
      // 模拟重置密码API调用
      setTimeout(() => {
        ElMessage.success(`用户 ${currentUser.value.username} 的密码已重置`)
        resetPwdOpen.value = false
      }, 500)
    }
  })
}

/**
 * 修改用户状态
 */
function handleStatusChange(row) {
  const status = row.active
  ElMessageBox.confirm(`确认要${status ? '启用' : '停用'}该用户吗？`, '警告', {
    confirmButtonText: '确认',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    updateUser(row.id, { active: status }).then(() => {
      ElMessage.success(`${status ? '启用' : '停用'}成功`)
    })
  }).catch(() => {
    row.active = !row.active // 取消时恢复状态
  })
}

/**
 * 验证用户名
 */
function validateUsername(rule, value, callback) {
  if (!validUsername(value)) {
    callback(new Error('请输入有效的用户名'))
  } else {
    callback()
  }
}
</script>

<style scoped>
.app-container {
  padding: 20px;
}

.filter-container {
  padding-bottom: 10px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.form-inline {
  display: flex;
  flex-wrap: wrap;
}

.right-toolbar {
  white-space: nowrap;
}

.role-tag {
  margin-right: 5px;
}
</style> 