import { CategorySidebar } from '../features/categories/components/CategorySidebar'

export function HomePage() {
  return (
    <div className="flex flex-col gap-6 lg:flex-row">
      {/* Сайдбар категорий */}
      <aside className="w-full shrink-0 lg:w-56">
        <div className="rounded-lg border border-gray-200 bg-white p-4">
          <h2 className="mb-3 text-sm font-semibold uppercase tracking-wide text-gray-400">
            Категории
          </h2>
          <CategorySidebar />
        </div>
      </aside>

      {/* Каталог */}
      <div className="flex-1">
        <h1 className="mb-1 text-2xl font-bold">Каталог объявлений</h1>
        <p className="mb-6 text-gray-500">
          Здесь появится список объявлений с поиском, фильтрами и пагинацией.
        </p>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <div key={i} className="overflow-hidden rounded-lg border border-gray-200 bg-white">
              <div className="aspect-square animate-pulse bg-gray-100" />
              <div className="space-y-2 p-3">
                <div className="h-4 w-2/3 animate-pulse rounded bg-gray-100" />
                <div className="h-5 w-1/3 animate-pulse rounded bg-gray-100" />
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
