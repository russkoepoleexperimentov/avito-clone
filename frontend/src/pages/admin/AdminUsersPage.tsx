import { useAdminUsers, useSetUserBlocked } from '../../features/admin/hooks'
import { formatDate } from '../../lib/format'

export function AdminUsersPage() {
  const { data: users, isLoading } = useAdminUsers()
  const setBlocked = useSetUserBlocked()

  if (isLoading) return <p className="text-gray-500">Загрузка…</p>

  return (
    <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
      <table className="w-full text-sm">
        <thead className="border-b border-gray-200 text-left text-gray-500">
          <tr>
            <th className="p-3">Имя</th>
            <th className="p-3">Email</th>
            <th className="p-3">Роли</th>
            <th className="p-3">Регистрация</th>
            <th className="p-3">Статус</th>
            <th className="p-3"></th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100">
          {users?.map((u) => {
            const isAdmin = u.roles.includes('Admin')
            return (
              <tr key={u.id}>
                <td className="p-3 font-medium">{u.displayName}</td>
                <td className="p-3 text-gray-600">{u.email}</td>
                <td className="p-3 text-gray-600">{u.roles.join(', ')}</td>
                <td className="p-3 text-gray-400">{formatDate(u.createdAt)}</td>
                <td className="p-3">
                  {u.isBlocked ? (
                    <span className="rounded bg-red-100 px-2 py-0.5 text-xs text-red-700">
                      Заблокирован
                    </span>
                  ) : (
                    <span className="rounded bg-green-100 px-2 py-0.5 text-xs text-green-700">
                      Активен
                    </span>
                  )}
                </td>
                <td className="p-3 text-right">
                  {!isAdmin && (
                    <button
                      onClick={() =>
                        setBlocked.mutate({ userId: u.id, blocked: !u.isBlocked })
                      }
                      disabled={setBlocked.isPending}
                      className={`rounded-md px-3 py-1 text-xs font-medium disabled:opacity-60 ${
                        u.isBlocked
                          ? 'text-green-600 hover:bg-green-50'
                          : 'text-red-600 hover:bg-red-50'
                      }`}
                    >
                      {u.isBlocked ? 'Разблокировать' : 'Заблокировать'}
                    </button>
                  )}
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
