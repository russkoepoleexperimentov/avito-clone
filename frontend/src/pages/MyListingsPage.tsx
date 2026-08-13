import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useMyListings, useDeleteListing } from '../features/listings/hooks'
import { statusLabels, type ListingStatus } from '../features/listings/types'
import { formatPrice } from '../lib/format'

const filters: { value: ListingStatus | 'all'; label: string }[] = [
  { value: 'all', label: 'Все' },
  { value: 'Active', label: 'Активные' },
  { value: 'Sold', label: 'Проданные' },
  { value: 'Archived', label: 'Снятые' },
]

export function MyListingsPage() {
  const [filter, setFilter] = useState<ListingStatus | 'all'>('all')
  const { data: listings, isLoading } = useMyListings(filter === 'all' ? undefined : filter)
  const del = useDeleteListing()

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Мои объявления</h1>
        <Link
          to="/listings/new"
          className="rounded-md bg-brand-600 px-3 py-2 text-sm font-medium text-white hover:bg-brand-700"
        >
          Разместить
        </Link>
      </div>

      <div className="mb-4 flex gap-2">
        {filters.map((f) => (
          <button
            key={f.value}
            onClick={() => setFilter(f.value)}
            className={`rounded-full px-3 py-1 text-sm ${
              filter === f.value
                ? 'bg-brand-600 text-white'
                : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
            }`}
          >
            {f.label}
          </button>
        ))}
      </div>

      {isLoading ? (
        <p className="text-gray-500">Загрузка…</p>
      ) : !listings || listings.length === 0 ? (
        <p className="text-gray-500">Объявлений пока нет.</p>
      ) : (
        <ul className="divide-y divide-gray-200 overflow-hidden rounded-lg border border-gray-200 bg-white">
          {listings.map((l) => (
            <li key={l.id} className="flex items-center gap-4 p-4">
              <div className="flex-1">
                <Link to={`/listings/${l.id}`} className="font-medium hover:text-brand-600">
                  {l.title}
                </Link>
                <div className="mt-1 text-sm text-gray-500">
                  {formatPrice(l.price)} · {l.city} ·{' '}
                  <span className="text-gray-400">{statusLabels[l.status]}</span>
                </div>
              </div>
              <button
                onClick={() => del.mutate(l.id)}
                disabled={del.isPending}
                className="rounded-md px-3 py-1 text-sm text-red-600 hover:bg-red-50 disabled:opacity-60"
              >
                Удалить
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
