import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../features/auth/hooks'

/** Пускает только администраторов. */
export function AdminRoute() {
  const { isAuthenticated, user } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }
  if (!user?.roles.includes('Admin')) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}
