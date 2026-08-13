export function HomePage() {
  return (
    <div>
      <h1 className="mb-1 text-2xl font-bold">Каталог объявлений</h1>
      <p className="mb-6 text-gray-500">
        Здесь появится список объявлений с поиском, фильтрами и пагинацией.
      </p>

      {/* Заглушка сетки карточек — реальные данные подключим на этапе 1. */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {Array.from({ length: 8 }).map((_, i) => (
          <div
            key={i}
            className="overflow-hidden rounded-lg border border-gray-200 bg-white"
          >
            <div className="aspect-square animate-pulse bg-gray-100" />
            <div className="space-y-2 p-3">
              <div className="h-4 w-2/3 animate-pulse rounded bg-gray-100" />
              <div className="h-5 w-1/3 animate-pulse rounded bg-gray-100" />
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
