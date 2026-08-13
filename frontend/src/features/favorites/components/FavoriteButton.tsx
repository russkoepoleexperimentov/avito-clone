import type { MouseEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../auth/hooks'
import { useToggleFavorite } from '../hooks'

interface Props {
  listingId: string
  isFavorite: boolean
  /** Вариант отображения: иконка поверх карточки или кнопка с подписью. */
  variant?: 'icon' | 'button'
}

export function FavoriteButton({ listingId, isFavorite, variant = 'icon' }: Props) {
  const { isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const toggle = useToggleFavorite()

  const handleClick = (e: MouseEvent) => {
    e.preventDefault()
    e.stopPropagation()
    if (!isAuthenticated) {
      navigate('/login')
      return
    }
    toggle.mutate({ listingId, isFavorite })
  }

  const heart = (
    <svg
      viewBox="0 0 24 24"
      className="h-5 w-5"
      fill={isFavorite ? 'currentColor' : 'none'}
      stroke="currentColor"
      strokeWidth={2}
    >
      <path d="M12 21s-6.7-4.3-9.3-8.2C1 10.3 1.7 6.8 4.8 5.6 7 4.8 9.2 5.8 12 8.4c2.8-2.6 5-3.6 7.2-2.8 3.1 1.2 3.8 4.7 2.1 7.2C18.7 16.7 12 21 12 21z" />
    </svg>
  )

  if (variant === 'button') {
    return (
      <button
        onClick={handleClick}
        disabled={toggle.isPending}
        className={`flex items-center gap-2 rounded-md border px-4 py-2 text-sm font-medium transition-colors disabled:opacity-60 ${
          isFavorite
            ? 'border-red-200 bg-red-50 text-red-600'
            : 'border-gray-300 text-gray-700 hover:bg-gray-50'
        }`}
      >
        {heart}
        {isFavorite ? 'В избранном' : 'В избранное'}
      </button>
    )
  }

  return (
    <button
      onClick={handleClick}
      disabled={toggle.isPending}
      aria-label={isFavorite ? 'Убрать из избранного' : 'В избранное'}
      className={`rounded-full bg-white/90 p-1.5 shadow-sm transition-colors disabled:opacity-60 ${
        isFavorite ? 'text-red-500' : 'text-gray-400 hover:text-red-500'
      }`}
    >
      {heart}
    </button>
  )
}
