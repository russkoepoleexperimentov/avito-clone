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
import { ProtectedRoute } from './components/ProtectedRoute'

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

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}
