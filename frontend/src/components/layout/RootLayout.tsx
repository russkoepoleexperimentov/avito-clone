import { Outlet } from 'react-router-dom'
import { Header } from './Header'

/** Каркас страницы: общая шапка + область контента. */
export function RootLayout() {
  return (
    <div className="flex min-h-full flex-col">
      <Header />
      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-6">
        <Outlet />
      </main>
      <footer className="border-t border-gray-200 bg-white py-6 text-center text-sm text-gray-400">
        ResalePlatform — pet-проект · {new Date().getFullYear()}
      </footer>
    </div>
  )
}
