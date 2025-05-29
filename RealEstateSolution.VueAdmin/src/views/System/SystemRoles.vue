<template>
  <div class="system-roles">
    <div class="page-header">
      <h2>角色管理</h2>
      <p>管理系统角色和权限分配</p>
    </div>
    
    <!-- 操作区域 -->
    <el-card class="action-card">
      <el-button type="success" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        新增角色
      </el-button>
      <el-button 
        type="danger" 
        @click="handleBatchDelete"
        :disabled="selectedRoles.length === 0"
      >
        <el-icon><Delete /></el-icon>
        批量删除
      </el-button>
    </el-card>

    <!-- 角色列表 -->
    <el-card class="table-card">
      <el-table
        :data="roleList"
        v-loading="loading"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="id" label="角色ID" width="80" />
        <el-table-column prop="name" label="角色名称" width="150" />
        <el-table-column prop="description" label="角色描述" min-width="200" show-overflow-tooltip />
        <el-table-column prop="permissions" label="权限数量" width="120">
          <template #default="scope">
            <el-tag type="info">{{ scope.row.permissions?.length || 0 }} 个权限</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
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
              type="info"
              size="small"
              @click="handlePermissions(scope.row)"
            >
              权限
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(scope.row)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 角色详情/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="500px"
      @close="handleDialogClose"
    >
      <el-form
        ref="roleFormRef"
        :model="roleForm"
        :rules="roleRules"
        label-width="100px"
      >
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="roleForm.name" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item label="角色描述">
          <el-input
            v-model="roleForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入角色描述"
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

    <!-- 权限分配对话框 -->
    <el-dialog
      v-model="permissionDialogVisible"
      title="权限分配"
      width="600px"
      @close="handlePermissionDialogClose"
    >
      <div class="permission-content">
        <div class="role-info">
          <h4>角色：{{ currentRole?.name }}</h4>
          <p>{{ currentRole?.description || '暂无描述' }}</p>
        </div>
        
        <el-divider />
        
        <div class="permission-tree">
          <el-tree
            ref="permissionTreeRef"
            :data="permissionTreeData"
            :props="treeProps"
            show-checkbox
            node-key="id"
            :default-checked-keys="checkedPermissions"
            @check="handlePermissionCheck"
          />
        </div>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="permissionDialogVisible = false">取消</el-button>
          <el-button
            type="primary"
            @click="handlePermissionSubmit"
            :loading="permissionLoading"
          >
            保存
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Plus, Delete } from '@element-plus/icons-vue'
import {
  getRoles,
  createRole,
  updateRole,
  deleteRole,
  getPermissions,
  getRolePermissions,
  assignRolePermissions,
  type Role,
  type Permission,
  type CreateRoleRequest,
  type UpdateRoleRequest,
  type AssignPermissionsRequest
} from '@/api/user'

// 响应式数据
const loading = ref(false)
const submitLoading = ref(false)
const permissionLoading = ref(false)
const roleList = ref<Role[]>([])
const permissionList = ref<Permission[]>([])
const selectedRoles = ref<Role[]>([])

// 对话框相关
const dialogVisible = ref(false)
const permissionDialogVisible = ref(false)
const dialogTitle = ref('')
const isViewMode = ref(false)
const roleFormRef = ref<FormInstance>()
const permissionTreeRef = ref()

// 当前操作的角色
const currentRole = ref<Role | null>(null)

// 角色表单
const roleForm = reactive<Partial<CreateRoleRequest & UpdateRoleRequest & { id?: string }>>({
  name: '',
  description: ''
})

// 权限相关
const checkedPermissions = ref<string[]>([])
const permissionTreeData = ref<any[]>([])

// 树形控件属性
const treeProps = {
  children: 'children',
  label: 'name'
}

// 表单验证规则
const roleRules: FormRules = {
  name: [
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 20, message: '角色名称长度在 2 到 20 个字符', trigger: 'blur' }
  ]
}

// 获取角色列表
const fetchRoleList = async () => {
  loading.value = true
  try {
    const roles = await getRoles()
    roleList.value = roles
  } catch (error) {
    console.error('获取角色列表失败:', error)
    ElMessage.error('获取角色列表失败')
  } finally {
    loading.value = false
  }
}

// 获取权限列表
const fetchPermissionList = async () => {
  try {
    const permissions = await getPermissions()
    permissionList.value = permissions
    buildPermissionTree()
  } catch (error) {
    console.error('获取权限列表失败:', error)
    ElMessage.error('获取权限列表失败')
  }
}

// 构建权限树
const buildPermissionTree = () => {
  const moduleMap = new Map<string, any>()
  
  permissionList.value.forEach(permission => {
    if (!moduleMap.has(permission.module)) {
      moduleMap.set(permission.module, {
        id: permission.module,
        name: permission.module,
        children: []
      })
    }
    
    moduleMap.get(permission.module).children.push({
      id: permission.id,
      name: permission.name,
      description: permission.description
    })
  })
  
  permissionTreeData.value = Array.from(moduleMap.values())
}

// 新增角色
const handleAdd = () => {
  dialogTitle.value = '新增角色'
  isViewMode.value = false
  resetRoleForm()
  dialogVisible.value = true
}

// 查看角色
const handleView = (role: Role) => {
  dialogTitle.value = '角色详情'
  isViewMode.value = true
  Object.assign(roleForm, role)
  dialogVisible.value = true
}

// 编辑角色
const handleEdit = (role: Role) => {
  dialogTitle.value = '编辑角色'
  isViewMode.value = false
  Object.assign(roleForm, role)
  dialogVisible.value = true
}

// 权限管理
const handlePermissions = async (role: Role) => {
  currentRole.value = role
  
  try {
    const rolePermissions = await getRolePermissions(role.id)
    checkedPermissions.value = rolePermissions.map(p => p.id)
    permissionDialogVisible.value = true
  } catch (error) {
    console.error('获取角色权限失败:', error)
    ElMessage.error('获取角色权限失败')
  }
}

// 删除角色
const handleDelete = async (role: Role) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除角色 "${role.name}" 吗？此操作不可恢复！`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await deleteRole(role.id)
    ElMessage.success('删除成功')
    fetchRoleList()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除角色失败:', error)
      ElMessage.error('删除失败')
    }
  }
}

// 批量删除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(
      `确定要删除选中的 ${selectedRoles.value.length} 个角色吗？此操作不可恢复！`,
      '批量删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const deletePromises = selectedRoles.value.map(role => deleteRole(role.id))
    await Promise.all(deletePromises)
    
    ElMessage.success('批量删除成功')
    fetchRoleList()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('批量删除失败:', error)
      ElMessage.error('批量删除失败')
    }
  }
}

// 表格选择变化
const handleSelectionChange = (selection: Role[]) => {
  selectedRoles.value = selection
}

// 提交角色表单
const handleSubmit = async () => {
  if (!roleFormRef.value) return
  
  try {
    await roleFormRef.value.validate()
    submitLoading.value = true
    
    if (roleForm.id) {
      // 编辑
      const updateData: UpdateRoleRequest = {
        name: roleForm.name,
        description: roleForm.description
      }
      await updateRole(roleForm.id, updateData)
      ElMessage.success('更新成功')
    } else {
      // 新增
      const createData: CreateRoleRequest = {
        name: roleForm.name!,
        description: roleForm.description
      }
      await createRole(createData)
      ElMessage.success('创建成功')
    }
    
    dialogVisible.value = false
    fetchRoleList()
  } catch (error) {
    console.error('提交失败:', error)
    ElMessage.error('操作失败')
  } finally {
    submitLoading.value = false
  }
}

// 权限选择变化
const handlePermissionCheck = (data: any, checked: any) => {
  // 处理权限选择逻辑
}

// 提交权限分配
const handlePermissionSubmit = async () => {
  if (!currentRole.value) return
  
  try {
    permissionLoading.value = true
    
    const checkedNodes = permissionTreeRef.value.getCheckedNodes()
    const permissionIds = checkedNodes
      .filter((node: any) => !node.children) // 只获取叶子节点
      .map((node: any) => node.id)
    
    const assignData: AssignPermissionsRequest = {
      permissionIds
    }
    
    await assignRolePermissions(currentRole.value.id, assignData)
    ElMessage.success('权限分配成功')
    permissionDialogVisible.value = false
    fetchRoleList()
  } catch (error) {
    console.error('权限分配失败:', error)
    ElMessage.error('权限分配失败')
  } finally {
    permissionLoading.value = false
  }
}

// 对话框关闭
const handleDialogClose = () => {
  roleFormRef.value?.resetFields()
  resetRoleForm()
}

// 权限对话框关闭
const handlePermissionDialogClose = () => {
  currentRole.value = null
  checkedPermissions.value = []
}

// 重置角色表单
const resetRoleForm = () => {
  Object.assign(roleForm, {
    id: undefined,
    name: '',
    description: ''
  })
}

// 初始化
onMounted(() => {
  fetchRoleList()
  fetchPermissionList()
})
</script>

<style scoped>
.system-roles {
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

.action-card {
  margin-bottom: 16px;
}

.table-card {
  margin-bottom: 16px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.permission-content {
  max-height: 500px;
  overflow-y: auto;
}

.role-info h4 {
  margin: 0 0 8px 0;
  color: #333;
}

.role-info p {
  margin: 0;
  color: #666;
  font-size: 14px;
}

.permission-tree {
  margin-top: 16px;
}
</style> 