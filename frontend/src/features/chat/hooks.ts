import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { getConversations, getMessages, sendMessage, startConversation } from './api'
import type { ChatMessage } from './types'

export function useConversations() {
  return useQuery({ queryKey: ['conversations'], queryFn: getConversations })
}

export function useMessages(conversationId: string) {
  return useQuery({
    queryKey: ['messages', conversationId],
    queryFn: () => getMessages(conversationId),
    enabled: !!conversationId,
  })
}

export function useSendMessage(conversationId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (text: string) => sendMessage(conversationId, text),
    onSuccess: (message) => {
      // Добавляем своё сообщение в кэш (входящие прилетают через SignalR).
      qc.setQueryData<ChatMessage[]>(['messages', conversationId], (old) =>
        old ? [...old, message] : [message],
      )
      qc.invalidateQueries({ queryKey: ['conversations'] })
    },
  })
}

export function useStartConversation() {
  return useMutation({ mutationFn: startConversation })
}
