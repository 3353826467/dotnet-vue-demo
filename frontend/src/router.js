import { createRouter, createWebHistory } from 'vue-router'
import Login from './views/Login.vue'
import Tasks from './views/Tasks.vue'
import { getToken } from './api'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/tasks' },
    { path: '/login', component: Login },
    { path: '/tasks', component: Tasks, meta: { requiresAuth: true } }
  ]
})

// 未登录访问受保护页面 → 跳回登录页
router.beforeEach((to) => {
  if (to.meta.requiresAuth && !getToken()) return '/login'
})

export default router
