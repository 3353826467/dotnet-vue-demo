import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    proxy: {
      // 开发环境把 /api 转发到后端 5000 端口，避免跨域
      '/api': 'http://localhost:5000'
    }
  }
})
