import { useState, type FormEvent } from 'react'
import type { SortOption } from '../catalog'

interface Props {
  params: URLSearchParams
  onApply: (updates: Record<string, string | null>) => void
}

const sortOptions: { value: SortOption; label: string }[] = [
  { value: 'newest', label: 'Сначала новые' },
  { value: 'price_asc', label: 'Дешевле' },
  { value: 'price_desc', label: 'Дороже' },
]

const inputClass =
  'rounded-md border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500'

export function CatalogFilters({ params, onApply }: Props) {
  const [search, setSearch] = useState(params.get('q') ?? '')
  const [city, setCity] = useState(params.get('city') ?? '')
  const [minPrice, setMinPrice] = useState(params.get('minPrice') ?? '')
  const [maxPrice, setMaxPrice] = useState(params.get('maxPrice') ?? '')

  const apply = (e: FormEvent) => {
    e.preventDefault()
    onApply({
      q: search.trim() || null,
      city: city.trim() || null,
      minPrice: minPrice || null,
      maxPrice: maxPrice || null,
    })
  }

  const reset = () => {
    setSearch('')
    setCity('')
    setMinPrice('')
    setMaxPrice('')
    onApply({ q: null, city: null, minPrice: null, maxPrice: null })
  }

  return (
    <form onSubmit={apply} className="mb-4 space-y-3 rounded-lg border border-gray-200 bg-white p-4">
      <div className="flex flex-wrap gap-3">
        <input
          className={`${inputClass} min-w-[200px] flex-1`}
          placeholder="Поиск по названию или описанию"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <input
          className={`${inputClass} w-40`}
          placeholder="Город"
          value={city}
          onChange={(e) => setCity(e.target.value)}
        />
      </div>

      <div className="flex flex-wrap items-center gap-3">
        <input
          type="number"
          min={0}
          className={`${inputClass} w-32`}
          placeholder="Цена от"
          value={minPrice}
          onChange={(e) => setMinPrice(e.target.value)}
        />
        <input
          type="number"
          min={0}
          className={`${inputClass} w-32`}
          placeholder="до"
          value={maxPrice}
          onChange={(e) => setMaxPrice(e.target.value)}
        />

        <select
          className={inputClass}
          value={params.get('sort') ?? 'newest'}
          onChange={(e) => onApply({ sort: e.target.value === 'newest' ? null : e.target.value })}
        >
          {sortOptions.map((o) => (
            <option key={o.value} value={o.value}>
              {o.label}
            </option>
          ))}
        </select>

        <div className="ml-auto flex gap-2">
          <button
            type="button"
            onClick={reset}
            className="rounded-md px-3 py-2 text-sm text-gray-500 hover:bg-gray-100"
          >
            Сбросить
          </button>
          <button
            type="submit"
            className="rounded-md bg-brand-600 px-4 py-2 text-sm font-medium text-white hover:bg-brand-700"
          >
            Найти
          </button>
        </div>
      </div>
    </form>
  )
}
