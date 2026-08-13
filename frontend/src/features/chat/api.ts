import { api } from '../../lib/api'
import type { ChatMessage, Conversation } from './types'

export async function startConversation(listingId: string): Promise<string> {
  const res = await api.post<{ conversationId: string }>('/chat/conversations', { listingId })
  return res.data.conversationId
}

export async function getConversations(): Promise<Conversation[]> {
  const res = await api.get<Conversation[]>('/chat/conversations')
  return res.data
}

export async function getMessages(conversationId: string): Promise<ChatMessage[]> {
  const res = await api.get<ChatMessage[]>(`/chat/conversations/${conversationId}/messages`)
  return res.data
}

export async function sendMessage(conversationId: string, text: string): Promise<ChatMessage> {
  const res = await api.post<ChatMessage>(`/chat/conversations/${conversationId}/messages`, { text })
  return res.data
}
