import { Routes, Route } from 'react-router-dom'
import { RootLayout } from './components/layout/RootLayout'
import { HomePage } from './pages/HomePage'
import { LoginPage } from './pages/LoginPage'
import { RegisterPage } from './pages/RegisterPage'
import { ListingDetailsPage } from './pages/ListingDetailsPage'
import { CreateListingPage } from './pages/CreateListingPage'
import { EditListingPage } from './pages/EditListingPage'
import { MyListingsPage } from './pages/MyListingsPage'
import { FavoritesPage } from './pages/FavoritesPage'
import { ProfilePage } from './pages/ProfilePage'
import { NotFoundPage } from './pages/NotFoundPage'
import { ConversationsPage } from './pages/ConversationsPage'
import { ConversationPage } from './pages/ConversationPage'
import { AdminLayout } from './pages/admin/AdminLayout'
import { AdminUsersPage } from './pages/admin/AdminUsersPage'
import { AdminCategoriesPage } from './pages/admin/AdminCategoriesPage'
import { AdminListingsPage } from './pages/admin/AdminListingsPage'
import { ProtectedRoute } from './components/ProtectedRoute'
import { AdminRoute } from './components/AdminRoute'

export default function App() {
  return (
    <Routes>
      <Route element={<RootLayout />}>
        {/* Публичные */}
        <Route index element={<HomePage />} />
        <Route path="listings/:id" element={<ListingDetailsPage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />

        {/* Только для авторизованных */}
        <Route element={<ProtectedRoute />}>
          <Route path="listings/new" element={<CreateListingPage />} />
          <Route path="listings/:id/edit" element={<EditListingPage />} />
          <Route path="my-listings" element={<MyListingsPage />} />
          <Route path="favorites" element={<FavoritesPage />} />
          <Route path="chat" element={<ConversationsPage />} />
          <Route path="chat/:id" element={<ConversationPage />} />
          <Route path="profile" element={<ProfilePage />} />
        </Route>

        {/* Только для администраторов */}
        <Route element={<AdminRoute />}>
          <Route path="admin" element={<AdminLayout />}>
            <Route index element={<AdminUsersPage />} />
            <Route path="users" element={<AdminUsersPage />} />
            <Route path="categories" element={<AdminCategoriesPage />} />
            <Route path="listings" element={<AdminListingsPage />} />
          </Route>
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}
