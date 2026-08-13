import { useSearchParams } from 'react-router-dom'
import { CategorySidebar } from '../features/categories/components/CategorySidebar'
import { useCategories } from '../features/categories/hooks'
import { findCategoryBySlug } from '../features/categories/utils'
import { useListings } from '../features/listings/hooks'
import type { ListingQuery, SortOption } from '../features/listings/catalog'
import { ListingCard } from '../features/listings/components/ListingCard'
import { CatalogFilters } from '../features/listings/components/CatalogFilters'
import { Pagination } from '../components/Pagination'

const PAGE_SIZE = 12

export function HomePage() {
  const [params, setParams] = useSearchParams()
  const { data: categories } = useCategories()

  const categorySlug = params.get('category') ?? undefined
  const activeCategory = categories && categorySlug
    ? findCategoryBySlug(categories, categorySlug)
    : undefined

  const query: ListingQuery = {
    search: params.get('q') ?? undefined,
    categorySlug,
    minPrice: params.get('minPrice') ? Number(params.get('minPrice')) : undefined,
    maxPrice: params.get('maxPrice') ? Number(params.get('maxPrice')) : undefined,
    city: params.get('city') ?? undefined,
    sort: (params.get('sort') as SortOption) ?? undefined,
    page: params.get('page') ? Number(params.get('page')) : 1,
    pageSize: PAGE_SIZE,
  }

  const { data, isLoading, isError } = useListings(query)

  /** Обновляет один параметр URL, сбрасывая страницу (кроме смены самой страницы). */
  const updateParam = (key: string, value: string | null, resetPage = true) => {
    const next = new URLSearchParams(params)
    if (value) next.set(key, value)
    else next.delete(key)
    if (resetPage && key !== 'page') next.delete('page')
    setParams(next)
  }

  /** Обновляет сразу несколько параметров (для формы фильтров) и сбрасывает страницу. */
  const setMany = (updates: Record<string, string | null>) => {
    const next = new URLSearchParams(params)
    for (const [key, value] of Object.entries(updates)) {
      if (value) next.set(key, value)
      else next.delete(key)
    }
    next.delete('page')
    setParams(next)
  }

  return (
    <div className="flex flex-col gap-6 lg:flex-row">
      <aside className="w-full shrink-0 lg:w-56">
        <div className="rounded-lg border border-gray-200 bg-white p-4">
          <h2 className="mb-3 text-sm font-semibold uppercase tracking-wide text-gray-400">
            Категории
          </h2>
          <CategorySidebar />
        </div>
      </aside>

      <div className="flex-1">
        <h1 className="mb-4 text-2xl font-bold">Каталог объявлений</h1>

        <CatalogFilters params={params} onApply={setMany} />

        {activeCategory && (
          <div className="mb-4">
            <span className="inline-flex items-center gap-2 rounded-full bg-brand-50 px-3 py-1 text-sm text-brand-700">
              {activeCategory.name}
              <button
                onClick={() => updateParam('category', null)}
                className="text-brand-500 hover:text-brand-700"
                aria-label="Сбросить категорию"
              >
                ✕
              </button>
            </span>
          </div>
        )}

        {isLoading ? (
          <p className="text-gray-500">Загрузка…</p>
        ) : isError ? (
          <p className="text-red-600">Не удалось загрузить объявления.</p>
        ) : !data || data.items.length === 0 ? (
          <p className="text-gray-500">Ничего не найдено. Попробуйте изменить фильтры.</p>
        ) : (
          <>
            <p className="mb-3 text-sm text-gray-400">Найдено: {data.totalCount}</p>
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 xl:grid-cols-4">
              {data.items.map((l) => (
                <ListingCard key={l.id} listing={l} />
              ))}
            </div>
            <Pagination
              page={data.page}
              totalPages={data.totalPages}
              onChange={(p) => updateParam('page', String(p), false)}
            />
          </>
        )}
      </div>
    </div>
  )
}
