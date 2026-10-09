<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import api, { clearToken } from '../api'

const router = useRouter()
const tasks = ref([])
const title = ref('')
const desc = ref('')
const error = ref('')
const editingId = ref(null)
const editingTitle = ref('')

onMounted(load)

async function load() {
  try {
    const { data } = await api.get('/tasks')
    tasks.value = data
  } catch (e) {
    error.value = '加载失败：' + (e.response?.data?.message || e.message)
  }
}

async function add() {
  if (!title.value.trim()) return
  await api.post('/tasks', { title: title.value, description: desc.value })
  title.value = ''
  desc.value = ''
  await load()
}

async function toggle(task) {
  await api.put(`/tasks/${task.id}`, { isDone: !task.isDone })
  await load()
}

function startEdit(task) {
  editingId.value = task.id
  editingTitle.value = task.title
}

async function saveEdit(task) {
  const t = editingTitle.value.trim()
  if (t) await api.put(`/tasks/${task.id}`, { title: t })
  editingId.value = null
  await load()
}

async function remove(id) {
  await api.delete(`/tasks/${id}`)
  await load()
}

function logout() {
  clearToken()
  router.push('/login')
}
</script>

<template>
  <div class="task-page">
    <header>
      <h2>我的任务 <span class="count">{{ tasks.length }}</span></h2>
      <button class="ghost" @click="logout">退出登录</button>
    </header>

    <div class="add-row">
      <input v-model="title" placeholder="新任务标题" @keyup.enter="add" />
      <input v-model="desc" placeholder="描述（可选）" @keyup.enter="add" />
      <button class="primary" @click="add">添加</button>
    </div>

    <p v-if="error" class="error">{{ error }}</p>

    <ul class="list">
      <li v-for="t in tasks" :key="t.id" :class="{ done: t.isDone }">
        <input type="checkbox" :checked="t.isDone" @change="toggle(t)" />

        <template v-if="editingId === t.id">
          <input class="edit" v-model="editingTitle" @keyup.enter="saveEdit(t)" />
          <button class="primary sm" @click="saveEdit(t)">保存</button>
        </template>
        <template v-else>
          <div class="body">
            <span class="title">{{ t.title }}</span>
            <span v-if="t.description" class="desc">{{ t.description }}</span>
          </div>
          <button class="ghost sm" @click="startEdit(t)">编辑</button>
        </template>

        <button class="danger sm" @click="remove(t.id)">删除</button>
      </li>
      <li v-if="!tasks.length" class="empty">暂无任务，先添加一条吧</li>
    </ul>
  </div>
</template>
