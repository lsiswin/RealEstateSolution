<template>
  <div class="property-add">
    <div class="page-header">
      <h2>添加房源</h2>
      <p>录入新的房源信息</p>
    </div>
    
    <el-card>
      <el-form :model="form" label-width="120px">
        <el-form-item label="房源标题">
          <el-input v-model="form.title" placeholder="请输入房源标题" />
        </el-form-item>
        <el-form-item label="房源类型">
          <el-select v-model="form.type" placeholder="请选择房源类型">
            <el-option label="住宅" value="住宅" />
            <el-option label="商业" value="商业" />
            <el-option label="别墅" value="别墅" />
          </el-select>
        </el-form-item>
        <el-form-item label="面积(㎡)">
          <el-input-number v-model="form.area" :min="1" />
        </el-form-item>
        <el-form-item label="价格(万)">
          <el-input-number v-model="form.price" :min="1" :precision="2" />
        </el-form-item>
        <el-form-item label="位置">
          <el-input v-model="form.location" placeholder="请输入位置" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="submitForm">提交</el-button>
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const form = ref({
  title: '',
  type: '',
  area: 0,
  price: 0,
  location: ''
})

const submitForm = () => {
  console.log('提交表单:', form.value)
}

const resetForm = () => {
  form.value = {
    title: '',
    type: '',
    area: 0,
    price: 0,
    location: ''
  }
}
</script>

<style scoped>
.property-add {
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
</style> 