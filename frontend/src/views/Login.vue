<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api, { setToken } from '../api'

const router = useRouter()
const mode = ref('login') // login | register
const username = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  if (!username.value || !password.value) {
    error.value = '请输入用户名和密码'
    return
  }
  loading.value = true
  try {
    const { data } = await api.post(`/auth/${mode.value}`, {
      username: username.value,
      password: password.value
    })
    setToken(data.token)
    router.push('/tasks')
  } catch (e) {
    error.value = e.response?.data?.message || '请求失败，请检查后端是否启动'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-card">
    <h1>任务管理</h1>
    <p class="sub">ASP.NET Core Web API + Vue 3 · JWT 登录示例</p>

    <div class="tabs">
      <button :class="{ active: mode === 'login' }" @click="mode = 'login'">登录</button>
      <button :class="{ active: mode === 'register' }" @click="mode = 'register'">注册</button>
    </div>

    <input v-model="username" placeholder="用户名" @keyup.enter="submit" />
    <input v-model="password" type="password" placeholder="密码（注册至少 6 位）" @keyup.enter="submit" />

    <p v-if="error" class="error">{{ error }}</p>

    <button class="primary" :disabled="loading" @click="submit">
      {{ loading ? '请稍候…' : mode === 'login' ? '登录' : '注册并登录' }}
    </button>
  </div>
</template>
