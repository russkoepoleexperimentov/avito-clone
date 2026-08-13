import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useAdminListings, useAdminDeleteListing } from '../../features/admin/hooks'
import { statusLabels } from '../../features/listings/types'
import { formatPrice, formatDate } from '../../lib/format'
import { Pagination } from '../../components/Pagination'

export function AdminListingsPage() {
  const [page, setPage] = useState(1)
  const { data, isLoading } = useAdminListings(page)
  const del = useAdminDeleteListing()

  if (isLoading) return <p className="text-gray-500">Загрузка…</p>

  return (
    <div>
      <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200 text-left text-gray-500">
            <tr>
              <th className="p-3">Название</th>
              <th className="p-3">Цена</th>
              <th className="p-3">Город</th>
              <th className="p-3">Продавец</th>
              <th className="p-3">Статус</th>
              <th className="p-3">Создано</th>
              <th className="p-3"></th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {data?.items.map((l) => (
              <tr key={l.id}>
                <td className="p-3 font-medium">
                  <Link to={`/listings/${l.id}`} className="hover:text-brand-600">
                    {l.title}
                  </Link>
                </td>
                <td className="p-3">{formatPrice(l.price)}</td>
                <td className="p-3 text-gray-600">{l.city}</td>
                <td className="p-3 text-gray-600">{l.sellerName}</td>
                <td className="p-3 text-gray-500">{statusLabels[l.status]}</td>
                <td className="p-3 text-gray-400">{formatDate(l.createdAt)}</td>
                <td className="p-3 text-right">
                  <button
                    onClick={() => {
                      if (confirm(`Удалить «${l.title}»?`)) del.mutate(l.id)
                    }}
                    disabled={del.isPending}
                    className="rounded-md px-3 py-1 text-xs font-medium text-red-600 hover:bg-red-50 disabled:opacity-60"
                  >
                    Удалить
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {data && (
        <Pagination page={data.page} totalPages={data.totalPages} onChange={setPage} />
      )}
    </div>
  )
}
