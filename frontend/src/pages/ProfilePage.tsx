import { useAuth } from '../features/auth/hooks'

export function ProfilePage() {
  const { user } = useAuth()

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="mb-4 text-2xl font-bold">Профиль</h1>

      <dl className="divide-y divide-gray-200 rounded-lg border border-gray-200 bg-white">
        <div className="flex justify-between px-4 py-3">
          <dt className="text-gray-500">Имя</dt>
          <dd className="font-medium">{user?.displayName ?? '—'}</dd>
        </div>
        <div className="flex justify-between px-4 py-3">
          <dt className="text-gray-500">Email</dt>
          <dd className="font-medium">{user?.email ?? '—'}</dd>
        </div>
        <div className="flex justify-between px-4 py-3">
          <dt className="text-gray-500">Город</dt>
          <dd className="font-medium">{user?.city ?? '—'}</dd>
        </div>
        <div className="flex justify-between px-4 py-3">
          <dt className="text-gray-500">Роли</dt>
          <dd className="font-medium">{user?.roles.join(', ') ?? '—'}</dd>
        </div>
      </dl>

      <p className="mt-4 text-sm text-gray-500">
        Редактирование профиля (телефон, город, аватар) — следующий шаг этапа 1.
      </p>
    </div>
  )
}
