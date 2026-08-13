import { Link } from 'react-router-dom'

export function RegisterPage() {
  return (
    <div className="mx-auto max-w-sm">
      <h1 className="mb-6 text-2xl font-bold">Регистрация</h1>

      <form className="space-y-4">
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Имя</label>
          <input
            className="w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500"
            placeholder="Как вас зовут"
            disabled
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Email</label>
          <input
            type="email"
            className="w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500"
            placeholder="you@example.com"
            disabled
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Пароль</label>
          <input
            type="password"
            className="w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500"
            placeholder="••••••••"
            disabled
          />
        </div>
        <button
          type="button"
          className="w-full rounded-md bg-brand-600 py-2 font-medium text-white hover:bg-brand-700"
          disabled
        >
          Создать аккаунт
        </button>
      </form>

      <p className="mt-4 text-sm text-gray-500">
        Уже есть аккаунт?{' '}
        <Link to="/login" className="text-brand-600 hover:underline">
          Войти
        </Link>
      </p>
    </div>
  )
}
