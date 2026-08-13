import { QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter } from 'react-router-dom'
import type { ReactNode } from 'react'
import { queryClient } from '../lib/queryClient'
import { ChatRealtimeProvider } from '../features/chat/ChatRealtimeProvider'

/** Глобальные провайдеры приложения: маршрутизация + серверное состояние + realtime-чат. */
export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <ChatRealtimeProvider>{children}</ChatRealtimeProvider>
      </BrowserRouter>
    </QueryClientProvider>
  )
}
