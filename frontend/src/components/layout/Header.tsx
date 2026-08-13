import { Link, NavLink } from 'react-router-dom'

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `px-3 py-2 rounded-md text-sm font-medium transition-colors ${
    isActive ? 'text-brand-700 bg-brand-50' : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'
  }`

export function Header() {
  return (
    <header className="border-b border-gray-200 bg-white">
      <div className="mx-auto flex max-w-6xl items-center gap-4 px-4 py-3">
        <Link to="/" className="text-xl font-bold text-brand-600">
          Resale<span className="text-gray-900">Platform</span>
        </Link>

        <nav className="ml-4 hidden items-center gap-1 sm:flex">
          <NavLink to="/" end className={navLinkClass}>
            Каталог
          </NavLink>
          <NavLink to="/favorites" className={navLinkClass}>
            Избранное
          </NavLink>
          <NavLink to="/my-listings" className={navLinkClass}>
            Мои объявления
          </NavLink>
        </nav>

        <div className="ml-auto flex items-center gap-2">
          <Link
            to="/listings/new"
            className="rounded-md bg-brand-600 px-3 py-2 text-sm font-medium text-white transition-colors hover:bg-brand-700"
          >
            Разместить
          </Link>
          <Link
            to="/login"
            className="rounded-md px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100"
          >
            Войти
          </Link>
        </div>
      </div>
    </header>
  )
}
