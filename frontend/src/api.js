import axios from 'axios'
import router from './router'

const api = axios.create({ baseURL: '/api' })

export function getToken() {
  return localStorage.getItem('token')
}

export function setToken(token) {
  localStorage.setItem('token', token)
}

export function clearToken() {
  localStorage.removeItem('token')
}

// 请求拦截：自动带上 JWT
api.interceptors.request.use((config) => {
  const token = getToken()
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// 响应拦截：401 说明登录过期，踢回登录页
api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      clearToken()
      router.push('/login')
    }
    return Promise.reject(err)
  }
)

export default api
