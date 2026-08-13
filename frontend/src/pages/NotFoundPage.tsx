import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <div className="py-20 text-center">
      <p className="text-6xl font-bold text-brand-600">404</p>
      <h1 className="mt-4 text-xl font-semibold">Страница не найдена</h1>
      <Link to="/" className="mt-6 inline-block text-brand-600 hover:underline">
        Вернуться в каталог
      </Link>
    </div>
  )
}
