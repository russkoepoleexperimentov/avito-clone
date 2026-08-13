import axios from 'axios'

/**
 * Единый экземпляр axios для общения с backend.
 * baseURL берётся из VITE_API_URL (по умолчанию /api, проксируется Vite в dev).
 */
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '/api',
  headers: { 'Content-Type': 'application/json' },
})

// Заготовка: подставляем access-токен в каждый запрос.
// TODO(этап 1): брать токен из хранилища auth (zustand) и обновлять по refresh.
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Заготовка обработки ошибок/401. Полноценный refresh-flow добавим на этапе аутентификации.
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // TODO(этап 1): при 401 пытаться обновить токен через /auth/refresh.
    return Promise.reject(error)
  },
)
