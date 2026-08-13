import { useMutation } from '@tanstack/react-query'
import { login as loginApi, register as registerApi } from './api'
import { useAuthStore } from './store'
import type { AuthResponse, LoginRequest, RegisterRequest } from './types'

function useAuthMutation<TVars>(fn: (vars: TVars) => Promise<AuthResponse>) {
  const setSession = useAuthStore((s) => s.setSession)
  return useMutation({
    mutationFn: fn,
    onSuccess: (data) => setSession(data),
  })
}

export function useLogin() {
  return useAuthMutation<LoginRequest>(loginApi)
}

export function useRegister() {
  return useAuthMutation<RegisterRequest>(registerApi)
}

/** Текущий пользователь и статус аутентификации из стора. */
export function useAuth() {
  const user = useAuthStore((s) => s.user)
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)
  const clear = useAuthStore((s) => s.clear)
  return { user, isAuthenticated, logout: clear }
}
