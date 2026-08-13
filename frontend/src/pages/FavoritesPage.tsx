import { Link } from 'react-router-dom'
import { useFavorites } from '../features/favorites/hooks'
import { ListingCard } from '../features/listings/components/ListingCard'

export function FavoritesPage() {
  const { data: favorites, isLoading, isError } = useFavorites()

  return (
    <div>
      <h1 className="mb-4 text-2xl font-bold">Избранное</h1>

      {isLoading ? (
        <p className="text-gray-500">Загрузка…</p>
      ) : isError ? (
        <p className="text-red-600">Не удалось загрузить избранное.</p>
      ) : !favorites || favorites.length === 0 ? (
        <p className="text-gray-500">
          Пока пусто. Отмечайте объявления сердечком в{' '}
          <Link to="/" className="text-brand-600 hover:underline">
            каталоге
          </Link>
          .
        </p>
      ) : (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 xl:grid-cols-4">
          {favorites.map((l) => (
            <ListingCard key={l.id} listing={l} />
          ))}
        </div>
      )}
    </div>
  )
}
