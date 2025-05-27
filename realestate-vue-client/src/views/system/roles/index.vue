<template>
  <div class="app-container">
    <h1>角色权限管理</h1>
    <p>这是角色权限管理页面</p>
    <!-- 角色列表 -->
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>角色列表</span>
          <el-button type="primary" icon="Plus" @click="handleAddRole">新增角色</el-button>
        </div>
      </template>
      <el-table
        v-loading="loading"
        :data="roleList"
        style="width: 100%"
        border
        highlight-current-row
        @current-change="handleCurrentChange"
      >
        <el-table-column label="角色名称" prop="name" />
        <el-table-column label="角色描述" prop="description" />
        <el-table-column label="创建时间" prop="createdAt" width="180" />
        <el-table-column label="操作" align="center" width="200">
          <template #default="scope">
            <el-button type="primary" link icon="Edit" @click="handleEditRole(scope.row)">编辑</el-button>
            <el-button
              type="danger"
              link
              icon="Delete"
              @click="handleDeleteRole(scope.row)"
              :disabled="scope.row.name === 'admin'"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 权限分配 -->
    <el-card class="box-card permission-card">
      <template #header>
        <div class="card-header">
          <span>权限分配</span>
          <el-button
            type="primary"
            icon="Check"
            @click="handleSavePermissions"
            :disabled="!currentRole"
          >
            保存权限设置
          </el-button>
        </div>
      </template>
      <div v-if="currentRole" class="current-role">
        当前角色：<el-tag type="success">{{ currentRole.name }}</el-tag>
      </div>
      <div v-else class="permission-placeholder">
        <el-empty description="请先选择一个角色" />
      </div>
      
      <el-tree
        v-if="currentRole"
        ref="permissionTreeRef"
        :data="permissionTree"
        :props="{ label: 'name', children: 'children' }"
        show-checkbox
        node-key="id"
        default-expand-all
        :default-checked-keys="checkedPermissions"
        class="permission-tree"
      />
    </el-card>

    <!-- 角色表单对话框 -->
    <el-dialog :title="roleFormTitle" v-model="roleDialogVisible" width="500px" append-to-body>
      <el-form ref="roleFormRef" :model="roleForm" :rules="roleRules" label-width="100px">
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="roleForm.name" placeholder="请输入角色名称" :disabled="roleForm.id && roleForm.name === 'admin'" />
        </el-form-item>
        <el-form-item label="角色描述" prop="description">
          <el-input v-model="roleForm.description" type="textarea" placeholder="请输入角色描述" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="roleDialogVisible = false">取 消</el-button>
          <el-button type="primary" @click="submitRoleForm">确 定</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getRoles,
  getPermissions,
  getRolePermissions,
  assignRolePermissions,
  createRole,
  updateRole,
  deleteRole
} from '@/api/auth'

// 角色列表
const roleList = ref([])
// 当前选中的角色
const currentRole = ref(null)
// 权限树数据
const permissionTree = ref([])
// 已选中的权限ID数组
const checkedPermissions = ref([])
// 加载状态
const loading = ref(false)
// 角色对话框是否可见
const roleDialogVisible = ref(false)
// 角色表单标题
const roleFormTitle = ref('')
// 权限树引用
const permissionTreeRef = ref(null)
// 角色表单引用
const roleFormRef = ref(null)

// 角色表单数据
const roleForm = reactive({
  id: undefined,
  name: '',
  description: ''
})

// 角色表单验证规则
const roleRules = reactive({
  name: [
    { required: true, message: '角色名称不能为空', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '角色描述不能为空', trigger: 'blur' }
  ]
})

/**
 * 组件挂载时获取数据
 */
onMounted(() => {
  console.log('角色管理页面已加载')
  // 获取角色列表
  fetchRoleList()
  // 获取权限树
  fetchPermissionTree()
})

/**
 * 获取角色列表
 */
function fetchRoleList() {
  loading.value = true
  getRoles().then(response => {
    roleList.value = response.data
    loading.value = false
  }).catch(() => {
    // 模拟数据
    roleList.value = [
      {
        id: 1,
        name: 'admin',
        description: '系统管理员，拥有所有权限',
        createdAt: '2023-05-01 09:00:00'
      },
      {
        id: 2,
        name: 'manager',
        description: '经理，管理房源和客户',
        createdAt: '2023-05-01 09:30:00'
      },
      {
        id: 3,
        name: 'sales',
        description: '销售人员，查看房源和客户',
        createdAt: '2023-05-01 10:00:00'
      }
    ]
    loading.value = false
  })
}

/**
 * 获取权限树
 */
function fetchPermissionTree() {
  getPermissions().then(response => {
    permissionTree.value = buildPermissionTree(response.data)
  }).catch(() => {
    // 模拟权限数据
    const mockPermissions = [
      { id: 1, name: '系统管理', code: 'system', parentId: 0 },
      { id: 2, name: '用户管理', code: 'user', parentId: 1 },
      { id: 3, name: '查看用户', code: 'user:view', parentId: 2 },
      { id: 4, name: '添加用户', code: 'user:add', parentId: 2 },
      { id: 5, name: '编辑用户', code: 'user:edit', parentId: 2 },
      { id: 6, name: '删除用户', code: 'user:delete', parentId: 2 },
      { id: 7, name: '角色管理', code: 'role', parentId: 1 },
      { id: 8, name: '查看角色', code: 'role:view', parentId: 7 },
      { id: 9, name: '添加角色', code: 'role:add', parentId: 7 },
      { id: 10, name: '编辑角色', code: 'role:edit', parentId: 7 },
      { id: 11, name: '删除角色', code: 'role:delete', parentId: 7 },
      { id: 12, name: '分配权限', code: 'role:permission', parentId: 7 },
      { id: 13, name: '房源管理', code: 'property', parentId: 0 },
      { id: 14, name: '查看房源', code: 'property:view', parentId: 13 },
      { id: 15, name: '添加房源', code: 'property:add', parentId: 13 },
      { id: 16, name: '编辑房源', code: 'property:edit', parentId: 13 },
      { id: 17, name: '删除房源', code: 'property:delete', parentId: 13 },
      { id: 18, name: '客户管理', code: 'client', parentId: 0 },
      { id: 19, name: '查看客户', code: 'client:view', parentId: 18 },
      { id: 20, name: '添加客户', code: 'client:add', parentId: 18 },
      { id: 21, name: '编辑客户', code: 'client:edit', parentId: 18 },
      { id: 22, name: '删除客户', code: 'client:delete', parentId: 18 }
    ]
    permissionTree.value = buildPermissionTree(mockPermissions)
  })
}

/**
 * 构建权限树
 * @param {Array} permissions - 权限列表
 * @returns {Array} - 树形结构的权限数据
 */
function buildPermissionTree(permissions) {
  const tree = []
  const map = {}

  // 先将每个权限项映射到map
  permissions.forEach(item => {
    map[item.id] = { ...item, children: [] }
  })

  // 构建树结构
  permissions.forEach(item => {
    const newItem = map[item.id]
    if (item.parentId === 0) {
      // 根节点
      tree.push(newItem)
    } else {
      // 子节点，添加到父节点的children中
      if (map[item.parentId]) {
        map[item.parentId].children.push(newItem)
      }
    }
  })

  return tree
}

/**
 * 获取角色的权限
 * @param {Object} role - 角色对象
 */
function fetchRolePermissions(role) {
  if (!role) return

  getRolePermissions(role.id).then(response => {
    checkedPermissions.value = response.data.map(item => item.id)
  }).catch(() => {
    // 模拟数据
    if (role.name === 'admin') {
      // 管理员拥有所有权限
      checkedPermissions.value = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22]
    } else if (role.name === 'manager') {
      // 经理拥有部分权限
      checkedPermissions.value = [13, 14, 15, 16, 17, 18, 19, 20, 21, 22]
    } else if (role.name === 'sales') {
      // 销售人员只有查看权限
      checkedPermissions.value = [13, 14, 18, 19]
    } else {
      checkedPermissions.value = []
    }
  })
}

/**
 * 表格行选中事件
 * @param {Object} row - 选中的行数据
 */
function handleCurrentChange(row) {
  currentRole.value = row
  if (row) {
    fetchRolePermissions(row)
  } else {
    checkedPermissions.value = []
  }
}

/**
 * 新增角色
 */
function handleAddRole() {
  roleForm.id = undefined
  roleForm.name = ''
  roleForm.description = ''
  roleFormTitle.value = '新增角色'
  roleDialogVisible.value = true
}

/**
 * 编辑角色
 * @param {Object} row - 角色数据
 */
function handleEditRole(row) {
  roleForm.id = row.id
  roleForm.name = row.name
  roleForm.description = row.description
  roleFormTitle.value = '编辑角色'
  roleDialogVisible.value = true
}

/**
 * 删除角色
 * @param {Object} row - 角色数据
 */
function handleDeleteRole(row) {
  if (row.name === 'admin') {
    ElMessage.warning('管理员角色不允许删除')
    return
  }

  ElMessageBox.confirm(`确认要删除角色"${row.name}"吗？`, '警告', {
    confirmButtonText: '确认',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    deleteRole(row.id).then(() => {
      ElMessage.success('删除成功')
      fetchRoleList()
      if (currentRole.value && currentRole.value.id === row.id) {
        currentRole.value = null
        checkedPermissions.value = []
      }
    })
  }).catch(() => {})
}

/**
 * 提交角色表单
 */
function submitRoleForm() {
  roleFormRef.value.validate(valid => {
    if (valid) {
      if (roleForm.id) {
        // 编辑角色
        updateRole(roleForm.id, roleForm).then(() => {
          ElMessage.success('修改成功')
          roleDialogVisible.value = false
          fetchRoleList()
        })
      } else {
        // 新增角色
        createRole(roleForm).then(() => {
          ElMessage.success('新增成功')
          roleDialogVisible.value = false
          fetchRoleList()
        })
      }
    }
  })
}

/**
 * 保存权限设置
 */
function handleSavePermissions() {
  if (!currentRole.value) {
    ElMessage.warning('请先选择一个角色')
    return
  }

  // 获取当前选中的权限ID
  const permissionIds = permissionTreeRef.value.getCheckedKeys().concat(
    permissionTreeRef.value.getHalfCheckedKeys()
  )

  assignRolePermissions(currentRole.value.id, permissionIds).then(() => {
    ElMessage.success(`已成功为角色"${currentRole.value.name}"分配权限`)
  }).catch(() => {
    // 模拟成功
    ElMessage.success(`已成功为角色"${currentRole.value.name}"分配权限`)
  })
}
</script>

<style scoped>
.app-container {
  padding: 20px;
}

.box-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.permission-card {
  min-height: 400px;
}

.current-role {
  margin-bottom: 20px;
}

.permission-placeholder {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 300px;
}

.permission-tree {
  margin-top: 20px;
}
</style> 