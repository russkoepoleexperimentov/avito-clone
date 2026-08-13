interface Props {
  page: number
  totalPages: number
  onChange: (page: number) => void
}

/** Простая пагинация: назад/вперёд + номер текущей страницы. */
export function Pagination({ page, totalPages, onChange }: Props) {
  if (totalPages <= 1) return null

  return (
    <div className="mt-6 flex items-center justify-center gap-3">
      <button
        onClick={() => onChange(page - 1)}
        disabled={page <= 1}
        className="rounded-md border border-gray-300 px-3 py-1 text-sm disabled:opacity-40 hover:bg-gray-50"
      >
        Назад
      </button>
      <span className="text-sm text-gray-500">
        {page} из {totalPages}
      </span>
      <button
        onClick={() => onChange(page + 1)}
        disabled={page >= totalPages}
        className="rounded-md border border-gray-300 px-3 py-1 text-sm disabled:opacity-40 hover:bg-gray-50"
      >
        Вперёд
      </button>
    </div>
  )
}
