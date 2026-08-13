import { useNavigate } from 'react-router-dom'
import { useCreateListing } from '../features/listings/hooks'
import { ListingForm } from '../features/listings/components/ListingForm'
import { getApiErrorMessage } from '../lib/errors'
import type { CreateListingRequest } from '../features/listings/types'

export function CreateListingPage() {
  const navigate = useNavigate()
  const create = useCreateListing()

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="mb-6 text-2xl font-bold">Новое объявление</h1>

      <ListingForm
        submitLabel="Опубликовать"
        pendingLabel="Публикуем…"
        isPending={create.isPending}
        errorMessage={create.isError ? getApiErrorMessage(create.error) : undefined}
        onSubmit={(values) => {
          const { status: _status, ...data } = values
          create.mutate(data as CreateListingRequest, {
            onSuccess: ({ id }) => navigate(`/listings/${id}`),
          })
        }}
      />
    </div>
  )
}
