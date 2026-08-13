import { useParams, useNavigate, Link } from 'react-router-dom'
import { useListing, useDeleteListing } from '../features/listings/hooks'
import { conditionLabels, statusLabels } from '../features/listings/types'
import { useAuth } from '../features/auth/hooks'
import { formatPrice, formatDate } from '../lib/format'

export function ListingDetailsPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { user } = useAuth()
  const { data: listing, isLoading, isError } = useListing(id!)
  const del = useDeleteListing()

  if (isLoading) return <p className="text-gray-500">Загрузка…</p>
  if (isError || !listing) return <p className="text-gray-500">Объявление не найдено.</p>

  const isOwner = user?.id === listing.userId

  const handleDelete = () => {
    if (confirm('Удалить объявление?')) {
      del.mutate(listing.id, { onSuccess: () => navigate('/my-listings') })
    }
  }

  return (
    <div className="mx-auto max-w-3xl">
      <Link to="/" className="text-sm text-brand-600 hover:underline">
        ← К каталогу
      </Link>

      <div className="mt-3 grid grid-cols-1 gap-6 md:grid-cols-2">
        {/* Фото (появятся на следующем шаге) */}
        <div className="flex aspect-square items-center justify-center rounded-lg border border-gray-200 bg-gray-100 text-gray-400">
          {listing.imageUrls.length > 0 ? (
            <img
              src={listing.imageUrls[0]}
              alt={listing.title}
              className="h-full w-full rounded-lg object-cover"
            />
          ) : (
            'Нет фото'
          )}
        </div>

        <div>
          <h1 className="text-2xl font-bold">{listing.title}</h1>
          <p className="mt-2 text-3xl font-bold text-brand-600">{formatPrice(listing.price)}</p>

          <dl className="mt-4 space-y-1 text-sm text-gray-600">
            <div className="flex gap-2">
              <dt className="text-gray-400">Состояние:</dt>
              <dd>{conditionLabels[listing.condition]}</dd>
            </div>
            <div className="flex gap-2">
              <dt className="text-gray-400">Категория:</dt>
              <dd>{listing.categoryName}</dd>
            </div>
            <div className="flex gap-2">
              <dt className="text-gray-400">Город:</dt>
              <dd>{listing.city}</dd>
            </div>
            <div className="flex gap-2">
              <dt className="text-gray-400">Статус:</dt>
              <dd>{statusLabels[listing.status]}</dd>
            </div>
            <div className="flex gap-2">
              <dt className="text-gray-400">Продавец:</dt>
              <dd>{listing.sellerName}</dd>
            </div>
          </dl>

          {isOwner ? (
            <div className="mt-6 flex gap-2">
              <button
                onClick={handleDelete}
                disabled={del.isPending}
                className="rounded-md border border-red-300 px-4 py-2 text-sm font-medium text-red-600 hover:bg-red-50 disabled:opacity-60"
              >
                Удалить
              </button>
            </div>
          ) : (
            <button className="mt-6 rounded-md bg-brand-600 px-5 py-2 font-medium text-white hover:bg-brand-700">
              Написать продавцу
            </button>
          )}
        </div>
      </div>

      <div className="mt-6">
        <h2 className="mb-2 text-lg font-semibold">Описание</h2>
        <p className="whitespace-pre-wrap text-gray-700">{listing.description}</p>
      </div>

      <p className="mt-6 text-xs text-gray-400">
        Опубликовано {formatDate(listing.createdAt)} · {listing.viewsCount} просмотров
      </p>
    </div>
  )
}
