import { useEffect, type ReactNode } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useAuthStore } from '../auth/store'
import type { ChatMessage } from './types'

/**
 * Держит SignalR-соединение с хабом чата, пока пользователь авторизован.
 * Входящие сообщения кладёт в кэш TanStack Query, чтобы открытый диалог
 * и список диалогов обновлялись в реальном времени.
 */
export function ChatRealtimeProvider({ children }: { children: ReactNode }) {
  const qc = useQueryClient()
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated)

  useEffect(() => {
    if (!isAuthenticated) return

    const connection = new HubConnectionBuilder()
      .withUrl('/hubs/chat', {
        accessTokenFactory: () => useAuthStore.getState().accessToken ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('ReceiveMessage', (message: ChatMessage) => {
      qc.setQueryData<ChatMessage[]>(['messages', message.conversationId], (old) =>
        old ? [...old, message] : old,
      )
      qc.invalidateQueries({ queryKey: ['conversations'] })
    })

    connection.start().catch(() => {
      /* при неудаче автоподключение попробует снова */
    })

    return () => {
      connection.stop()
    }
  }, [isAuthenticated, qc])

  return <>{children}</>
}
