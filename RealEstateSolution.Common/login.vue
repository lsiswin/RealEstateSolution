<!-- 代码已包含 CSS：使用 TailwindCSS , 安装 TailwindCSS 后方可看到布局样式效果 -->

<template>
  <div class="min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4">
    <div class="max-w-[440px] w-full space-y-8 bg-white p-8 rounded-lg shadow-md">
      <!-- 顶部切换 -->
      <div class="flex justify-center space-x-8 mb-8">
        <button 
          v-for="role in roles" 
          :key="role.value"
          @click="currentRole = role.value"
          :class="[
            'pb-2 px-4 font-medium text-base transition-all relative',
            currentRole === role.value ? 'text-[#2A3F54]' : 'text-gray-400'
          ]"
        >
          {{ role.label }}
          <div 
            v-if="currentRole === role.value"
            class="absolute bottom-0 left-0 w-full h-0.5 bg-[#2A3F54]"
          ></div>
        </button>
      </div>

      <!-- 表单区域 -->
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <!-- 手机号 -->
        <div>
          <div class="relative">
            <input
              v-model="form.phone"
              type="text"
              class="w-full h-[44px] pl-[120px] pr-4 border rounded-lg focus:border-[#2A3F54] outline-none transition-colors"
              :class="{'border-[#FF6B6B]': errors.phone}"
              placeholder="请输入手机号"
            />
            <div class="absolute left-0 top-0 h-full flex items-center">
              <button 
                type="button"
                class="ml-4 flex items-center space-x-1 text-gray-600"
                @click="showAreaCodes = !showAreaCodes"
              >
                <span>+86</span>
                <el-icon class="text-sm"><ArrowDown /></el-icon>
              </button>
            </div>
          </div>
          <p v-if="errors.phone" class="mt-1 text-sm text-[#FF6B6B]">{{ errors.phone }}</p>
        </div>

        <!-- 密码 -->
        <div>
          <div class="relative">
            <input
              v-model="form.password"
              :type="showPassword ? 'text' : 'password'"
              class="w-full h-[44px] px-4 border rounded-lg focus:border-[#2A3F54] outline-none transition-colors"
              :class="{'border-[#FF6B6B]': errors.password}"
              placeholder="请输入密码"
            />
            <button 
              type="button"
              class="absolute right-4 top-1/2 -translate-y-1/2"
              @click="showPassword = !showPassword"
            >
              <el-icon v-if="showPassword"><View /></el-icon>
              <el-icon v-else><Hide /></el-icon>
            </button>
          </div>
          <p v-if="errors.password" class="mt-1 text-sm text-[#FF6B6B]">{{ errors.password }}</p>
        </div>

        <!-- 验证码 -->
        <div>
          <div class="flex space-x-4">
            <input
              v-model="form.code"
              type="text"
              class="flex-1 h-[44px] px-4 border rounded-lg focus:border-[#2A3F54] outline-none transition-colors"
              :class="{'border-[#FF6B6B]': errors.code}"
              placeholder="请输入验证码"
            />
            <button
              type="button"
              :disabled="counting"
              @click="sendCode"
              class="w-[120px] h-[44px] text-[#2A3F54] border border-[#2A3F54] rounded-lg hover:bg-gray-50 !rounded-button whitespace-nowrap"
            >
              {{ counting ? `${counter}s后重试` : '获取验证码' }}
            </button>
          </div>
          <p v-if="errors.code" class="mt-1 text-sm text-[#FF6B6B]">{{ errors.code }}</p>
        </div>

        <!-- 协议 -->
        <div class="flex items-start">
          <el-checkbox v-model="form.agreement" :class="{'is-error': errors.agreement}">
            <span class="text-sm text-gray-600">
              我已阅读并同意
              <a href="#" class="text-[#2A3F54]">《用户服务协议》</a>
              和
              <a href="#" class="text-[#2A3F54]">《隐私政策》</a>
            </span>
          </el-checkbox>
        </div>

        <!-- 提交按钮 -->
        <button
          type="submit"
          class="w-full h-[44px] bg-[#2A3F54] text-white rounded-lg hover:bg-opacity-90 transition-colors !rounded-button whitespace-nowrap"
        >
          {{ isLogin ? '登录' : '注册' }}
        </button>

        <!-- 切换登录/注册 -->
        <div class="text-center">
          <button
            type="button"
            @click="isLogin = !isLogin"
            class="text-sm text-[#2A3F54]"
          >
            {{ isLogin ? '没有账号？立即注册' : '已有账号？立即登录' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { View, Hide, ArrowDown } from '@element-plus/icons-vue';

const roles = [
  { label: '经纪人', value: 'agent' },
  { label: '客户', value: 'client' }
];

const currentRole = ref('agent');
const isLogin = ref(true);
const showPassword = ref(false);
const showAreaCodes = ref(false);
const counting = ref(false);
const counter = ref(60);

const form = reactive({
  phone: '',
  password: '',
  code: '',
  agreement: false
});

const errors = reactive({
  phone: '',
  password: '',
  code: '',
  agreement: false
});

const validateForm = () => {
  let isValid = true;
  errors.phone = '';
  errors.password = '';
  errors.code = '';
  errors.agreement = false;

  if (!form.phone) {
    errors.phone = '请输入手机号';
    isValid = false;
  } else if (!/^1[3-9]\d{9}$/.test(form.phone)) {
    errors.phone = '请输入正确的手机号';
    isValid = false;
  }

  if (!form.password) {
    errors.password = '请输入密码';
    isValid = false;
  } else if (form.password.length < 6) {
    errors.password = '密码长度不能少于6位';
    isValid = false;
  }

  if (!form.code) {
    errors.code = '请输入验证码';
    isValid = false;
  } else if (form.code.length !== 6) {
    errors.code = '请输入6位验证码';
    isValid = false;
  }

  if (!form.agreement) {
    errors.agreement = true;
    isValid = false;
  }

  return isValid;
};

const sendCode = () => {
  if (!form.phone) {
    errors.phone = '请先输入手机号';
    return;
  }
  if (!/^1[3-9]\d{9}$/.test(form.phone)) {
    errors.phone = '请输入正确的手机号';
    return;
  }
  
  counting.value = true;
  counter.value = 60;
  
  const timer = setInterval(() => {
    counter.value--;
    if (counter.value <= 0) {
      clearInterval(timer);
      counting.value = false;
    }
  }, 1000);

  ElMessage.success('验证码已发送');
};

const handleSubmit = () => {
  if (!validateForm()) return;
  
  ElMessage.success(isLogin.value ? '登录成功' : '注册成功');
};
</script>

<style scoped>
.el-checkbox.is-error :deep(.el-checkbox__input .el-checkbox__inner) {
  border-color: #FF6B6B;
}
</style>

