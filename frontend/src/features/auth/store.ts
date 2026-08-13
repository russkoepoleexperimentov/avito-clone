import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { AuthResponse, User } from './types'

interface AuthState {
  user: User | null
  accessToken: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  setSession: (auth: AuthResponse) => void
  setTokens: (accessToken: string, refreshToken: string) => void
  clear: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
      setSession: (auth) =>
        set({
          user: auth.user,
          accessToken: auth.accessToken,
          refreshToken: auth.refreshToken,
          isAuthenticated: true,
        }),
      setTokens: (accessToken, refreshToken) => set({ accessToken, refreshToken }),
      clear: () =>
        set({ user: null, accessToken: null, refreshToken: null, isAuthenticated: false }),
    }),
    { name: 'resale-auth' },
  ),
)
