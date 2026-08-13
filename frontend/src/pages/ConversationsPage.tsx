import { Link } from 'react-router-dom'
import { useConversations } from '../features/chat/hooks'

export function ConversationsPage() {
  const { data: conversations, isLoading, isError } = useConversations()

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="mb-4 text-2xl font-bold">Сообщения</h1>

      {isLoading ? (
        <p className="text-gray-500">Загрузка…</p>
      ) : isError ? (
        <p className="text-red-600">Не удалось загрузить диалоги.</p>
      ) : !conversations || conversations.length === 0 ? (
        <p className="text-gray-500">Диалогов пока нет.</p>
      ) : (
        <ul className="divide-y divide-gray-200 overflow-hidden rounded-lg border border-gray-200 bg-white">
          {conversations.map((c) => (
            <li key={c.id}>
              <Link to={`/chat/${c.id}`} className="flex items-center gap-3 p-4 hover:bg-gray-50">
                <div className="h-12 w-12 shrink-0 overflow-hidden rounded-md bg-gray-100">
                  {c.listingImageUrl ? (
                    <img src={c.listingImageUrl} alt="" className="h-full w-full object-cover" />
                  ) : (
                    <div className="flex h-full w-full items-center justify-center text-xs text-gray-300">
                      нет
                    </div>
                  )}
                </div>
                <div className="min-w-0 flex-1">
                  <div className="flex items-baseline justify-between gap-2">
                    <span className="truncate font-medium">{c.otherUserName}</span>
                    <span className="shrink-0 text-xs text-gray-400">{c.listingTitle}</span>
                  </div>
                  <p className="truncate text-sm text-gray-500">
                    {c.lastMessageText ?? 'Нет сообщений'}
                  </p>
                </div>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
