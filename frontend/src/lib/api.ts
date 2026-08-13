import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '../features/auth/store'
import type { AuthResponse } from '../features/auth/types'

const baseURL = import.meta.env.VITE_API_URL ?? '/api'

/** Основной инстанс с перехватчиками (токен + авто-refresh). */
export const api = axios.create({
  baseURL,
  headers: { 'Content-Type': 'application/json' },
})

/** Отдельный «чистый» инстанс для refresh — без перехватчиков, чтобы не зациклиться. */
const plain = axios.create({ baseURL, headers: { 'Content-Type': 'application/json' } })

// Подставляем access-токен в каждый запрос.
api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Единый in-flight refresh, чтобы параллельные 401 не запускали несколько обновлений.
let refreshing: Promise<string | null> | null = null

async function refreshTokens(): Promise<string | null> {
  const { refreshToken, setTokens, clear } = useAuthStore.getState()
  if (!refreshToken) return null

  try {
    const res = await plain.post<AuthResponse>('/auth/refresh', { refreshToken })
    setTokens(res.data.accessToken, res.data.refreshToken)
    return res.data.accessToken
  } catch {
    clear()
    return null
  }
}

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as (InternalAxiosRequestConfig & { _retry?: boolean }) | undefined
    const isAuthEndpoint = original?.url?.includes('/auth/')

    if (error.response?.status === 401 && original && !original._retry && !isAuthEndpoint) {
      original._retry = true

      refreshing ??= refreshTokens().finally(() => {
        refreshing = null
      })
      const newToken = await refreshing

      if (newToken) {
        original.headers.Authorization = `Bearer ${newToken}`
        return api(original)
      }
    }

    return Promise.reject(error)
  },
)
