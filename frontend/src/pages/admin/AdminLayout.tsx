import { NavLink, Outlet } from 'react-router-dom'

const tabClass = ({ isActive }: { isActive: boolean }) =>
  `rounded-md px-3 py-2 text-sm font-medium ${
    isActive ? 'bg-brand-600 text-white' : 'text-gray-600 hover:bg-gray-100'
  }`

export function AdminLayout() {
  return (
    <div>
      <h1 className="mb-4 text-2xl font-bold">Админ-панель</h1>
      <nav className="mb-6 flex gap-2 border-b border-gray-200 pb-3">
        <NavLink to="/admin/users" className={tabClass}>
          Пользователи
        </NavLink>
        <NavLink to="/admin/categories" className={tabClass}>
          Категории
        </NavLink>
        <NavLink to="/admin/listings" className={tabClass}>
          Объявления
        </NavLink>
      </nav>
      <Outlet />
    </div>
  )
}
