import { useEffect, useRef, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useMessages, useSendMessage } from '../features/chat/hooks'
import { useAuth } from '../features/auth/hooks'

export function ConversationPage() {
  const { id } = useParams<{ id: string }>()
  const { user } = useAuth()
  const { data: messages, isLoading } = useMessages(id!)
  const send = useSendMessage(id!)
  const [text, setText] = useState('')
  const bottomRef = useRef<HTMLDivElement>(null)

  // Автоскролл вниз при появлении новых сообщений.
  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages])

  const handleSend = () => {
    const value = text.trim()
    if (!value) return
    send.mutate(value)
    setText('')
  }

  return (
    <div className="mx-auto flex h-[70vh] max-w-2xl flex-col rounded-lg border border-gray-200 bg-white">
      <div className="border-b border-gray-200 p-3">
        <Link to="/chat" className="text-sm text-brand-600 hover:underline">
          ← К списку диалогов
        </Link>
      </div>

      <div className="flex-1 space-y-2 overflow-y-auto p-4">
        {isLoading ? (
          <p className="text-gray-500">Загрузка…</p>
        ) : !messages || messages.length === 0 ? (
          <p className="text-center text-sm text-gray-400">
            Сообщений пока нет. Напишите первым!
          </p>
        ) : (
          messages.map((m) => {
            const isMine = m.senderId === user?.id
            return (
              <div key={m.id} className={`flex ${isMine ? 'justify-end' : 'justify-start'}`}>
                <div
                  className={`max-w-[75%] rounded-2xl px-3 py-2 text-sm ${
                    isMine ? 'bg-brand-600 text-white' : 'bg-gray-100 text-gray-900'
                  }`}
                >
                  <p className="whitespace-pre-wrap break-words">{m.text}</p>
                  <p className={`mt-1 text-[10px] ${isMine ? 'text-brand-100' : 'text-gray-400'}`}>
                    {new Date(m.createdAt).toLocaleTimeString('ru-RU', {
                      hour: '2-digit',
                      minute: '2-digit',
                    })}
                  </p>
                </div>
              </div>
            )
          })
        )}
        <div ref={bottomRef} />
      </div>

      <div className="flex gap-2 border-t border-gray-200 p-3">
        <input
          className="flex-1 rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500"
          placeholder="Сообщение…"
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
              e.preventDefault()
              handleSend()
            }
          }}
        />
        <button
          onClick={handleSend}
          disabled={send.isPending || !text.trim()}
          className="rounded-md bg-brand-600 px-4 py-2 font-medium text-white hover:bg-brand-700 disabled:opacity-50"
        >
          Отправить
        </button>
      </div>
    </div>
  )
}
