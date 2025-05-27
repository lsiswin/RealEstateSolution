<template>
  <div class="login-container">
    <el-card class="login-card">
      <div class="login-header">
        <h2>房产管理系统</h2>
        <p>欢迎登录</p>
      </div>
      
      <el-form
        ref="loginFormRef"
        :model="loginForm"
        :rules="loginRules"
        class="login-form"
        autocomplete="on"
        label-position="top"
      >
        <el-form-item label="用户名" prop="username">
          <el-input
            v-model="loginForm.username"
            placeholder="请输入用户名"
            type="text"
            autocomplete="on"
          >
            <template #prefix>
              <el-icon><User /></el-icon>
            </template>
          </el-input>
        </el-form-item>

        <el-form-item label="密码" prop="password">
          <el-input
            v-model="loginForm.password"
            placeholder="请输入密码"
            :type="passwordVisible ? 'text' : 'password'"
            autocomplete="on"
          >
            <template #prefix>
              <el-icon><Lock /></el-icon>
            </template>
            <template #suffix>
              <el-icon @click="togglePasswordVisible">
                <View v-if="passwordVisible" />
                <Hide v-else />
              </el-icon>
            </template>
          </el-input>
        </el-form-item>

        <el-form-item>
          <el-button 
            :loading="loading" 
            type="primary" 
            style="width: 100%" 
            @click="handleLogin"
          >
            登录
          </el-button>
        </el-form-item>
        
        <div class="login-tips">
          <p>默认账号: admin</p>
          <p>默认密码: 123456</p>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useStore } from 'vuex'
import { useRouter, useRoute } from 'vue-router'
import { User, Lock, View, Hide } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

const store = useStore()
const router = useRouter()
const route = useRoute()

const loginFormRef = ref(null)
const loading = ref(false)
const passwordVisible = ref(false)

const loginForm = reactive({
  username: 'admin',
  password: '123456'
})

const loginRules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
}

const togglePasswordVisible = () => {
  passwordVisible.value = !passwordVisible.value
}

const handleLogin = async () => {
  try {
    await loginFormRef.value.validate()
    
    loading.value = true
    
    try {
      // 登录
      await store.dispatch('user/login', loginForm)
      
      // 获取用户角色
      const roles = store.state.user.roles
      
      // 根据角色生成可访问路由
      await store.dispatch('permission/generateRoutes', roles)
      
      // 登录成功，根据重定向参数决定跳转路径
      const redirectPath = route.query.redirect || '/'
      router.push(redirectPath)
      
      ElMessage.success('登录成功')
    } catch (error) {
      console.error('登录失败:', error)
      ElMessage.error(error.message || '登录失败，请检查用户名和密码')
    } finally {
      loading.value = false
    }
  } catch (error) {
    console.error('表单验证失败:', error)
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f0f2f5;
}

.login-card {
  width: 400px;
  padding: 20px;
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h2 {
  margin: 0;
  font-size: 24px;
  color: #303133;
}

.login-header p {
  margin: 10px 0 0 0;
  font-size: 16px;
  color: #606266;
}

.login-form {
  width: 100%;
}

.login-tips {
  margin-top: 20px;
  text-align: center;
  color: #909399;
  font-size: 14px;
}

.login-tips p {
  margin: 5px 0;
}
</style> 