# TaskApi · ASP.NET Core 8 + Vue 3 全栈任务管理 Demo

一个用于演示 **.NET 后端 + Vue 前端** 全栈能力的最小项目：JWT 登录/注册 + 任务的增删改查（CRUD）。

## 技术栈

| 层 | 技术 |
|---|---|
| 后端 | ASP.NET Core 8 Web API、EF Core（SQLite）、JWT Bearer 认证、BCrypt 密码加密、Swagger |
| 前端 | Vue 3（Composition API）、Vite、Vue Router、Axios |

## 功能

- **认证**：注册 / 登录，服务端用 BCrypt 存密码哈希，签发 JWT（7 天有效）
- **CRUD**：任务的新增、查询、编辑、删除、完成状态切换
- **权限隔离**：每个用户只能看到 / 操作自己创建的任务（按 JWT 里的 `NameIdentifier` claim 过滤）
- **前端路由守卫**：未登录访问受保护页面自动跳回登录页；401 自动登出
- **开发环境代理**：Vite 把 `/api` 转发到后端 5000 端口，前端无跨域问题

## 目录结构

```
backend/
├── Controllers/
│   ├── AuthController.cs      # 注册 / 登录 / 签发 JWT
│   └── TasksController.cs     # 任务 CRUD（[Authorize] 保护）
├── Data/AppDbContext.cs       # EF Core 上下文（SQLite）
├── Models/
│   ├── User.cs
│   └── TaskItem.cs
├── Program.cs                 # JWT / CORS / Swagger / 自动建库
└── appsettings.json

frontend/
├── src/
│   ├── api.js                 # axios 封装：自动带 JWT、401 拦截
│   ├── router.js              # 路由 + 登录守卫
│   ├── views/
│   │   ├── Login.vue          # 登录 / 注册
│   │   └── Tasks.vue          # 任务列表 CRUD
│   └── main.js
├── index.html
└── vite.config.js
```

## 启动方式

### 1. 启动后端（端口 5000）

```bash
cd backend
dotnet run --urls http://localhost:5000
```

首次启动会自动在 `backend/taskapi.db` 创建 SQLite 数据库，Swagger 文档在 `http://localhost:5000/swagger`。

### 2. 启动前端（端口 5173）

```bash
cd frontend
npm install
npm run dev
```

浏览器打开 `http://localhost:5173`，先注册一个账号，登录后即可进行任务的增删改查。

## 演示的后端要点

- RESTful 路由约定（`GET/POST/PUT/DELETE /api/tasks`）
- `[Authorize]` 属性 + JWT Bearer 中间件做接口保护
- EF Core LINQ 查询与 SQLite 持久化
- DTO 与实体分离、统一异常返回格式
- 每条数据按 `UserId` 做行级归属校验（越权访问返回 404）

## 演示的前端要点

- Vue 3 `<script setup>` 组合式 API
- axios 请求/响应拦截器（自动附带 token、401 自动登出）
- Vue Router 全局守卫做登录态控制
- 组件化的登录页与列表页
