import { Link } from 'react-router-dom'

export function LoginPage() {
  return (
    <div className="mx-auto max-w-sm">
      <h1 className="mb-6 text-2xl font-bold">Вход</h1>

      {/* Заглушка формы. Валидацию (RHF + zod) и запрос к /auth/login добавим на этапе 1. */}
      <form className="space-y-4">
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
          Войти
        </button>
      </form>

      <p className="mt-4 text-sm text-gray-500">
        Нет аккаунта?{' '}
        <Link to="/register" className="text-brand-600 hover:underline">
          Зарегистрироваться
        </Link>
      </p>
    </div>
  )
}
