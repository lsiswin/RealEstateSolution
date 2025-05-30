<template>
  <div class="layout-container">
    <!-- 顶部系统信息栏 -->
    <el-header class="layout-header">
      <div class="header-content">
        <div class="header-left">
          <el-icon class="header-icon"><OfficeBuilding /></el-icon>
          <span class="system-title">房产中介管理系统</span>
        </div>
        <div class="header-right">
          <el-dropdown @command="handleCommand">
            <span class="user-info">
              <el-icon><User /></el-icon>
              <span>{{ userStore.userInfo?.realName || userStore.userInfo?.userName || '用户' }}</span>
              <el-icon class="el-icon--right"><arrow-down /></el-icon>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">个人中心</el-dropdown-item>
                <el-dropdown-item command="changePassword">修改密码</el-dropdown-item>
                <el-dropdown-item command="logout" divided>退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </div>
    </el-header>

    <!-- 主体内容区域 -->
    <el-container class="layout-main">
      <!-- 左侧导航栏 -->
      <el-aside class="layout-sidebar">
        <el-menu
          :default-active="$route.path"
          class="sidebar-menu"
          background-color="#001529"
          text-color="#fff"
          active-text-color="#1890ff"
          router
        >
          <template v-for="route in menuRoutes" :key="route.path">
            <!-- 有子菜单的情况 -->
            <el-sub-menu v-if="route.children && route.children.length > 0" :index="route.path">
              <template #title>
                <el-icon><component :is="route.meta?.icon" /></el-icon>
                <span>{{ route.meta?.title }}</span>
              </template>
              <el-menu-item
                v-for="child in route.children"
                :key="child.path"
                :index="route.path + '/' + child.path"
              >
                <el-icon><component :is="child.meta?.icon" /></el-icon>
                <span>{{ child.meta?.title }}</span>
              </el-menu-item>
            </el-sub-menu>
            <!-- 没有子菜单的情况 -->
            <el-menu-item v-else :index="route.path">
              <el-icon><component :is="route.meta?.icon" /></el-icon>
              <span>{{ route.meta?.title }}</span>
            </el-menu-item>
          </template>
        </el-menu>
      </el-aside>

      <!-- 右侧内容区域 -->
      <el-main class="layout-content">
        <router-view />
      </el-main>
    </el-container>

    <!-- 个人中心对话框 -->
    <el-dialog
      v-model="profileDialogVisible"
      title="个人中心"
      width="600px"
      :before-close="handleProfileClose"
    >
      <el-form
        ref="profileFormRef"
        :model="profileForm"
        :rules="profileRules"
        label-width="100px"
        v-loading="profileLoading"
      >
        <el-form-item label="用户名">
          <el-input v-model="profileForm.userName" disabled />
        </el-form-item>
        <el-form-item label="真实姓名" prop="realName">
          <el-input v-model="profileForm.realName" placeholder="请输入真实姓名" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="profileForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="电话" prop="phone">
          <el-input v-model="profileForm.phone" placeholder="请输入电话号码" />
        </el-form-item>
        <el-form-item label="角色">
          <el-tag v-for="role in profileForm.roles" :key="role" style="margin-right: 8px;">
            {{ role }}
          </el-tag>
        </el-form-item>
        <el-form-item label="注册时间">
          <span>{{ formatDate(profileForm.createTime) }}</span>
        </el-form-item>
        <el-form-item label="最后登录">
          <span>{{ profileForm.lastLoginTime ? formatDate(profileForm.lastLoginTime) : '暂无记录' }}</span>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="handleProfileClose">取消</el-button>
          <el-button type="primary" @click="handleProfileSave" :loading="profileLoading">
            保存
          </el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 修改密码对话框 -->
    <el-dialog
      v-model="passwordDialogVisible"
      title="修改密码"
      width="500px"
      :before-close="handlePasswordClose"
    >
      <el-form
        ref="passwordFormRef"
        :model="passwordForm"
        :rules="passwordRules"
        label-width="100px"
        v-loading="passwordLoading"
      >
        <el-form-item label="原密码" prop="oldPassword">
          <el-input
            v-model="passwordForm.oldPassword"
            type="password"
            placeholder="请输入原密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="新密码" prop="newPassword">
          <el-input
            v-model="passwordForm.newPassword"
            type="password"
            placeholder="请输入新密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input
            v-model="passwordForm.confirmPassword"
            type="password"
            placeholder="请再次输入新密码"
            show-password
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="handlePasswordClose">取消</el-button>
          <el-button type="primary" @click="handlePasswordSave" :loading="passwordLoading">
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import router from '@/router'
import { useUserStore } from '@/stores/user'
import { logout } from '@/api/user'
import { getCurrentUser, changePassword as changeUserPassword, updateProfile as updateUserProfile, type ChangePasswordRequest, type UpdateProfileRequest } from '@/api/user'
import {ArrowDown, OfficeBuilding, User} from "@element-plus/icons-vue"

const routerInstance = useRouter()
const userStore = useUserStore()

// 对话框显示状态
const profileDialogVisible = ref(false)
const passwordDialogVisible = ref(false)

// 加载状态
const profileLoading = ref(false)
const passwordLoading = ref(false)

// 表单引用
const profileFormRef = ref<FormInstance>()
const passwordFormRef = ref<FormInstance>()

// 个人信息表单
const profileForm = reactive({
  userName: '',
  realName: '',
  email: '',
  phone: '',
  roles: [] as string[],
  createTime: '',
  lastLoginTime: ''
})

// 修改密码表单
const passwordForm = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// 个人信息表单验证规则
const profileRules: FormRules = {
  realName: [
    { required: true, message: '请输入真实姓名', trigger: 'blur' },
    { min: 2, max: 20, message: '真实姓名长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱地址', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  phone: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ]
}

// 修改密码表单验证规则
const passwordRules: FormRules = {
  oldPassword: [
    { required: true, message: '请输入原密码', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请再次输入新密码', trigger: 'blur' },
    {
      validator: (_rule, value, callback) => {
        if (value !== passwordForm.newPassword) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 获取菜单路由
const menuRoutes = computed(() => {
  const routes = router.getRoutes()
  const mainRoute = routes.find(r => r.path === '/')
  let filteredRoutes = mainRoute?.children?.filter(child => child.meta?.title) || []
  
  // 基于角色过滤菜单
  const userRoles = userStore.userInfo?.roles || []
  console.log('角色列表',userRoles)
  const isAdmin = userRoles.includes('管理员') || userRoles.includes('admin')
  const isAgent = userRoles.includes('经纪人') || userRoles.includes('broker')
  
  // 过滤菜单项
  filteredRoutes = filteredRoutes.map(route => {
    //不显示隐藏的子菜单
    if (route.children) {
      route.children = route.children.filter(child => !child.meta?.hideInMenu)
    }
    
    // 如果是合同管理菜单
    if (route.path === '/contract') {
      // 只有管理员和经纪人可以访问合同管理
      if (!isAdmin && !isAgent) {
        return null
      }
    }
    
    // 如果是系统管理菜单
    if (route.path === '/system') {
      const systemChildren = route.children?.filter(child => {
        // 管理员可以看到所有系统管理功能
        if (isAdmin) {
          return true
        }
        // 经纪人只能看到用户管理，不能看到角色管理
        if (isAgent) {
          return child.path !== 'roles'
        }
        return false
      }) || []
      
      // 如果没有可显示的子菜单，则不显示该菜单
      if (systemChildren.length === 0) {
        return null
      }
      
      return {
        ...route,
        children: systemChildren
      }
    }
    
    // 其他菜单项的权限控制可以在这里添加
    return route
  }).filter(Boolean) as any[] // 过滤掉null值
  
  return filteredRoutes
})

// 格式化日期
const formatDate = (dateString: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString('zh-CN')
}

// 处理下拉菜单命令
const handleCommand = async (command: string) => {
  switch (command) {
    case 'profile':
      await showProfileDialog()
      break
    case 'changePassword':
      showPasswordDialog()
      break
    case 'logout':
      await handleLogout()
      break
  }
}

// 显示个人中心对话框
const showProfileDialog = async () => {
  profileLoading.value = true
  try {
    const userInfo = await getCurrentUser()
    
    // 填充表单数据
    profileForm.userName = userInfo.userName
    profileForm.realName = userInfo.realName || ''
    profileForm.email = userInfo.email
    profileForm.phone = userInfo.phone || ''
    profileForm.roles = userInfo.roles
    profileForm.createTime = userInfo.createTime
    profileForm.lastLoginTime = userInfo.lastLoginTime || ''
    
    profileDialogVisible.value = true
  } catch (error) {
    console.error('获取用户信息失败:', error)
    ElMessage.error('获取用户信息失败')
  } finally {
    profileLoading.value = false
  }
}

// 显示修改密码对话框
const showPasswordDialog = () => {
  // 重置表单
  passwordForm.oldPassword = ''
  passwordForm.newPassword = ''
  passwordForm.confirmPassword = ''
  passwordDialogVisible.value = true
}

// 处理个人信息保存
const handleProfileSave = async () => {
  if (!profileFormRef.value) return
  
  try {
    await profileFormRef.value.validate()
    
    profileLoading.value = true
    
    const updateData: UpdateProfileRequest = {
      userName: profileForm.userName,
      email: profileForm.email
    }
    
    await updateUserProfile(updateData)
    
    // 更新store中的用户信息
    if (userStore.userInfo) {
      userStore.userInfo.email = profileForm.email
      // 更新localStorage
      localStorage.setItem('userInfo', JSON.stringify(userStore.userInfo))
    }
    
    ElMessage.success('个人信息更新成功')
    profileDialogVisible.value = false
  } catch (error) {
    console.error('更新个人信息失败:', error)
    ElMessage.error('更新个人信息失败')
  } finally {
    profileLoading.value = false
  }
}

// 处理密码修改保存
const handlePasswordSave = async () => {
  if (!passwordFormRef.value) return
  
  try {
    await passwordFormRef.value.validate()
    
    passwordLoading.value = true
    
    const changePasswordData: ChangePasswordRequest = {
      currentPassword: passwordForm.oldPassword,
      newPassword: passwordForm.newPassword
    }
    
    await changeUserPassword(changePasswordData)
    
    ElMessage.success('密码修改成功，请重新登录')
    passwordDialogVisible.value = false
    
    // 延迟一秒后自动登出
    setTimeout(() => {
      handleLogout()
    }, 1000)
  } catch (error) {
    console.error('修改密码失败:', error)
    ElMessage.error('修改密码失败')
  } finally {
    passwordLoading.value = false
  }
}

// 处理个人信息对话框关闭
const handleProfileClose = () => {
  profileDialogVisible.value = false
  profileFormRef.value?.resetFields()
}

// 处理密码对话框关闭
const handlePasswordClose = () => {
  passwordDialogVisible.value = false
  passwordFormRef.value?.resetFields()
}

// 处理登出
const handleLogout = async () => {
  try {
    await ElMessageBox.confirm('确定要退出登录吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    // 调用登出API
    if (userStore.refreshToken) {
      try {
        await logout(userStore.refreshToken)
      } catch (error) {
        console.error('登出API调用失败:', error)
      }
    }
    
    // 清除用户信息
    userStore.clearUserInfo()
    
    // 跳转到登录页
    routerInstance.push('/login')
    
    ElMessage.success('已退出登录')
  } catch (error) {
    // 用户取消登出
  }
}
</script>

<style scoped>
.layout-container {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.layout-header {
  height: 60px;
  background: #fff;
  border-bottom: 1px solid #e4e7ed;
  box-shadow: 0 1px 4px rgba(0,21,41,.08);
  padding: 0 20px;
  display: flex;
  align-items: center;
}

.header-content {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left {
  display: flex;
  align-items: center;
}

.header-icon {
  font-size: 24px;
  color: #1890ff;
  margin-right: 12px;
}

.system-title {
  font-size: 20px;
  font-weight: bold;
  color: #333;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 4px;
  transition: background-color 0.3s;
}

.user-info:hover {
  background-color: #f5f5f5;
}

.user-info .el-icon {
  margin-right: 8px;
}

.layout-main {
  flex: 1;
  overflow: hidden;
}

.layout-sidebar {
  width: 200px;
  background: #001529;
  overflow-y: auto;
}

.sidebar-menu {
  border-right: none;
  height: 100%;
}

.sidebar-menu .el-menu-item,
.sidebar-menu .el-sub-menu__title {
  height: 50px;
  line-height: 50px;
}

.layout-content {
  background: #f0f2f5;
  overflow-y: auto;
  padding: 20px;
}
</style> 