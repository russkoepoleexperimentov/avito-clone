import { useNavigate, useParams } from 'react-router-dom'
import { useListing, useUpdateListing } from '../features/listings/hooks'
import { ListingForm } from '../features/listings/components/ListingForm'
import { getApiErrorMessage } from '../lib/errors'
import type { UpdateListingRequest } from '../features/listings/types'

export function EditListingPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: listing, isLoading, isError } = useListing(id!)
  const update = useUpdateListing(id!)

  if (isLoading) return <p className="text-gray-500">Загрузка…</p>
  if (isError || !listing) return <p className="text-gray-500">Объявление не найдено.</p>

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="mb-6 text-2xl font-bold">Редактирование объявления</h1>

      <ListingForm
        withStatus
        submitLabel="Сохранить"
        pendingLabel="Сохраняем…"
        isPending={update.isPending}
        errorMessage={update.isError ? getApiErrorMessage(update.error) : undefined}
        defaultValues={{
          title: listing.title,
          description: listing.description,
          price: listing.price,
          condition: listing.condition,
          city: listing.city,
          categoryId: listing.categoryId,
          status: listing.status,
        }}
        onSubmit={(values) => {
          update.mutate(values as UpdateListingRequest, {
            onSuccess: () => navigate(`/listings/${id}`),
          })
        }}
      />
    </div>
  )
}
