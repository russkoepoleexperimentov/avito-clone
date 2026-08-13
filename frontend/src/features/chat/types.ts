export interface ChatMessage {
  id: string
  conversationId: string
  senderId: string
  text: string
  isRead: boolean
  createdAt: string
}

export interface Conversation {
  id: string
  listingId: string
  listingTitle: string
  listingImageUrl: string | null
  otherUserId: string
  otherUserName: string
  lastMessageText: string | null
  lastMessageAt: string
}
