import { Link } from 'react-router-dom'
import { useCategories } from '../hooks'

/** Боковая панель с деревом категорий. Ссылки ведут в каталог с фильтром (?category=slug). */
export function CategorySidebar() {
  const { data: categories, isLoading, isError } = useCategories()

  if (isLoading) {
    return (
      <div className="space-y-2">
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="h-5 w-3/4 animate-pulse rounded bg-gray-100" />
        ))}
      </div>
    )
  }

  if (isError || !categories) {
    return <p className="text-sm text-gray-400">Не удалось загрузить категории</p>
  }

  return (
    <nav className="space-y-4">
      {categories.map((root) => (
        <div key={root.id}>
          <Link
            to={`/?category=${root.slug}`}
            className="block font-semibold text-gray-900 hover:text-brand-600"
          >
            {root.name}
          </Link>
          {root.children.length > 0 && (
            <ul className="mt-1 space-y-1">
              {root.children.map((child) => (
                <li key={child.id}>
                  <Link
                    to={`/?category=${child.slug}`}
                    className="block text-sm text-gray-600 hover:text-brand-600"
                  >
                    {child.name}
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </div>
      ))}
    </nav>
  )
}
