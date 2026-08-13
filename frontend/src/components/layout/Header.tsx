import { Link, NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../../features/auth/hooks'

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `px-3 py-2 rounded-md text-sm font-medium transition-colors ${
    isActive ? 'text-brand-700 bg-brand-50' : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'
  }`

export function Header() {
  const { user, isAuthenticated, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/')
  }

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
          {isAuthenticated && (
            <NavLink to="/chat" className={navLinkClass}>
              Сообщения
            </NavLink>
          )}
        </nav>

        <div className="ml-auto flex items-center gap-2">
          <Link
            to="/listings/new"
            className="rounded-md bg-brand-600 px-3 py-2 text-sm font-medium text-white transition-colors hover:bg-brand-700"
          >
            Разместить
          </Link>

          {isAuthenticated ? (
            <>
              <Link
                to="/profile"
                className="rounded-md px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100"
              >
                {user?.displayName ?? 'Профиль'}
              </Link>
              <button
                onClick={handleLogout}
                className="rounded-md px-3 py-2 text-sm font-medium text-gray-500 hover:bg-gray-100"
              >
                Выйти
              </button>
            </>
          ) : (
            <Link
              to="/login"
              className="rounded-md px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100"
            >
              Войти
            </Link>
          )}
        </div>
      </div>
    </header>
  )
}
