import { useParams } from 'react-router-dom'

export function ListingDetailsPage() {
  const { id } = useParams<{ id: string }>()

  return (
    <div>
      <h1 className="mb-1 text-2xl font-bold">Карточка объявления</h1>
      <p className="text-gray-500">
        Объявление <code className="rounded bg-gray-100 px-1">{id}</code>. Детальную
        страницу с галереей фото, ценой и кнопкой «Написать продавцу» подключим на этапе 1.
      </p>
    </div>
  )
}
